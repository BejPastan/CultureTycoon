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
    //needs getter
    public Needs[] GetNeeds()
    {
        return needs;
    }

    public StoryFragment[] GetCompletedStory()
    {
        StoryFragment[] completedStory = new StoryFragment[0];
        for(int i = 0; i<storyToUnlocked.Count; i++)
        {
            Array.Resize(ref completedStory, completedStory.Length + 1);
            completedStory[completedStory.Length - 1] = storyToUnlocked[i];
        }

        return completedStory;
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
        foreach(StoryCondition condition in conditions)
        {
            foreach(Needs need in needs)
            {
                if(need.type == condition.type)
                {
                    if(need.toFill <=0)
                    {
                        condition.PassComplete();
                    }
                }
            }
        }

        foreach(StoryCondition condition in conditions)
        {
            if(!condition.completed)
            {
                return false;
            }
        }
        return true;
    }
}

[System.Serializable]
public struct StoryCondition
{
    [SerializeField] public bool completed;
    [SerializeField] public NeedsType type;
    [SerializeField] public int numberOfPasses;
    [SerializeField] public static int neededPasses;

    public void PassComplete()
    {
        numberOfPasses++;
        if(numberOfPasses >= neededPasses)
        {
            completed = true;
        }
    }
}