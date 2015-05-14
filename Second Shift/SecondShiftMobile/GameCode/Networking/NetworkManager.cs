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
using System.ComponentModel;

namespace SecondShiftMobile.Networking
{
    public enum SocketRole { None, Host, Client }
    public class DataReceivedEventArgs : EventArgs
    {

    }
    public class NetworkLoggedEventArgs : EventArgs
    {
        public string Log;
    }
    public static class NetworkManager
    {
        [DefaultValue(SocketRole.None)]
        public static SocketRole SocketRole { get; private set;}
        static List<SocketManager> sockets = new List<SocketManager>();
        public const int Port = 31650;
        public static event EventHandler PeerConnected;
        public static event EventHandler<NetworkLoggedEventArgs> Logged;
        static bool timerStarted = false;
        public static void AddSocket(SocketManager socket)
        {
            sockets.Add(socket);
            if (PeerConnected != null)
                PeerConnected(null, null);
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
        public static void Send(this SocketMessage message)
        {
            foreach (var s in sockets)
            {
                s.Send(message);
            }
        }
        public static bool NetworkIdExists(String id)
        {
            for (int i = 0; i < Global.Game.NumberOfObjects; i++)
            {
                if (Global.Game.objArray[i].NetworkID == id)
                {
                    return true;
                }
            }
            return false;
        }
        public static void SpawnPlayer(Player p)
        {
            var sm = new SocketMessage();
            sm.Info.BaseAddress = "SpawnPlayer";
            sm.Info["Pos"] = p.Pos.ToStageString();
            sm.Info["NetworkId"] = p.NetworkID;
            sm.Info["Type"] = p.GetType().ToString();
            sm.Info["Name"] = p.Name;
            Send(sm);
        }
        public static void SpawnPlayers()
        {
            foreach (var p in Global.Game.FindObjects<Player>())
            {
                if (!p.IsNetworkControlled)
                    SpawnPlayer(p);
            }
        }

        public static void ProcessIncomingSocketMessage(SocketMessage sm)
        {
            if (string.IsNullOrWhiteSpace(sm.NetworkId))
            {
                if (sm.Info.BaseAddress == "SpawnPlayer")
                {
                    if (sm.Info.ContainsKey("Type") && sm.Info.ContainsKey("Pos"))
                    {
                        Type t = Type.GetType(sm.Info["Type"]);
                        var pos = StageObjectPropertyConverter.GetVector3(sm.Info["Pos"]);
                        string networkId = null;
                        if (sm.Info.ContainsKey("NetworkId"))
                        {
                            networkId = sm.Info["NetworkId"];
                            if (NetworkIdExists(networkId))
                            {
                                return;
                            }
                        }
                        string name = null;
                        if (sm.Info.ContainsKey("Name"))
                        {
                            name = sm.Info["Name"];
                        }
                        var obj = StageObjectData.Create(t, pos, name, networkId);
                        Log("Spawned network player with Id: " + networkId);
                        var sm1 = new SocketMessage();
                        sm1.Info.BaseAddress = "ConfirmSpawn";
                        sm1.NetworkId = networkId;
                        sm1.Send();
                        obj.IsNetworkControlled = true;
                        SpawnPlayers();
                    }
                }
                else if (sm.Info.BaseAddress == "ScreenRect")
                {
                    string r = sm.Info["rect"];
                    Global.NetworkScreenRect = StageObjectPropertyConverter.GetRectangle(r); 
                }
            }
             
            else
            {
                for (int i = 0; i < Global.Game.NumberOfObjects; i++)
                {
                    if (Global.Game.objArray[i].NetworkID == sm.NetworkId)
                    {
                        Global.Game.objArray[i].ReceiveSocketMessage(sm);
                        break;
                    }
                }
            }
        }
        public static void BeginListening(string ipAddress)
        {
            SocketRole = Networking.SocketRole.Host;
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
            SocketRole = Networking.SocketRole.Client;
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
