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
        /// <summary>
        /// ツールバータイトル
        /// </summary>
        [DataMember]
        public string Title { get; set; }
        /// <summary>
        /// フッター
        /// </summary>
        [DataMember]
        public string Fotter { get; set; }
        /// <summary>
        /// プログレス表示中なら真
        /// </summary>
        [DataMember]
        public bool ShowProgress { get; set; }
        /// <summary>
        /// アプリケーション復帰中の文言
        /// </summary>
        [DataMember]
        public string InitialText { get; set; }
    }
}
