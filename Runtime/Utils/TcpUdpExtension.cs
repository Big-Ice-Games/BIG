#region license

// Copyright (c) 2025, Big Ice Games
// All rights reserved.

#endregion

using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

#pragma warning disable CS8601

namespace BIG
{
    public static partial class TcpUdpExtension
    {
        private const int PREFIX = 4;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] ReceiveFromTcp(this TcpClient client)
        {
            byte[] receivePrefix = new byte[PREFIX];
            int receiveTotal = 0;

            int receive = client.Client.Receive(receivePrefix, 0, 4, 0);
            int receiveSize = BitConverter.ToInt32(receivePrefix, 0);
            int receiveDataLeft = receiveSize;
            byte[] data = new byte[receiveSize];
            while (receiveTotal < receiveSize)
            {
                receive = client.Client.Receive(data, receiveTotal, receiveDataLeft, 0);
                if (receive == 0)
                {
                    break;
                }
                receiveTotal += receive;
                receiveDataLeft -= receive;
            }
            return data;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SendTcp(this TcpClient client, byte[] data)
        {
            int sendingSize = data.Length;
            int sendingDataLeft = sendingSize;
            var sendPrefix = BitConverter.GetBytes(sendingSize);
            int sendingSent = client.Client.Send(sendPrefix);
            int sendingTotal = 0;

            while (sendingTotal < sendingSize)
            {
                sendingSent = client.Client.Send(data, sendingTotal, sendingDataLeft, SocketFlags.None);
                sendingTotal += sendingSent;
                sendingDataLeft -= sendingSent;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ReceiveDataFromUdp(this UdpClient client, ref IPEndPoint ipEndPoint, out byte[] result)
        {
            return (result = (client?.Available > 0) ? client.Receive(ref ipEndPoint) : null) != null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SendUdp(this UdpClient client, ref IPEndPoint ipEndPoint, byte[] data)
        {
            try
            {
                client?.Send(data, data.Length, ipEndPoint);
            }
            catch (Exception e)
            {
                client.Log($"Exception occur during sending through udp. \n{data.Length}\n{e.StackTrace}");
                throw;
            }
        }
    }
}