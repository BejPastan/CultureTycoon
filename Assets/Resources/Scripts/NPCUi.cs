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
                needsUI[i].slider.value = npcScriptable.needs[i].value - npcScriptable.needs[i].toFill;
                needsUI[i].fillText.text = (npcScriptable.needs[i].value - npcScriptable.needs[i].toFill) + " / " + npcScriptable.needs[i].value;
            }
            yield return new WaitForSeconds(0.25f);
        }
    }

    public void HideNPC()
    {
        npcUI.gameObject.SetActive(false);
    }
}