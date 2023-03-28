// This is a rudimentary implementation of a websocket server. Feel free to extend it.
//
// Inspired by https://developer.mozilla.org/en-US/docs/Web/API/WebSockets_API/Writing_WebSocket_server
//
// See also: https://hpbn.co/websocket/
//
// Other side of the coin: implementing client-side websockets in Unity WebGL build:
//
//  https://gamedev.stackexchange.com/questions/176842/how-can-i-use-websockets-in-a-unity-webgl-project

using System;
using System.Text;
using System.Threading;
using System.Collections.Generic;

using WebSockets;

class ServerDemo {
    static List<WebSocketConnection> clients;

    public static void Broadcast(NetworkPacket packet) {
        foreach (var cl in clients) {
            cl.Send(packet);
		}
	}

    // This is an example of server functionality (just an echo and a broadcast + some very simple string processing)
    //  Add your own server functionality here.
    public static void OnPacketReceive(NetworkPacket packet, WebSocketConnection connection) {
        string text = Encoding.UTF8.GetString(packet.Data);
        Console.WriteLine("Received a packet: {0}",text);

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


    public static void Main() {
        clients = new List<WebSocketConnection>();

        WebsocketListener listener = new WebsocketListener();
        listener.Start();

        while (true) {
            Thread.Sleep(1);

            // Check for new connections:
            listener.Update();
            while (listener.Pending()) {
                WebSocketConnection ws = listener.AcceptConnection(OnPacketReceive);
                clients.Add(ws);
                Console.WriteLine("A client connected from "+ws.RemoteEndPoint.Address);
            }

            // Process current connections (this may lead to a callback to OnPacketReceive):
            for (int i=0;i<clients.Count;i++) {
                if (clients[i].Status==ConnectionStatus.Connected) {
                    clients[i].Update(); 
				} else {
                    clients.RemoveAt(i);
					Console.WriteLine("Removing disconnected client. #active clients: {0}",clients.Count);
                    i--;
				}
			}
        }
    }
}
