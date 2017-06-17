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

        public override void OnCreate(Bundle savedInstanceState)
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });

            return inflater.Inflate(Resource.Layout.Birth, container, false);
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

            // �R���g���[���i�ϐ��͖����Ă������j
            var description = View.FindViewById<TitleTextView>(Resource.Id.titleTextViewDescription);
            var year = View.FindViewById<TextInputView>(Resource.Id.textInputViewYear);
            var month = View.FindViewById<TextInputView>(Resource.Id.textInputViewMonth);
            var day = View.FindViewById<TextInputView>(Resource.Id.textInputViewDay);

            // �r���[���f������
            _VM = _MainActivity.BizLogic.GetViewModel<BirthViewModel>();

            // VM�C�x���g
            _VM.PropertyChanged += _VM_PropertyChanged;

            // �R���g���[���C�x���g
            year.TextChanged += BirthFragment_TextChanged;
            month.TextChanged += BirthFragment_TextChanged;
            day.TextChanged += BirthFragment_TextChanged;
            View.FindViewById<Button>(Resource.Id.buttonCommit).Click += BirthFragment_Click;

            // �C�x���g���o�C���h������ɃR���g���[���ɏ����ݒ肷��ƃG���[��Ԃ��m�肷��
            description.Text = _VM.Description;
            year.Hint = _VM.YearTitle;
            year.Text = _VM.Year.ToString();
            month.Hint = _VM.MonthTitle;
            month.Text = _VM.Month.ToString();
            day.Hint = _VM.DayTitle;
            day.Text = _VM.Day.ToString();
        }

        public override void OnPause()
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnPause();

            // �����Ńo�C���h��������
            View.FindViewById<Button>(Resource.Id.buttonCommit).Click -= BirthFragment_Click;
            View.FindViewById<TextInputView>(Resource.Id.textInputViewYear).TextChanged -= BirthFragment_TextChanged;
            View.FindViewById<TextInputView>(Resource.Id.textInputViewMonth).TextChanged -= BirthFragment_TextChanged;
            View.FindViewById<TextInputView>(Resource.Id.textInputViewDay).TextChanged -= BirthFragment_TextChanged;

            _VM.PropertyChanged -= _VM_PropertyChanged;

            _VM = null;
        }

        public override void OnDestroy()
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnDestroy();
        }

        private void BirthFragment_Click(object sender, EventArgs e)
        {
            _VM.Commit();
        }

        private void BirthFragment_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            // ���l�ϊ�
            int num;
            if (!int.TryParse(e.Text.ToString(), out num))
            {
                System.Diagnostics.Debug.WriteLine("invalid value " + e.Text.ToString());
                num = 0;
            }

            var v = sender as TextInputView;
            switch (v.Id)
            {
                case Resource.Id.textInputViewYear:
                    _VM.Year = num;
                    break;

                case Resource.Id.textInputViewMonth:
                    _VM.Month = num;
                    break;

                case Resource.Id.textInputViewDay:
                    _VM.Day = num;
                    break;

                default:
                    System.Diagnostics.Debug.WriteLine("unknown CTRL EVENT " + v.Id);
                    break;
            }
        }

        private void _VM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("-- PropertyChanged {0} {1}", new[] { MethodBase.GetCurrentMethod().Name, e.PropertyName });
            switch (e.PropertyName)
            {
                case nameof(_VM.CanCommit):
                    View.FindViewById<Button>(Resource.Id.buttonCommit).Enabled = _VM.CanCommit;
                    break;

                case nameof(_VM.Errors):
                    View.FindViewById<TextInputView>(Resource.Id.textInputViewYear).Errors = _VM.Errors?[nameof(_VM.Year)];
                    View.FindViewById<TextInputView>(Resource.Id.textInputViewMonth).Errors = _VM.Errors?[nameof(_VM.Month)];
                    View.FindViewById<TextInputView>(Resource.Id.textInputViewDay).Errors = _VM.Errors?[nameof(_VM.Day)];
                    break;

                case nameof(_VM.IsError):
                    View.FindViewById<TextInputView>(Resource.Id.textInputViewYear).IsError = _VM.IsError[nameof(_VM.Year)];
                    View.FindViewById<TextInputView>(Resource.Id.textInputViewMonth).IsError = _VM.IsError[nameof(_VM.Month)];
                    View.FindViewById<TextInputView>(Resource.Id.textInputViewDay).IsError = _VM.IsError[nameof(_VM.Day)];
                    break;

                case nameof(_VM.Year):
                case nameof(_VM.Month):
                case nameof(_VM.Day):
                    var v = sender.GetType().GetProperty(e.PropertyName).GetValue(sender).ToString();
                    if (v.Equals("0"))
                    {
                        v = "";
                    }
                    TextInputView c = null;
                    switch (e.PropertyName)
                    {
                        case nameof(_VM.Year):
                            c = View.FindViewById<TextInputView>(Resource.Id.textInputViewYear);
                            break;
                        case nameof(_VM.Month):
                            c = View.FindViewById<TextInputView>(Resource.Id.textInputViewMonth);
                            break;
                        case nameof(_VM.Day):
                            c = View.FindViewById<TextInputView>(Resource.Id.textInputViewDay);
                            break;
                    }
                    if (!c.Text.Equals(v))
                    {
                        c.Text = v;
                    }
                    break;

                default:
                    System.Diagnostics.Debug.WriteLine("unknown VM EVENT " + e.PropertyName);
                    break;
            }

        }
    }
}