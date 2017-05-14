using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMvvm
{
    public interface INativeCall
    {
        /// <summary>
        /// アラート表示
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="labelYes"></param>
        /// <param name="labelNo"></param>
        /// <returns></returns>
        Task<bool> ShowAlertAsync(string title, string message, string labelYes, string labelNo);
        /// <summary>
        /// 水平方向に画面遷移する
        /// </summary>
        /// <param name="page">遷移先画面名</param>
        /// <param name="forward">進む方向なら真</param>
        void NavigateTo(string page, bool forward);
        /// <summary>
        /// ページをスタックする
        /// </summary>
        /// <param name="page"></param>
        void Push(string page);
        /// <summary>
        /// ページをポップする
        /// </summary>
        void Pop();
        /// <summary>
        /// ファイルから文字列を取り出す
        /// </summary>
        /// <param name="name">ファイル名</param>
        /// <returns>文字列</returns>
        Task<string> LoadFileAsync(string name);
        /// <summary>
        /// 文字列をファイルに保存する
        /// </summary>
        /// <param name="name">ファイル名</param>
        /// <param name="data">文字列</param>
        Task SaveFileAsync(string name, string data);
        /// <summary>
        /// UIスレッドに戻して実行する
        /// 戻り値なしでUIスレッドでawaitしながら処理する
        /// 呼び出し元をAsyncにしない投げっぱなし関数
        /// </summary>
        /// <param name="callback"></param>
        void RunUIThread(Action callback);
    }
}
