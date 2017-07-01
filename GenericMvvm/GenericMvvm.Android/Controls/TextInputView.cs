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
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Support.V4.Graphics.Drawable;
using Android.Support.V4.Content;

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
        /// �t���[�e�B���O���x���̏ꍇ�̓^�C�g���������͎��̃v���[�X�z���_�[�ɂȂ��Ă���
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
            set
            {
                // �t���[�e�B���O���x���������̓G���[���Ɍ듮�삷��Ή�
                // �K�{�o���f�[�V�����̔���
                if (value != null && value.Count() > 0 && string.IsNullOrEmpty(Text))
                {
                    // �K�{�ŃG���[�̏ꍇ
                    FindViewById<TextInputLayout>(Resource.Id.textInputLayout).Error = null;
                }
                else
                {
                    FindViewById<TextInputLayout>(Resource.Id.textInputLayout).Error = (value == null) ? null : string.Join("\n", value);
                }
            }
        }
        /// <summary>
        /// �G���[�������͐^
        /// </summary>
        public bool IsError
        {
            set
            {
                // �t���[�e�B���O���x���������̓G���[���Ɍ듮�삷��Ή�
                if (value)
                {
                    // �G���[
                    if (string.IsNullOrEmpty(Text))
                    {
                        // �K�{�G���[
                        FindViewById<TextInputLayout>(Resource.Id.textInputLayout).ErrorEnabled = false;
                        FindViewById<ImageView>(Resource.Id.imageViewRequired).SetImageDrawable(GetRequiredDrawable(Resource.Color.colorRequiredError));
                    }
                    else
                    {
                        // ���̑��̃G���[
                        FindViewById<TextInputLayout>(Resource.Id.textInputLayout).ErrorEnabled = value;
                        FindViewById<ImageView>(Resource.Id.imageViewRequired).SetImageDrawable(GetRequiredDrawable(Resource.Color.colorRequiredInfo));
                    }

                    FindViewById<ImageView>(Resource.Id.imageViewRequired).Visibility = ViewStates.Visible;
                }
                else
                {
                    // ����
                    FindViewById<TextInputLayout>(Resource.Id.textInputLayout).ErrorEnabled = value;
                    FindViewById<ImageView>(Resource.Id.imageViewRequired).Visibility = ViewStates.Gone;
                }
            }
        }
        /// <summary>
        /// ���F�����K�{�A�C�R���摜�𓾂�
        /// ���ʌ݊��̂��߃T�|�[�g���C�u�����Œ��F����
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private Drawable GetRequiredDrawable(int c)
        {
            var d = DrawableCompat.Wrap(ContextCompat.GetDrawable(Context, Android.Resource.Drawable.IcDialogAlert));
            DrawableCompat.SetTint(d, ContextCompat.GetColor(Context, c));
            DrawableCompat.SetTintMode(d, PorterDuff.Mode.SrcIn);
            return d;
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