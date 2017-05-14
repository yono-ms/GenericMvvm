using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace GenericMvvm
{
    /// <summary>
    /// JSONをGETするクライアント
    /// </summary>
    class GetHttpClient
    {
        /// <summary>
        /// エラーなら真
        /// </summary>
        protected bool _IsError;
        /// <summary>
        /// エラーなら真
        /// </summary>
        public bool IsError
        {
            get { return _IsError; }
        }
        /// <summary>
        /// エラーダイアログに表示するメッセージ
        /// </summary>
        protected List<string> _ErrorMessages;
        /// <summary>
        /// エラーダイアログに表示するメッセージ
        /// </summary>
        public IEnumerable<string> ErrorMessages
        {
            get { return _ErrorMessages; }
        }
        /// <summary>
        /// 受信したデータ
        /// </summary>
        protected string _Json;
        /// <summary>
        /// 受信したデータ
        /// </summary>
        public string Json
        {
            get { return _Json; }
        }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GetHttpClient()
        {
            _IsError = false;
            _ErrorMessages = new List<string>();
        }
        public async Task<T> Execute<T>(string url, Dictionary<string, string> param)
        {
            try
            {
                // エラーをクリアしておく
                _ErrorMessages.Clear();
                _IsError = false;

                // パラメータチェック
                if (param == null || param?.Count == 0)
                {
                    _ErrorMessages.Add("パラメータを指定していません");
                    _IsError = true;
                    return default(T);
                }

                // パラメータセット
                var strParam = "?";
                foreach (var e in param)
                {
                    if (!strParam.EndsWith("?"))
                    {
                        strParam += "&";
                    }
                    strParam += (e.Key + "=" + e.Value);
                }
                var request = url + strParam;
                Debug.WriteLine("REQUEST=" + request);

                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync(request);
                    if (!response.IsSuccessStatusCode)
                    {
                        _IsError = true;
                        _ErrorMessages.Add("サーバーからのエラーメッセージ");
                        _ErrorMessages.Add(response.StatusCode.ToString());
                        _ErrorMessages.Add(response.ReasonPhrase.ToString());
                        return default(T);
                    }
                    // デバッグ用に一旦文字列に変換
                    _Json = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine("RESPONSE=" + _Json);

                    // 文字列からストリームに変換してからデコード
                    using (var sr = new MemoryStream(Encoding.UTF8.GetBytes(_Json)))
                    {
                        var ser = new DataContractJsonSerializer(typeof(T));
                        var resp = (T)ser.ReadObject(sr);
                        return resp;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message + ex.InnerException?.Message);
                _IsError = true;
                _ErrorMessages.Add(ex.GetType().ToString());
                _ErrorMessages.Add(ex.Message);
                _ErrorMessages.Add(ex.InnerException?.Message);
                return default(T);
            }
        }
    }
}
