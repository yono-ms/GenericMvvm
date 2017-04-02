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
    class NativeCallAndroid : INativeCall
    {
        private Activity _Activity;

        private SemaphoreSlim _Sem = new SemaphoreSlim(1, 1);
        private const string KEY = "AppPreference";

        public NativeCallAndroid(Activity activity)
        {
            _Activity = activity;
        }

        public void DismissProgress()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// �s�����̈悩����o��
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
            }
            finally
            {
                _Sem.Release();
            }
            return result;
        }

        public void NavigateTo(string page, bool forward)
        {
            throw new NotImplementedException();
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
        /// �s�����̈�ɕۑ�����
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
                var dialog = new AlertDialog.Builder(_Activity);
                dialog.SetTitle(title);
                dialog.SetMessage(message);
                dialog.SetPositiveButton(labelYes, (s, e) => { tcs.SetResult(true); });
                if (!string.IsNullOrEmpty(labelNo))
                {
                    dialog.SetNegativeButton(labelNo, (s, e) => { tcs.SetResult(false); });
                }
                dialog.Show();
            });

            var result = await tcs.Task;
            return result;
        }

        public void ShowProgress()
        {
            throw new NotImplementedException();
        }
    }
}