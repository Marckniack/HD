using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using System;

public class NetManager : NetworkManager
{

    #region Singleton

    //Funziona anche se il singleton cè gia nella classe padre

    public static NetManager instance = null;

    private void Awake()
    {

        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }
        DontDestroyOnLoad(this);
    }

    #endregion

    [HideInInspector]
    public bool isHost = false;
    [HideInInspector]
    public bool isTeacher = false;
    
    [SerializeField] GameObject teacherPrefab;
    [SerializeField] GameObject studentPrefab;

    public Animator roofAnimator;
    public int spawnPointIndex;

    void Start()
    {
        XRSettings.enabled = false;
        NetDiscovery.instance.Initialize();
    }

    public void InitServer()
    {
        isHost = true;
        NetDiscovery.instance.StartAsServer();
        StartServer();

    }

    public void InitClient()
    {
        NetDiscovery.instance.StartAsClient();
    }

    public void InitSync()
    {
        StartClient();
    }


    #region Overrides

    /// <summary>
    /// Chiamato sul server quando un client si disconnette dal server
    /// </summary>
    /// <param name="conn"></param>
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);

        if (!isHost)
        {
            NetDiscovery.instance.StopBroadcast();
            SceneManager.LoadScene("Connection");
        }

    }

    /// <summary>
    /// Chiamato sul client quando si disconnette dal server
    /// </summary>
    /// <param name="conn"></param>
    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);

        NetDiscovery.instance.StopBroadcast();
        SceneManager.LoadScene("Connection");
        
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader)
    {
        GameObject playerPrefab;
        GameObject player;
        Transform startPos;
        PlayerInfoMessage msg;

        msg = extraMessageReader.ReadMessage<PlayerInfoMessage>();


        if (msg.isTeacher)
        {
            playerPrefab = teacherPrefab;
            player = (GameObject)Instantiate(playerPrefab, SpawnManager.instance.teacherPos, Quaternion.identity);
        }
        else
        {
            playerPrefab = studentPrefab;
            startPos = SpawnManager.instance.spawnPoints[msg.spawnPoint];
            player = (GameObject)Instantiate(playerPrefab, startPos.position, startPos.rotation);
        }

       
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);

    }

    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        // I need to call this when the scene preparation on the client is completed
        ClientScene.Ready(conn);

        // Create a PlayerInfoMessage object and store player information
        PlayerInfoMessage msg = new PlayerInfoMessage();

        if (isTeacher)
        {
            msg.isTeacher = true;
        }
        else
        {
            msg.isTeacher = false;
        }

        //SpawnPoint scelto dal player
        msg.spawnPoint = spawnPointIndex;

        // Send the AddPlayer message to the server.
        // At that time, add additional information (PlayerInfoMessage) to the third argument.
        ClientScene.AddPlayer(conn, 0, msg);
    }

    /// <summary>
    /// Quando il server si connette creo una camera per se stesso
    /// </summary>
    /// <param name="sceneName"></param>
    public override void OnServerSceneChanged(string sceneName)
    {

        if (isHost)
        {
            GameObject serverCamera = new GameObject("Server Camera");
            serverCamera.AddComponent<Camera>();

            serverCamera.transform.SetPositionAndRotation(new Vector3(0, 3, -7), Quaternion.Euler(30, 0, 0));
        }

    }




    #endregion

}
