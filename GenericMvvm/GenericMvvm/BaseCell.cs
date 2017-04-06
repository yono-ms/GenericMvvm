using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GenericMvvm
{
    /// <summary>
    /// プロパティエラー付き構造体
    /// </summary>
    [DataContract]
    public class BaseCell : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// プロパティエラー辞書
        /// </summary>
        protected Dictionary<string, ObservableCollection<string>> _Dict;
        /// <summary>
        /// 通知型の辞書
        /// </summary>
        public class ObservableDictionary
        {
            private BaseCell _baseCell;

            public ObservableDictionary(BaseCell baseCell)
            {
                this._baseCell = baseCell;
            }

            public ObservableCollection<string> this[string name]
            {
                get
                {
                    if (string.IsNullOrWhiteSpace(name)) return null;
                    if (!_baseCell._Dict.ContainsKey(name)) return null;
                    return _baseCell._Dict[name];
                }
            }
        }
        /// <summary>
        /// 通知型辞書の実体
        /// </summary>
        protected ObservableDictionary _Errors;
        /// <summary>
        /// 通知型辞書のプロパティ
        /// </summary>
        public ObservableDictionary Errors
        {
            get { return _Errors; }
        }
        /// <summary>
        /// エラー有無を真偽で返す
        /// </summary>
        public class CheckErrors
        {
            private BaseCell _baseCell;
            public CheckErrors(BaseCell baseCell)
            {
                this._baseCell = baseCell;
            }
            public bool this[string name]
            {
                get
                {
                    if (_baseCell.Errors != null && _baseCell.Errors[name] != null && _baseCell.Errors[name].Count > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }
        /// <summary>
        /// エラー有無の実体
        /// </summary>
        private CheckErrors _IsError;
        /// <summary>
        /// エラー有無のプロパティ
        /// </summary>
        public CheckErrors IsError
        {
            get { return _IsError; }
        }
        /// <summary>
        /// プロパティエラーなし
        /// </summary>
        public bool CanCommit { get { return _Dict.Count == 0; } }
        /// <summary>
        /// プロパティバリデーション
        /// </summary>
        /// <param name="propertyName">プロパティ名</param>
        /// <param name="value">値</param>
        protected void ValidateProperty(string propertyName, object value)
        {
            var context = new ValidationContext(this) { MemberName = propertyName };
            var validationErrors = new List<ValidationResult>();
            if (!Validator.TryValidateProperty(value, context, validationErrors))
            {
                // エラーあり
                var errors = validationErrors.Select(i => i.ErrorMessage);
                var list = new ObservableCollection<string>(errors);
                if (_Dict.ContainsKey(propertyName))
                {
                    _Dict.Remove(propertyName);
                }
                _Dict.Add(propertyName, list);
            }
            else
            {
                // エラーなし
                _Dict.Remove(propertyName);
            }

            // 通知
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Errors)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsError)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanCommit)));
        }
        /// <summary>
        /// 派生クラスからプロパティイベントをキックする
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void PropertyChangedInvoke(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(sender, e);
        }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BaseCell()
        {
            _Dict = new Dictionary<string, ObservableCollection<string>>();
            _Errors = new ObservableDictionary(this);
            _IsError = new CheckErrors(this);
        }
    }
}
