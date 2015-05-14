using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
#if WINDOWS_PHONE
using Windows.Networking.Connectivity;
#endif

namespace SecondShiftMobile.Networking
{
    public class SocketManager : IDisposable
    {
        public SocketRole Role { get; protected set; }
        Socket socket;
        SocketAsyncEventArgs send, receive;
        public const int MaxBufferSize = 2048;
        byte[] receiveBytes;
        bool receiving = false, receiveRequested = false, sending = false, accepting = false;
        string messageBuffer = "";
        Queue<SocketMessage> messageQueue;
        public SocketManager()
        {
            messageQueue = new Queue<SocketMessage>();
        }
        public void Listen(string fallbackAddress, int port)
        {
            if (accepting)
                return;
            accepting = true;
            string ip = NetworkManager.GetLocalIp();
            if (ip == null)
                ip = fallbackAddress;
            var endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            Role = SocketRole.Host;
            var listener = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            
            listener.Bind(endPoint);
            listener.Listen(10);
            SocketAsyncEventArgs e = new SocketAsyncEventArgs();
            e.Completed+=accept_Completed;
            NetworkManager.Log("Listening at " + endPoint.ToString());
            listener.AcceptAsync(e);
            /*try
            {
                while (true)
                {
                    
                    listener.AcceptAsync(e);
                    Thread.Sleep(200);
                };
            }
            catch (Exception ex)
            {
                NetworkManager.Log("Error listening for connection");
            }*/
            //NetworkManager.Log("Finished listening at " + endPoint.ToString());
        }

        private void accept_Completed(object sender, SocketAsyncEventArgs e)
        {
            accepting = false;
            socket = e.AcceptSocket;
            NetworkManager.Log("Received connection from " + socket.RemoteEndPoint);
            NetworkManager.AddSocket(this);
        }
        void listenCallback(IAsyncResult ar)
        {
            var listener = (Socket)ar.AsyncState;
#if !WINDOWS_PHONE
            socket = listener.EndAccept(ar);
#endif
            NetworkManager.Log("Received connection from " + socket.RemoteEndPoint);
            NetworkManager.AddSocket(this);
        }
        public void Connect(string ipAddress, int port)
        {
            NetworkManager.Log("Connecting to " + ipAddress + ":" + port);
            var address = IPAddress.Parse(ipAddress);
            Role = SocketRole.Client;
            socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            SocketAsyncEventArgs e = new SocketAsyncEventArgs();
            e.RemoteEndPoint = new DnsEndPoint(address.ToString(), port);
            e.Completed += e_Completed;
            socket.ConnectAsync(e);
        }

        public void Receive()
        {
            if (receiving)
            {
                receiveRequested = true;
                return;
            }
            receiveRequested = false;
            receiving = true;
            if (receive == null)
            {
                receive = new SocketAsyncEventArgs();
                receive.RemoteEndPoint = socket.RemoteEndPoint;
                receive.Completed += receive_Completed;
                receive.SetBuffer(new byte[MaxBufferSize],0, MaxBufferSize);
            }
            socket.ReceiveAsync(receive);
        }

        void receive_Completed(object sender, SocketAsyncEventArgs e)
        {
            receiving = false; 
            if (e.SocketError == SocketError.Success)
            {
                var response = Encoding.UTF8.GetString(e.Buffer, e.Offset, e.BytesTransferred).Trim('\0');
                //NetworkManager.Log("Received data: " + response);
                int socketEndingIndex = 0;
                bool addToSocketBuffer = true;
                int respondStartIndex = 0;
                for (int i = 0; i < response.Length; i++)
                {
                    if (response[i] == SocketMessage.EndingString[socketEndingIndex])
                    {
                        socketEndingIndex++;
                        if (socketEndingIndex == SocketMessage.EndingString.Length)
                        {
                            addToSocketBuffer = false;
                            messageBuffer += response.Substring(respondStartIndex, (i + 1) - respondStartIndex);
                            respondStartIndex = i + 1;
                            NetworkManager.Log("Received data: " + messageBuffer);
                            var sm = new SocketMessage(messageBuffer);
                            messageBuffer = "";
                            NetworkManager.ProcessIncomingSocketMessage(sm);
                            socketEndingIndex = 0;
                        }
                    }
                    else
                    {
                        addToSocketBuffer = true;
                        socketEndingIndex = 0;
                    }
                }
                if (addToSocketBuffer)
                    messageBuffer += response;
            }
            if (receiveRequested)
            {
                Receive();
            }
        }

        public void Send(SocketMessage message)
        {
            if (sending)
            {
                messageQueue.Enqueue(message);
                return;
            }
            sending = true;
            Helper.Write("Sending message: " + message);
            if (send == null)
            {
                send = new SocketAsyncEventArgs();
                send.RemoteEndPoint = socket.RemoteEndPoint;
                send.Completed += send_Completed;
            }
            var bytes = message.GetBytes();
            send.SetBuffer(bytes, 0, bytes.Length);
            socket.SendAsync(send);
        }

        void send_Completed(object sender, SocketAsyncEventArgs e)
        {
            Helper.Write("Message sent");
            sending = false;
            if (messageQueue.Count > 0)
            {
                Send(messageQueue.Dequeue());
            }
        }

        void e_Completed(object sender, SocketAsyncEventArgs e)
        {
            e.Completed -= e_Completed;
            if (e.SocketError == SocketError.Success)
            {
                NetworkManager.Log("Socket connected");
            }
            else
            {
                NetworkManager.Log("Socket connection error: " + e.SocketError);
            }
            NetworkManager.AddSocket(this);
        }



        public void Dispose()
        {
            if (socket != null)
            {
                socket.Dispose();
            }
        }
    }
}
