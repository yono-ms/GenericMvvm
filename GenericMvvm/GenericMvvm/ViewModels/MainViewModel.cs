using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GenericMvvm
{
    /// <summary>
    /// メイン画面のビューモデル
    /// </summary>
    [DataContract]
    public class MainViewModel : BaseViewModel
    {
        private string _Title;
        /// <summary>
        /// ツールバータイトル
        /// </summary>
        [DataMember]
        public string Title
        {
            get { return _Title; }
            set { _Title = value; ValidateProperty(nameof(Title), value); }
        }

        private string _Footer;

        /// <summary>
        /// フッター
        /// </summary>
        [DataMember]
        public string Footer
        {
            get { return _Footer; }
            set { _Footer = value; ValidateProperty(nameof(Footer), value); }
        }

        private bool _ShowProgress;

        /// <summary>
        /// プログレス表示中なら真
        /// </summary>
        [DataMember]
        public bool ShowProgress
        {
            get { return _ShowProgress; }
            set { _ShowProgress = value; ValidateProperty(nameof(ShowProgress), value); }
        }

        [DataMember]
        private bool _ShowBackButton;
        /// <summary>
        /// バックボタンを表示するなら真
        /// </summary>
        public bool ShowBackButton
        {
            get { return _ShowBackButton; }
            set { _ShowBackButton = value; ValidateProperty(nameof(ShowBackButton), value); }
        }

        /// <summary>
        /// アプリケーション復帰中の文言
        /// </summary>
        public string InitialText { get { return "アプリケーション起動"; } }

        public override void KickStart()
        {
            base.KickStart();
            // 表示位置の復元をリクエスト
            _BizLogic.RestorePage();
        }
    }
}
