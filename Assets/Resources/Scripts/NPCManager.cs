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

    public void InstantiateNPC()
    {
        //get random NPC that is not active from the list
        NPC npc = npcs[Random.Range(0, npcs.Count)];
        if (!npc.active)
        {
            //instantiate the NPC
            GameObject npcObject = Instantiate(Resources.Load<GameObject>(npc.prefPath), spawnPoint, Quaternion.identity);
            npcObject.AddComponent<NPC>().PasteComponent(npc);
            npc = npcObject.GetComponent<NPC>();
            npc.StartNPC();
        }
        else
        {
            Debug.Log("NPC is active");
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
