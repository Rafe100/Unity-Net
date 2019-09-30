using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class TCPNetClient : NetClient {

    public TCPNetClient(string netHost,int netPort) {
        this.host = netHost;
        this.port = netPort;
    }

    protected override void InitSocket() {
        try {
            base.InitSocket();
            this.socket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        } catch (Exception e) {
            // exception
        }
    }

    protected override void Connect() {
        base.Connect();
        this.socket.Connect(endPoint);
        isConnected = true;
        StartRev();
        var connecMsg = new ReceiveData(ClientProtocol.MsgId_connect, new CusNetMesConnected());
        NetWorkMessageEnqueue(connecMsg);
        Debug.Log("tcp connect");
    }

    public override void StartRev() {
        this.transporter.TCPStartRev();
    }

    public override int SendTo(ArraySegment<byte> sendArraySegment) {
        return this.socket.Send(sendArraySegment.Array, sendArraySegment.Offset, sendArraySegment.Count, SocketFlags.None);
    }


    public override void Dispose() {
        base.Dispose();
        Debug.Log("tcp socket is close");
    }

}
