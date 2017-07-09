using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Threading;

namespace GenericMvvm.Droid
{
    /// <summary>
    /// AndroidのOS依存部分
    /// この実装ではMainActivityの存在が前提となっている
    /// </summary>
    class NativeCallAndroid : INativeCall
    {
        private MainActivity _Activity;

        private SemaphoreSlim _Sem = new SemaphoreSlim(1, 1);
        private const string KEY = "AppPreference";

        private Dictionary<string, Type> _StringToPages;
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="activity"></param>
        public NativeCallAndroid(MainActivity activity)
        {
            _Activity = activity;
            _StringToPages = new Dictionary<string, Type>();
            _StringToPages.Add("Name", typeof(NameFragment));
            _StringToPages.Add("Birth", typeof(BirthFragment));
            _StringToPages.Add("Address", typeof(AddressFragment));
            _StringToPages.Add("Confirm", typeof(ConfirmFragment));
            _StringToPages.Add("Finish", typeof(FinishFragment));
        }

        /// <summary>
        /// 不揮発領域から取り出す
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<string> LoadFileAsync(string name)
        {
            string result = null;

            await _Sem.WaitAsync();
            try
            {
                result = await Task.Run(() =>
                {
                    var sp = _Activity.GetSharedPreferences(KEY, FileCreationMode.Private);
                    return sp.GetString(name, null);

                });

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                Toast.MakeText(_Activity, ex.Message, ToastLength.Long).Show();
            }
            finally
            {
                _Sem.Release();
            }
            return result;
        }
        /// <summary>
        /// 画面遷移
        /// </summary>
        /// <param name="page"></param>
        /// <param name="forward"></param>
        public void NavigateTo(string page, bool forward)
        {
            if (_StringToPages.ContainsKey(page))
            {
                _Activity.NavigateTo(_StringToPages[page], forward);
            }
            else
            {
                var message = string.Format("ページ {0} が存在しません。", page);
                System.Diagnostics.Debug.WriteLine(message);
                Toast.MakeText(_Activity, message, ToastLength.Long).Show();
            }
        }

        public void Pop()
        {
            throw new NotImplementedException();
        }

        public void Push(string page)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 不揮発領域に保存する
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task SaveFileAsync(string name, string data)
        {
            await _Sem.WaitAsync();
            try
            {
                await Task.Run(() =>
                {
                    var sp = _Activity.GetSharedPreferences(KEY, FileCreationMode.Private);
                    var editor = sp.Edit();
                    editor.PutString(name, data);
                    editor.Apply();
                });

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                Toast.MakeText(_Activity, ex.Message, ToastLength.Long).Show();
            }
            finally
            {
                _Sem.Release();
            }
        }

        public async Task<bool> ShowAlertAsync(string title, string message, string labelYes, string labelNo)
        {
            var tcs = new TaskCompletionSource<bool>();

            _Activity.RunOnUiThread(() =>
            {
                var fragment = CustomDialogFragment.NewInstance(title, message, labelYes, labelNo);
                _Activity.DialogClick += (s, e) =>
                {
                    if (e.Label.Equals(labelYes))
                    {
                        tcs.TrySetResult(true);
                    }
                    else
                    {
                        tcs.TrySetResult(false);
                    }
                };
                fragment.Show(_Activity.SupportFragmentManager, "dialog");
            });

            var result = await tcs.Task;
            return result;
        }
        /// <summary>
        /// UIスレッドで実行する
        /// </summary>
        /// <param name="callback"></param>
        public void RunUIThread(Action callback)
        {
            _Activity.RunOnUiThread(callback);
        }
        /// <summary>
        /// アプリを終了する
        /// </summary>
        public void ExitApplication()
        {
            _Activity.MoveTaskToBack(true);
        }
    }
}