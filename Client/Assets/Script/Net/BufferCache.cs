using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BufferCache {

    public int ReadPtr {
        get { return this.readPtr; }
        set { this.readPtr = value; }
    }
    public int WritePtr {
        get { return this.writePtr; }
    }

    public byte[] Buffer {
        get { return this.buffer; }
    }

    int readPtr;
    int writePtr;
    byte[] buffer;

    public MemoryStream GetMemoryStream(int dataLength) {
        MemoryStream ms = new MemoryStream(buffer, readPtr, dataLength, false);
        readPtr += dataLength;
        return ms;
    }

    public BufferCache(int len = 1024 * 64) {
        buffer = new byte[len];
        writePtr = 0;
        readPtr = 0;
    }

    public int Length {
        get {
            return this.writePtr - this.readPtr;
        }
    }

    public int Space {
        get { return buffer.Length - this.writePtr; }
    }

    public int Write(byte[] data , int dataLength) {

        if(dataLength > Space) {
            throw new Exception("not enough space for buffer");
        }
        Array.Copy(data, 0, buffer, writePtr, dataLength);
        writePtr += dataLength;
        return dataLength;
    }

    public void LogPtr() {
        Extension.Lg("Buffer Cache readPtr:" + readPtr + "  wirtePtr:" + WritePtr);
    }

    public UInt16 ReadUInt16NotAddPtr() {
        UInt16 v = BitConverter.ToUInt16(buffer, readPtr);
        return v;
    }

    public UInt16 ReadUInt16() {
        UInt16 v = BitConverter.ToUInt16(buffer, readPtr);
        readPtr++;
        readPtr++;
        return v;
    }

    public Int32 Read3Byte() {
        Int32 t = 0;
        byte t0 = buffer[this.readPtr];
        this.readPtr++;
        byte t1 = buffer[this.readPtr];
        this.readPtr++;
        byte t2 = buffer[this.readPtr];
        this.readPtr++;
        t = t2 * 256 * 256 + t1 * 256 + t0;
        return t;
    }

    public void Crunch() {
        if(this.readPtr == 0) {
            return;
        }
        if(readPtr == writePtr) {
            this.readPtr = 0;
            this.writePtr = 0;
        } else {
            Array.Copy(this.buffer, readPtr, buffer, 0, writePtr - readPtr);
            writePtr = writePtr - readPtr;
            readPtr = 0;
        }
    }


    public void Clear() {
        this.writePtr = 0;
        this.readPtr = 0;
    }

}
