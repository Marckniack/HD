using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.XR;
//using Leap.Unity;

public class PlayerBehaviour : NetworkBehaviour
{

    public GameObject playerModel;
    public GameObject playerBackground;
    //public HandModelManager playerHands;
    public string playerName;

    //Variabili per interpolazione players
    [SyncVar]
    Vector3 realPosition = Vector3.zero;
    [SyncVar]
    Quaternion realRotation;

    private float updateInterval;

    [Range(1f, 30f)]
    public float updatesForSecond = 9f;

    [Range(0.1f, 1.0f)]
    public float lerpSpeed = 0.1f;


    // Use this for initialization
    void Start()
    {
        //Se lo script è attaccato al modello del player locale
        if (isLocalPlayer)
        {
            XRSettings.enabled = true;
            playerModel.SetActive(false);
            //Assegno ip al nome
            playerName = NetworkManager.singleton.networkAddress;
            //Dico di cambiare il nome
            CmdSendNameToServer(playerName);

        }
    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            GetComponent<Camera>().enabled = false;

            //Aggiorno la posizione dei player connessi
            //Movimento
            transform.position = Vector3.Lerp(transform.position, realPosition, 0.1f);
            transform.rotation = Quaternion.Lerp(transform.rotation, realRotation, 0.1f);
        }
        else
        {
            // intervallo di aggiornamento server
            updateInterval += Time.deltaTime;
            if (updateInterval > 1f / updatesForSecond)
            {
                updateInterval = 0;
                CmdSyncPlayerTransform(transform.position, transform.rotation);
            }

        }

    }

    [Command]
    void CmdSendNameToServer(string nameToChange)
    {
        RpcSetPlayerName(nameToChange);
    }

    [ClientRpc]
    void RpcSetPlayerName(string nameToChange)
    {
        gameObject.name = nameToChange;
    }


    /// <summary>
    /// Sincronizzazione con il server
    /// </summary>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    [Command]
    void CmdSyncPlayerTransform(Vector3 position, Quaternion rotation)
    {
        realPosition = position;
        realRotation = rotation;
    }

}

class PlayerInfoMessage : MessageBase
{
    public bool isTeacher;
    public int spawnPoint;
}