using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomProtocol;
using System;

public class ProtoBufTest : MonoBehaviour {
    
    Transporter t = new Transporter(null, new NetClient(),null);
	// Use this for initialization
    void Start()
    {
        MsgClientLoginRsp req = new MsgClientLoginRsp();
        UInt16 id;
        req.gate_ip = "192.11.111.1";
        req.gate_port = 1001;
        var sengment = t.Encode(req,out id);

        ReceiveData r = t.Decode(sengment.Array, sengment.Array.Length);
        Debug.Log("id:" + r.MsgId);
        Debug.Log(r.MsgObject.GetType().ToString());
        MsgClientLoginRsp d= r.MsgObject as MsgClientLoginRsp;
        Debug.Log(d.gate_ip + ":" + d.gate_port);
    }
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.A))
        {
            Empty req = new Empty();
            UInt16 id;
            var sengment = t.Encode(req, out id);

            ReceiveData r = t.Decode(sengment.Array, sengment.Array.Length);
            Debug.Log("id:" + r.MsgId);
            Debug.Log(r.MsgObject.GetType().ToString());
        }
	}
}
