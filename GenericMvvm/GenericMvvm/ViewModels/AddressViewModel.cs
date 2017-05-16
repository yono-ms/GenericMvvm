using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GenericMvvm
{
    [DataContract]
    public class AddressViewModel : BaseViewModel
    {
        private string _PostalCode;
        /// <summary>
        /// 郵便番号
        /// </summary>
        [Required(ErrorMessage = "郵便番号を入力してください。")]
        [RegularExpression(@"^\d+$", ErrorMessage = "数字で入力してください。")]
        [StringLength(7, MinimumLength =7, ErrorMessage = "7文字で入力してください。")]
        [DataMember]
        public string PostalCode
        {
            get { return _PostalCode; }
            set
            {
                _PostalCode = value; ValidateProperty(nameof(PostalCode), value);
                CanCommandGet = !IsError[nameof(PostalCode)];
            }
        }

        private string _Address;
        /// <summary>
        /// 住所
        /// </summary>
        [Required(ErrorMessage = "住所を入力してください。")]
        [StringLength(20, ErrorMessage = "20文字で入力してください。")]
        [DataMember]
        public string Address
        {
            get { return _Address; }
            set { _Address = value; ValidateProperty(nameof(Address), value); }
        }

        private string _AddressKana;
        /// <summary>
        /// 住所（ふりがな）
        /// </summary>
        [Required(ErrorMessage = "住所（ふりがな）を入力してください。")]
        [RegularExpression(@"^\p{IsHiragana}+$", ErrorMessage = "ひらがなで入力してください。")]
        [StringLength(40, ErrorMessage = "40文字で入力してください。")]
        [DataMember]
        public string AddressKana
        {
            get { return _AddressKana; }
            set { _AddressKana = value; ValidateProperty(nameof(AddressKana), value); }
        }

        private IEnumerable<ZipCloudResponse.result> _ResponseResults;
        /// <summary>
        /// 検索結果
        /// </summary>
        public IEnumerable<ZipCloudResponse.result> ResponseResults
        {
            get { return _ResponseResults; }
            set
            {
                _ResponseResults = value; ValidateProperty(nameof(ResponseResults), value);
                if (_ResponseResults == null)
                {
                    ResponseResultHeader = "郵便番号に該当する住所は存在しません。";
                }
                else
                {
                    ResponseResultHeader = null;
                }
            }
        }

        private string _ResponseResultHeader;
        /// <summary>
        /// 検索結果ヘッダ
        /// </summary>
        public string ResponseResultHeader
        {
            get { return _ResponseResultHeader; }
            set { _ResponseResultHeader = value; ValidateProperty(nameof(ResponseResultHeader), value); }
        }


        public string Description { get { return "住所を入力してください。\n必須項目なので入力しないと先に進めません。"; } }
        public string PostalCodeTitle { get { return "郵便番号"; } }
        public string AddressTitle { get { return "住所"; } }
        public string AddressKanaTitle { get { return "住所（ふりがな）"; } }
        public string PostalCodePlaceholder { get { return "1234567"; } }
        public string AddressPlaceholder { get { return "東京都港区六本木１－２－３"; } }
        public string AddressKanaPlaceholder { get { return "とうきょうとみなとくろっぽんぎ"; } }

        /// <summary>
        /// 郵便番号検索実行
        /// </summary>
        public void CommandGet()
        {
            _BizLogic.CommandGetZipCloud();
        }

        private bool _CanCommandGet;
        /// <summary>
        /// 郵便番号検索ボタン活性化
        /// </summary>
        public bool CanCommandGet
        {
            get { return _CanCommandGet; }
            set { _CanCommandGet = value; ValidateProperty(nameof(CanCommandGet), value); }
        }

        public string CommanGetLabel { get { return "郵便番号検索"; } }

        public void CommandCopy()
        {
            var item = ResponseResults.ElementAt(SelectedIndex);
            Address = item.address1 + item.address2 + item.address3;
            var kana = item.kana1 + item.kana2 + item.kana3;
            AddressKana = Kana.ToHiragana(Kana.ToPadding(Kana.ToZenkakuKana(kana)));
        }
        private bool _CanCommandCopy;
        /// <summary>
        /// コピー可否
        /// </summary>
        public bool CanCommandCopy
        {
            get { return _CanCommandCopy; }
            set { _CanCommandCopy = value; ValidateProperty(nameof(CanCommandCopy), value); }
        }

        public string CommanCopyLabel { get { return "選択した住所をコピーする"; } }

        private int _SelectedIndex;
        /// <summary>
        /// 郵便番号検索結果の選択
        /// </summary>
        public int SelectedIndex
        {
            get { return _SelectedIndex; }
            set
            {
                _SelectedIndex = value; ValidateProperty(nameof(SelectedIndex), value);
                CanCommandCopy = true;
            }
        }


    }
}
