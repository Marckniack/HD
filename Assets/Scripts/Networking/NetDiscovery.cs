using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Networking;
using System;

public class NetDiscovery : NetworkDiscovery {
    
    #region Singleton

    //Funziona anche se il singleton cè gia nella classe padre

    public static NetDiscovery instance = null;

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
    }

    #endregion

    /// <summary>
    /// Metodo che avvisa quando è stato ricevuto un broadcast
    /// </summary>
    /// <param name="fromAddress"></param>
    /// <param name="data"></param>
    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        
        if (!NetManager.instance.isHost)
        {
            NetManager.instance.networkAddress = fromAddress.Substring(fromAddress.LastIndexOf(":") + 1);
            
            if (!NetManager.instance.isTeacher)
            {
                MenuManager.instance.ShowSync();
            }
            else
            {
                NetManager.instance.InitSync();
            }


        }

    }
}
