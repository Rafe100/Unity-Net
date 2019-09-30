using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System;

public class NetClient : IDisposable
{
    public const Int32 MAX_PACKAGE_LENGTH = 1024 * 64 * 4;
    public bool isConnected;
    protected string host;
    protected int port;
    protected Socket socket;
    protected IPEndPoint endPoint;
    protected Thread thread;
    protected byte[] buffer;
    protected Transporter transporter;
    Queue<System.Object> revQueue = new Queue<object>(29);

   

    public NetClient()
    {
        buffer = new byte[MAX_PACKAGE_LENGTH];
    }

    public virtual void Start()
    {
        InitSocket();
        Connect();
    }

    public void NetWorkMessageEnqueue(ReceiveData revData) {
        if(revData == null) {
            return;
        }
        this.revQueue.Enqueue(revData);
    }

    public ReceiveData NetWorkMessageDequeue() {
        if (revQueue.Count <= 0) {
            return null;
        }
        return this.revQueue.Dequeue() as ReceiveData;
    }

    public Socket GetSocket() {
        return socket;
    }


    protected virtual void Adress() {
        IPAddress address;
        if (!IPAddress.TryParse(this.host, out address)) {
            IPHostEntry hostEntry = Dns.GetHostEntry(this.host);
            IPAddress[] ipAddrs = hostEntry.AddressList;
            address = ipAddrs[0];
        }
        host = address.ToString();
        endPoint = new IPEndPoint(address, port);
    }

    protected virtual void InitSocket() {
        Adress();
    }
    
    protected virtual void InitTransporter() {
        this.transporter = new Transporter(this, this.socket);
    }

    protected virtual void Connect() {
        try {
            InitTransporter();
        } catch(Exception e) {
            Debug.Log("connect exception:" + e.ToString());
        }
    }

    public void Send(object obj) {
        if(transporter == null) {
            Debug.Log("the netClient transporter is null");
            return;
        }
        this.transporter.Send(obj);
    }

    public virtual int SendTo(ArraySegment<byte> segment) {
        return 0;
    }

    public virtual void Dispose() {
        this.socket?.Shutdown(SocketShutdown.Both);
        this.socket?.Close();
        this.socket = null;
        this.transporter?.Dispose();
        this.transporter = null;
    }


    public virtual void StartRev() {

    }
}
