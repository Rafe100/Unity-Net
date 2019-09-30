using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class UDPNetClient : NetClient
{
    EndPoint remoteEndPoint;
    string remoteHost;
    int remotePort;
    public UDPNetClient(string netHost, int netPort,string rHost,int rPort) {
        this.host = netHost;
        this.port = netPort;
        this.remoteHost = rHost;
        this.remotePort = rPort;
    }

    protected override void Adress() {
        base.Adress();
        IPAddress address;
        if (!IPAddress.TryParse(this.remoteHost, out address)) {
            IPHostEntry hostEntry = Dns.GetHostEntry(this.remoteHost);
            IPAddress[] ipAddrs = hostEntry.AddressList;
            address = ipAddrs[0];
        }
        remoteHost = address.ToString();
        remoteEndPoint = new IPEndPoint(address, remotePort);
    }

    protected override void InitSocket()
    {
        try {
            base.InitSocket();
            this.socket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            this.socket.Bind(this.endPoint);
        }
        catch (Exception e)
        {
            Debug.Log("InitSocket exception:" + e.ToString());
        }
    }

    protected override void Connect() {
        base.Connect();
        StartRev();
        var connecMsg = new ReceiveData(ClientProtocol.MsgId_connect, new CusNetMesConnected());
        NetWorkMessageEnqueue(connecMsg);
    }

    public override void StartRev() {
        this.transporter.UDPStartRev();
    }


    public override int SendTo(ArraySegment<byte> sendArraySegment) {
        return this.socket.SendTo(sendArraySegment.Array, sendArraySegment.Offset,sendArraySegment.Count, SocketFlags.None, this.remoteEndPoint);
    }

    public override void Dispose() {
        base.Dispose();
        Debug.Log("udp socket is close");
    }


}
