using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class ClientProtocol
{

    static ClientProtocol()
    {
        _idDic[MsgId_connectReq] = typeof(CustomProtocol.PlayerConnectReq);
        _typeDic[typeof(CustomProtocol.PlayerConnectReq)] = MsgId_connectReq;

        _idDic[MsgId_connectRsp] = typeof(CustomProtocol.PlayerConnectRsp);
        _typeDic[typeof(CustomProtocol.PlayerConnectRsp)] = MsgId_connectRsp;

        _idDic[MsgId_connect] = typeof(CusNetMesConnected);
        _typeDic[typeof(CusNetMesConnected)] = MsgId_connect;
    }


    public static Type GetTypeById(UInt16 id)
    {
        Type value;
        if (_idDic.TryGetValue(id, out value))
            return value;
        return null;
    }

    public static UInt16 GetIdByType(Type t)
    {
        UInt16 value;
        if (_typeDic.TryGetValue(t, out value))
            return value;
        return 0;
    }
    private static Dictionary<UInt16, Type> _idDic = new Dictionary<UInt16, Type>();
    private static Dictionary<Type, UInt16> _typeDic = new Dictionary<Type, UInt16>();


    /// <summary>
    /// Message Id
    /// </summary>
    
    //连接
    public const UInt16 MsgId_connectReq = 1;
    public const UInt16 MsgId_connectRsp = 2;
    public const UInt16 MsgId_connect = 3;


}


public class CusNetMesConnected {

}