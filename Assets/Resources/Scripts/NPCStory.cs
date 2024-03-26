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
public partial class StoryFragment
{
    //public bool completed;
    public string story;
    [SerializeField] StoryCondition[] conditions;
    [SerializeField] GameObject[] unlockedFurnitures;

    public bool CheckCompletion(Needs[] needs, out GameObject[] unlocked)
    {
        Debug.Log($"CheckCompletion for {story}");
        bool completed = false;
        unlocked = null;
        foreach(StoryCondition condition in conditions)
        {
            foreach(Needs need in needs)
            {
                if(need.type == condition.type)
                {
                    if(need.toFill <=0)
                    {
                        if(condition.PassComplete())
                        {
                            completed = true;
                            unlocked = unlockedFurnitures;
                        }
                    }
                }
            }
        }
        return completed;
    }
}

[System.Serializable]
public partial class StoryCondition
{
    [SerializeField] bool completed;
    [SerializeField] public NeedsType type;
    [SerializeField] int numberOfPasses;
    [SerializeField] int neededPasses;

    public bool PassComplete()
    {
        Debug.Log($"PassComplete check");
        //to many times check if pass after one turn
        numberOfPasses++;
        Debug.Log($"check: {numberOfPasses}");
        if(numberOfPasses >= neededPasses)
        {
            Debug.Log($"PassComplete needed Passes: {neededPasses} Number of Passes:{numberOfPasses}");
            completed = true;
        }
        return completed;
    }
}