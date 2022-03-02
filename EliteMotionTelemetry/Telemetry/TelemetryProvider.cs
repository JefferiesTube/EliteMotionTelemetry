using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace EliteMotionTelemetry.Telemetry
{
    public class TelemetryProvider
    {
        private Socket _socket;
        private IPAddress _ip;
        private IPEndPoint _ep;

        public bool Active { get; private set; }

        public void Open(int port)
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _ip = IPAddress.Parse("127.0.0.1");
            _ep = new IPEndPoint(_ip, port);
            Active = true;
        }

        public void Close()
        {
            _socket.Close();
            Active = false;
        }

        public void SendDatagram(MotionData data)
        {
            if (!Active)
                return;

            List<byte> datagram = new List<byte>();
            datagram.AddRange(BitConverter.GetBytes(data.Speed));
            datagram.AddRange(BitConverter.GetBytes(data.Roll));
            datagram.AddRange(BitConverter.GetBytes(data.Pitch));
            datagram.AddRange(BitConverter.GetBytes(data.Yaw));
            datagram.AddRange(BitConverter.GetBytes(data.Heave));
            datagram.AddRange(BitConverter.GetBytes(data.Sway));
            datagram.AddRange(BitConverter.GetBytes(data.Surge));

            _socket.SendTo(datagram.ToArray(), _ep);
        }
    }
}