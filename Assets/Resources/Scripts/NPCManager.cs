using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    [SerializeField] List<NPC> npcs;
    [SerializeField] Vector3 spawnPoint;

    public void AddNPC(NPC npc)
    {
        npcs.Add(npc);
    }

    private void InstantiateNPC()
    {
        //get random NPC that is not active from the list
        NPC npc = npcs[Random.Range(0, npcs.Count)];
        if (!npc.active)
        {
            //instantiate the NPC
            GameObject npcObject = Instantiate(Resources.Load<GameObject>(npc.prefPath), spawnPoint, Quaternion.identity);
            //get the NPC component
            NPC npcComponent = npcObject.GetComponent<NPC>();
            //set the values of the NPC
            npcComponent.SetValues(npc.name, npc.age, npc.freeTime, npc.story, npc.prefPath);
            //set the NPC as active
            npc.active = true;
        }
        else
        {
            foreach (NPC n in npcs)
            {
                if (!n.active)
                {
                    InstantiateNPC();
                    return;
                }
            }
        }
    }

    public void DestroyNPC(NPC npcToDestroy)
    {
        npcToDestroy.active = false;
        Destroy(npcToDestroy.gameObject);
    }
}
