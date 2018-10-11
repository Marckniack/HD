using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour {

    #region Singleton

    //Funziona anche se il singleton cè gia nella classe padre

    public static SpawnManager instance = null;

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

    public List<Transform> spawnPoints = new List<Transform>();
    public Vector3 teacherPos = new Vector3(2, 0, 0);

    // Use this for initialization
    void Start () {

        foreach (Transform child in gameObject.transform)
        {
            spawnPoints.Add(child);
        }

	}
}