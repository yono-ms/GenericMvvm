using Android.Content;
using Android.OS;
using Android.Views;
using System.Reflection;
using Android.Support.V4.App;
using Android.Widget;
using Android.Support.Design.Widget;

namespace GenericMvvm.Droid
{
    public class NameFragment : Fragment
    {
        const string FORMAT = "----NameFragmentEvent---- {0}";

        MainActivity _MainActivity;
        NameViewModel _VM;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            return inflater.Inflate(Resource.Layout.Name, container, false);

            // �e�[�}�ɐݒ肵���g�����W�b�V�����A�j���[�V�����������Ȃ��̂ł����ŃZ�b�g���Ă݂邪����
            //var wrapper = new ContextThemeWrapper(Activity, Resource.Style.AppTheme);
            //var li = inflater.CloneInContext(wrapper);
            //return li.Inflate(Resource.Layout.Name, container, false);
        }

        public override void OnAttach(Context context)
        {
            base.OnAttach(context);
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });

            _MainActivity = context as MainActivity;
        }

        public override void OnDetach()
        {
            base.OnDetach();
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });

            _MainActivity = null;
        }

        public override void OnResume()
        {
            base.OnResume();
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });

            // �����Ńo�C���h����
            _VM = _MainActivity.BizLogic.GetViewModel<NameViewModel>();

            // �����l�ݒ�
            View.FindViewById<TextView>(Resource.Id.textViewDescription).Text = _VM.Description;
            View.FindViewById<TextInputLayout>(Resource.Id.textInputLayout1).Hint = _VM.LastNameTitle;
            View.FindViewById<TextInputEditText>(Resource.Id.textInputEditText1).Text = _VM.LastName;

            // VM�C�x���g
            _VM.PropertyChanged += _VM_PropertyChanged;

            // �R���g���[���C�x���g
            View.FindViewById<TextInputEditText>(Resource.Id.textInputEditText1).TextChanged += NameFragment_TextChanged;
        }
        /// <summary>
        /// �R���g���[������̃C�x���g
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NameFragment_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            var v = sender as TextInputEditText;
            switch (v.Id)
            {
                case Resource.Id.textInputEditText1:
                    _VM.LastName = e.Text.ToString();
                    break;

                default:
                    System.Diagnostics.Debug.WriteLine("unknown CTRL EVENT " + v.Id);
                    break;
            }
        }

        /// <summary>
        /// VM����̃C�x���g
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _VM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(_VM.Errors):
                    System.Diagnostics.Debug.WriteLine("-- PropertyChanged " + e.PropertyName);
                    View.FindViewById<TextInputLayout>(Resource.Id.textInputLayout1).Error = _VM.Errors?[nameof(_VM.LastName)]?[0];
                    break;

                case nameof(_VM.IsError):
                    System.Diagnostics.Debug.WriteLine("-- PropertyChanged " + e.PropertyName);
                    View.FindViewById<TextInputLayout>(Resource.Id.textInputLayout1).ErrorEnabled = _VM.IsError[nameof(_VM.LastName)];
                    break;

                case nameof(_VM.LastName):
                    System.Diagnostics.Debug.WriteLine("-- PropertyChanged " + e.PropertyName);
                    var et = View.FindViewById<TextInputEditText>(Resource.Id.textInputEditText1);
                    if (!et.Text.Equals(_VM.LastName))
                    {
                        et.Text = _VM.LastName;
                    }
                    break;

                default:
                    System.Diagnostics.Debug.WriteLine("unknown VM EVENT " + e.PropertyName);
                    break;
            }
        }

        public override void OnPause()
        {
            base.OnPause();
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });

            // �����Ńo�C���h��������
            _VM.PropertyChanged -= _VM_PropertyChanged;
            _VM = null;

            View.FindViewById<TextInputEditText>(Resource.Id.textInputEditText1).TextChanged -= NameFragment_TextChanged;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
        }
    }
}