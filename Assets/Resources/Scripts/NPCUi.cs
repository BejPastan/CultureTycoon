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
    private NPCScriptable npc;

    [System.Serializable]
    public struct NeedsUI
    {
        public Slider slider;
        public TextMeshProUGUI fillText;
    }

    public void ShowNPC(NPCScriptable npc)
    {
        StopCoroutine(UpdateValues());
        npcUI.gameObject.SetActive(true);
        this.npc = npc;
        StartCoroutine(UpdateValues());
    }

    //corutine to update NPCValues
    public IEnumerator UpdateValues()
    {
        while (npcUI.gameObject.active)
        {
            for (int i = 0; i < needsUI.Length; i++)
            {
                needsUI[i].slider.value = npc.needs[i].value - npc.needs[i].toFill;
                needsUI[i].fillText.text = (npc.needs[i].value - npc.needs[i].toFill) + " / " + npc.needs[i].value;
            }
            yield return new WaitForSeconds(1);
        }
    }

    public void HideNPC()
    {
        npcUI.gameObject.SetActive(false);
    }
}