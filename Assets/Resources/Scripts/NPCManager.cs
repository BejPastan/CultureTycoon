using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    [SerializeField] List<NPC> npcs;
    [SerializeField] public Vector3 spawnPoint;
    [SerializeField] int NPCCount;
    [SerializeField] NPCCreator creator;

    //calc new number of NPCs
    public void SetNewNPCNumber(int happinesSum)
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
    }


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
