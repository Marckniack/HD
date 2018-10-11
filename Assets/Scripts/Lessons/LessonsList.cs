using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "LessonContainer", menuName = "Holodeck/LessonContainer", order = 1)]
public class LessonsList : ScriptableObject {

    public List<LessonItem> lessonItem = new List<LessonItem>();

}


[Serializable]
public class LessonItem
{
    public string name;
    public GameObject Lesson;
    public Sprite icon;

}
