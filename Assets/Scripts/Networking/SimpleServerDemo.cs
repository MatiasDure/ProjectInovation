using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using WebSockets;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using System.Net.Sockets;
using System.IO;
using System.Text.RegularExpressions;

public class SimpleServerDemo : MonoBehaviour
{
    public static SimpleServerDemo Instance { get; private set; }

    private const string JOIN_REQUEST = "j";
    private const string MOVE_REQUEST = "m";
    private const string CHAR_SELECT_REQUEST = "cs";
    private const string SWITCH_SCENE_REQUEST = "ss";
    private const string SELF_CHAR = "sc";
    private const int HEARTBEAT_DELAY_ALLOWED = 4;

    [SerializeField] PlayerMovement testObj;
    [SerializeField] PlayerMovement[] testObjs;
    [SerializeField] FollowObjectTransform waterBag;
    [SerializeField] byte amountPlayersAllowed = 4;
    [SerializeField] TextMeshProUGUI amountPlayers;

    List<WebSocketConnection> clients;
    WebsocketListener listener;
    Dictionary<int, PlayerMovement> idPlayerObj = new();
    List<WebSocketClient> cls = new();
    List<WebSocketClient> faultyClients = new();
    private int ids = 0;

    [SerializeField] string gamePlayScene; 

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
        bool portFound = false;
        int port = 4444;
        while (!portFound)
        {
            try
            {
                listener = new WebsocketListener(port);
                listener.Start();
                portFound = true;
            }
            catch (SocketException e)
            {
                port = UnityEngine.Random.Range(4444, 5555);
                Debug.Log(", new port = " + port);
            }
        }

        // Replace the port number in the JavaScript file
        string filePath = Path.Combine(Application.streamingAssetsPath, "../../TestClient/websocket.js");
        string fileContent = File.ReadAllText(filePath);
        string newContent = Regex.Replace(fileContent, @"(wsUri\+':)\d+(\/)", @"wsUri+':"+ port.ToString() + @"/");


        File.WriteAllText(filePath, newContent);


        // Create a list of active connections:
        clients = new List<WebSocketConnection>();

        PlayerMovement.OnPlayerLostHealth += (PlayerMovement player) =>
        {
            foreach(var playerMovement in idPlayerObj)
            {
                if (playerMovement.Value != player) continue;

                WebSocketClient clientWhoLostLife = FindClientById(playerMovement.Key);

                string lostLife = "lh";
                NetworkPacket outPacket = new(Encoding.UTF8.GetBytes(lostLife));
                clientWhoLostLife.clientConnection.Send(outPacket);
            }
        };

        SceneManager.sceneLoaded += (Scene scene, LoadSceneMode mode) =>
        {
            if(scene.name.Equals(gamePlayScene))
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
                        idPlayerObj[c.id].transform.position = CheckPointManager.Instance.checkPointPositions[0].position + Vector2.up;

                        FollowObjectTransform newWaterBag = Instantiate(waterBag);
                        newWaterBag.toFollow = idPlayerObj[c.id].transform;
                        idPlayerObj[c.id].waterBag = newWaterBag;
                        idPlayerObj[c.id].rbToShader._renderer = newWaterBag._renderer;


                        idPlayerObj[c.id].info = info;
                        CameraFollow.instance.AddPlayerToFollow(idPlayerObj[c.id]);
                        Spline.Instance.AddPlayerToTrack(idPlayerObj[c.id]);
                        CheckPointManager.Instance.AddPlayer(idPlayerObj[c.id]);
                    }
                    try
                    {
                        string informCharacter = $"{SELF_CHAR}:{c.SelectedChar}";
                        NetworkPacket outPacket = new NetworkPacket(Encoding.UTF8.GetBytes(informCharacter));
                        c.clientConnection.Send(outPacket);
                    }
                    catch
                    {
                        faultyClients.Add(c);
                    }
                }
                canMove = true;
            }
        };

        CheckPointManager.OnPlayerWon += (charName) =>
        {
            canMove = false;
            WinnerJson.WriteString("winner", charName, false);
            string text = $"gf:{charName}";
            byte[] outString = Encoding.UTF8.GetBytes(text);
            NetworkPacket packet = new NetworkPacket(outString);
            Broadcast(packet);
            SceneManager.LoadScene("FinishGameScene");
        };
    }

    void Update() { 
            // Check for new connections:
        listener.Update();

        if (amountPlayersAllowed > cls.Count && !canMove)
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
               // Debug.Log("A client connected from " + ws.RemoteEndPoint.Address);
                OnClientConnected?.Invoke(clients.Count);
                SoundManager.Instance.PlaySound(SoundManager.Sound.PlayerJoin);

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
            cls.Remove(faultyClient);

            //players not spawned yet
            if (!canMove) continue;

            //if in game, destroy the instantiated player object
            Destroy(idPlayerObj[faultyClient.id].waterBag.gameObject);
            Destroy(idPlayerObj[faultyClient.id].gameObject);
            CameraFollow.instance.RemovePlayerToFollow(idPlayerObj[faultyClient.id]);
            Spline.Instance.RemovePlayerFromTrack(idPlayerObj[faultyClient.id]);
            CheckPointManager.Instance.RemovePlayer(idPlayerObj[faultyClient.id]);
        }
        faultyClients.Clear();

        if (previousAmount != cls.Count) amountPlayers.text = cls.Count + " / 4";
    }

    private void CheckForFaultyClients()
    {
        foreach (WebSocketClient client in cls)
        {
            //Debug.LogWarning(client.LastHeartBeat);
            if ((DateTime.Now - client.LastHeartBeat).TotalSeconds > HEARTBEAT_DELAY_ALLOWED)
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
            //Debug.LogWarning(text);
            string id = division[0];
            string header = division[1];

            //Debug.LogWarning("header " + header);

            if(header.Equals(MOVE_REQUEST))
            {
                if (canMove)
                {
                    string[] vectorStr = division[2].Split(",");
                    Vector3 vecT = new(float.Parse(vectorStr[0]), float.Parse(vectorStr[1]));
                    idPlayerObj[int.Parse(id)].Move(vecT / 500);
                }
            }
            else if(header.Equals(CHAR_SELECT_REQUEST))
            {
                //game already started
                if (canMove) return;

                string chosenChar = division[2];
                bool successfullyAssignedChar = cl.SetCharacter(chosenChar);
                
                if(successfullyAssignedChar)
                {
                    SoundManager.Instance.PlaySound(SoundManager.Sound.PlayerSelected);
                    byte[] outstring = Encoding.UTF8.GetBytes("csr:"+chosenChar);
                    NetworkPacket outPacket = new NetworkPacket(outstring);
                    Broadcast(outPacket, cl);
                }

                //checking that all clients selected characters
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
                //game already started
                if (canMove) return;

                if (++amountCalled == cls.Count)
                {
                    SceneManager.LoadScene(gamePlayScene);
                }
            }
           // Debug.LogWarning("-------------------------------------------------------------------"+header);
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

    private void Broadcast(NetworkPacket packet, WebSocketClient ignore = null) 
    {
        foreach (var cl in cls) 
        {
            if (cl == ignore) continue;

            try
            {
                cl.clientConnection.Send(packet);
            }
            catch
            {
                faultyClients.Add(cl);
            }
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

    private WebSocketClient FindClientById(int pId)
    {
        foreach(WebSocketClient client in cls)
        {
            if (client.id != pId) continue;

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

    public bool SetCharacter(string character)
    {
        if (!CharacterManager.Instance.IsCharacterAvailable(character) ||
            !TryParseStringToCharacter(character, out CharacterManager.Characters parsedChar)) return false;

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

        return true;
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
