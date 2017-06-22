using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Reflection;

namespace GenericMvvm.Droid
{
    /// <summary>
    /// カスタムコントロール専用バインドユーティリティ
    /// </summary>
    public class TextInputViewBind
    {
        const string FORMAT = "----TextInputViewBind {0} ---- {1}";
        /// <summary>
        /// VMのプロパティの型
        /// </summary>
        public enum ConverterType
        {
            /// <summary>
            /// 変換無し
            /// </summary>
            NONE,
            /// <summary>
            /// 数値
            /// </summary>
            INT
        }
        /// <summary>
        /// フラグメントのアイテムビュー
        /// </summary>
        View _View;
        /// <summary>
        /// ビューモデル
        /// </summary>
        BaseViewModel _VM;
        /// <summary>
        /// プロパティ名からコントロールを得る
        /// </summary>
        Dictionary<string, int> ResId;
        /// <summary>
        /// リソースIDからプロパティ名を得る
        /// </summary>
        Dictionary<int, string> PropName;
        /// <summary>
        /// プロパティ名からコンバーターを得る
        /// </summary>
        Dictionary<string, ConverterType> PropType;
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="view">フラグメント</param>
        /// <param name="vm">ビューモデル</param>
        public TextInputViewBind(View view, BaseViewModel vm)
        {
            _View = view;
            _VM = vm;
            ResId = new Dictionary<string, int>();
            PropName = new Dictionary<int, string>();
            PropType = new Dictionary<string, ConverterType>();
        }
        /// <summary>
        /// バインド情報を追加する
        /// </summary>
        /// <param name="propertyName">VMのプロパティ名</param>
        /// <param name="controlId">コントロールのリソースID</param>
        /// <param name="hint">コントロールのタイトルテキスト</param>
        /// <param name="conv">変換方法</param>
        public void Add(string propertyName, int controlId, string hint, ConverterType conv = ConverterType.NONE)
        {
            ResId.Add(propertyName, controlId);
            PropName.Add(controlId, propertyName);
            _View.FindViewById<TextInputView>(controlId).Hint = hint;
            PropType.Add(propertyName, conv);
        }
        /// <summary>
        /// VMのプロパティ名で検索
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(string key)
        {
            if (key.Equals(nameof(_VM.Errors)))
            {
                return true;
            }
            if (key.Equals(nameof(_VM.IsError)))
            {
                return true;
            }
            return ResId.ContainsKey(key);
        }
        /// <summary>
        /// コントロールIDで検索
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(int key)
        {
            return PropName.ContainsKey(key);
        }
        /// <summary>
        /// コントロールイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            var view = sender as TextInputView;
            if (PropName.ContainsKey(view.Id))
            {
                var vmValue = ConvToViewModel(e.Text.ToString(), PropType[PropName[view.Id]]);
                System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name, vmValue });
                _VM.GetType().GetProperty(PropName[view.Id]).SetValue(_VM, vmValue);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name, "UNKNOWN ResId=" + view.Id });
            }
        }
        /// <summary>
        /// コンバーター（コントロールからVMプロパティ型）
        /// </summary>
        /// <param name="v"></param>
        /// <param name="converterType"></param>
        /// <returns></returns>
        private object ConvToViewModel(string v, ConverterType converterType)
        {
            if (converterType == ConverterType.INT)
            {
                System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name, v });
                var result = string.IsNullOrEmpty(v) ? 0 : Convert.ChangeType(v, TypeCode.Int32);
                return result;
            }
            else
            {
                return v;
            }
        }
        /// <summary>
        /// コンバーター（VMプロパティ型からコントロール）
        /// </summary>
        /// <param name="v"></param>
        /// <param name="converterType"></param>
        /// <returns></returns>
        private string ConvFromViewModel(object v, ConverterType converterType)
        {
            if (converterType == ConverterType.INT)
            {
                System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name, v.ToString() });
                if ((int)v == 0)
                {
                    return "";
                }
                else
                {
                    return v.ToString();
                }
            }
            else
            {
                return v.ToString();
            }
        }

        /// <summary>
        /// VMイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(_VM.Errors):
                    foreach (var item in PropName)
                    {
                        _View.FindViewById<TextInputView>(item.Key).Errors = _VM.Errors?[item.Value];
                    }
                    break;

                case nameof(_VM.IsError):
                    foreach (var item in PropName)
                    {
                        _View.FindViewById<TextInputView>(item.Key).IsError = _VM.IsError[item.Value];
                    }
                    break;

                default:
                    if (ResId.ContainsKey(e.PropertyName))
                    {
                        var v = sender.GetType().GetProperty(e.PropertyName).GetValue(sender);
                        System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name, v });
                        var value = ConvFromViewModel(v, PropType[e.PropertyName]);
                        var control = _View.FindViewById<TextInputView>(ResId[e.PropertyName]);
                        
                        // VMからのイベントは循環しないように同じ値を設定しない
                        if (!string.IsNullOrEmpty(value) && !value.Equals(control.Text))
                        {
                            _View.FindViewById<TextInputView>(ResId[e.PropertyName]).Text = value;
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name, "SAME VALUE Name=" + e.PropertyName });
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name, "UNKNOWN Name=" + e.PropertyName });
                    }
                    break;
            }
        }
        /// <summary>
        /// TwoWay初期値設定
        /// </summary>
        public void Start()
        {
            foreach (var item in PropName)
            {
                _View.FindViewById<TextInputView>(item.Key).TextChanged += TextChanged;

                var v = _VM.GetType().GetProperty(item.Value).GetValue(_VM);
                var value = ConvFromViewModel(v, PropType[item.Value]);
                _View.FindViewById<TextInputView>(item.Key).Text = value;
            }
        }
        /// <summary>
        /// TwoWay停止
        /// </summary>
        public void Stop()
        {
            foreach (var item in PropName)
            {
                _View.FindViewById<TextInputView>(item.Key).TextChanged -= TextChanged;
            }
        }
    }
}