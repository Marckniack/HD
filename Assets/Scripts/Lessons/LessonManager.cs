using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class LessonManager : NetworkBehaviour
{
    #region Singleton

    public static LessonManager instance = null;

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


    public int stepIndex = 0;   //Indice Step
    public int lessonIndex = 0; //Indice Lezioni

    public LessonsList lessonsList; //ScriptableObject con lista lezioni
    public GameObject activeLesson = null;  //Lezione Attiva
    public GameObject activeStep = null;    //Step Attivo

    public Animator roofAnimator;

    private void Start()
    {
        

        // Disabilita il Canvas se non sei il Teacher
        if (!NetManager.instance.isTeacher)
        {
            roofAnimator = GameObject.FindGameObjectWithTag("Roof").GetComponent<Animator>();
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    #region Commands

    /// <summary>
    /// Funzione eseguita dal teacher al server che comanda il server di cambiare la lezione a tutti i clients
    /// </summary>
    /// <param name="lessonIndex">Indice lezione</param>
    [Command]
    public void CmdChangeLesson(int lessonIndex)
    {
        RpcChangeLesson(lessonIndex);
    }

    /// <summary>
    /// Funzione eseguita dal teacher al server che comanda il server di cambiare a tutti i clients lo step
    /// </summary>
    [Command]
    public void CmdNextStep()
    {
        RpcNextStep();
    }

    /// <summary>
    /// Funzione eseguita dal teacher al server che comanda il server di cambiare a tutti i clients lo step
    /// </summary>
    [Command]
    public void CmdPreviousStep()
    {
        RpcPreviousStep();
    }

    /// <summary>
    /// Funzione eseguita dal teacher al server che comanda il server di cambiare a tutti i clients animazione portellone della scena
    /// </summary>
    [Command]
    public void CmdOpenRoof()
    {
        RpcOpenRoof();
    }

    #endregion

    #region ClientRPC

    /// <summary>
    /// Funzione eseguita dal server ai client che cambia la lezione
    /// </summary>
    /// <param name="index">Indice lezione</param>
    [ClientRpc]
    public void RpcChangeLesson(int index)
    {
        //Se il player locale non è un teacher
        if (!NetManager.instance.isTeacher)
        {
            if(index >= 0 && index < lessonsList.lessonItem.Count)
            {
                Destroy(activeLesson);  //Distruggo la lezione attuale
                lessonIndex = index;    //cambio il lesson index
                activeLesson = Instantiate(lessonsList.lessonItem[index].Lesson); //Cambio la lezione attiva
                activeLesson.transform.position = FindObjectOfType<SpawnManager>().transform.position; //Cambio la posizione della lezione
                CleanStep();    //Nascondo tutti gli step della lezione
                activeStep = activeLesson.transform.GetChild(0).gameObject; //Cambio lo step attivo
                activeStep.SetActive(true); //Mostro lo step
            }
            else
            {
                Debug.LogWarning("Lezione Non Trovata");
            }
        }
    }

    /// <summary>
    /// Funzione eseguita dal server al client che abilita lo step precedente
    /// </summary>
    [ClientRpc]
    public void RpcPreviousStep()
    {
        //Se esiste una lezione attiva e lo stepindex è maggiore di 0
        if (activeLesson && stepIndex > 0)
        {
            activeStep.SetActive(false);    //nascondo lo step attivo
            stepIndex--;    //diminuisco di 1 lindex    
            activeStep = activeLesson.transform.GetChild(stepIndex).gameObject; //cambio lo step attivo
            activeStep.SetActive(true); //attivo il nuovo step
        }
    }


    /// <summary>
    /// Funzione eseguita dal server al client che abilita lo step successivo
    /// </summary>
    [ClientRpc]
    public void RpcNextStep()
    {
        //Se esiste una lezione attiva e lo stepindex è minore del numero di step nella lezione
        if (activeLesson && stepIndex < activeLesson.transform.childCount - 1)
        {
            activeStep.SetActive(false);
            stepIndex++;
            activeStep = activeLesson.transform.GetChild(stepIndex).gameObject;
            activeStep.SetActive(true);
        }
    }


    [ClientRpc]
    public void RpcOpenRoof()
    {
        if (!NetManager.instance.isTeacher)
        {
            roofAnimator.SetTrigger("open");
        }
            
    }


    /// <summary>
    /// Funzione che nasconde tutti gli step
    /// </summary>
    public void CleanStep()
    {
        if (!NetManager.instance.isTeacher)
        {
            foreach (Transform step in activeLesson.transform)
            {
                step.gameObject.SetActive(false);
            }
            stepIndex = 0;
        }
    }

    #endregion
}
