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

namespace GenericMvvm.Droid
{
    public class BirthFragment : Android.Support.V4.App.Fragment
    {
        const string FORMAT = "----BirthFragment Event---- {0}";

        /// <summary>
        /// �e��ʂ̓A�^�b�`�C�x���g�ŊǗ�����
        /// </summary>
        MainActivity _MainActivity;
        /// <summary>
        /// VM�̓t�H�A�O���E���h���A�Ŏ擾���Ȃ���
        /// </summary>
        BirthViewModel _VM;
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

            return inflater.Inflate(Resource.Layout.Birth, container, false);
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
        /// ���̉�ʂ̐���
        /// </summary>
        public override void OnResume()
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnResume();

            // �r���[���f������
            _VM = _MainActivity.BizLogic.GetViewModel<BirthViewModel>();

            // �J�X�^���R���g���[���o�C���h���
            _TextInputViewBind = new TextInputViewBind(View, _VM);
            _TextInputViewBind.Add(new TextInputViewBind.Info
            {
                PropName = nameof(_VM.Year),
                PropType = TextInputViewBind.ConverterType.INT,
                ResId = Resource.Id.textInputViewYear,
                InputType = Android.Text.InputTypes.ClassNumber,
                Hint = _VM.YearTitle
            });
            _TextInputViewBind.Add(new TextInputViewBind.Info
            {
                PropName = nameof(_VM.Month),
                PropType = TextInputViewBind.ConverterType.INT,
                ResId = Resource.Id.textInputViewMonth,
                InputType = Android.Text.InputTypes.ClassNumber,
                Hint = _VM.MonthTitle
            });
            _TextInputViewBind.Add(new TextInputViewBind.Info
            {
                PropName=nameof(_VM.Day),
                PropType = TextInputViewBind.ConverterType.INT,
                ResId = Resource.Id.textInputViewDay,
                InputType = Android.Text.InputTypes.ClassNumber,
                ImeOption = Android.Views.InputMethods.ImeAction.Done,
                Hint = _VM.DayTitle
            });

            // VM�C�x���g
            _VM.PropertyChanged += _VM_PropertyChanged;

            // �R���g���[���C�x���g
            View.FindViewById<Button>(Resource.Id.buttonCommit).Click += BirthFragment_Click;

            // �C�x���g���o�C���h������ɃR���g���[���ɏ����ݒ肷��ƃG���[��Ԃ��m�肷��
            // �����l�ݒ� OneWay
            View.FindViewById<TitleTextView>(Resource.Id.titleTextViewDescription).Text = _VM.Description;
            View.FindViewById<Button>(Resource.Id.buttonCommit).Text = _VM.CommitLabel;
            View.FindViewById<Button>(Resource.Id.buttonCommit).Enabled = _VM.CanCommit;

            // �����l�ݒ� TwoWay
            _TextInputViewBind.Start();
        }
        /// <summary>
        /// �o�b�N�O���E���h�ڍs
        /// ���̉�ʂ̔p��
        /// </summary>
        public override void OnPause()
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnPause();

            // �����Ńo�C���h��������
            View.FindViewById<Button>(Resource.Id.buttonCommit).Click -= BirthFragment_Click;

            _TextInputViewBind.Stop();

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
        /// ���̉�ʂ̓{�^����1��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BirthFragment_Click(object sender, EventArgs e)
        {
            _VM.Commit();
        }
        /// <summary>
        /// VM����̏�ԕω��ʒm
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _VM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("-- BirthFragment {0} {1}", new[] { MethodBase.GetCurrentMethod().Name, e.PropertyName });
            switch (e.PropertyName)
            {
                case nameof(_VM.CanCommit):
                    // �{�^��������
                    View.FindViewById<Button>(Resource.Id.buttonCommit).Enabled = _VM.CanCommit;
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
    }
}