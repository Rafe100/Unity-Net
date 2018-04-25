using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BitTest : MonoBehaviour {

     byte[] d = new byte[]{ 1, 0, 1,1  };
    // Use this for initialization
    void Start()
    {
        Int16 l = BitConverter.ToInt16(d, 2);
        Debug.Log("result : " + l);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
