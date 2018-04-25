using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

public class StreamTest : MonoBehaviour {

    private static byte[] fullBuf = new byte[1024];
    private static MemoryStream wrStream = new MemoryStream(fullBuf);
    private static BinaryWriter bnWriter = new BinaryWriter(wrStream);
	// Use this for initialization
	void Start () {
        bnWriter.Seek(2, SeekOrigin.Begin);
        bnWriter.Write(UInt16.MaxValue);
        bnWriter.Seek(0, SeekOrigin.Begin);
        bnWriter.Write((byte.MaxValue));
        bnWriter.Seek(1, SeekOrigin.Begin);
        bnWriter.Write((byte.MaxValue));
        bnWriter.Seek(0, SeekOrigin.Begin);
        bnWriter.Write((byte.MinValue));
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
