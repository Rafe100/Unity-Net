using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessagHandler 
{
    Dictionary<int, Action<ReceiveData>> handlers = new Dictionary<int, Action<ReceiveData>>();
    Dictionary<int, Action<ReceiveData>> onceHandlers = new Dictionary<int, Action<ReceiveData>>();

    public void Regist(int key,Action<ReceiveData> callBack) {
        if (!handlers.ContainsKey(key)) {
            handlers.Add(key, null);
        }
        handlers[key] += callBack;
    }

    public void RegistOnce(int key, Action<ReceiveData> callBack) {
        if (!onceHandlers.ContainsKey(key)) {
            onceHandlers.Add(key, null);
        }
        onceHandlers[key] += callBack;
    }

    public void Dispatch(ReceiveData rev) {
        if (handlers.ContainsKey(rev.MsgId)) {
            var handler = handlers[rev.MsgId];
            if (handler != null) {
                handler(rev);
            }
        }
        if (onceHandlers.ContainsKey(rev.MsgId)) {
            var handler = onceHandlers[rev.MsgId];
            if (handler != null) {
                handler(rev);
                onceHandlers[rev.MsgId] = null;
            }
        }
    }

}
