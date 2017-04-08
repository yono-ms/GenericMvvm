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
                instance = _Instances[typeof(T)] as T;
            }
            else
            {
                instance = new T();
                _Instances.Add(typeof(T), instance);
            }
            LoadInitialData<T>(instance);
            return instance;
        }

        /// <summary>
        /// 生成したViewModelに初期値を設定する
        /// </summary>
        /// <typeparam name="T">ViewModelの型</typeparam>
        /// <param name="instance">インスタンス</param>
        private void LoadInitialData<T>(T instance) where T : BaseViewModel, new()
        {
            var vm = instance as MainViewModel;
            if (vm != null)
            {
                vm.Title = "生成直後のタイトルです";
                vm.Footer = "生成直後のフッターです";
            }
        }

        /// <summary>
        /// インスタンス辞書。
        /// この辞書は不揮発領域に保存しないので、コミットしていない情報は消える。
        /// </summary>
        Dictionary<Type, object> _Instances;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BizLogic()
        {
            _Instances = new Dictionary<Type, object>();
        }
    }
}
