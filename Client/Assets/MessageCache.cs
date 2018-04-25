using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MessageCache
{

    public MessageCache(Int32 maxLength)
    {
        _cache = new byte[maxLength];
        _rdPtr = 0;
        _wrPtr = 0;
    }
    private byte[] _cache;
    private Int32 _rdPtr;
    private Int32 _wrPtr;
}
