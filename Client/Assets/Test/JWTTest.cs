using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomProtocol;

public class JWTTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
        
        NetWorkManager.Initialize();
        NetWorkManager.Instance.InitializeNetClient();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.A))
        {
            NetWorkManager.Instance.Disconnect();
            NetWorkManager.Instance.InitializeNetClient("127.0.0.1 ",3002);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            //NetWorkManager.Instance.LoginTestReq("zz", "123");
            MsgClientTestLoginReq req = new MsgClientTestLoginReq();
            req.account = "zz";
            req.passwd = "123";
            NetWorkManager.Instance.JustSend(req);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            NetWorkManager.Instance.Disconnect();
            NetWorkManager.Instance.InitializeNetClient();
        }
        
	}
}
