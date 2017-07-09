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
    /// Android��OS�ˑ�����
    /// ���̎����ł�MainActivity�̑��݂��O��ƂȂ��Ă���
    /// </summary>
    class NativeCallAndroid : INativeCall
    {
        private MainActivity _Activity;

        private SemaphoreSlim _Sem = new SemaphoreSlim(1, 1);
        private const string KEY = "AppPreference";

        private Dictionary<string, Type> _StringToPages;
        /// <summary>
        /// �R���X�g���N�^
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
                Toast.MakeText(_Activity, ex.Message, ToastLength.Long).Show();
            }
            finally
            {
                _Sem.Release();
            }
            return result;
        }
        /// <summary>
        /// ��ʑJ��
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
                var message = string.Format("�y�[�W {0} �����݂��܂���B", page);
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
        /// UI�X���b�h�Ŏ��s����
        /// </summary>
        /// <param name="callback"></param>
        public void RunUIThread(Action callback)
        {
            _Activity.RunOnUiThread(callback);
        }
        /// <summary>
        /// �A�v�����I������
        /// </summary>
        public void ExitApplication()
        {
            _Activity.MoveTaskToBack(true);
        }
    }
}