using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using UnityEngine;

class HTTPServer : MonoBehaviour
{
    HttpListener listener = new();
    //delegate bool RequestHandler(HttpListenerRequest req);
    public event EventHandler<(Uri url, WebSocket socket)> WebsocketHandler;
    List<string> wsproto = [];

    void Awake()
    {
        listener.Prefixes.Add("http://localhost:8111");
    }

    bool kill = true;

    async void StartServer()
    {
        while (kill)
        {
            HttpListenerContext? context = await listener.GetContextAsync();
            if (context == null) return;
            string path = context.Request.Url.LocalPath;
            if (NOX.Resources.GetResourceNames().Contains(path.Substring(1)))
            {
                
            }
        }
    }
}