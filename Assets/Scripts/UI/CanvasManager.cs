using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    public Button btnNext;
    public Button btnPrevious;
    public Button btnOpenRoof;
    bool isRoofOpen;

    private void Start()
    {

        if (!NetManager.instance.isTeacher)
        {
            gameObject.SetActive(false);
        }

        // Permette di eseguire delle funzioni al click dei bottoni
        btnNext.onClick.AddListener(ButtonNext);
        btnPrevious.onClick.AddListener(ButtonPrevious);
        btnOpenRoof.onClick.AddListener(OpenRoof);
    }

    /// <summary>
    /// Avvia l'animazione di apertura del tetto.
    /// </summary>
    public void OpenRoof()
    {
        if(!isRoofOpen)
        {
            LessonManager.instance.CmdOpenRoof();
            // Impedisce di chiamare la funzione più di una volta
            isRoofOpen = true;
        }
    }

    // Funzione eseguita alla pressione del bottone "Next".
    // Non abbiamo collegato direttamente CmdNextStep() alla pressione del bottone perche essendo un [Command] la cosa crea qualche problema
    public void ButtonNext()
    {
        Debug.Log("Premuto NEXT");

        LessonManager.instance.CmdNextStep();
    }

    public void ButtonPrevious()
    {
        Debug.Log("Premuto PREVIOUS");

        LessonManager.instance.CmdPreviousStep();
    }

    /// <summary>
    /// Invia al server l'indice della lezione selezionata. L'indice viene impostato nell'inspector del bottone.
    /// </summary>
    public void ButtonSelectLesson(int lessonIndex)
    {
        LessonManager.instance.CmdChangeLesson(lessonIndex);
    }
}
