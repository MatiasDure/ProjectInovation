using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Text;
using WebSockets;
using TMPro;
using System;
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
    List<WebSocketClient> faultyClients = new();
    private int ids = 0;

    //to move
    bool canMove = false;

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
                WinnerJson.WriteString("players", "", false);   
                foreach (WebSocketClient c in cls)
                {
                    foreach (PlayerMovement pi in testObjs)
                    {
                        var info = pi.gameObject.GetComponent<PlayerInfo>();
                        if (!info.CharName.Equals(c.SelectedChar.ToString())) continue;

                        WinnerJson.WriteString("players",info.CharName, true);
                        idPlayerObj[c.id] = Instantiate(pi);
                        Debug.LogWarning(c.id);
                    }
                }
                canMove = true;
            }
        };

        CheckWinCondition.OnPlayerWon += (charName) =>
        {
            string text = $"gf:{charName}";
            byte[] outString = Encoding.UTF8.GetBytes(text);
            NetworkPacket packet = new NetworkPacket(outString);
            Broadcast(packet);

            SceneManager.LoadScene("FinishGameScene");
        };

    }

    void Update()
    {
        // Check for new connections:
        listener.Update();

        if (amountPlayersAllowed > cls.Count)
        {
            while (listener.Pending())
            {
                WebSocketConnection ws = listener.AcceptConnection(OnPacketReceive);
                clients.Add(ws);
                cls.Add(new WebSocketClient(ids, ws));
                Debug.LogWarning(cls[cls.Count - 1]);

                amountPlayers.text = cls.Count + " / 4";
                //keyValuePairs.Add(ids, Instantiate(testObj));
                byte[] buffer = Encoding.UTF8.GetBytes("ja:" + ids++);
                NetworkPacket packet = new(buffer);
                ws.Send(packet);
                Debug.Log("A client connected from " + ws.RemoteEndPoint.Address);
                OnClientConnected?.Invoke(clients.Count);

                //inform all clients of new client connected
            }
        }

        for (int i = 0; i < cls.Count; i++)
        {
            if (cls[i].clientConnection.Status == ConnectionStatus.Connected)
            {
                cls[i].clientConnection.Update();
            }
            else
            {
                faultyClients.Add(cls[i]);
                Debug.LogWarning(string.Format("Removing disconnected client. #active clients: {0}", (cls.Count - 1).ToString()));
            }
        }

        CheckForFaultyClients();
        CleanFaultyClients();
    }

    private void CleanFaultyClients()
    {
        int previousAmount = cls.Count;
        foreach (WebSocketClient faultyClient in faultyClients)
        {
            Debug.LogWarning(faultyClient);
            cls.Remove(faultyClient);
        }
        faultyClients.Clear();

        if (previousAmount != cls.Count) amountPlayers.text = cls.Count + " / 4";
    }

    private void CheckForFaultyClients()
    {
        foreach (WebSocketClient client in cls)
        {
            if ((DateTime.Now - client.LastHeartBeat).TotalSeconds > 1)
            {
                faultyClients.Add(client);
                Debug.LogWarning(string.Format("Removing client with lateHeartbeat. #active clients: {0}", (cls.Count - 1).ToString()));
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

        WebSocketClient cl = FindClient(connection);

        if (cl == null) return;

        cl.UpdateHeartBeat();

        string text = Encoding.UTF8.GetString(packet.Data);

        //HandleStringPacket(text);

        byte[] bytes;

        if (!text.Equals("filler data"))
        {
            string[] division = text.Split(":");

            //We do this because the client didnt follow the criteria for string packets (id:request:args[])
            if (division.Length < 2) return;
            Debug.LogWarning(text);
            string id = division[0];
            string header = division[1];

            Debug.LogWarning("header " + header);

            if(header.Equals(MOVE_REQUEST))
            {
                if (canMove)
                {
                    Debug.Log("-------------We are moving by: " + division[2]);
                    string[] vectorStr = division[2].Split(",");
                    Vector3 vecT = new(float.Parse(vectorStr[0]), float.Parse(vectorStr[1]));
                    idPlayerObj[int.Parse(id)].Move(vecT / 500);
                    audioSrc.Play();
                }
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
                if(++amountCalled == cls.Count)
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
        string message = cl.clientConnection.RemoteEndPoint.ToString() + " says: " + text;
        bytes = Encoding.UTF8.GetBytes(message);
        Broadcast(new NetworkPacket(bytes));
    }

    private void Broadcast(NetworkPacket packet) 
    {
        foreach (var cl in cls) 
        {
            cl.clientConnection.Send(packet);
        }
    }

    private WebSocketClient FindClient(WebSocketConnection connection)
    {
        foreach (WebSocketClient client in cls)
        {
            if (client.clientConnection != connection) continue;
            
            return client;
        }
        return null;
    }

    private void HandleStringPacket(string stringPacket)
    {
        Console.WriteLine("Received a packet: {0}", stringPacket);
    }

    //private void HandleMoveRequest(int id, int vectorString)
    //{
    //    if (canMove)
    //    {
    //        Debug.Log("-------------We are moving by: " + vectorString);
    //        string[] vectorStr = vectorString.Split(",");
    //        Vector3 vecT = new(float.Parse(vectorStr[0]), float.Parse(vectorStr[1]));
    //        idPlayerObj[int.Parse(id)].Move(vecT / 500);
    //        audioSrc.Play();
    //    }
    //}
}

class WebSocketClient
{
    public int id { get; private set; }
    public WebSocketConnection clientConnection { get; private set; }
    public CharacterManager.Characters SelectedChar { get; private set; }
    public DateTime LastHeartBeat { get; private set; }

    public static event Action<int, string> OnCharSelected;
    public static event Action<int, string> OnChangedCharacter;

    public WebSocketClient(int pId, WebSocketConnection pClientConnection)
    {
        id = pId;
        clientConnection = pClientConnection;
        SelectedChar = CharacterManager.Characters.none;
        LastHeartBeat = DateTime.Now;
    }

    public void UpdateHeartBeat() => LastHeartBeat = DateTime.Now;

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
