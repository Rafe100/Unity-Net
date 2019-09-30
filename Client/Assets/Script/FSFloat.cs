using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct FSFloat 
{
    public const int precision = 10000;
    int val;

    public FSFloat(int v) {
        val = v;
    }

    public int GetValue() {
        return val;
    }

    public float ToFloat() {
        return val * 0.0001f;
    }


    public static bool operator ==(FSFloat a,FSFloat b) {
        return a.GetValue() == b.GetValue();
    }

    public static bool operator !=(FSFloat a, FSFloat b) {
        return a.GetValue() == b.GetValue();
    }

    public static bool operator >(FSFloat a, FSFloat b) {
        return a.GetValue() > b.GetValue();
    }

    public static bool operator <(FSFloat a, FSFloat b) {
        return a.GetValue() < b.GetValue();
    }

    public static bool operator >=(FSFloat a, FSFloat b) {
        return a.GetValue() >= b.GetValue();
    }

    public static bool operator <=(FSFloat a, FSFloat b) {
        return a.GetValue() <= b.GetValue();
    }

    public static implicit operator FSFloat(int t) {
        return new FSFloat(t);
    }

    public static explicit operator FSFloat(float t) {
        return new FSFloat((int)t);
    }

}
