using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    NPCScriptable scriptable;

    public void SetValues(string name, int age, float freeTime, NPCStory story, string path, Vector3 exit)
    {
        scriptable.SetValues(name, age, freeTime, story, path, exit);
    }
}
