using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    [SerializeField] List<NPCScriptable> npcs;
    [SerializeField] public Vector3 spawnPoint;
    [SerializeField] int NPCCount;
    [SerializeField] NPCCreator creator;

    private void Start()
    {
        for(int i = 0; i < NPCCount; i++)
        {
            creator.CreateNPC();
        }
        StartSpawning();
        
    }

    public async Task StartSpawning()
    {
        await Task.Delay(1000);
        StartCoroutine(Spawn());
    }

    public void EndSpawning()
    {
        StopCoroutine(Spawn());
    }

    //spawner corutine
    public IEnumerator Spawn()
    {
        while (true)
        {
            yield return new WaitForSeconds(30/NPCCount);
            InstantiateNPC();
        }
    }

    //calc new number of NPCs
    public void SetNewNPCNumber()
    {
        //check for each NPC if have avarageHappines lower then 75 and greater then 90
        for(int i = 0; i < npcs.Count; i++)
        {
            if (npcs[i].avarageHappines == 0)
            { return; }
            if (npcs[i].avarageHappines < 75)
            {
                //remove the NPC from the list
                npcs.RemoveAt(i);
            }
            else if (npcs[i].avarageHappines > 90)
            {
                creator.CreateNPC();
            }
        }
        while(npcs.Count < NPCCount)
        {
            creator.CreateNPC();
        }
    }

    public int GetHappines()
    {
        int sum = 0;
        foreach(NPCScriptable npc in npcs)
        {
            sum += (int)npc.avarageHappines;
        }
        return sum;
    }


    public void AddNPC(NPCScriptable npc)
    {
        Debug.Log("Add NPC");
        Debug.Log(npc.name);
        npcs.Add(npc);
    }

    public void InstantiateNPC()
    {
        //get random NPC that is not active from the list
        NPCScriptable npc = npcs[Random.Range(0, npcs.Count)];
        if (!npc.active)
        {
            //instantiate the NPC
            GameObject npcObject = Instantiate(Resources.Load<GameObject>(npc.prefPath), spawnPoint, Quaternion.identity);
            npcObject.AddComponent<NPCScriptable>().PasteComponent(npc);
            npc = npcObject.GetComponent<NPCScriptable>();
            npc.StartNPC();
        }
        else
        {
            Debug.Log("NPC is active");
            foreach (NPCScriptable n in npcs)
            {
                if (!n.active)
                {
                    InstantiateNPC();
                    return;
                }
            }
        }
    }

    public void DestroyNPC(NPCScriptable npcToDestroy)
    {
        npcToDestroy.active = false;
        Destroy(npcToDestroy.gameObject);
    }
}
