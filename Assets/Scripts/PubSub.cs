using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PubSub
{
    private static Queue<string> queue = new Queue<string>();
    private static Dictionary<string, List<MessageHandler>> listeners = new Dictionary<string, List<MessageHandler>>();

    public delegate void MessageHandler();

    public static void Send(string msg)
    {
        List<MessageHandler> handlers;
        if (listeners.TryGetValue(msg, out handlers))
        {
            foreach (var handler in handlers)
            {
                handler();
            }
        }
    }

    public static void Subscribe(string msg, MessageHandler handler)
    {
        List<MessageHandler> handlers;
        if (!listeners.TryGetValue(msg, out handlers))
        {
            handlers = new List<MessageHandler>();
            listeners.Add(msg, handlers);
        }
        handlers.Add(handler);
    }
}
