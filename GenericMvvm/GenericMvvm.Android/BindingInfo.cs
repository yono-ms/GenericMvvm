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
        /// ViewModel�̃v���p�e�B��
        /// </summary>
        public string VMProperty { get; set; }
        /// <summary>
        /// �R���g���[���̃I�u�W�F�N�g
        /// </summary>
        public object Control { get; set; }
        /// <summary>
        /// �R���g���[���̃v���p�e�B��
        /// </summary>
        public string ControlProperty { get; set; }
        /// <summary>
        /// �����̃v���p�e�B�����t���b�V������
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="vm"></param>
        /// <param name="dict"></param>
        public static void Start<T>(T vm, Dictionary<string, BindingInfo> dict)
        {
            foreach (var item in dict)
            {
                var v = vm.GetType().GetProperty(item.Key).GetValue(vm);
                vm.GetType().GetProperty(item.Key).SetValue(vm, v);
            }
        }
    }
}