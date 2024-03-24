using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NPCStory", menuName = "NPCStory")]
public class NPCStory : ScriptableObject
{
    [SerializeField] List<StoryFragment> storyToUnlocked;
    [SerializeField] public List<StoryFragment> unlockedStory;
    [SerializeField] public Needs[] needs;

    public StoryFragment[] GetCompletedStory()
    {
        return unlockedStory.ToArray();
    }

    public void CheckCompletion()
    {
        for(int i = 0; i<storyToUnlocked.Count; i++)
        {
            if(storyToUnlocked[i].CheckCompletion(needs, out GameObject[] unlocked))
            {
                unlockedStory.Add(storyToUnlocked[i]);
                storyToUnlocked.RemoveAt(i);
                i--;
                //find all EquipmentMenuUI and unlock new furnitures
                EquipmentMenuUI[] equipmentMenuUIs = Resources.FindObjectsOfTypeAll<EquipmentMenuUI>();
                foreach(EquipmentMenuUI equipmentMenuUI in equipmentMenuUIs)
                {
                    while(0<unlocked.Length)
                    {
                        if(equipmentMenuUI.GetRoomType() == unlocked[unlocked.Length-1].GetComponent<FurnitureData>().destinationType)
                        {
                            equipmentMenuUI.UnlockNewFurniture(unlocked[unlocked.Length-1]);
                            Array.Resize(ref unlocked, unlocked.Length-1);
                        }
                    }
                }

            }
        }
    }
}

[System.Serializable]
public struct StoryFragment
{
    //public bool completed;
    public string story;
    [SerializeField] StoryCondition[] conditions;
    [SerializeField] GameObject[] unlockedFurnitures;

    public bool CheckCompletion(Needs[] needs, out GameObject[] unlocked)
    {
        bool completed = true;
        unlocked = unlockedFurnitures;
        foreach(StoryCondition condition in conditions)
        {
            foreach(Needs need in needs)
            {
                if(need.type == condition.type)
                {
                    Debug.Log("Check " + need.type + " " + need.toFill);
                    if(need.toFill <=0)
                    {
                        if(!condition.PassComplete())
                        {
                            completed = false;
                            unlocked = null;
                        }
                    }
                }
            }
        }
        return completed;
    }
}

[System.Serializable]
public struct StoryCondition
{
    [SerializeField] public bool completed;
    [SerializeField] public NeedsType type;
    [SerializeField] public int numberOfPasses;
    [SerializeField] public static int neededPasses;

    public bool PassComplete()
    {
        numberOfPasses++;
        if(numberOfPasses >= neededPasses)
        {
            completed = true;
            
        }
        return completed;
    }
}