using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingleInstance<GameManager> {
    public int playerId;

    private void Awake() {
        NetWorkManager.Instance.Register(ClientProtocol.MsgId_connect, SocketConnected);
        NetWorkManager.Instance.RegisterOnce(ClientProtocol.MsgId_connectRsp, ConnectTcp);
        NetWorkManager.Instance.InitTcp();
    }

    void SocketConnected(ReceiveData rev) {
        Debug.Log("SocketConnected" + rev.MsgId);
    }

    void ConnectTcp(ReceiveData rev) {
        var revData = rev.MsgObject as CustomProtocol.PlayerConnectRsp;
        playerId = revData.playId;
        Debug.Log("the server playerid :" + playerId + "udpPort:" + revData.udpPort);
        NetWorkManager.Instance.CloseTcp();
        NetWorkManager.Instance.InitUdp(revData.udpPort);
        //test
        var o = new CustomProtocol.PlayerConnectRsp();
        o.playId = playerId + 1;
        NetWorkManager.Instance.SendUDP(o);
    }

    void ConnectRsp(ReceiveData rev) {
        var revData = rev.MsgObject as CustomProtocol.PlayerConnectRsp;
        playerId = revData.playId;
        Debug.Log("ConnectRsp" + playerId);
    }


}
