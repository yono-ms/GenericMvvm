using Android.Content;
using Android.OS;
using Android.Views;
using System.Reflection;
using Android.Support.V4.App;
using Android.Widget;
using Android.Support.Design.Widget;
using System.Collections.Generic;

namespace GenericMvvm.Droid
{
    /// <summary>
    /// ������\���ł��Ȃ��C���X�^���X�ϐ��͎����Ȃ�
    /// �e�v���p�e�B�͎g���Ƃ���OS���ۏ؂��郉�C�t�T�C�N��������o��
    /// </summary>
    public class NameFragment : Fragment
    {
        const string FORMAT = "----NameFragment Event---- {0}";

        /// <summary>
        /// �e��ʂ̓A�^�b�`�C�x���g�ŊǗ�����
        /// </summary>
        MainActivity _MainActivity;
        /// <summary>
        /// VM�̓t�H�A�O���E���h���A�Ŏ擾���Ȃ���
        /// </summary>
        NameViewModel _VM;
        /// <summary>
        /// VM�Ɉˑ����邽�߃o�C���h�����t�H�A�O���E���h���A�Ő������Ȃ���
        /// </summary>
        TextInputViewBind _TextInputViewBind;

        public override void OnCreate(Bundle savedInstanceState)
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            return inflater.Inflate(Resource.Layout.Name, container, false);

            // �e�[�}�ɐݒ肵���g�����W�b�V�����A�j���[�V�����������Ȃ��̂ł����ŃZ�b�g���Ă݂邪����
            //var wrapper = new ContextThemeWrapper(Activity, Resource.Style.AppTheme);
            //var li = inflater.CloneInContext(wrapper);
            //return li.Inflate(Resource.Layout.Name, container, false);
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

        /// <summary>
        /// �t�H�A�O���E���h���A
        /// </summary>
        public override void OnResume()
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnResume();

            // �r���[���f������
            _VM = _MainActivity.BizLogic.GetViewModel<NameViewModel>();

            // �J�X�^���R���g���[���o�C���h���
            _TextInputViewBind = new TextInputViewBind(View, _VM);
            _TextInputViewBind.Add(nameof(_VM.LastName), Resource.Id.textInputViewLastName, _VM.LastNameTitle);
            _TextInputViewBind.Add(nameof(_VM.FirstName), Resource.Id.textInputViewFirstName, _VM.FirstNameTitle);

            // VM�C�x���g
            _VM.PropertyChanged += _VM_PropertyChanged;

            // �R���g���[���C�x���g
            View.FindViewById<Button>(Resource.Id.buttonCommit).Click += NameFragment_Click;

            // �Œ�l�ݒ�
            View.FindViewById<TitleTextView>(Resource.Id.titleTextViewDescription).Text = _VM.Description;

            // TwoWay�����l�ݒ�
            _TextInputViewBind.Start();
        }

        /// <summary>
        /// �o�b�N�O���E���h�ڍs
        /// </summary>
        public override void OnPause()
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnPause();

            // �����Ńo�C���h��������
            View.FindViewById<Button>(Resource.Id.buttonCommit).Click += NameFragment_Click;

            _TextInputViewBind.Stop();

            _VM.PropertyChanged -= _VM_PropertyChanged;

            _TextInputViewBind = null;

            _VM = null;
        }

        public override void OnDestroy()
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnDestroy();
        }

        private void NameFragment_Click(object sender, System.EventArgs e)
        {
            var v = sender as Button;
            switch (v.Id)
            {
                case Resource.Id.buttonCommit:
                    _VM.Commit();
                    break;

                default:
                    System.Diagnostics.Debug.WriteLine("unknown CTRL EVENT " + v.Id);
                    break;
            }
        }

        /// <summary>
        /// VM����̃C�x���g�͏z���Ȃ��悤�ɓ����l��ݒ肵�Ȃ�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _VM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("-- PropertyChanged {0} {1}", new[] { MethodBase.GetCurrentMethod().Name, e.PropertyName });
            switch (e.PropertyName)
            {
                case nameof(_VM.CanCommit):
                    View.FindViewById<Button>(Resource.Id.buttonCommit).Enabled = _VM.CanCommit;
                    break;

                default:
                    if (_TextInputViewBind.ContainsKey(e.PropertyName))
                    {
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