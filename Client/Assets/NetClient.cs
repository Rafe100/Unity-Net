using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Timers;

public enum NetState
{
    CLOSED,
    CONNECTING,
    CONNECTED,
    DISCONNECTED,
    TIMEOUT,
    ERROR
}

public class NetClient : IDisposable {

    public ClientProtocol ClientProtocolMap;
    public Queue<System.Object> ReceiveQueue = new Queue<System.Object>();
    public Action OnConnected;
    public bool Disposed = false;
    public NetState NetWorkState
    {
        get { return this._netState; }
    }
    private Protocol _protocol;
    private Socket _socket;
    private string _host;
    private int _port;
    private IPEndPoint _endPoint;
    private NetState _netState = NetState.CLOSED;
    private System.Object _receiveLockObject = new System.Object();
    private ManualResetEvent _timeoutEvent = new ManualResetEvent(false);
    private System.Timers.Timer _timeOutTimer = new System.Timers.Timer();
    private int _timeoutMSec = 8000;
    
    public NetClient()
    {
        ClientProtocolMap = new ClientProtocol();
    }

    public void Initialize(string host,int port)
    {
        this._host = host;
        this._port = port;
       
        InitSocket();
    }

    bool InitSocket()
    {
        if (this._socket != null && this._socket.Connected)
            return true;
        try
        {
            IPAddress address;
            if (!IPAddress.TryParse(this._host, out address))
            {
                IPHostEntry hostEntry = Dns.GetHostEntry(this._host);
                IPAddress[] ipAddrs = hostEntry.AddressList;
                address = ipAddrs[0];
            }
            _host = address.ToString();
            _endPoint = new IPEndPoint(address, _port);
            if (address.AddressFamily.ToString() == ProtocolFamily.InterNetworkV6.ToString())
            {
                _socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
            }
            else
            {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }

            if (_socket == null)
            {
                NetStateChanged(NetState.ERROR);
                return false;
            }
            Connect();
            return true;
        }
        catch (Exception e)
        {
            Debug.Log("Net Client InitSocket Exception" + e.ToString());
            NetStateChanged(NetState.ERROR);
            return false;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void Connect()
    {
        NetStateChanged(NetState.CONNECTING);
        this._socket.BeginConnect(this._endPoint, new AsyncCallback(ConnectCallBack), this._socket);
        //超时判断
        this._timeOutTimer.Interval = _timeoutMSec;
        this._timeOutTimer.Elapsed += new ElapsedEventHandler(ConnectTimeOut);
        this._timeOutTimer.Enabled = true;
    }


    void ConnectCallBack(IAsyncResult result)
    {
        try
        {
            this._timeOutTimer.Enabled = false;
            this._socket.EndConnect(result);
            if (!this._socket.Connected)
            {
                throw new Exception("connect call back exception");
            }
            this._protocol = new Protocol(this, this._socket);
            this._protocol.StartReceive();
            NetStateChanged(NetState.CONNECTED);
            this._protocol.StartHeartBeat();
            if (OnConnected != null)
            {
                OnConnected();
            }
            Debug.Log("ConnectCallBack");
        }
        catch(Exception e)
        {
            NetStateChanged(NetState.ERROR);
            ReceiveData revData = new ReceiveData();
            NetMessageConnectFail fail = new NetMessageConnectFail();
            fail.Host = this._host;
            fail.Port = this._port;
            fail.Ecp = e;
            fail.Message = "connect fail";
            revData.MsgObject = fail;
            revData.MsgId = ClientProtocol.MsgId_connectFail;
            this.ReceiveQueue.Enqueue(revData);
        }
    }

    public void ConnectTimeOut(object source, ElapsedEventArgs e)
    {
        this._timeOutTimer.Enabled = false;
        NetStateChanged(NetState.TIMEOUT);
        NetWorkManager.Instance.TimeOut();
        Debug.Log(" ConnectTimeOut");
    }

    

    public void HeartBeatTimeOut()
    {
        NetStateChanged(NetState.TIMEOUT);
        NetWorkManager.Instance.TimeOut();
        Debug.Log(" HeartBeatTimeOut");
    }

    public bool Send(object msg)
    {
        return this._protocol.Send(msg);
    }

    public bool JustSend(object msg)
    {
        return this._protocol.JustSend(msg);
    }

    void NetStateChanged(NetState state)
    {
        _netState = state;
    }

    public void Disconnect()
    {
        this._socket.Shutdown(SocketShutdown.Both);
        this._socket.Close();
        this._socket = null;
        this._protocol.Disconnect();
        NetStateChanged(NetState.DISCONNECTED);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    // The bulk of the clean-up code
    protected virtual void Dispose(bool disposing)
    {
        if (this.Disposed)
            return;

        if (disposing)
        {
            // free managed resources
            if (this._protocol != null)
            {
                this._protocol.Dispose();
            }


            try
            {
                Disconnect();
            }
            catch (Exception)
            {
                
            }

            this.Disposed = true;
        }
    }
}

public class NetMessageConnectFail
{
    public string Host;
    public int Port;
    public string Message;
    public Exception Ecp;
}

public class NetMessageSendFail
{
    public UInt16 MessageId;
    public Exception Ecp;
}

public class ConnectState
{
    
}