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

        private string _PrefCode;
        /// <summary>
        /// レスポンス表示用住所
        /// </summary>
        public string PrefCode
        {
            get { return _PrefCode; }
            set { _PrefCode = value; ValidateProperty(nameof(PrefCode), value); }
        }
        private string _Address1;
        /// <summary>
        /// レスポンス表示用住所
        /// </summary>
        public string Address1
        {
            get { return _Address1; }
            set { _Address1 = value; ValidateProperty(nameof(Address1), value); }
        }
        private string _Address2;
        /// <summary>
        /// レスポンス表示用住所
        /// </summary>
        public string Address2
        {
            get { return _Address2; }
            set { _Address2 = value; ValidateProperty(nameof(Address2), value); }
        }
        private string _Address3;
        /// <summary>
        /// レスポンス表示用住所
        /// </summary>
        public string Address3
        {
            get { return _Address3; }
            set { _Address3 = value; ValidateProperty(nameof(Address3), value); }
        }
        private string _AddressKana1;
        /// <summary>
        /// レスポンス表示用住所（フリガナ）
        /// </summary>
        public string AddressKana1
        {
            get { return _AddressKana1; }
            set { _AddressKana1 = value; ValidateProperty(nameof(AddressKana1), value); }
        }
        private string _AddressKana2;
        /// <summary>
        /// レスポンス表示用住所（フリガナ）
        /// </summary>
        public string AddressKana2
        {
            get { return _AddressKana2; }
            set { _AddressKana2 = value; ValidateProperty(nameof(AddressKana2), value); }
        }
        private string _AddressKana3;
        /// <summary>
        /// レスポンス表示用住所（フリガナ）
        /// </summary>
        public string AddressKana3
        {
            get { return _AddressKana3; }
            set { _AddressKana3 = value; ValidateProperty(nameof(AddressKana3), value); }
        }

        public string Description { get { return "住所を入力してください。\n必須項目なので入力しないと先に進めません。"; } }
        public string PostalCodeTitle { get { return "郵便番号"; } }
        public string AddressTitle { get { return "住所"; } }
        public string AddressKanaTitle { get { return "住所（ふりがな）"; } }
    }
}
