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
        /// �e��ʂ̓A�^�b�`�C�x���g�ŊǗ�����
        /// </summary>
        MainActivity _MainActivity;
        /// <summary>
        /// VM�̓t�H�A�O���E���h���A�Ŏ擾���Ȃ���
        /// </summary>
        AddressViewModel _VM;
        /// <summary>
        /// �o�C���h���
        /// </summary>
        TextInputViewBind _TextInputViewBind;
        /// <summary>
        /// �����ł͉������Ȃ�
        /// </summary>
        /// <param name="savedInstanceState"></param>
        public override void OnCreate(Bundle savedInstanceState)
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnCreate(savedInstanceState);
        }
        /// <summary>
        /// ���C�A�E�g����r���[���\�z����
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
        /// �A�N�e�B�r�e�B��ۑ�����
        /// </summary>
        /// <param name="context"></param>
        public override void OnAttach(Context context)
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnAttach(context);

            _MainActivity = context as MainActivity;
        }
        /// <summary>
        /// �A�N�e�B�r�e�B���N���A����
        /// </summary>
        public override void OnDetach()
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnDetach();

            _MainActivity = null;
        }
        /// <summary>
        /// �t�H�A�O���E���h���A
        /// </summary>
        public override void OnResume()
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnResume();

            // �r���[���f������
            _VM = _MainActivity.BizLogic.GetViewModel<AddressViewModel>();

            // �J�X�^���R���g���[���o�C���h���
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

            // VM�C�x���g
            _VM.PropertyChanged += _VM_PropertyChanged;

            // �R���g���[���C�x���g
            View.FindViewById<Button>(Resource.Id.buttonCommit).Click += AddressFragment_Click;
            View.FindViewById<Button>(Resource.Id.buttonGet).Click += AddressFragment_Click;
            View.FindViewById<Button>(Resource.Id.buttonCopy).Click += AddressFragment_Click;

            // �����l�ݒ� OneWay
            View.FindViewById<TitleTextView>(Resource.Id.titleTextViewDescription).Text = _VM.Description;
            View.FindViewById<Button>(Resource.Id.buttonCommit).Text = _VM.CommitLabel;
            View.FindViewById<Button>(Resource.Id.buttonCommit).Enabled = _VM.CanCommit;
            View.FindViewById<Button>(Resource.Id.buttonGet).Text = _VM.CommanGetLabel;
            View.FindViewById<Button>(Resource.Id.buttonGet).Enabled = _VM.CanCommandGet;
            View.FindViewById<Button>(Resource.Id.buttonCopy).Text = _VM.CommanCopyLabel;
            View.FindViewById<Button>(Resource.Id.buttonCopy).Enabled = _VM.CanCommandCopy;

            // �����l�ݒ� TwoWay
            _TextInputViewBind.Start();

            // �����l�ݒ� Recycler
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
        /// �o�b�N�O���E���h�ڍs
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
        /// �����ł͉������Ȃ�
        /// </summary>
        public override void OnDestroy()
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnDestroy();
        }
        /// <summary>
        /// �{�^���C�x���g
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
        /// ���T�C�N���[�C�x���g
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Adapter_ItemClick(object sender, AddressAdapterClickEventArgs e)
        {
            _VM.SelectedIndex = e.Position;
        }

        /// <summary>
        /// VM����̏�ԕω��ʒm
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
                        // �J�X�^���R���g���[���̏�ԕω�
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
                        // ����
                        adapter.NotifyItemRangeInserted(e.NewStartingIndex, e.NewItems.Count);
                    }
                    else
                    {
                        // �P��
                        adapter.NotifyItemInserted(e.NewStartingIndex);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems.Count > 1)
                    {
                        // ����
                        adapter.NotifyItemRangeRemoved(e.OldStartingIndex, e.OldItems.Count);
                    }
                    else
                    {
                        // �P��
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