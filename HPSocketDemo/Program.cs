using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GetCIDbyBatchActivation;
using HPSocket;
using HPSocket.Http;
using HPSocket.Tcp;

namespace HPSocketDemo
{
    internal class Program
    {
        private static IHttpEasyServer hs;
        private static string html = string.Empty;
        static void Main(string[] args)
        {
            using (hs = new HttpEasyServer())
            {
                hs.Port = 8899;

                hs.OnRequestLine += Hs_OnRequestLine;

                if (!hs.Start())
                {
                    Console.WriteLine($@"httpServer.Start() error, code：{hs.ErrorCode}, message: {hs.ErrorMessage}");
                    return;
                }
                Console.WriteLine($@"bind: {hs.Address}:{hs.Port}");
                hs.Wait();
            }
            Console.ReadKey();
        }

        private static HttpParseResult Hs_OnRequestLine(IHttp sender, IntPtr connId, string method, string url)
        {

            hs.OnEasyMessageData += Hs_OnEasyMessageData;

            var t =Task.Factory.StartNew(() => {
                if (url.ToString().Contains("getcid/?iids="))
                {
                    string cid = string.Empty;
                    string str = System.Web.HttpUtility.UrlDecode(url);
                    string iid = Regex.Replace(str, "-", "").Replace(" ", "").Replace("getcid/?iids=", "");
                    iid = Regex.Match(iid, "[\\d]+").Value;
                    if (Regex.IsMatch(iid, "[\\d]{63}"))
                    {
                        iid = Regex.Match(iid, "[\\d]{63}").Value;
                    }
                    else if (Regex.IsMatch(iid, "[\\d]{54}"))
                    {
                        iid = Regex.Match(iid, "[\\d]{54}").Value;
                    }
                    //string iid = "690494024111248275641345743539870015978502549282339542426203524";
                    cid = XmlRequest.MSXmlRequest(1, iid, "00000-04249-038-820384-03-2052-9200.0000-0902023");
                    html = $"安装ID：{iid}</br>确认ID：{cid}";
                }
            });
            t.Wait();
            
            return HttpParseResult.Ok;
        }

        private static HttpParseResult Hs_OnEasyMessageData(IHttpEasyServer sender, IntPtr connId, byte[] data)
        {
            string ip = string.Empty;
            ushort port = 0;
            if (sender.GetRemoteAddress(connId, out ip, out port)) {

                Console.WriteLine($"user client：{ip}:{port}");
            }
            
            var headers = sender.GetAllHeaders(connId);
            var sb = new StringBuilder();
            foreach (var item in headers)
            {
                sb.Append($"{item.Name}: {item.Value}<br />");
            }

            string a =Encoding.UTF8.GetString(data);



            // 替换模板中的{{headers}}变量
            //var html = "<h1>获取确认id</h1>";

            // 响应体
            var responseBody = Encoding.GetEncoding("utf-8").GetBytes(html);
            // 响应头
            var responseHeaders = new List<NameValue>
            {
                new NameValue{ Name = "Content-Type", Value = "text/html;charset=\"utf-8\""},
            };

            // 发送响应数据到客户端
            if (!sender.SendResponse(connId, HttpStatusCode.Ok, responseHeaders, responseBody, responseBody.Length))
            {
                return HttpParseResult.Error;
            }
            Console.WriteLine($"user client：{ip}:{port}，response：{html}");
            // 不是保活连接踢掉
            if (sender.GetHeader(connId, "Connection") != "Keep-Alive")
            {
                sender.Release(connId);
            }

            return HttpParseResult.Ok;
        }

        private static string GetMsg(string msg)
        {
            byte[] bytes = Encoding.GetEncoding("GB2312").GetBytes(msg);
            return Encoding.GetEncoding("GB2312").GetString(bytes);
        }
    }
}
