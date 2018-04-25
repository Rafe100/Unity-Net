using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Timers;

public class TimerTest : MonoBehaviour {

    public double Interval = 2000.0d;
    Timer timer;
    // Use this for initialization
    void Start()
    {
        this.timer = new Timer();
        timer.Interval = Interval;
        timer.Elapsed += new ElapsedEventHandler(sendHeartBeat);
        timer.Enabled = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void sendHeartBeat(object source, ElapsedEventArgs e)
    {
        Debug.Log("interval elapse");
    }
    void OnDestroy()
    {
        this.timer.Stop();
    }
}
