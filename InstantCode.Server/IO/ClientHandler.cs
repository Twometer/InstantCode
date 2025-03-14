﻿using System;
using System.Net.Sockets;
using InstantCode.Protocol;
using InstantCode.Protocol.Crypto;
using InstantCode.Protocol.Handler;
using InstantCode.Protocol.IO;
using InstantCode.Protocol.Packets;
using InstantCode.Server.Model;
using InstantCode.Server.Utility;

namespace InstantCode.Server.IO
{
    public class ClientHandler
    {
        private const string Tag = "ClientHandler";

        private readonly TcpClient tcpClient;
        private readonly INetHandler netHandler;
        private readonly FixedDataStream dataStream;
       
        private static readonly IPacket[] RegisteredPackets =
        {
            new P00Login(),
            new P02NewSession(), 
            new P03CloseSession(), 
            new P04OpenStream(), 
            new P05StreamData(), 
            new P06CloseStream(), 
            new P07CodeChange(), 
            new P08CursorPosition(), 
            new P09Save()
        };

        public ClientData ClientData { get; }
        public string Ip => Util.GetIp(tcpClient);

        public ClientHandler(TcpClient tcpClient)
        {
            this.tcpClient = tcpClient;
            netHandler = new PacketHandler(this);
            dataStream = new FixedDataStream(tcpClient.GetStream());
            ClientData = new ClientData();
        }

        public async void StartReading()
        {
            Log.I(Tag, $"Client {Ip} connected");
            while (tcpClient.Connected)
            {
                try
                {
                    HandlePacket(await PacketSerializer.Deserialize(dataStream, CredentialStore.KeyHash));
                }
                catch (Exception e)
                {
                    Log.I(Tag, $"Client {Ip} lost connection: " + e);
                    break;
                }
            }

            ClientManager.RemoveClient(this);
        }

        private void HandlePacket(PacketBuffer outerBuffer)
        {
            var packetId = outerBuffer.ReadInt();
            var packetContent = new PacketBuffer(outerBuffer.ReadArray());
            foreach (var pack in RegisteredPackets)
            {
                if (pack.Id != packetId) continue;
                pack.Read(packetContent);
                pack.Handle(netHandler);
                break;
            }
        }

        // Packet layout: | IV | Packet Id | Packet content | 
        //                     |<-----AES ENCRYPTED ARR---->|
        //
        // all arrays are length-prefixed
        public void SendPacket(IPacket packet)
        {
            var serialized = PacketSerializer.Serialize(packet, CredentialStore.KeyHash);
            dataStream.Write(serialized, 0, serialized.Length);
        }
    }
}
