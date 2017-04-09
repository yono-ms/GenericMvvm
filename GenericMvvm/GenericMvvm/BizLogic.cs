﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace GenericMvvm
{
    /// <summary>
    /// アプリケーション機能の実体
    /// </summary>
    [DataContract]
    public class BizLogic
    {
        const string FORMAT = "----BizLogicEvent---- {0}";
        /// <summary>
        /// 永続性サポート
        /// </summary>
        static string Key = "SharedMemory";
        /// <summary>
        /// OS実装機能
        /// </summary>
        INativeCall _nc;
        /// <summary>
        /// 不揮発領域からロードする
        /// </summary>
        /// <param name="nc"></param>
        /// <returns></returns>
        public static async Task<BizLogic> LoadBizLogicAsync(INativeCall nc)
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { "LoadBizLogicAsync START" });

            BizLogic result;
            var json = await nc.LoadFileAsync(Key);
            if (json == null)
            {
                result = new BizLogic();
            }
            else
            {
                try
                {
                    using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                    {
                        var ser = new DataContractJsonSerializer(typeof(BizLogic));
                        result = ser.ReadObject(ms) as BizLogic;
                    }

                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                    //await nc.ShowAlertAsync("Exception", ex.Message, "OK", null);
                    result = new BizLogic();
                }
            }
            // 生成したインスタンスに引数を渡す
            result._nc = nc;

            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { "LoadBizLogicAsync END" });

            return result;
        }
        /// <summary>
        /// 不揮発領域にセーブする
        /// </summary>
        /// <returns></returns>
        public async Task SaveBizLogicAsync()
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { "SaveBizLogicAsync START" });

            try
            {
                using (var ms = new MemoryStream())
                {
                    var ser = new DataContractJsonSerializer(typeof(BizLogic));
                    ser.WriteObject(ms, this);
                    var bytes = ms.ToArray();
                    var json = Encoding.UTF8.GetString(bytes, 0, bytes.Length);

                    await _nc.SaveFileAsync(Key, json);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                await _nc.ShowAlertAsync("Exception", ex.Message, "OK", null);
            }

            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { "SaveBizLogicAsync END" });
        }
        /// <summary>
        /// ViewModelのインスタンスを生成する
        /// </summary>
        /// <typeparam name="T">ViewModelの型</typeparam>
        /// <returns>インスタンス</returns>
        public T GetViewModel<T>() where T : BaseViewModel, new()
        {
            T instance;
            if (_Instances.ContainsKey(typeof(T)))
            {
                // 過去に生成されたインスタンスが生きている
                instance = _Instances[typeof(T)] as T;
            }
            else
            {
                // 再起動後に初めて生成する
                instance = CreateViewModel<T>();
                _Instances.Add(typeof(T), instance);
            }
            return instance;
        }
        /// <summary>
        /// コミット済み情報からViewModelを生成する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private T CreateViewModel<T>() where T : BaseViewModel, new()
        {
            // 保存情報がある場合はクローンを作成して渡す
            if (typeof(T) == typeof(MainViewModel))
            {
                return DeepCopy<MainViewModel>(_SavedMainViewModel) as T;
            }
            // 保存情報がない場合はそのまま渡す
            return new T();
        }

        /// <summary>
        /// インスタンス辞書。
        /// ViewModelの実体はここで管理される。
        /// この辞書は不揮発領域に保存しないので、コミットしていない情報は消える。
        /// </summary>
        Dictionary<Type, object> _Instances;

        /// <summary>
        /// コミット済みの入力情報（Main）
        /// </summary>
        [DataMember]
        MainViewModel _SavedMainViewModel;

        /// <summary>
        /// ViewModelのディープコピー
        /// </summary>
        /// <typeparam name="T">ViewModelの型</typeparam>
        /// <param name="src">コピー元</param>
        /// <returns>クローン</returns>
        private T DeepCopy<T>(T src) where T : BaseViewModel, new()
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { "DeepCopy START" });

            T dst;

            try
            {
                using (var ms = new MemoryStream())
                {
                    var ser = new DataContractJsonSerializer(typeof(T));
                    // 一度json化
                    ser.WriteObject(ms, src);
                    // jsonから復元
                    ms.Position = 0;
                    dst = ser.ReadObject(ms) as T;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                dst = new T();
            }

            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { "DeepCopy END" });
            return dst;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BizLogic()
        {
            _Instances = new Dictionary<Type, object>();

            _SavedMainViewModel = new MainViewModel
            {
                Title = "生成直後のタイトルです",
                Footer = "生成直後のフッターです"
            };
        }
    }
}
