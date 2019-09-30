using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct FSVector4 
{
    FSFloat xValue;
    FSFloat yValue;
    FSFloat zValue;
    FSFloat wValue;

    public FSVector4(int x, int y, int z,int w) {
        xValue = x;
        yValue = y;
        zValue = z;
        wValue = w;
    }

    public Vector4 ToVec4() {
        return new Vector4(xValue.ToFloat(), yValue.ToFloat(), zValue.ToFloat(),wValue.ToFloat());
    }

    public FSFloat GetX() {
         return xValue; 
    }

    public FSFloat GetY() {
        return yValue;
    }

    public FSFloat GetZ() {
        return zValue;
    }

    public FSFloat GetW() {
        return wValue;
    }

    public static explicit operator FSVector4(Vector4 v) {
        return new FSVector4((int)v.x, (int)v.y, (int)v.z,(int)v.w);
    }

}
