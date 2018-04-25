using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
public class Protocol  {

    NetClient _netClient;
    Socket _socket;
    HandShakeService _handShakeService;
    HeartBeatService _heartBeatService;
    Transporter _transporter;
    public Protocol(NetClient nc,Socket socket)
    {
        this._netClient = nc;
        this._socket = socket;
        this._handShakeService = new HandShakeService();
        _heartBeatService = new HeartBeatService(5,this);
        this._transporter = new Transporter(socket,nc,this);
    }

    public void StartReceive()
    {
        this._transporter.StartReceive();
    }

    public void StartHeartBeat()
    {
        //this._heartBeatService.Start();
    }

    public void ResetHeartBeat()
    {
        this._heartBeatService.ResetTimeout();
    }


    public NetClient GetClient()
    {
        return this._netClient;
    }

    public bool Send(object msg)
    {
        return this._transporter.Send(msg);
    }

    public bool JustSend(object msg)
    {
        return this._transporter.JustSend(msg);
    }

    public void Disconnect()
    {
        _heartBeatService.Stop();
    }

    public void Dispose()
    {
        Disconnect();
    }
}
