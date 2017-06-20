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
    /// 生年月日入力
    /// </summary>
    [CustomValidation(typeof(BirthViewModel), "IsValidBirth")]
    [DataContract]
    public class BirthViewModel : BaseViewModel
    {
        private const int _MinYear = 1900;
        /// <summary>
        /// 生年月日（年）最小値
        /// </summary>
        public int MinYear
        {
            get { return _MinYear; }
        }

        private const int _MaxYear = 2017;
        /// <summary>
        /// 生年月日（年）最大値
        /// </summary>
        public int MaxYear
        {
            get { return _MaxYear; }
        }

        private int _Year;
        /// <summary>
        /// 生年月日（年）
        /// </summary>
        [Required(ErrorMessage = "年を入力してください。")]
        [Range(_MinYear, _MaxYear, ErrorMessage ="年は1900から2017の間で入力してください。")]
        [DataMember]
        public int Year
        {
            get { return _Year; }
            set { _Year = value; ValidateProperty(nameof(Year), value); }
        }

        private const int _MinMonth = 1;
        /// <summary>
        /// 生年月日（月）最小値
        /// </summary>
        public int MinMonth
        {
            get { return _MinMonth; }
        }
        private const int _MaxMonth = 12;
        /// <summary>
        /// 生年月日（月）最大値
        /// </summary>
        public int MaxMonth
        {
            get { return _MaxMonth; }
        }

        private int _Month;
        /// <summary>
        /// 生年月日（月）
        /// </summary>
        [Required(ErrorMessage = "月を入力してください。")]
        [Range(_MinMonth, _MaxMonth, ErrorMessage = "月は1から12の間で入力してください。")]
        [DataMember]
        public int Month
        {
            get { return _Month; }
            set { _Month = value; ValidateProperty(nameof(Month), value); }
        }

        private const int _MinDay = 1;
        /// <summary>
        /// 生年月日（日）最小値
        /// </summary>
        public int MinDay
        {
            get { return _MinDay; }
        }
        private const int _MaxDay = 31;
        /// <summary>
        /// 生年月日（日）最大値
        /// </summary>
        public int MaxDay
        {
            get { return _MaxDay; }
        }

        private int _Day;
        /// <summary>
        /// 生年月日（日）
        /// </summary>
        [Required(ErrorMessage = "日を入力してください。")]
        [Range(_MinDay, _MaxDay, ErrorMessage = "日は1から31の間で入力してください。")]
        [DataMember]
        public int Day
        {
            get { return _Day; }
            set { _Day = value; ValidateProperty(nameof(Day), value); }
        }

        public string YearTitle { get { return "年"; } }
        public string MonthTitle { get { return "月"; } }
        public string DayTitle { get { return "日"; } }
        public string YearPlaceholder { get { return "2000"; } }
        public string MonthPlaceholder { get { return "12"; } }
        public string DatPlaceholder { get { return "31"; } }
        public string Description { get { return "生年月日を入力してください。\n必須項目なので入力しないと先に進めません。"; } }

        public static ValidationResult IsValidBirth(BirthViewModel vm, ValidationContext context)
        {
            try
            {
                var birth = string.Format("{0}/{1}/{2}", vm.Year, vm.Month, vm.Day);
                var result = DateTime.Parse(birth);
                if (result > DateTime.Now)
                {
                    return new ValidationResult("未来の日付は入力できません。");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                return new ValidationResult("正しい日付ではありません。");
            }
            return ValidationResult.Success;
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
