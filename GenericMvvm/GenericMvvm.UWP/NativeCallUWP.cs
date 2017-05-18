using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace GenericMvvm.UWP
{
    class NativeCallUWP : INativeCall
    {
        private MainPage _MainPage;

        private SemaphoreSlim _Sem = new SemaphoreSlim(1, 1);

        private Dictionary<string, Type> _StringToPages;

        public NativeCallUWP(MainPage mainPage)
        {
            _MainPage = mainPage;
            _StringToPages = new Dictionary<string, Type>();
            _StringToPages.Add("Name", typeof(NamePage));
            _StringToPages.Add("Birth", typeof(BirthPage));
            _StringToPages.Add("Address", typeof(AddressPage));
            _StringToPages.Add("Confirm", typeof(ConfirmPage));
        }

        public async Task<string> LoadFileAsync(string name)
        {
            string ret = null;

            await _Sem.WaitAsync();
            try
            {
                var folder = ApplicationData.Current.LocalFolder;
                if ((await folder.GetFilesAsync()).Where(e => e.Name == name).Any())
                {
                    var file = await folder.GetFileAsync(name);
                    var text = await FileIO.ReadTextAsync(file);
                    ret = text;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            finally
            {
                _Sem.Release();
            }

            return ret;
        }

        public void NavigateTo(string page, bool forward)
        {
            if (_StringToPages.ContainsKey(page))
            {
                _MainPage.NavigateTo(_StringToPages[page], forward);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("NO PAGE " + page);
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

        public void RunUIThread(Action callback)
        {
            Task.Run(async () =>
            {
                await _MainPage.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    callback();
                });
            });
        }

        public async Task SaveFileAsync(string name, string data)
        {
            await _Sem.WaitAsync();
            try
            {
                var folder = ApplicationData.Current.LocalFolder;
                var file = await folder.CreateFileAsync(name, CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(file, data);

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
            bool result = false;

            var dialog = new ContentDialog();
            dialog.Title = title;
            dialog.Content = message;
            dialog.IsPrimaryButtonEnabled = true;
            dialog.PrimaryButtonText = labelYes;
            dialog.PrimaryButtonClick += (s, e) =>
            {
                result = true;
            };
            if (labelNo != null)
            {
                dialog.IsSecondaryButtonEnabled = true;
                dialog.SecondaryButtonText = labelNo;
            }
            await dialog.ShowAsync();
            return result;
        }

    }
}
