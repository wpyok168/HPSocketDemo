using HPSocket.Ssl;
using HPSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPSocketDemo
{
    public class HPSSL_Server
    {
        SslServer ssl;
        public static void SServer()
        {
            using (SslServer ssl = new SslServer())
            {
                ssl.Address = "0.0.0.0";
                ssl.Port = 8888;
                ssl.PemCertFile = "";
                ssl.PemKeyFile = "";
                if (!ssl.Initialize(false))
                {
                    Console.WriteLine("ssl环境初始化失败, 请检查证书是否存在, 证书密码是否正确");
                }
                ssl.OnAccept += Ssl_OnAccept;
                ssl.OnHandShake += Ssl_OnHandShake;
                ssl.OnReceive += Ssl_OnReceive;
                ssl.OnClose += Ssl_OnClose;
                ssl.OnShutdown += Ssl_OnShutdown;


            }
        }

        private static HandleResult Ssl_OnReceive(IServer sender, IntPtr connId, byte[] data)
        {
            if (sender == null || !sender.HasStarted)
            {
                return HandleResult.Error;
            }
            if (sender.State == ServiceState.Stopped)
            {

            }

            string recstr = Encoding.UTF8.GetString(data);

            return HandleResult.Ignore;
        }

        private static HandleResult Ssl_OnShutdown(IServer sender)
        {
            Console.WriteLine($"OnShutdown({sender.Address}:{sender.Port})");

            return HandleResult.Ok;
        }

        private static HandleResult Ssl_OnClose(IServer sender, IntPtr connId, SocketOperation socketOperation, int errorCode)
        {
            Console.WriteLine($"OnClose({connId}), socket operation: {socketOperation}, error code: {errorCode}");

            var client = sender.GetExtra<ClientInfo>(connId);
            if (client != null)
            {
                sender.RemoveExtra(connId);
                client.PacketData.Clear();
                return HandleResult.Error;
            }

            return HandleResult.Ok;
        }

        private static HandleResult Ssl_OnHandShake(IServer sender, IntPtr connId)
        {
            Console.WriteLine($"OnHandShake({connId})");

            // 设置附加数据, 用来做粘包处理
            sender.SetExtra(connId, new ClientInfo
            {
                ConnId = connId,
                PacketData = new List<byte>(),
            });

            return HandleResult.Ok;
        }

        private static HandleResult Ssl_OnAccept(IServer sender, IntPtr connId, IntPtr client)
        {
            // 获取客户端地址
            if (!sender.GetRemoteAddress(connId, out var ip, out var port))
            {
                return HandleResult.Error;
            }

            Console.WriteLine($"OnAccept({connId}), ip: {ip}, port: {port}");

            return HandleResult.Ok;
        }

    }

    public class ClientInfo
    {
        /// <summary>
        /// 连接id
        /// </summary>
        public IntPtr ConnId { get; set; }

        /// <summary>
        /// 封包数据
        /// </summary>
        public List<byte> PacketData { get; set; }

    }
}
