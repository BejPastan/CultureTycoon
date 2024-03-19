using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NPCCreator : MonoBehaviour
{
    [SerializeField] float freeTime;
    [SerializeField] string name;
    [SerializeField] int age;
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
        Debug.Log(path);
        //load the prefab from the path
        GameObject prefab = Resources.Load<GameObject>(path);
        //instantiate the prefab
        GameObject npc = Instantiate(prefab);
        //get the NPC component
        NPC npcComponent = npc.GetComponent<NPC>();
        //get a random story
        NPCStory randomStory = story[Random.Range(0, story.Length)];
        //set the values of the NPC
        npcComponent.SetValues(name, age, freeTime, randomStory, path);
    }
}

[System.Serializable]
public struct Needs
{
    public NeedsType type;
    public int value;
    public int toFill;
}
