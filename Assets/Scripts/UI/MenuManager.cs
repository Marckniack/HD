using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

    #region Singleton

    public static MenuManager instance = null;

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

    public Button serverButton;
    public Button clientButton;
    public Button teacherButton;
    public Button syncButton;
    public GameObject SpawnButtonsContainer;

    List<Button> spawnButtons = new List<Button>();
    
    void Start () {

        if (SystemInfo.deviceType != DeviceType.Desktop)
        {
            NetManager.instance.isTeacher = true;
            teacherButton.gameObject.SetActive(true);
            serverButton.gameObject.SetActive(false);
            clientButton.gameObject.SetActive(false);
        }

        InitButtons();
    }

    void InitButtons()
    {
        serverButton.onClick.AddListener(NetManager.instance.InitServer);
        clientButton.onClick.AddListener(NetManager.instance.InitClient);
        teacherButton.onClick.AddListener(NetManager.instance.InitClient);

        //Inizializzo i bottoni di spawn
        for (int i = 0; i < SpawnButtonsContainer.GetComponentsInChildren<Button>().Length; i++)
        {
            // Odio le closure
            int index = i;

            SpawnButtonsContainer.GetComponentsInChildren<Button>()[i].onClick.AddListener(
                delegate
                {
                    NetManager.instance.spawnPointIndex = index;
                    NetManager.instance.InitSync();
                }
             );
        }
    }

    public void ShowSync()
    {
        if (this)
        {
            serverButton.gameObject.SetActive(false);
            clientButton.gameObject.SetActive(false);
            teacherButton.gameObject.SetActive(false);
            SpawnButtonsContainer.SetActive(true);
        }

    }
}
