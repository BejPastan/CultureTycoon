using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    [SerializeField] List<NPC> npcs;

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
            GameObject npcObject = Instantiate(Resources.Load<GameObject>(npc.prefPath));
            //get the NPC component
            NPC npcComponent = npcObject.GetComponent<NPC>();
            //set the values of the NPC
            npcComponent.SetValues(npc.name, npc.age, npc.freeTime, npc.story, npc.prefPath);
            //set the NPC as active
            npc.active = true;
        }
        else
        {
            //if the NPC is active, try again
            InstantiateNPC();
        }
    }

    public void DestroyNPC(NPC npcToDestroy)
    {
        npcToDestroy.active = false;
        Destroy(npcToDestroy.gameObject);
    }
}
