using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct FSVector3
{
    FSFloat xValue;
    FSFloat yValue;
    FSFloat zValue;

    public FSVector3(int x, int y, int z) {
        xValue = x;
        yValue = y;
        zValue = z;
    }

    public Vector3 ToVec3() {
        return new Vector3(xValue.ToFloat(), yValue.ToFloat(), zValue.ToFloat());
    }

    public static explicit operator FSVector3(Vector3 v) {
        return new FSVector3((int)v.x, (int)v.y, (int)v.z);
    }

}
