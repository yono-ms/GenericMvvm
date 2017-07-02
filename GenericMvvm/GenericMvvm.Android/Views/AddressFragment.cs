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
using System.Collections.Specialized;

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
        /// バインド情報
        /// </summary>
        TextInputViewBind _TextInputViewBind;
        /// <summary>
        /// ここでは何もしない
        /// </summary>
        /// <param name="savedInstanceState"></param>
        public override void OnCreate(Bundle savedInstanceState)
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnCreate(savedInstanceState);
        }
        /// <summary>
        /// レイアウトからビューを構築する
        /// </summary>
        /// <param name="inflater"></param>
        /// <param name="container"></param>
        /// <param name="savedInstanceState"></param>
        /// <returns></returns>
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });

            return inflater.Inflate(Resource.Layout.Address, container, false);
        }
        /// <summary>
        /// アクティビティを保存する
        /// </summary>
        /// <param name="context"></param>
        public override void OnAttach(Context context)
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnAttach(context);

            _MainActivity = context as MainActivity;
        }
        /// <summary>
        /// アクティビティをクリアする
        /// </summary>
        public override void OnDetach()
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnDetach();

            _MainActivity = null;
        }
        /// <summary>
        /// フォアグラウンド復帰
        /// </summary>
        public override void OnResume()
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnResume();

            // ビューモデル生成
            _VM = _MainActivity.BizLogic.GetViewModel<AddressViewModel>();

            // カスタムコントロールバインド情報
            _TextInputViewBind = new TextInputViewBind(View, _VM);
            _TextInputViewBind.Add(new TextInputViewBind.Info
            {
                PropName = nameof(_VM.PostalCode),
                ResId = Resource.Id.textInputViewPostalCode,
                InputType = Android.Text.InputTypes.ClassNumber,
                ImeOption = Android.Views.InputMethods.ImeAction.Done,
                Hint = _VM.PostalCodeTitle
            });
            _TextInputViewBind.Add(new TextInputViewBind.Info
            {
                PropName = nameof(_VM.Address),
                ResId = Resource.Id.textInputViewAddress,
                InputType = Android.Text.InputTypes.ClassText,
                Hint = _VM.AddressTitle
            });
            _TextInputViewBind.Add(new TextInputViewBind.Info
            {
                PropName = nameof(_VM.AddressKana),
                ResId = Resource.Id.textInputViewAddressKana,
                InputType = Android.Text.InputTypes.ClassText,
                Hint = _VM.AddressKanaTitle
            });

            // VMイベント
            _VM.PropertyChanged += _VM_PropertyChanged;

            // コントロールイベント
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

            // 初期値設定 TwoWay
            _TextInputViewBind.Start();

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
        /// <summary>
        /// バックグラウンド移行
        /// </summary>
        public override void OnPause()
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnPause();

            View.FindViewById<Button>(Resource.Id.buttonCommit).Click -= AddressFragment_Click;
            View.FindViewById<Button>(Resource.Id.buttonGet).Click -= AddressFragment_Click;
            View.FindViewById<Button>(Resource.Id.buttonCopy).Click -= AddressFragment_Click;

            _TextInputViewBind.Stop();

            var recyclerView = View.FindViewById<RecyclerView>(Resource.Id.recyclerView);
            (recyclerView.GetAdapter() as AddressAdapter).ItemClick -= Adapter_ItemClick;

            _VM.PropertyChanged -= _VM_PropertyChanged;

            _TextInputViewBind = null;

            _VM = null;
        }
        /// <summary>
        /// ここでは何もしない
        /// </summary>
        public override void OnDestroy()
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnDestroy();
        }
        /// <summary>
        /// ボタンイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <summary>
        /// リサイクラーイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Adapter_ItemClick(object sender, AddressAdapterClickEventArgs e)
        {
            _VM.SelectedIndex = e.Position;
        }

        /// <summary>
        /// VMからの状態変化通知
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                    _VM.ResponseResults.CollectionChanged += ResponseResults_CollectionChanged;
                    break;

                case nameof(_VM.SelectedIndex):
                    (View.FindViewById<RecyclerView>(Resource.Id.recyclerView).GetAdapter() as AddressAdapter).SelectedIndex = _VM.SelectedIndex;
                    break;

                default:
                    if (_TextInputViewBind.ContainsKey(e.PropertyName))
                    {
                        // カスタムコントロールの状態変化
                        _TextInputViewBind.PropertyChanged(sender, e);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("unknown VM EVENT " + e.PropertyName);
                    }
                    break;
            }
        }

        private void ResponseResults_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var recyclerView = View.FindViewById<RecyclerView>(Resource.Id.recyclerView);
            var adapter = recyclerView.GetAdapter() as AddressAdapter;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems.Count > 1)
                    {
                        // 複数
                        adapter.NotifyItemRangeInserted(e.NewStartingIndex, e.NewItems.Count);
                    }
                    else
                    {
                        // 単数
                        adapter.NotifyItemInserted(e.NewStartingIndex);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems.Count > 1)
                    {
                        // 複数
                        adapter.NotifyItemRangeRemoved(e.OldStartingIndex, e.OldItems.Count);
                    }
                    else
                    {
                        // 単数
                        adapter.NotifyItemRemoved(e.OldStartingIndex);
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    adapter.NotifyDataSetChanged();
                    break;

                default:
                    System.Diagnostics.Debug.WriteLine("unknown ACTION " + e.Action);
                    break;
            }
        }
    }
}