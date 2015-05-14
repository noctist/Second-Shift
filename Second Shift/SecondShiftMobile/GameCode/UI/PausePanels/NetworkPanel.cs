using Microsoft.Xna.Framework;
using SecondShiftMobile.Networking;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace SecondShiftMobile.UI.PausePanels
{
    public class NetworkPanel : StackPanel
    {
        TextBlock hostGame, ipAddress, joinGame, status;
        public NetworkPanel()
        {
            Padding = new Thickness(0, 19, 0, 19);
            hostGame = new TextBlock() { Text = "host game", BackgroundColor = Color.DarkGray, Padding = new Thickness(19) };
            joinGame = new TextBlock() { Text = "join game", BackgroundColor = Color.DarkGray, Padding = new Thickness(19) };
            ipAddress = new TextBlock() { Text = "IP Address: " + NetworkManager.GetLocalIp() };
            status = new TextBlock() { Text = "Status: "  + NetworkManager.SocketRole};
            AddChild(hostGame, joinGame, ipAddress, status);
            hostGame.Clicked += hostGame_Clicked;
            joinGame.Clicked += joinGame_Clicked;
        }

        async void joinGame_Clicked(object sender, MouseEventArgs e)
        {
            string ip = await EnterText.GetText();
            IPAddress ipa;
            if (IPAddress.TryParse(ip, out ipa))
            {
                NetworkManager.ConnectTo(ip);
                setStatus();
                Global.GameState = GameState.Playing;
            }
            else
            {
                status.Text = "Invalid IP";
            }
        }

        void hostGame_Clicked(object sender, MouseEventArgs e)
        {
            NetworkManager.BeginListening("192.168.1.101");
            setStatus();
            Global.GameState = GameState.Playing;
        }
        void setStatus()
        {
            status.Text = "Status: " + NetworkManager.SocketRole;
        }
    }
}
