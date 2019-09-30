using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct FSQuaternion 
{
    public FSFloat xValue;
   
    public FSFloat yValue;
  
    public FSFloat zValue;
    
    public FSFloat wValue;

  
    public FSQuaternion(int x,int y,int z,int w) {
        xValue = x;
        yValue = y;
        zValue = z;
        wValue = w;
    }

    public FSQuaternion(FSFloat x, FSFloat y, FSFloat z, FSFloat w) {
        xValue = x;
        yValue = y;
        zValue = z;
        wValue = w;
    }

    public FSQuaternion(FSVector4 v) {
        xValue = v.GetX();
        yValue = v.GetY();
        zValue = v.GetZ();
        wValue = v.GetW();
    }

    public static implicit operator FSQuaternion(FSVector4 v) {
        return new FSQuaternion(v);
    }

    public Quaternion ToQuaternion() {
        return new Quaternion(xValue.ToFloat(), yValue.ToFloat(), zValue.ToFloat(), wValue.ToFloat());
    }



}
