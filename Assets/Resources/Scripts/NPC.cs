using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public bool active;
    public string name;
    public int age;
    public Needs[] needs;
    public float freeTime;
    public float freeTimeLeft;
    public NPCStory story;
    public string prefPath;

    public void SetValues(string name, int age, float freeTime, NPCStory story, string path)
    {
        this.name = name;
        this.age = age;
        needs = story.GetNeeds();
        this.freeTime = freeTime;
        this.story = story;
        prefPath = path;
        active = true;
    }

    private void Awake()
    {
        //here NPC must find place to go and start moving
    }
}
