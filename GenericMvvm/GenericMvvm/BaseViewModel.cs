﻿using System;
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
    /// ビューモデル基本クラス
    /// </summary>
    [DataContract]
    public class BaseViewModel : BaseStruct
    {
        /// <summary>
        /// デザイン時に使えるようにコンストラクタは引数無し
        /// </summary>
        public BaseViewModel() : base()
        {

        }
        /// <summary>
        /// 全体エラーの実体
        /// </summary>
        private ObservableCollection<string> _ObjectErrors;
        /// <summary>
        /// 全体エラーのプロパティ
        /// </summary>
        public ObservableCollection<string> ObjectErrors
        {
            get { return _ObjectErrors; }
            set { _ObjectErrors = value; ValidateProperty(nameof(ObjectErrors), value); }
        }
        /// <summary>
        /// ViewModel全体のバリデーションチェックを実行する。
        /// 全体バリデーションは各ViewModelのCustomValidationで実装しておく。
        /// </summary>
        /// <typeparam name="T">ViewModelの型</typeparam>
        /// <param name="vm">ViewModelの実体</param>
        public bool IsValidViewModel<T>(T vm)
        {
            var result = true;

            var context = new ValidationContext(vm);
            var validationErrors = new List<ValidationResult>();
            if (!Validator.TryValidateObject(vm, context, validationErrors, true))
            {
                // エラーあり
                var errors = validationErrors.Select(i => i.ErrorMessage);
                ObjectErrors = new ObservableCollection<string>(errors);
                result = false;
            }
            else
            {
                // エラーなし
                ObjectErrors = null;
            }
            // 通知
            PropertyChangedInvoke(this, new PropertyChangedEventArgs(nameof(ObjectErrors)));
            return result;
        }

    }
}
