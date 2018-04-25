using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Timers;
using CustomProtocol;

public class HeartBeatService  {

    int interval;
    public int timeout;
    Timer timer;
    DateTime lastTime;

    Protocol protocol;
    HeartBeat heartBeat = new HeartBeat();
    public HeartBeatService(int interval, Protocol protocol)
    {
        this.interval = interval * 1000;
        this.protocol = protocol;
    }

    internal void ResetTimeout()
    {
        this.timeout = 0;
        lastTime = DateTime.Now;
    }

    void SendHeartBeat(object source, ElapsedEventArgs e)
    {
        TimeSpan span = DateTime.Now - lastTime;
        timeout = (int)span.TotalMilliseconds;

        //check timeout
        if (timeout > interval * 2)
        {
            protocol.GetClient().HeartBeatTimeOut();
            //stop();
            return;
        }

        //Send heart beat
        protocol.Send(this.heartBeat);
    }

    public void Start()
    {
        if (interval < 1000) return;
        this.timer = new Timer();
        timer.Interval = interval;
        timer.Elapsed += new ElapsedEventHandler(SendHeartBeat);
        timer.Enabled = true;
        timeout = 0;
        lastTime = DateTime.Now;
    }

    public void Stop()
    {
        if (this.timer != null)
        {
            this.timer.Enabled = false;
            this.timer.Dispose();
        }
    }

}
