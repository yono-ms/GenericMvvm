using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Support.V4.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System.Reflection;
using Android.Support.V7.Widget;

namespace GenericMvvm.Droid
{
    public class AddressFragment : Fragment
    {
        const string FORMAT = "----AddressFragment Event---- {0}";

        /// <summary>
        /// 親画面はアタッチイベントで管理する
        /// </summary>
        MainActivity _MainActivity;
        /// <summary>
        /// VMはフォアグラウンド復帰で取得しなおす
        /// </summary>
        AddressViewModel _VM;
        /// <summary>
        /// VMのプロパティ名からコントロールのリソースIDを得る辞書
        /// </summary>
        Dictionary<string, int> _ResouceIds = new Dictionary<string, int>();

        public override void OnCreate(Bundle savedInstanceState)
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });

            return inflater.Inflate(Resource.Layout.Address, container, false);
        }

        public override void OnAttach(Context context)
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnAttach(context);

            _MainActivity = context as MainActivity;
        }

        public override void OnDetach()
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnDetach();

            _MainActivity = null;
        }

        public override void OnResume()
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnResume();

            // ビューモデル生成
            _VM = _MainActivity.BizLogic.GetViewModel<AddressViewModel>();

            // 辞書作成
            _ResouceIds.Add(nameof(_VM.PostalCode), Resource.Id.textInputViewPostalCode);
            _ResouceIds.Add(nameof(_VM.Address), Resource.Id.textInputViewAddress);
            _ResouceIds.Add(nameof(_VM.AddressKana), Resource.Id.textInputViewAddressKana);

            // VMイベント
            _VM.PropertyChanged += _VM_PropertyChanged;

            // コントロールイベント
            View.FindViewById<TextInputView>(Resource.Id.textInputViewPostalCode).TextChanged += AddressFragment_TextChanged;
            View.FindViewById<TextInputView>(Resource.Id.textInputViewAddress).TextChanged += AddressFragment_TextChanged;
            View.FindViewById<TextInputView>(Resource.Id.textInputViewAddressKana).TextChanged += AddressFragment_TextChanged;
            View.FindViewById<Button>(Resource.Id.buttonCommit).Click += AddressFragment_Click;
            View.FindViewById<Button>(Resource.Id.buttonGet).Click += AddressFragment_Click;
            View.FindViewById<Button>(Resource.Id.buttonCopy).Click += AddressFragment_Click;

            // 初期値設定 OneWay
            View.FindViewById<TitleTextView>(Resource.Id.titleTextViewDescription).Text = _VM.Description;
            View.FindViewById<Button>(Resource.Id.buttonCommit).Text = _VM.CommitLabel;
            View.FindViewById<Button>(Resource.Id.buttonCommit).Enabled = _VM.CanCommit;
            View.FindViewById<Button>(Resource.Id.buttonGet).Text = _VM.CommanGetLabel;
            View.FindViewById<Button>(Resource.Id.buttonGet).Enabled = _VM.CanCommandGet;
            View.FindViewById<Button>(Resource.Id.buttonCopy).Text = _VM.CommanCopyLabel;
            View.FindViewById<Button>(Resource.Id.buttonCopy).Enabled = _VM.CanCommandCopy;
            // 初期値設定 EditText
            View.FindViewById<TextInputView>(Resource.Id.textInputViewPostalCode).Text = _VM.PostalCode;
            View.FindViewById<TextInputView>(Resource.Id.textInputViewPostalCode).Hint = _VM.PostalCodeTitle;
            View.FindViewById<TextInputView>(Resource.Id.textInputViewPostalCode).InputType = Android.Text.InputTypes.ClassNumber;
            View.FindViewById<TextInputView>(Resource.Id.textInputViewAddress).Text = _VM.Address;
            View.FindViewById<TextInputView>(Resource.Id.textInputViewAddress).Hint = _VM.AddressTitle;
            View.FindViewById<TextInputView>(Resource.Id.textInputViewAddress).InputType = Android.Text.InputTypes.ClassText;
            View.FindViewById<TextInputView>(Resource.Id.textInputViewAddressKana).Text = _VM.AddressKana;
            View.FindViewById<TextInputView>(Resource.Id.textInputViewAddressKana).Hint = _VM.AddressKanaTitle;
            View.FindViewById<TextInputView>(Resource.Id.textInputViewAddressKana).InputType = Android.Text.InputTypes.ClassText;
            // 初期値設定 Recycler
            if (string.IsNullOrEmpty(_VM.ResponseResultHeader))
            {
                View.FindViewById<TextView>(Resource.Id.textViewHeader).Visibility = ViewStates.Gone;
            }
            else
            {
                View.FindViewById<TextView>(Resource.Id.textViewHeader).Text = _VM.ResponseResultHeader;
                View.FindViewById<TextView>(Resource.Id.textViewHeader).Visibility = ViewStates.Visible;
            }
            var recyclerView = View.FindViewById<RecyclerView>(Resource.Id.recyclerView);
            recyclerView.SetLayoutManager(new LinearLayoutManager(Context));
            var adapter = new AddressAdapter(_VM.ResponseResults);
            adapter.ItemClick += Adapter_ItemClick;
            recyclerView.SetAdapter(adapter);
        }

        public override void OnPause()
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnPause();

            View.FindViewById<TextInputView>(Resource.Id.textInputViewPostalCode).TextChanged -= AddressFragment_TextChanged;
            View.FindViewById<TextInputView>(Resource.Id.textInputViewAddress).TextChanged -= AddressFragment_TextChanged;
            View.FindViewById<TextInputView>(Resource.Id.textInputViewAddressKana).TextChanged -= AddressFragment_TextChanged;
            View.FindViewById<Button>(Resource.Id.buttonCommit).Click -= AddressFragment_Click;
            View.FindViewById<Button>(Resource.Id.buttonGet).Click -= AddressFragment_Click;
            View.FindViewById<Button>(Resource.Id.buttonCopy).Click -= AddressFragment_Click;

            var recyclerView = View.FindViewById<RecyclerView>(Resource.Id.recyclerView);
            (recyclerView.GetAdapter() as AddressAdapter).ItemClick -= Adapter_ItemClick;

            _VM.PropertyChanged -= _VM_PropertyChanged;

            _ResouceIds.Clear();

            _VM = null;
        }

        public override void OnDestroy()
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnDestroy();
        }

        private void AddressFragment_Click(object sender, EventArgs e)
        {
            var view = sender as Button;
            switch (view.Id)
            {
                case Resource.Id.buttonCommit:
                    _VM.Commit();
                    break;

                case Resource.Id.buttonGet:
                    _VM.CommandGet();
                    break;

                case Resource.Id.buttonCopy:
                    _VM.CommandCopy();
                    break;

                default:
                    System.Diagnostics.Debug.WriteLine("unknown CTRL EVENT " + view.Id);
                    break;
            }
        }

        private void Adapter_ItemClick(object sender, AddressAdapterClickEventArgs e)
        {
            _VM.SelectedIndex = e.Position;
        }

        private void AddressFragment_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            var view = sender as TextInputView;
            switch (view.Id)
            {
                case Resource.Id.textInputViewPostalCode:
                    _VM.PostalCode = View.FindViewById<TextInputView>(Resource.Id.textInputViewPostalCode).Text;
                    break;

                case Resource.Id.textInputViewAddress:
                    _VM.Address = View.FindViewById<TextInputView>(Resource.Id.textInputViewAddress).Text;
                    break;

                case Resource.Id.textInputViewAddressKana:
                    _VM.AddressKana = View.FindViewById<TextInputView>(Resource.Id.textInputViewAddressKana).Text;
                    break;

                default:
                    System.Diagnostics.Debug.WriteLine("unknown CTRL EVENT " + view.Id);
                    break;
            }
        }

        private void _VM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(_VM.CanCommit):
                    View.FindViewById<Button>(Resource.Id.buttonCommit).Enabled = _VM.CanCommit;
                    break;

                case nameof(_VM.CanCommandGet):
                    View.FindViewById<Button>(Resource.Id.buttonGet).Enabled = _VM.CanCommandGet;
                    break;

                case nameof(_VM.CanCommandCopy):
                    View.FindViewById<Button>(Resource.Id.buttonCopy).Enabled = _VM.CanCommandCopy;
                    break;

                case nameof(_VM.Errors):
                    View.FindViewById<TextInputView>(Resource.Id.textInputViewPostalCode).Errors = _VM.Errors?[nameof(_VM.PostalCode)];
                    View.FindViewById<TextInputView>(Resource.Id.textInputViewAddress).Errors = _VM.Errors?[nameof(_VM.Address)];
                    View.FindViewById<TextInputView>(Resource.Id.textInputViewAddressKana).Errors = _VM.Errors?[nameof(_VM.AddressKana)];
                    break;

                case nameof(_VM.IsError):
                    View.FindViewById<TextInputView>(Resource.Id.textInputViewPostalCode).IsError = _VM.IsError[nameof(_VM.PostalCode)];
                    View.FindViewById<TextInputView>(Resource.Id.textInputViewAddress).IsError = _VM.IsError[nameof(_VM.Address)];
                    View.FindViewById<TextInputView>(Resource.Id.textInputViewAddressKana).IsError = _VM.IsError[nameof(_VM.AddressKana)];
                    break;

                case nameof(_VM.ResponseResultHeader):
                    if (string.IsNullOrEmpty(_VM.ResponseResultHeader))
                    {
                        View.FindViewById<TextView>(Resource.Id.textViewHeader).Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        View.FindViewById<TextView>(Resource.Id.textViewHeader).Text = _VM.ResponseResultHeader;
                        View.FindViewById<TextView>(Resource.Id.textViewHeader).Visibility = ViewStates.Visible;
                    }
                    break;

                case nameof(_VM.ResponseResults):
                    var recyclerView = View.FindViewById<RecyclerView>(Resource.Id.recyclerView);
                    (recyclerView.GetAdapter() as AddressAdapter).ItemClick -= Adapter_ItemClick;
                    var adapter = new AddressAdapter(_VM.ResponseResults);
                    adapter.ItemClick += Adapter_ItemClick;
                    recyclerView.SetAdapter(adapter);
                    break;

                default:
                    if (_ResouceIds.ContainsKey(e.PropertyName))
                    {
                        System.Diagnostics.Debug.WriteLine("-- PropertyChanged {0} {1}", new[] { MethodBase.GetCurrentMethod().Name, e.PropertyName });
                        var v = sender.GetType().GetProperty(e.PropertyName).GetValue(sender).ToString();
                        var c = View.FindViewById<TextInputView>(_ResouceIds[e.PropertyName]);
                        if (!v.Equals(c.Text))
                        {
                            c.Text = v;
                            System.Diagnostics.Debug.WriteLine("{0}={1}", new[] { e.PropertyName, v});
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("unknown VM EVENT " + e.PropertyName);
                    }
                    break;
            }
        }
    }
}