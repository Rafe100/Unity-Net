using ProtoBuf.Meta;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class Transporter: IDisposable {
    const int PROTO_HEAD_LENGTH = 2;
    const int PROTO_HEAD_MSGtYPE = 2;
    const int PROTO_HEAD_SYMBOL = 4;
    public const Int32 MAX_PACKAGE_LENGTH = 1024 * 64 * 4;

    NetClient nClient;
    MemoryStream memoryStream;
    BinaryWriter binaryWriter;
    byte[] buffer;
    Socket socket;
    Thread tcpRevThread;
    Thread udpRevThread;
    BufferCache revCache;

    Transporter() {

    }

    public Transporter(NetClient nc,Socket netSocket) {
        buffer = new byte[MAX_PACKAGE_LENGTH];
        memoryStream = new MemoryStream(buffer);
        binaryWriter = new BinaryWriter(memoryStream);
        revCache = new BufferCache(MAX_PACKAGE_LENGTH);
        if (nc == null || netSocket == null) {
            Debug.Log("netclient or socket is null");
            return;
        }
        this.nClient = nc;
        this.socket = netSocket;
    }

    public void TCPStartRev() {
        tcpRevThread = new Thread(new ThreadStart(RevTCP));
        tcpRevThread.Start();
    }

    public void UDPStartRev() {
        udpRevThread = new Thread(new ThreadStart(RevUDP));
        udpRevThread.Start();
    }

    void RevTCP() {
        while (true) {
            try {
                int len = this.socket.Receive(buffer, 0, buffer.Length, SocketFlags.None);
                Debug.Log("revTcp len" + len + "revCache read:" + revCache.ReadPtr + "revCache WritePtr:" + revCache.WritePtr);
                if (len > 0) {
                    revCache.Write(buffer, len);
                    ReceiveData rd;
                    do {
                        rd = Decode(revCache);
                        if (rd != null) {
                            this.nClient.NetWorkMessageEnqueue(rd);
                        }
                    } while (rd != null);
                    revCache.Crunch();
                } else {
                    string v = "the len is zero";
                }
            }catch(Exception e) {
                string exp = e.ToString();
                Debug.Log("rev exception :" + exp);
                break;
            }
        }
    }

    void RevUDP() {
        while (true) {
            try {
                EndPoint remoteEp = new IPEndPoint(IPAddress.Any, 100);
                int len = this.socket.ReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref remoteEp);
                Debug.Log("revUdp len" + len);
                if (len > 0) {
                    revCache.Write(buffer, len);
                    ReceiveData rd;
                    do {
                        rd = Decode(revCache);
                        if (rd != null) {
                            this.nClient.NetWorkMessageEnqueue(rd);
                        }
                    } while (rd != null);
                    revCache.Crunch();
                } else {
                    string v = "the len is zero";
                }
            }catch (Exception e) {
            string exp = e.ToString();
            Debug.Log("rev exception :" + exp);
            break;
        }
    }
    }

    public ReceiveData Decode(BufferCache cache) {

        if (cache.Length < sizeof(Int16) * 2)
            return null;
        int startPtr = cache.ReadPtr;
        int packageLength = cache.ReadUInt16();
#if net_log
        Debug.Log("decode read first 2 byte packageLength:" + packageLength);
#endif
        if (packageLength > Transporter.MAX_PACKAGE_LENGTH) {
            Debug.Log("Transporter receive package length over max length :" + packageLength);
            cache.ReadPtr = startPtr;
            return null;
        }
        if(cache.Length < packageLength) {
            Debug.Log("decode packageLenght over cache length packageLength:" + packageLength + "cache.Length:" + cache.Length);
            cache.ReadPtr = startPtr;
            return null;
        }
        UInt16 typeId = cache.ReadUInt16();
#if net_log
        Debug.Log("decode typeId:" + typeId);
#endif
        Type packageType = ClientProtocol.GetTypeById(typeId);
        if (packageType == null) {
            Debug.Log("Transporter receive package can not find id" + typeId);
            return null;
        }
        int packageBodyLength = packageLength - 2 - 4;
        cache.ReadUInt16();
        cache.ReadUInt16();
        ReceiveData data = new ReceiveData();
        data.MsgId = typeId;
        data.MsgObject = Activator.CreateInstance(packageType);
        MemoryStream mStream = cache.GetMemoryStream(packageBodyLength);
        RuntimeTypeModel.Default.Deserialize(mStream, data.MsgObject, packageType);
        return data;
    }


    public bool Send(object msg) {
        try {
            UInt16 id;
            ArraySegment<byte> sendArraySegment = Encode(msg, out id);
            SendStateObject sendState = new SendStateObject();
            sendState.StateSocket = this.socket;
            sendState.Size = sendArraySegment.Count;
            sendState.Id = id;
            int t = this.nClient.SendTo(sendArraySegment);
            return true;
        } catch (Exception e) {
            Debug.Log("transporter send exception :" + e.ToString());
            return false;
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
    public ArraySegment<byte> Encode(object msg, out UInt16 id) {

        memoryStream.Seek(0, SeekOrigin.Begin);
        binaryWriter.Seek(0, SeekOrigin.Begin);
        binaryWriter.Write(UInt16.MinValue);
        UInt16 msgId = ClientProtocol.GetIdByType(msg.GetType());
#if net_log
        Debug.Log("encode id is :" + msgId);
#endif
        id = msgId;
        binaryWriter.Write(msgId);
        binaryWriter.Write(UInt32.MinValue);
        RuntimeTypeModel.Default.Serialize(memoryStream, msg);
#if net_log
        Debug.Log("encode after Serialize:" + memoryStream.Position);
#endif
        int totalLength = (int)memoryStream.Position;
        UInt16 length = (UInt16)(memoryStream.Position - 2);
        binaryWriter.Seek(0, SeekOrigin.Begin);
        binaryWriter.Write(length);
        byte[] fullBuffer = new byte[totalLength];
        Array.Copy(this.buffer, fullBuffer, totalLength);
        ArraySegment<byte> encodeResult = new ArraySegment<byte>(fullBuffer, 0, totalLength);
        return encodeResult;
    }

    public void Dispose() {
        tcpRevThread?.Abort();
        udpRevThread?.Abort();
        tcpRevThread = null;
        udpRevThread = null;
    }



}

public class ReceiveData {
    public UInt16 MsgId;
    public object MsgObject;
    public ReceiveData() {

    }

    public ReceiveData(UInt16 id , object obj) {
        MsgId = id;
        MsgObject = obj;
    }
}

class SendStateObject {
    public Socket StateSocket;
    public int Size;
    public UInt16 Id;
}
