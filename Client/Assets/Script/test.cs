using CustomProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Vector2 a = new Vector2(0f, 0f);
        //Vector2 b = new Vector2(1.0f, 1.0f);

        //var v = Vector2.Lerp(a, b,0.3f);
        //FSFloat t = 1;
        //Debug.Log(v.ToString());
        //FSQuaternion r = new FSVector4(1,1,1,1);
        //Quaternion

        var r = new Transporter(null,null);
        var obj = new CustomProtocol.PlayerConnectRsp();
        obj.playId = 1001;
        obj.udpPort = 3001;
        ushort t = 0;
        var seg = r.Encode(obj,out t);
        var c = new BufferCache();
        c.Write(seg.Array, seg.Array.Length);
        var y = r.Decode(c);
        Debug.Log("id:" + y.MsgId);
        var result = y.MsgObject as PlayerConnectRsp;
        Debug.Log("playId:" + result.playId + " "+ result.udpPort);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
