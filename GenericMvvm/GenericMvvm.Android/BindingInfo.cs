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

namespace GenericMvvm.Droid
{
    public class BindingInfo
    {
        /// <summary>
        /// ViewModelのプロパティ名
        /// </summary>
        public string VMProperty { get; set; }
        /// <summary>
        /// コントロールのオブジェクト
        /// </summary>
        public object Control { get; set; }
        /// <summary>
        /// コントロールのプロパティ名
        /// </summary>
        public string ControlProperty { get; set; }
        /// <summary>
        /// 辞書のプロパティをリフレッシュする
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="vm"></param>
        /// <param name="dict"></param>
        public static void Start<T>(T vm, Dictionary<string, BindingInfo> dict)
        {
            foreach (var item in dict)
            {
                var i = vm.GetType().GetProperty(item.Key);
                var v = i.GetValue(vm);
                System.Diagnostics.Debug.WriteLine("Type={0} Name={1} Value={2}", new[] { i.PropertyType.ToString(), i.Name, v });

                if ((v == null) && (i.PropertyType == typeof(string)))
                {
                    System.Diagnostics.Debug.WriteLine("null string convert");
                    v = "";
                }
                i.SetValue(vm, v);
            }
        }
    }
}