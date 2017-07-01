using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Support.Design.Widget;
using Android.Text;
using static Android.Widget.TextView;
using Android.Views.InputMethods;
using System.Reflection;

namespace GenericMvvm.Droid
{
    /// <summary>
    /// �o�C���h�ł���e�L�X�g���̓R���g���[��
    /// �ǉ������v���p�e�B�Ƀo�C���h����
    /// </summary>
    public class TextInputView : LinearLayout
    {
        /// <summary>
        /// �e�L�X�g���̓C�x���g
        /// </summary>
        public event EventHandler<Android.Text.TextChangedEventArgs> TextChanged;
        /// <summary>
        /// IME���^�[���L�[�ݒ�
        /// ���ցE�����Ȃ�
        /// </summary>
        public ImeAction ImeOptions
        {
            get { return FindViewById<TextInputEditText>(Resource.Id.textInputEditText).ImeOptions; }
            set { FindViewById<TextInputEditText>(Resource.Id.textInputEditText).ImeOptions = value; }
        }
        /// <summary>
        /// IME�^�C�v
        /// �����E�������͂Ȃ�
        /// </summary>
        public InputTypes InputType
        {
            get { return FindViewById<TextInputEditText>(Resource.Id.textInputEditText).InputType; }
            set { FindViewById<TextInputEditText>(Resource.Id.textInputEditText).InputType = value; }
        }
        /// <summary>
        /// ���͂���������
        /// </summary>
        public string Text
        {
            get { return FindViewById<TextInputEditText>(Resource.Id.textInputEditText).Text; }
            set { FindViewById<TextInputEditText>(Resource.Id.textInputEditText).Text = value; }
        }
        /// <summary>
        /// �v���[�X�z���_�[���^�C�g��
        /// �}�e���A���̏ꍇ�̓^�C�g���������͎��̃v���[�X�z���_�[�ɂȂ��Ă���
        /// </summary>
        public string Hint
        {
            get { return FindViewById<TextInputLayout>(Resource.Id.textInputLayout).Hint; }
            set { FindViewById<TextInputLayout>(Resource.Id.textInputLayout).Hint = value; }
        }
        /// <summary>
        /// �G���[������̔z��
        /// </summary>
        public IEnumerable<string> Errors
        {
            set { FindViewById<TextInputLayout>(Resource.Id.textInputLayout).Error = (value==null) ? null : string.Join("\n", value); }
        }
        /// <summary>
        /// �G���[�������͐^
        /// </summary>
        public bool IsError
        {
            set { FindViewById<TextInputLayout>(Resource.Id.textInputLayout).ErrorEnabled = value; }
        }
        /// <summary>
        /// �R���X�g���N�^�Q
        /// </summary>
        /// <param name="context"></param>
        /// <param name="attrs"></param>
        public TextInputView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize();
        }
        /// <summary>
        /// �R���X�g���N�^�R
        /// </summary>
        /// <param name="context"></param>
        /// <param name="attrs"></param>
        /// <param name="defStyle"></param>
        public TextInputView(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize();
        }
        /// <summary>
        /// �R���X�g���N�^����
        /// </summary>
        private void Initialize()
        {
            Inflate(Context, Resource.Layout.TextInputView, this);

            var textInputEditText = FindViewById<TextInputEditText>(Resource.Id.textInputEditText);

            textInputEditText.TextChanged += TextInputView_TextChanged;
            textInputEditText.SetOnEditorActionListener(new OnEditorActionListener());
        }
        /// <summary>
        /// �e�L�X�g���̓C�x���g
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextInputView_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            TextChanged?.Invoke(this, e);
        }
        /// <summary>
        /// IME�C�x���g
        /// </summary>
        public class OnEditorActionListener : Java.Lang.Object, IOnEditorActionListener
        {
            public bool OnEditorAction(TextView v, [GeneratedEnum] ImeAction actionId, KeyEvent e)
            {
                System.Diagnostics.Debug.WriteLine("{0} ACTION={1} EVENT={2}", new[]
                {
                    MethodBase.GetCurrentMethod().Name,
                    actionId.ToString(),
                    e?.ToString()
                });

                if (actionId == ImeAction.Done)
                {
                    // �I���̃A�N�V������IME������
                    var imm = v.Context.GetSystemService(Context.InputMethodService) as InputMethodManager;
                    imm.HideSoftInputFromWindow(v.WindowToken, HideSoftInputFlags.None);

                    // �����ŃN���A����Ɛ擪�Ɉړ�����炵��
                    v.ClearFocus();

                    // �C�x���g������̏ꍇ�͐^��Ԃ�
                    return true;
                }

                // �����ł͉��������ɃV�X�e���ɓn���Ƃ��͋U��Ԃ�
                return false;
            }
        }
    }
}