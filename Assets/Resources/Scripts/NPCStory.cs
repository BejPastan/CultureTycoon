using System;
using System.Collections;
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
            if(storyToUnlocked[i].CheckCompletion(needs))
            {
                unlockedStory.Add(storyToUnlocked[i]);
                storyToUnlocked.RemoveAt(i);
                i--;
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
    [SerializeField] FurnitureData[] unlockedFurnitures;

    public bool CheckCompletion(Needs[] needs)
    {
        bool completed = true;
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