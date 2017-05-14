﻿using System;
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
            set { _PostalCode = value; ValidateProperty(nameof(PostalCode), value); }
        }

        private string _Address;
        /// <summary>
        /// 住所
        /// </summary>
        [Required(ErrorMessage = "住所を入力してください。")]
        [StringLength(10, ErrorMessage = "10文字で入力してください。")]
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
        [StringLength(20, ErrorMessage = "20文字で入力してください。")]
        [DataMember]
        public string AddressKana
        {
            get { return _AddressKana; }
            set { _AddressKana = value; ValidateProperty(nameof(AddressKana), value); }
        }

        private IEnumerable<ZipCloudResponse.result> _ResponseResults;

        public IEnumerable<ZipCloudResponse.result> ResponseResults
        {
            get { return _ResponseResults; }
            set { _ResponseResults = value; ValidateProperty(nameof(ResponseResults), value); }
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
        public bool CanCommandGet { get { return !IsError[nameof(PostalCode)]; } }
        public string CommanGetLabel { get { return "郵便番号検索"; } }

    }
}
