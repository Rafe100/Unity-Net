using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class ThreadTest : MonoBehaviour {


    private ThreadTestObject tObject = new ThreadTestObject();
    Thread t;

	// Use this for initialization
	void Start () {
        tObject.name = "98k";
        tObject.index = 1;
        t = new Thread(processNext);
        t.Start();
	}
	
	// Update is called once per frame
	void Update () {

        if(Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("index --" + tObject.index);
        }
		
	}

    void  processNext()
    {
        tObject.index++;
    }

}

class ThreadTestObject
{
    public string name;
    public int index;
}