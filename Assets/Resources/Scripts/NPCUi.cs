using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPCui : MonoBehaviour
{
    [SerializeField] private Transform npcUI;
    [SerializeField] private TextMeshProUGUI name;
    [SerializeField] private TextMeshProUGUI age;
    [SerializeField] private NeedsUI[] needsUI;
    [SerializeField] private RectTransform storyUI;
    [SerializeField] private GameObject storyPref;
    private NPC npc;
    private NPCScriptable npcScriptable;

    [System.Serializable]
    public struct NeedsUI
    {
        public Slider slider;
        public TextMeshProUGUI fillText;
    }

    public void ShowNPC(NPC npc)
    {
        if(this.npc!= null)
        {
            this.npc.RemoveFromUI();
        }
        StopCoroutine(UpdateValues());
        npcUI.gameObject.SetActive(true);
        this.npc = npc;
        npcScriptable = npc.scriptable;
        ShowBasicInfo();
        ShowStory();
        StartCoroutine(UpdateValues());
    }

    private void ShowBasicInfo()
    {
        name.text = npcScriptable.name;
        age.text = npcScriptable.age.ToString();
    }

    //corutine to update NPCValues
    public IEnumerator UpdateValues()
    {
        while (npcUI.gameObject.active)
        {
            for (int i = 0; i < needsUI.Length; i++)
            {
                needsUI[i].slider.value = npcScriptable.story.needs[i].value - npcScriptable.story.needs[i].toFill;
                needsUI[i].fillText.text = (npcScriptable.story.needs[i].value - npcScriptable.story.needs[i].toFill) + " / " + npcScriptable.story.needs[i].value;
            }
            ShowStory();
            yield return new WaitForSeconds(0.25f);
        }
    }

    private void ShowStory()
    {
        StoryFragment[] completedStory = npcScriptable.story.GetCompletedStory();

        for (int i = 0; i < completedStory.Length; i++)
        {
            RectTransform story = Instantiate(storyPref, storyUI).GetComponent<RectTransform>();
            Vector2 pos = new Vector2(0, -i * 120 -20);
            story.anchoredPosition = pos;
            story.GetComponentInChildren<TextMeshProUGUI>().text = completedStory[i].story;
        }
        storyUI.sizeDelta = new Vector2(0, completedStory.Length * 120 +10);
        storyUI.anchoredPosition = new Vector2(0, 0);
    }

    private void OnMouseOver()
    {
        //handle scrolling
        if(Input.mouseScrollDelta.y > 0)
        {
            storyUI.position += new Vector3(0, 10, 0);
        }
        else if(Input.mouseScrollDelta.y < 0)
        {
            storyUI.position += new Vector3(0, -10, 0);
        }
    }

    public void HideNPC()
    {
        npcUI.gameObject.SetActive(false);
    }
}