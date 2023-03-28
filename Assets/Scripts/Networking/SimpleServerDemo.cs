using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Text;
using WebSockets;

public class SimpleServerDemo : MonoBehaviour
{
    List<WebSocketConnection> clients;
    WebsocketListener listener;

    void Start()
    {
        // Create a server that listens for connection requests:
        listener = new WebsocketListener();
        listener.Start();

        // Create a list of active connections:
        clients = new List<WebSocketConnection>();
    }

    void Update()
    {
        // Check for new connections:
        listener.Update();
        while (listener.Pending()) {
            WebSocketConnection ws = listener.AcceptConnection(OnPacketReceive);
            clients.Add(ws);
            Debug.Log("A client connected from " + ws.RemoteEndPoint.Address);
        }

        // Process current connections (this may lead to a callback to OnPacketReceive):
        for (int i = 0; i < clients.Count; i++) {
            if (clients[i].Status == ConnectionStatus.Connected) {
                clients[i].Update();
            } else {
                clients.RemoveAt(i);
                Console.WriteLine("Removing disconnected client. #active clients: {0}", clients.Count);
                i--;
            }
        }
    }

    /// <summary>
    /// This method is called by WebSocketConnections when their Update method is called and a packet comes in.
    /// From here you can implement your own server functionality 
    ///   (parse the (string) package data, and depending on contents, call other methods, implement game play rules, etc). 
    /// Currently it only does some very simple string processing, and echoes and broadcasts a message.
    /// </summary>
    void OnPacketReceive(NetworkPacket packet, WebSocketConnection connection) {
        string text = Encoding.UTF8.GetString(packet.Data);
        Console.WriteLine("Received a packet: {0}", text);

        byte[] bytes;

        //// echo:
        string response = "You said: " + text;
        bytes = Encoding.UTF8.GetBytes(response);
        connection.Send(new NetworkPacket(bytes));

        //// broadcast:
        string message = connection.RemoteEndPoint.ToString() + " says: " + text;
        bytes = Encoding.UTF8.GetBytes(message);
        Broadcast(new NetworkPacket(bytes));
    }

    void Broadcast(NetworkPacket packet) {
        foreach (var cl in clients) {
            cl.Send(packet);
        }
    }
}
