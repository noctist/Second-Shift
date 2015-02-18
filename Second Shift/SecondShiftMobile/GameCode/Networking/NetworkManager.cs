using System;
using System.Collections.Generic;
using System.Text;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
#if WINDOWS_PHONE
using Windows.System.Threading;
#endif
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace SecondShiftMobile.Networking
{
    public enum SocketRole { Server, Client }
    public class DataReceivedEventArgs : EventArgs
    {

    }
    public class NetworkLoggedEventArgs : EventArgs
    {
        public string Log;
    }
    public static class NetworkManager
    {
        static List<SocketManager> sockets = new List<SocketManager>();
        public const int Port = 31650;
        public static event EventHandler<NetworkLoggedEventArgs> Logged;
        static bool timerStarted = false;
        public static void AddSocket(SocketManager socket)
        {
            sockets.Add(socket);
        }

        public static string GetLocalIp()
        {
#if PC
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress IpAddress = null;
            foreach (var ip in ipHostInfo.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    IpAddress = ip;
                }
            }
            if (IpAddress != null)
            {
                return IpAddress.ToString();
            }
            else
            {
                return "127.0.0.1";
            }
#elif WINDOWS_PHONE
            string address = null;

            var hostnames = Windows.Networking.Connectivity.NetworkInformation.GetHostNames();
            foreach (var hn in hostnames)
            {
                if (hn.IPInformation != null && hn.IPInformation.NetworkAdapter.IanaInterfaceType == 71)
                {
                    var ip = IPAddress.Parse(hn.DisplayName);
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                        address = hn.DisplayName;
                    
                }
            }
            return address;
#endif
        }

        static void startTimer()
        {
            if (timerStarted)
                return;
            timerStarted = true;
            Thread thread = new Thread(sendReceiveMethod);
            thread.Start();
        }
        static void sendReceiveMethod()
        {
            while (timerStarted)
            {
                foreach (var s in sockets)
                {
                    //if (s.Role == SocketRole.Client)
                        s.Receive();
                }
                Thread.Sleep(10);
            }
        }
        public static void Send(SocketMessage message)
        {
            foreach (var s in sockets)
            {
                s.Send(message);
            }
        }
        public static void BeginListening(string ipAddress)
        {
            startTimer();
            Thread thread = new Thread(() =>
                {
                    SocketManager socket = new SocketManager();
                    socket.Listen(ipAddress, Port);
                });
            thread.IsBackground = true;
            thread.Start();
        }
        public static void Dispose()
        {
            timerStarted = false;
            foreach (var s in sockets)
                s.Dispose();
        }
        public static void ConnectTo(string ipAddress)
        {
            startTimer();
            SocketManager socket = new SocketManager();
            socket.Connect(ipAddress, Port);
        }
        public static void Log(string log)
        {
            if (Logged != null)
            {
                Logged(null, new NetworkLoggedEventArgs() { Log = log });
            }
        }
        public static Task<string[]> FindAddresses()
        {
            TaskCompletionSource<string[]> tcs = new TaskCompletionSource<string[]>();
            Thread thread = new Thread(() =>
                {

                });
            return tcs.Task;
        }
        static void findAddresses()
        {
            for (int i = 0; i < 50; i++)
            {
                string ip = "192.168.1." + i;
            }
        }
    }
}
