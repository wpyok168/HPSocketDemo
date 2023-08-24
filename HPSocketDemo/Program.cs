using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GetCIDbyBatchActivation;
using HPSocket;
using HPSocket.Http;
using HPSocket.Ssl;
using HPSocket.Tcp;

namespace HPSocketDemo
{
    internal class Program
    {
        
        static void Main(string[] args)
        {
            //HP_Server.HPServerStart();
            HPSocket.WebSocket.WebSocketAgent ws = new HPSocket.WebSocket.WebSocketAgent("ws://socket.2580123.xyz:9000");
           
            
            ws.OnOpen += Ws_OnOpen;
            ws.OnMessage += Ws_OnMessage;
            ws.OnClose += Ws_OnClose;
            ws.Start(); 
            ws.Connect();
            ws.Wait();
            Console.ReadKey();  
        }

        private static HandleResult Ws_OnClose(IWebSocketAgent sender, IntPtr connId, SocketOperation socketOperation, int errorCode)
        {
            Console.WriteLine("Ws_OnClose");

            return HandleResult.Ok;
        }

        private static HandleResult Ws_OnMessage(IWebSocketAgent sender, IntPtr connId, bool final, HPSocket.WebSocket.OpCode opCode, byte[] mask, byte[] data)
        {
            Console.WriteLine("Ws_OnMessage");

            return HandleResult.Ok;
        }

        private static HandleResult Ws_OnOpen(IWebSocketAgent sender, IntPtr connId)
        {

            Console.WriteLine("Ws_OnOpen");
            byte[] data = Encoding.UTF8.GetBytes("Receiver," + "428501242062805542591338980" + "\0");
            sender.Send(connId, HPSocket.WebSocket.OpCode.Binary, data, data.Length);
            data = Encoding.UTF8.GetBytes("262146609713962967633989564285012420628055425913389800769011840");
            sender.Send(connId, HPSocket.WebSocket.OpCode.Binary , data, data.Length);

            return HandleResult.Ok;
        }
    }

    public class HpScoketSSL
    {
        
        
    }
    /// <summary>
    /// 客户信息
    /// </summary>
    
}
