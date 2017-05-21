﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        INativeCall _NC;
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
            result._NC = nc;

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

                    await _NC.SaveFileAsync(Key, json);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                await _NC.ShowAlertAsync("Exception", ex.Message, "OK", null);
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
                instance._BizLogic = this;
                if (typeof(T) != typeof(ConfirmViewModel))
                {
                    _Instances.Add(typeof(T), instance);
                }
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
            else if (typeof(T) == typeof(NameViewModel))
            {
                return DeepCopy<NameViewModel>(_SavedNameViewModel) as T;
            }
            else if (typeof(T) == typeof(BirthViewModel))
            {
                return DeepCopy<BirthViewModel>(_SavedBirthViewModel) as T;
            }
            else if (typeof(T) == typeof(AddressViewModel))
            {
                return DeepCopy<AddressViewModel>(_SavedAddressViewModel) as T;
            }
            else if (typeof(T) == typeof(ConfirmViewModel))
            {
                // 確認画面は保存する必要はない
                return CreateConfirmViewModel() as T;
            }
            // 保存情報がない場合はそのまま渡す
            return new T();
        }

        /// <summary>
        /// 確認画面を作る
        /// </summary>
        /// <returns></returns>
        private BaseViewModel CreateConfirmViewModel()
        {
            var confirmViewModel = new ConfirmViewModel();
            confirmViewModel.ConfirmList.Add(new ConfirmViewModel.ConfirmCell
            {
                Title = "お名前",
                Value = _SavedNameViewModel.LastName + " " + _SavedNameViewModel.FirstName
            });
            confirmViewModel.ConfirmList.Add(new ConfirmViewModel.ConfirmCell
            {
                Title = "生年月日",
                Value = _SavedBirthViewModel.Year + "年 " + _SavedBirthViewModel.Month + "月 " + _SavedBirthViewModel.Day + "日"
            });
            confirmViewModel.ConfirmList.Add(new ConfirmViewModel.ConfirmCell
            {
                Title = "住所",
                Value = _SavedAddressViewModel.Address
            });
            confirmViewModel.ConfirmList.Add(new ConfirmViewModel.ConfirmCell
            {
                Title = "住所（ふりがな）",
                Value = _SavedAddressViewModel.AddressKana
            });

            return confirmViewModel;
        }

        /// <summary>
        /// インスタンス辞書。
        /// ViewModelの実体はここで管理される。
        /// この辞書は不揮発領域に保存しないので、コミットしていない情報は消える。
        /// </summary>
        Dictionary<Type, BaseViewModel> _Instances;

        /// <summary>
        /// コミット済みの入力情報（Main）
        /// </summary>
        [DataMember]
        MainViewModel _SavedMainViewModel;
        /// <summary>
        /// コミット済みの入力情報（Name）
        /// </summary>
        [DataMember]
        NameViewModel _SavedNameViewModel;
        /// <summary>
        /// コミット済みの入力情報（Birth）
        /// </summary>
        [DataMember]
        BirthViewModel _SavedBirthViewModel;
        /// <summary>
        /// コミット済みの入力情報（Address）
        /// </summary>
        [DataMember]
        AddressViewModel _SavedAddressViewModel;
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
        /// 中断画面を復元する
        /// </summary>
        public void RestorePage()
        {
            if (string.IsNullOrEmpty(CurrentPage))
            {
                NavigateTo("Name", true);
            }
            else
            {
                NavigateTo(CurrentPage, true);
            }
        }
        /// <summary>
        /// 現在表示中のページ
        /// </summary>
        [DataMember]
        public string CurrentPage { get; private set; }
        /// <summary>
        /// 入力確定
        /// </summary>
        public void Commit()
        {
            if (CurrentPage.Equals("Name"))
            {
                _SavedNameViewModel = DeepCopy(_Instances[typeof(NameViewModel)] as NameViewModel);
                // 不揮発領域に保存
                NavigateTo("Birth", true);
            }
            else if (CurrentPage.Equals("Birth"))
            {
                _SavedBirthViewModel = DeepCopy(_Instances[typeof(BirthViewModel)] as BirthViewModel);
                // 不揮発領域に保存
                NavigateTo("Address", true);
            }
            else if (CurrentPage.Equals("Address"))
            {
                _SavedAddressViewModel = DeepCopy(_Instances[typeof(AddressViewModel)] as AddressViewModel);
                // 不揮発領域に保存
                NavigateTo("Confirm", true);
            }
        }
        public void GoBack()
        {
            if (CurrentPage.Equals("Birth"))
            {
                NavigateTo("Name", false);
            }
            else if (CurrentPage.Equals("Address"))
            {
                NavigateTo("Birth", false);
            }
            else if (CurrentPage.Equals("Confirm"))
            {
                NavigateTo("Address", false);
            }
        }
        /// <summary>
        /// 画面遷移
        /// </summary>
        /// <param name="page"></param>
        /// <param name="forward"></param>
        private void NavigateTo(string page, bool forward)
        {
            var mvm = _Instances[typeof(MainViewModel)] as MainViewModel;

            if (_ViewModelInfos.ContainsKey(page))
            {
                mvm.ObjectErrors = null;

                var vmi = _ViewModelInfos[page];
                mvm.Title = vmi.Title;
                mvm.Footer = vmi.Footer;
                mvm.ShwoBackButton = vmi.ShowBackButton;

                _NC.NavigateTo(page, forward);

                CurrentPage = page;

                // 画面遷移で保存
                Task.Run(() => SaveBizLogicAsync());
            }
            else
            {
                mvm.ObjectErrors = new ObservableCollection<string>(new[] { "unknown page " + page });
            }
        }
        /// <summary>
        /// メインのエラー領域にVMのエラーを表示する
        /// </summary>
        public void ShowError()
        {
            var mvm = _Instances[typeof(MainViewModel)] as MainViewModel;
            var vmi = _ViewModelInfos[CurrentPage];
            var page = _Instances[vmi.Type];
            mvm.ObjectErrors = page.ObjectErrors;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BizLogic()
        {
            _ViewModelInfos = new Dictionary<string, ViewModelInfo>();
            _ViewModelInfos.Add("Name", new ViewModelInfo { Type=typeof(NameViewModel), Title="お名前入力", Footer="copylight", ShowBackButton=false });
            _ViewModelInfos.Add("Birth", new ViewModelInfo { Type = typeof(BirthViewModel), Title = "生年月日入力", Footer = "copylight", ShowBackButton=true });
            _ViewModelInfos.Add("Address", new ViewModelInfo { Type = typeof(AddressViewModel), Title = "住所入力", Footer = "copylight", ShowBackButton = true });
            _ViewModelInfos.Add("Confirm", new ViewModelInfo { Type = typeof(ConfirmViewModel), Title = "入力内容の確認", Footer = "copylight", ShowBackButton = true });

            _Instances = new Dictionary<Type, BaseViewModel>();

            _SavedMainViewModel = new MainViewModel
            {
                Title = "生成直後のタイトルです",
                Footer = "生成直後のフッターです"
            };

            _SavedNameViewModel = new NameViewModel();

            _SavedBirthViewModel = new BirthViewModel();

            _SavedAddressViewModel = new AddressViewModel();
        }
        /// <summary>
        /// ページ名からViewModel情報を検索する辞書
        /// </summary>
        private Dictionary<string, ViewModelInfo> _ViewModelInfos;
        /// <summary>
        /// ViewModelの静的情報
        /// </summary>
        public class ViewModelInfo
        {
            public Type Type { get; set; }
            public string Title { get; set; }
            public string Footer { get; set; }
            public bool ShowBackButton { get; set; }
        }

        /// <summary>
        /// 郵便番号検索を実行する
        /// </summary>
        public void CommandGetZipCloud()
        {
            var mvm = _Instances[typeof(MainViewModel)] as MainViewModel;
            var avm = _Instances[typeof(AddressViewModel)] as AddressViewModel;

            // エラークリア
            mvm.ObjectErrors = null;
            mvm.ObjectErrors = new ObservableCollection<string>();

            if (avm != null)
            {
                // プログレス表示
                mvm.ShowProgress = true;
                // パラメータ準備
                var url = "http://zipcloud.ibsnet.co.jp/api/search";
                var param = new Dictionary<string, string>();
                param.Add("zipcode", avm.PostalCode);

                // API実行
                var client = new GetHttpClient();
                _NC.RunUIThread(async () =>
                {
                    var resp = await client.Execute<ZipCloudResponse>(url, param);
                    if (!client.IsError)
                    {
                        if (resp != null)
                        {

                            if (string.IsNullOrEmpty(resp.status))
                            {
                                // とりあえず
                                mvm.ObjectErrors.Add(resp.status);
                                mvm.ObjectErrors.Add(resp.message);
                                mvm.ObjectErrors.Add(client.Json);
                            }
                            else
                            {
                                // プロパティ更新
                                avm.ResponseResults = resp.results;
                            }
                        }
                        else
                        {
                            // なぜかエラー
                            mvm.ObjectErrors.Add("ZipCloudResponseがnullです");
                        }
                    }
                    else
                    {
                        foreach (var item in client.ErrorMessages)
                        {
                            mvm.ObjectErrors.Add(item);
                        }
                    }

                    // プログレス非表示
                    mvm.ShowProgress = false;
                });

            }
            else
            {
                mvm.ObjectErrors.Add("AddresViewModelが存在しません");
            }
        }
    }
}
