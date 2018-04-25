using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using CustomProtocol;

public class ClientProtocol
{

    public ClientProtocol()
    {
        _idDic[MsgId_logInTestReq] = typeof(MsgClientTestLoginReq);
        _typeDic[typeof(MsgClientTestLoginReq)] = MsgId_logInTestReq;

        _idDic[MsgId_logInRsp] = typeof(MsgClientLoginRsp);
        _typeDic[typeof(MsgClientLoginRsp)] = MsgId_logInRsp;

        _idDic[MsgId_heartBeat] = typeof(HeartBeat);
        _typeDic[typeof(HeartBeat)] = MsgId_heartBeat;

        _idDic[MsgId_connectFail] = typeof(NetMessageConnectFail);
        _typeDic[typeof(NetMessageConnectFail)] = MsgId_connectFail;

        _idDic[MsgId_sendFail] = typeof(NetMessageSendFail);
        _typeDic[typeof(NetMessageSendFail)] = MsgId_sendFail;

    }


    public Type GetTypeById(UInt16 id)
    {
        Type value;
        if (_idDic.TryGetValue(id, out value))
            return value;
        return null;
    }

    public UInt16 GetIdByType(Type t)
    {
        UInt16 value;
        if (_typeDic.TryGetValue(t, out value))
            return value;
        return 0;
    }
    private Dictionary<UInt16, Type> _idDic = new Dictionary<UInt16, Type>();
    private Dictionary<Type, UInt16> _typeDic = new Dictionary<Type, UInt16>();


    /// <summary>
    /// Message Id
    /// </summary>
    
    //连接失败
    public const UInt16 MsgId_connectFail = 0xFFFF;
    //连接超时
    public const UInt16 MsgId_connectTimeOut = 0xFFFe;
    //发送失败
    public const UInt16 MsgId_sendFail = 0xFFFd;
    //心跳
    public const UInt16 MsgId_heartBeat = 0;

    //登录请求
    public const UInt16 MsgId_logInTestReq = 1;

    //登录请求返回
    public const UInt16 MsgId_logInRsp = 11;

    
}
