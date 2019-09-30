using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extension
{
    public static void Lg(string str) {
        string ex = "<color=green>";
        string eb = "</color>";
        Debug.Log(ex + str + eb);
    }
}
