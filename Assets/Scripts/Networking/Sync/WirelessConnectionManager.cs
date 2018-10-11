using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;


public class WirelessConnectionManager : MonoBehaviour {

    public string WirelessSSID = "Holodeck";
    public string WirelessPassword = "testingDeck";

    Process process = new Process();
    ProcessStartInfo startInfo = new ProcessStartInfo();

    /// <summary>
    /// Funzione che crea una rete Wireless Ad Hoc dal server
    /// </summary>
    void CreateNetwork()
    {
        //Eseguo il cmd come admin e eseguo comandi bash per creare una rete ad hoc e avviarla
        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
        startInfo.FileName = "powershell.exe";
        startInfo.Arguments = "/C Start-Process 'cmd' -Verb runAs -ArgumentList netsh wlan del filter permission=denyall networktype=adhoc " +
            "& netsh wlan set hostednetwork mode=allow ssid="+ WirelessSSID + " key="+ WirelessPassword + " " +
            "& netsh wlan start hostednetwork";
        process.StartInfo = startInfo;
        process.Start();
    }

    /// <summary>
    /// Funzione che ferma la rete Wireless Ad Hoc Creata dal server
    /// </summary>
    void StopCreatedNetwork()
    {
        //Eseguo il cmd come admin e eseguo comandi bash per bloccare la rete ad hoc creata
        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
        startInfo.FileName = "powershell.exe";
        startInfo.Arguments = "/C Start-Process 'cmd' -Verb runAs -ArgumentList netsh wlan stop hostednetwork";
        process.StartInfo = startInfo;
        process.Start();
    }

    /// <summary>
    /// Funzione che Connette il client alla rete Ad Hoc creata dal server
    /// </summary>
    void ConnectToNetwork()
    {
        //Eseguo il cmd come admin e eseguo comandi bash per Connettere il client alla rete ad hoc creata dal server
        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
        startInfo.FileName = "powershell.exe";
        startInfo.Arguments = "/C Start-Process 'cmd' -Verb runAs -ArgumentList netsh wlan connect ssid='"+WirelessSSID+"' key='"+ WirelessPassword + "' interface='Wi-Fi'";
        process.StartInfo = startInfo;
        process.Start();
    }

    /// <summary>
    /// Funzione che Controlla se il client è connesso al server tramite la rete Ad Hoc da lui creata
    /// </summary>
    void CheckConnectedToWiFi()
    {
        //Eseguo il cmd come admin e eseguo comandi bash per visualizzare la rete wireless a cui il client è connesso e filtro se ssid della rete wifi è quella della rete ad hoc
        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
        startInfo.FileName = "powershell.exe";
        startInfo.Arguments = "/C Start-Process 'cmd' -Verb runAs -ArgumentList netsh wlan show profiles | findstr /c:'Holodeck' & if %errorlevel% == 0 (echo 0) else (echo 1)";
        process.StartInfo = startInfo;
        process.Start();
        

        UnityEngine.Debug.Log(process.StandardOutput.ReadToEnd());
    }
}
