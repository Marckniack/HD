using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Diagnostics;

public static class MRFileManager
{
    /// <summary>
    /// Copia di file di configurazione Mixed Reality da una cartella condivisa al computer locale.
    /// </summary>
    /// <param name="serverIp">Indirizzo IP del computer da cui copiare i file.</param>
    public static void Sync(string serverIp)
    {
        string sourcePath = @"\\" + serverIp + @"\HoloLensSensors";
        string targetPath = @"C:\ProgramData\WindowsHolographicDevices\SpatialStore\HoloLensSensors";

        // TODO: assicurati che abilitare/disabilitare le porte USB sia davvero necessario
        SetUSBEnabled(false);
        Copy(sourcePath, targetPath);
        SetUSBEnabled(true);
    }
    
    /// <summary>
    /// Copia una cartella e tutto il suo contenuto in un certo percorso. ATTENZIONE: se la cartella di destinazione esiste già, verrà eliminata e sostituita.
    /// </summary>
    /// <param name="sourceDir">Percorso della cartella sorgente.</param>
    /// <param name="targetDir">Percorso della cartella di destinazione.</param>
    private static void Copy(string sourceDir, string targetDir)
    {
        // Se esiste la cartella condivisa da cui prendere i file
        if (Directory.Exists(sourceDir))
        {
            // Se c'è già una cartella locale, cancellala insieme a tutti i suoi contenuti
            if (Directory.Exists(targetDir))
            {
                Directory.Delete(targetDir, true);
            }

            // Crea la cartella locale
            Directory.CreateDirectory(targetDir);

            // Copia tutti i file dalla cartella sorgente in quella locale
            foreach (var file in Directory.GetFiles(sourceDir))
                File.Copy(file, Path.Combine(targetDir, Path.GetFileName(file)));

            // Copia tutte le cartelle dalla cartella sorgente in quella locale
            foreach (var directory in Directory.GetDirectories(sourceDir))
                Copy(directory, Path.Combine(targetDir, Path.GetFileName(directory)));
        }
        else
        {
            UnityEngine.Debug.LogError("Cartella sorgente non trovata.");
        }
    }

    /// <summary>
    /// Abilita o disabilita l'accesso delle applicazioni alle porte USB, necessario per operare sulla cartella dei file di configurazione senza che il visore interferisca.
    /// </summary>
    /// <param name="value">True per abilitare, false per disabilitare.</param>
    private static void SetUSBEnabled(bool value)
    {
        Process process = new Process();
        ProcessStartInfo startInfo = new ProcessStartInfo();
        // Permette di eseguire lo script come amministratore
        string runAsAdmin = "/C Start-Process powershell -verb runas -ArgumentList";
        string cmdCommand;

        // https://stackoverflow.com/a/13267533
        if (value)
        {
            cmdCommand = " 'Get-PnpDevice -Class \"Holographic\" | Enable-PnpDevice -Confirm:$false' ";
        }
        else
        {
            cmdCommand = " 'Get-PnpDevice -Class \"Holographic\" | Disable-PnpDevice -confirm:$false' ";
        }

        //Eseguo il cmd come admin e eseguo comandi bash per creare una rete ad hoc e avviarla
        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
        startInfo.FileName = "powershell.exe";
        startInfo.Arguments = runAsAdmin + cmdCommand;
        process.StartInfo = startInfo;
        process.Start();
        process.WaitForExit();
    }

    public static void ShareHolodeckFolder()
    {
        // Net Share HoloLensSensors = C:\ProgramData\WindowsHolographicDevices\SpatialStore\HoloLensSensors "/GRANT:Everyone,READ"
        Process process = new Process();
        ProcessStartInfo startInfo = new ProcessStartInfo();
        // Permette di eseguire lo script come amministratore
        string runAsAdmin = "/C Start-Process powershell -verb runas -ArgumentList";
        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
        startInfo.FileName = "powershell.exe";
        startInfo.Arguments = runAsAdmin + " 'Net Share HoloLensSensors=C:\\ProgramData\\WindowsHolographicDevices\\SpatialStore\\HoloLensSensors \"/GRANT:Everyone,READ\"' ";
        process.StartInfo = startInfo;
        process.Start();
        process.WaitForExit();
    }


}