using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NPCCreator : MonoBehaviour
{
    [SerializeField] float[] freeTime;
    [SerializeField] string[] name;
    [SerializeField] int[] age;
    [SerializeField] NPCStory[] story;
    [SerializeField] NPCManager manager;
    [SerializeField] int maxSum = 300;
    [SerializeField] int minSum = 200;


    //make this method callable from inspector
    public void StartCreating()
    {
        CreateNPC();
    }

    public void CreateNPC()
    {
        //get paths to all NPC prefabs in the Resources/Prefabs/NPC folder
        string[] paths = System.IO.Directory.GetFiles("Assets/Resources/Prefabs/NPC", "*.prefab");
        //get a random path
        string path = paths[Random.Range(0, paths.Length)];
        //change path to work with Resources.Load
        path = path.Replace(@"\", "/");
        path = path.Replace("Assets/Resources/", "");
        path = path.Replace(".prefab", "");

        NPCScriptable npcComponent = ScriptableObject.CreateInstance<NPCScriptable>();
        //get a random story
        NPCStory randomStory = Instantiate(story[Random.Range(0, story.Length)]);
        //set the values of the NPC
        npcComponent.SetValues(name[Random.Range(0, name.Length)], age[Random.Range(0, age.Length)], freeTime[Random.Range(0, freeTime.Length)], randomStory, path, manager.transform.position);
        manager.AddNPC(npcComponent);
    }
}

[System.Serializable]
public struct Needs
{
    public NeedsType type;
    /// <summary>
    /// max value
    /// </summary>
    public int value;
    public float toFill;
}
