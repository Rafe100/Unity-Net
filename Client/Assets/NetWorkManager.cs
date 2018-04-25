using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using CustomProtocol;

public class NetWorkManager : MonoSingleton<NetWorkManager> {

    NetClient _netClient;
    Dictionary<UInt16, object> _sendSyncCache = new Dictionary<UInt16, object>();
    //login 服务器返回
    MsgClientLoginRsp loginServerRsp;
	// Use this for initialization
	void Start () {
		
	}

    public void InitializeNetClient(string host = "127.0.0.1 ",int port = 3001)
    {
        _netClient = new NetClient();
        _netClient.Initialize(host, port);
    }


    /// <summary>
    /// Client 2 Server
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="isSync">是否需要同步</param>
    /// <param name="syncMsgId">需要同步的情况，服务器返回消息id</param>
    void SendMessage(object msg,bool isSync = false,UInt16 syncMsgId = UInt16.MinValue)
    {
        ReqMessage req = new ReqMessage();
        req.Message = msg;
        if(isSync)
        {
            if(!_sendSyncCache.ContainsKey(syncMsgId))
            {
                _sendSyncCache.Add(syncMsgId, msg);
                OpenSyncUI();
            }
        }
        this._netClient.Send(msg);
    }

 

	// Update is called once per frame
    void Update()
    {

        if (_netClient != null)
        {
            if (_netClient.ReceiveQueue.Count <= 0)
            {
                return;
            }
            ReceiveData revData = _netClient.ReceiveQueue.Dequeue() as ReceiveData;
            if (this._sendSyncCache.ContainsKey(revData.MsgId))
            {
                this._sendSyncCache.Remove(revData.MsgId);
            }
            if (this._sendSyncCache.Count <= 0)
            {
                CloseSyncUI();
            }
            HandleReceiveMessage(revData);
        }

    }

    void HandleReceiveMessage(ReceiveData revData)
    {
        if(revData == null)
        {
            return;
        }
        switch(revData.MsgId)
        {
            case ClientProtocol.MsgId_connectFail:
                //连接失败
                ConnectFail(revData);
                break;
            case ClientProtocol.MsgId_connectTimeOut:
                //超时
                break;
            case ClientProtocol.MsgId_sendFail:
                //请求失败
                SendFail(revData);
                break;
            case ClientProtocol.MsgId_logInRsp:
                //登录LogIn服务器返回
                LoginTestRsp(revData);
                break;
            
        }
    }

    void ConnectFail(ReceiveData rspObj)
    {
         NetMessageConnectFail fail = rspObj.MsgObject as NetMessageConnectFail;
        Debug.Log("connect fail:" + fail.Ecp.ToString());
    }

    void ConnectTimeOut()
    {

    }

    void SendFail(ReceiveData rspObj)
    {
        NetMessageSendFail fail = rspObj.MsgObject as NetMessageSendFail;
        Debug.Log("connect fail:" + fail.MessageId  + fail.Ecp.ToString());
    }

    public void LoginTestReq(string account,string password)
    {
        MsgClientTestLoginReq req = new MsgClientTestLoginReq();
        req.account = account;
        req.passwd = password;
        SendMessage(req);
    }

    /// <summary>
    /// test 
    /// </summary>
    /// <param name="obj"></param>
    public void JustSend(object obj)
    {
        this._netClient.JustSend(obj);
    }

    public void LoginTestRsp(ReceiveData rspObj)
    {
        loginServerRsp = rspObj.MsgObject as MsgClientLoginRsp;
        this._netClient.Disconnect();
        this._netClient.OnConnected = ConnectGateServer;

        this._netClient.Initialize(loginServerRsp.gate_ip, loginServerRsp.gate_port);
    }

    void ConnectGateServer()
    {
        //after connect to login connect to gate
    }

  

    /// <summary>
    /// 打开同步UI
    /// </summary>
    void OpenSyncUI()
    {

    }

    /// <summary>
    /// 关闭同步UI
    /// </summary>
    void CloseSyncUI()
    {

    }

    public void Disconnect()
    {
        this._netClient.Disconnect();
    }

    public void TimeOut()
    {

    }

    void OnDestroy()
    {
        Debug.Log("NetWorkManager destroy");
        if(_netClient != null && !_netClient.Disposed)
        {
            _netClient.Dispose();
        }
    }
}

public class ReqMessage
{
    public object Message;
    public bool isSync;
}
