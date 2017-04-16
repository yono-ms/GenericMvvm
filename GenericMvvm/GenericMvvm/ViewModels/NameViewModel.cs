using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GenericMvvm
{
    /// <summary>
    /// 姓名入力
    /// </summary>
    [CustomValidation(typeof(NameViewModel), "IsValidName")]
    [DataContract]
    public class NameViewModel : BaseViewModel
    {
        private string _LastName;
        /// <summary>
        /// お名前（姓）
        /// </summary>
        [Required(ErrorMessage = "姓を入力してください。")]
        [RegularExpression("[^0-9A-Za-z]+", ErrorMessage ="英数字は使用できません。")]
        [StringLength(3, ErrorMessage = "お名前（姓）は{1}桁以内で入力してください。")]
        [DataMember]
        public string LastName
        {
            get { return _LastName; }
            set { _LastName = value; ValidateProperty(nameof(LastName), value); }
        }

        private string _FirstName;
        /// <summary>
        /// お名前（名）
        /// </summary>
        [Required(ErrorMessage = "名を入力してください。")]
        [RegularExpression("[^0-9A-Za-z]+", ErrorMessage = "英数字は使用できません。")]
        [StringLength(3, ErrorMessage = "お名前（名）は{1}桁以内で入力してください。")]
        [DataMember]
        public string FirstName
        {
            get { return _FirstName; }
            set { _FirstName = value; ValidateProperty(nameof(FirstName), value); }
        }

        public string LastNameTitle { get { return "お名前（姓）"; } }
        public string FirstNameTitle { get { return "お名前（名）"; } }

        public string LastNamePlaceholder { get { return "山田"; } }
        public string FirstNamePlaceholder { get { return "太郎"; } }

        public string Description { get { return "お名前を入力してください。\n必須項目なので入力しないと先に進めません。"; } }

        /// <summary>
        /// カスタムバリデーション
        /// </summary>
        /// <param name="vm"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static ValidationResult IsValidName(NameViewModel vm, ValidationContext context)
        {
            // 何度も使用する場合はここからBizLogicのバリデーション関数を呼ぶ
            if (vm.LastName.Equals("山田") && vm.FirstName.Equals("太郎"))
            {
                return new ValidationResult("山田太郎だけは許せません");
            }

            return ValidationResult.Success;
        }

        public NameViewModel()
        {
            // 特になし
        }

        public override void KickStart()
        {
            base.KickStart();

            // 特になし
        }

        public override void Commit()
        {
            base.Commit();

            if (IsValidViewModel(this))
            {
                // 画面遷移
                _BizLogic.Commit();
            }
            else
            {
                // エラー表示
                _BizLogic.ShowError();
            }
        }
    }
}
