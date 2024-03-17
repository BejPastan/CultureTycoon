using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NPCStory", menuName = "NPCStory")]
public class NPCStory : ScriptableObject
{
    [SerializeField] StoryFragment[] storyFragments;
    [SerializeField] Needs[] needs;
    //needts getter
    public Needs[] GetNeeds()
    {
        return needs;
    }
}

[System.Serializable]
public struct StoryFragment
{
    public string story;
    [SerializeField] StoryCondition[] conditions;

}

[System.Serializable]
public struct StoryCondition
{
    [SerializeField] bool completed;
    [SerializeField] NeedsType type;
    [SerializeField] int value;
    [SerializeField] int numberOfPasses;
}