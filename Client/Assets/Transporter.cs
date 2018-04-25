using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;
using ProtoBuf;
using ProtoBuf.Meta;
using ProtoBuf.Serializers;
using System.IO;

public class Transporter
{
    public const Int32 MAX_PACKAGE_LENGTH = 1024 * 64 * 4;
    private  byte[] buffer ;
    private  MemoryStream memoryStream ;
    private  BinaryWriter binaryWriter ;
    private NetClient _netClient;
    private Protocol _protocol;
    private Socket _socket;
    private StateObject _stateObject = new StateObject();
    private IAsyncResult _asyncReceive;
    private IAsyncResult _asyncSend;
    private object _revQueueLockObject = new object();
    public Transporter(Socket socket,NetClient netClient,Protocol proto)
    {
        this._socket = socket;
        this._netClient = netClient;
        this._protocol = proto;
        buffer = new byte[MAX_PACKAGE_LENGTH];
        memoryStream = new MemoryStream(buffer);
        binaryWriter = new BinaryWriter(memoryStream);
    }

    public void StartReceive()
    {
        this.Receive();
    }

    void Receive()
    {
        _asyncReceive = this._socket.BeginReceive(_stateObject.buffer, 0, _stateObject.buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), _stateObject);
    }

    void ReceiveCallBack(IAsyncResult asyncResult)
    {
        StateObject state = asyncResult.AsyncState as StateObject;
        try
        {
            int receiveLength = this._socket.EndReceive(asyncResult);
            if(receiveLength > 0)
            {
                ReceiveData revData = Decode(state.buffer, receiveLength);
                Debug.Log("ReceiveData: [Id]" + revData.MsgId + "[Type]" + revData.MsgObject.GetType().ToString());
                this._protocol.ResetHeartBeat();
                lock (_revQueueLockObject)
                {
                    _netClient.ReceiveQueue.Enqueue(revData);
                }
            }

            //继续接收
            _asyncReceive = this._socket.BeginReceive(_stateObject.buffer, 0, _stateObject.buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), _stateObject);

        }
        catch(Exception e)
        {
            Disconnect();
        }
    }

    public ReceiveData Decode(byte[] cache, Int32 revLength)
    {
      
        if (revLength < sizeof(Int16) * 2)
            return null;
        int packageLength = BitConverter.ToUInt16(cache, 0);
        Debug.Log("decode pck length:" + packageLength);
        if(packageLength > Transporter.MAX_PACKAGE_LENGTH)
        {
            Debug.Log("Transporter receive package length over max length :" + packageLength);
            return null;
        }
        UInt16 typeId = BitConverter.ToUInt16(cache, 2);
        Debug.Log("decode typeId:" + typeId);
        Type packageType = this._netClient.ClientProtocolMap.GetTypeById(typeId);
        if (packageType == null)
        {
            Debug.Log("Transporter receive package can not find id" + typeId );
            return null;
        }
        int packageBodyLength = packageLength - 2 - 4;
        ReceiveData data = new ReceiveData();
        data.MsgId = typeId;

        data.MsgObject = Activator.CreateInstance(packageType);
        MemoryStream mStream = new MemoryStream(cache, 8, packageBodyLength, false);
//#if UNITY_IPHONE && !UNITY_EDITOR
            //ProtobufSerializer serializer = new ProtobufSerializer();
            //serializer.Deserialize(rCache.GetStream(pakBodyLength), ccMsg, pakType);
        
//#else
        //RuntimeTypeModel.Default.Deserialize(rCache.GetStream(pakBodyLength), ccMsg, pakType);
//#endif

        RuntimeTypeModel.Default.Deserialize(mStream, data.MsgObject, packageType);
        return data;
    }

    public bool JustSend(object msg)
    {
        ArraySegment<byte> sendArraySegment = JustEncode(msg);
        SendStateObject sendState = new SendStateObject();
        sendState.StateSocket = this._socket;
        sendState.Size = sendArraySegment.Count;
        sendState.Id = 0;
        _asyncSend = this._socket.BeginSend(sendArraySegment.Array, sendArraySegment.Offset, sendArraySegment.Count, SocketFlags.None, new AsyncCallback(SendCallback), sendState);
        return true;
    }

    public bool Send(object msg)
    {
        try
        {
            UInt16 id;
            ArraySegment<byte> sendArraySegment = Encode(msg, out id);
            SendStateObject sendState = new SendStateObject();
            sendState.StateSocket = this._socket;
            sendState.Size = sendArraySegment.Count;
            sendState.Id = id;
            _asyncSend = this._socket.BeginSend(sendArraySegment.Array, sendArraySegment.Offset, sendArraySegment.Count, SocketFlags.None, new AsyncCallback(SendCallback), sendState);
            return true;
        }
        catch(Exception e)
        {
            return false;
        }
    }

    private void SendCallback(IAsyncResult result)
    {
        SendStateObject state = result.AsyncState as SendStateObject;
        try
        {
            int l = state.StateSocket.EndSend(result);
        }
        catch(Exception e)
        {
            ReceiveData revData = new ReceiveData();
            NetMessageSendFail fail = new NetMessageSendFail();
            fail.MessageId = state.Id;
            fail.Ecp = e;
            revData.MsgObject = fail;
            revData.MsgId = ClientProtocol.MsgId_connectFail;        
            this._netClient.ReceiveQueue.Enqueue(revData);
            
        }
    }

    /// <summary>
    /// 2byte - message length
    /// 2byte - message type
    /// 4byte - not use
    /// protobuf
    /// extern
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public ArraySegment<byte> Encode(object msg, out UInt16 id)
    {
       
        memoryStream.Seek(0, SeekOrigin.Begin);
        binaryWriter.Seek(0, SeekOrigin.Begin);
        binaryWriter.Write(UInt16.MinValue);
        UInt16 msgId = this._netClient.ClientProtocolMap.GetIdByType(msg.GetType());
        id = msgId;
        binaryWriter.Write(msgId);
        Debug.Log("encode msgId:" + memoryStream.Position);
        binaryWriter.Write(UInt32.MinValue);
        Debug.Log("encode + 4:" + memoryStream.Position);
        RuntimeTypeModel.Default.Serialize(memoryStream, msg);
        Debug.Log("encode :" + memoryStream.Position);
        int totalLength = (int)memoryStream.Position;
        UInt16 length = (UInt16)(memoryStream.Position - 2);
        binaryWriter.Seek(0, SeekOrigin.Begin);
        binaryWriter.Write(length);
        byte[] fullBuffer = new byte[totalLength];
        Array.Copy(this.buffer, fullBuffer, totalLength);
        ArraySegment<byte> encodeResult = new ArraySegment<byte>(fullBuffer, 0, totalLength);
        return encodeResult;
    }
    public ArraySegment<byte> JustEncode(object msg)
    {

        memoryStream.Seek(0, SeekOrigin.Begin);
        binaryWriter.Seek(0, SeekOrigin.Begin);
       
        RuntimeTypeModel.Default.Serialize(memoryStream, msg);
        Debug.Log("encode :" + memoryStream.Position);
        int totalLength = (int)memoryStream.Position;
     
        byte[] fullBuffer = new byte[totalLength];
        Array.Copy(this.buffer, fullBuffer, totalLength);
        ArraySegment<byte> encodeResult = new ArraySegment<byte>(fullBuffer, 0, totalLength);
        return encodeResult;
    }


    void Disconnect()
    {
       
    }
}

public class ReceiveData
{
    public UInt16 MsgId;
    public object MsgObject;
}

class StateObject
{
    public Socket StateSocket;
    public const int BufferSize = 1024;
    public byte[] buffer = new byte[BufferSize];
}

class SendStateObject
{
    public Socket StateSocket;
    public int Size;
    public UInt16 Id;
}