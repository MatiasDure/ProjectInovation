using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Text;
using WebSockets;
using TMPro;
using System;
using UnityEditor.PackageManager;
using UnityEngine.TextCore.Text;
using UnityEngine.XR;
using UnityEngine.SceneManagement;

public class SimpleServerDemo : MonoBehaviour
{
    public static SimpleServerDemo Instance { get; private set; }

    private const string JOIN_REQUEST = "j";
    private const string MOVE_REQUEST = "m";
    private const string CHAR_SELECT_REQUEST = "cs";
    private const string SWITCH_SCENE_REQUEST = "ss";

    [SerializeField] PlayerMovement testObj;
    [SerializeField] PlayerMovement[] testObjs;
    [SerializeField] AudioSource audioSrc;
    [SerializeField] byte amountPlayersAllowed = 4;
    [SerializeField] TextMeshProUGUI amountPlayers;

    List<WebSocketConnection> clients;
    WebsocketListener listener;
    Dictionary<int, PlayerMovement> idPlayerObj = new();
    List<WebSocketClient> cls = new();
    private int ids = 0;

    public static event Action<int> OnClientConnected;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else Destroy(this.gameObject);
    }

    void Start()
    {
        // Create a server that listens for connection requests:
        listener = new WebsocketListener();
        listener.Start();

        // Create a list of active connections:
        clients = new List<WebSocketConnection>();

        SceneManager.sceneLoaded += (Scene scene, LoadSceneMode mode) =>
        {
            if(scene.name.Equals("TestV2"))
            {
                foreach (WebSocketClient c in cls)
                {
                    foreach (PlayerMovement pi in testObjs)
                    {
                        var info = pi.gameObject.GetComponent<PlayerInfo>();
                        if (!info.CharName.Equals(c.SelectedChar.ToString())) continue;

                        idPlayerObj.Add(c.id, Instantiate(pi));
                    }
                }
            }
        };
    }

    void Update()
    {
        // Check for new connections:
        listener.Update();

        if (amountPlayersAllowed > clients.Count)
        {
            while (listener.Pending()) {
                WebSocketConnection ws = listener.AcceptConnection(OnPacketReceive);
                clients.Add(ws);
                cls.Add(new WebSocketClient(ids, ws));
                amountPlayers.text = clients.Count + " / 4";
                //keyValuePairs.Add(ids, Instantiate(testObj));
                byte[] buffer = Encoding.UTF8.GetBytes("ja:"+ids++);
                NetworkPacket packet = new(buffer);
                ws.Send(packet);
                Debug.Log("A client connected from " + ws.RemoteEndPoint.Address);
                OnClientConnected?.Invoke(clients.Count);

                //inform all clients of new client connected
            }
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

    private int amountCalled = 0;

    void OnPacketReceive(NetworkPacket packet, WebSocketConnection connection) {

        WebSocketClient cl = null;
        foreach (WebSocketClient client in cls)
        {
            if (client.clientConnection == connection)
            {
                cl = client;
            }
        }

        string text = Encoding.UTF8.GetString(packet.Data);
        Console.WriteLine("Received a packet: {0}", text);

        byte[] bytes;

        if (!text.Equals("filler data"))
        {
            string[] division = text.Split(":");

            Debug.Log(text);
            string id = division[0];
            string header = division[1];

            Debug.LogWarning("header " + header);

            if(header.Equals(MOVE_REQUEST))
            {
                Debug.Log("-------------We are moving by: " + division[2]);
                string[] vectorStr = division[2].Split(",");
                Vector3 vecT = new(float.Parse(vectorStr[0]), float.Parse(vectorStr[1]));
                idPlayerObj[int.Parse(id)].Move(vecT / 500);
                audioSrc.Play();
            }
            else if (header.Equals(JOIN_REQUEST))
            {
                Debug.LogWarning("Clients wants to join: ");
                //keyValuePairs.Add(ids++, Instantiate(testObj));
                audioSrc.Play();
                //bytes = Encoding.UTF8.GetBytes("ja"); //sending join accepted
                //connection.Send(new NetworkPacket(bytes));
                //cl.clientConnection.Send(new NetworkPacket(bytes));
            }
            else if(header.Equals(CHAR_SELECT_REQUEST))
            {
                cl.SetCharacter(division[2]);
                string chosenChar = division[2];
                Debug.Log(chosenChar);

                //checking that all clients chose a skin
                if (clients.Count < 1) return;

                foreach(WebSocketClient c in cls)
                {
                    if (c.SelectedChar == CharacterManager.Characters.none) return;
                }

                byte[] outString = Encoding.UTF8.GetBytes("cc:" + clients.Count);
                NetworkPacket informPacket = new NetworkPacket(outString);
                Broadcast(informPacket);
                
            }
            else if(header.Equals(SWITCH_SCENE_REQUEST))
            {
                if(++amountCalled == clients.Count)
                {
                    SceneManager.LoadScene("TestV2");
                }
            }
            Debug.LogWarning("-------------------------------------------------------------------"+header);
        }
        //// echo:
        string response = "You said: " + text;
        bytes = Encoding.UTF8.GetBytes(response);
        //connection.Send(new NetworkPacket(bytes));
        cl.clientConnection.Send(new NetworkPacket(bytes));
        // broadcast:
        string message = cl.clientConnection.RemoteEndPoint.ToString() + " says: " + text;// connection.RemoteEndPoint.ToString() + " says: " + text;
        bytes = Encoding.UTF8.GetBytes(message);
        Broadcast(new NetworkPacket(bytes));
    }

    void Broadcast(NetworkPacket packet) {
        foreach (var cl in clients) {
            cl.Send(packet);
        }
    }
}

class WebSocketClient
{
    public int id { get; private set; }
    public WebSocketConnection clientConnection { get; private set; }
    public CharacterManager.Characters SelectedChar { get; private set; }

    public static event Action<int, string> OnCharSelected;
    public static event Action<int, string> OnChangedCharacter;

    public WebSocketClient(int pId, WebSocketConnection pClientConnection)
    {
        id = pId;
        clientConnection = pClientConnection;
        SelectedChar = CharacterManager.Characters.none;
    }

    public void SetCharacter(string character)
    {
        if (!CharacterManager.Instance.IsCharacterAvailable(character) ||
            !TryParseStringToCharacter(character, out CharacterManager.Characters parsedChar)) return;

        if (SelectedChar != CharacterManager.Characters.none) ChangeCharacter(parsedChar);
        else
        {
            SelectedChar = parsedChar;
            OnCharSelected.Invoke(id, character);
            Debug.LogWarning($"client with id {id} chose character {SelectedChar}");
        }


        string response = "ca:"+SelectedChar.ToString();
        byte[] bytes = Encoding.UTF8.GetBytes(response);
        //connection.Send(new NetworkPacket(bytes));
        clientConnection.Send(new NetworkPacket(bytes));
    }

    public void ChangeCharacter(CharacterManager.Characters character)
    {
        SelectedChar = character;
        OnChangedCharacter.Invoke(id, character.ToString());
    }

    private bool TryParseStringToCharacter(string character, out CharacterManager.Characters parsedChar)
    {
        if (!Enum.TryParse(typeof(CharacterManager.Characters), character, out object selectedChar))
        {
            parsedChar = CharacterManager.Characters.none;
            return false;
        }
        parsedChar = (CharacterManager.Characters) selectedChar;
        return true;
    }
}
