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
        await Task.Delay(10000);
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
            yield return new WaitForSeconds(10);
            SelectNPC();
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
        npcs.Add(npc);
    }

    public void InstaniateNPC(NPCScriptable npcData)
    {
        GameObject npcObject = Instantiate(Resources.Load<GameObject>(npcData.prefPath), spawnPoint, Quaternion.identity);
        NPC npc = npcObject.GetComponent<NPC>();
        npc.npcScriptable = npcData;
        npc.StartNPC();
    }

    public void SelectNPC()
    {
        for(int i = 0; i < npcs.Count; i++)
        {
            if (!npcs[i].active)
            {
                InstaniateNPC(npcs[i]);
                return;
            }
        }
    }

    public void DestroyNPC(NPC npcToDestroy)
    {
        npcToDestroy.npcScriptable.active = false;
        Destroy(npcToDestroy.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("NPC"))
        {
            other.GetComponent<NPC>().RemoveNPC();
        }
    }
}
