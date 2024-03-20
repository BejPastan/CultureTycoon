using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour
{
    public bool active = false;
    public string name;
    public int age;
    public Needs[] needs;
    public float freeTime;
    public float freeTimeLeft;
    public NPCStory story;
    public string prefPath;
    public NavMeshAgent agent;

    private FurnitureData[] furnitures;
    public Dictionary<NeedsType, RoomType> furnitureForNeeds = new Dictionary<NeedsType, RoomType>();

    public void SetValues(string name, int age, float freeTime, NPCStory story, string path)
    {
        Debug.Log("Set values");
        this.name = name;
        this.age = age;
        needs = story.GetNeeds();
        this.freeTime = freeTime;
        this.story = story;
        prefPath = path;
    }

    public void PasteComponent(NPC original)
    {
        name = original.name;
        age = original.age;
        needs = original.needs;
        freeTime = original.freeTime;
        story = original.story;
        prefPath = original.prefPath;
    }

    public async void StartNPC()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        
        active = true;
        Debug.Log("Awake");
        furnitureForNeeds.Add(NeedsType.art, RoomType.artWorkshop);
        furnitureForNeeds.Add(NeedsType.DIY, RoomType.Workshop);
        furnitureForNeeds.Add(NeedsType.music, RoomType.scene);
        furnitureForNeeds.Add(NeedsType.play, RoomType.playground);
        furnitureForNeeds.Add(NeedsType.science, RoomType.scienceLab);
        Debug.Log("Set furniture for needs");

        Debug.Log(needs.Length);
        for (int i = 0; i < needs.Length; i++)
        {
            needs[i].toFill = needs[i].value;
            Debug.Log(needs[i].type + " " + needs[i].value);
        }
        freeTimeLeft = freeTime;

        furnitures = FindObjectsOfType<FurnitureData>();
        SortFurnitures();
        MoveToNext();
    }

    private void SortFurnitures()
    {
        Debug.Log("Sort furnitures");
        //sort needs by value
        for (int i = 0; i < needs.Length; i++)
        {
            for (int j = i + 1; j < needs.Length; j++)
            {
                if (needs[i].toFill > needs[j].toFill)
                {
                    Needs temp = needs[i];
                    needs[i] = needs[j];
                    needs[j] = temp;
                }
            }
        }

        int apropriatePos = 0;
        //sort furniture to fit needs using dictionary
        for (int i = 0; i < needs.Length; i++)
        {
            for(int j = 0; j < furnitures.Length; j++)
            {
                if (furnitures[j]==null)
                {
                    for (int k = j; k < furnitures.Length-1; k++)
                    {
                        furnitures[k] = furnitures[k + 1];
                    }
                    Array.Resize(ref furnitures, furnitures.Length - 1);
                }
                if (furnitures[j].destinationType == furnitureForNeeds[needs[i].type])
                {
                    FurnitureData temp = furnitures[apropriatePos];
                    furnitures[apropriatePos] = furnitures[j];
                    furnitures[j] = temp;
                    apropriatePos++;
                    break;
                }
            }
        }
    }

    private async void MoveToNext()
    {
        //wait to end of frame
        agent.SetDestination(furnitures[0].transform.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<FurnitureData>() != null)
            if (other.GetComponent<FurnitureData>() == furnitures[0])
            {
                //stop moving
                agent.SetDestination(transform.position);
                UseFurniture();
            }
    }

    private async Task UseFurniture()
    {
        Debug.Log("Start using furniture");
        //here must get animation
        if(furnitures[0].UseFurniture(out AnimatorController animatior, out RoomType furnitureType, out float quality, out Vector3 usingSlot))
        {
            agent.SetDestination(usingSlot);
            Debug.Log("Get data from furniture");
            //get animator controller from NPC transform
            AnimatorController defaultAnimation = GetComponent<Animator>().runtimeAnimatorController as AnimatorController;
            //set new animation
            GetComponent<Animator>().runtimeAnimatorController = animatior;
            Debug.Log("Set data to NPC");
            //get needs type from furniture type
            int needsId = 0;
            for (int i = 0; i < needs.Length; i++)
            {
                if (furnitureForNeeds[needs[i].type] == furnitureType)
                {
                    needsId = i;
                    break;
                }
            }
            Debug.Log("set needs id");
            //start getting needs points
            while (freeTimeLeft > 0 && needs[needsId].toFill > 0)
            {
                await Task.Delay(500);
                needs[needsId].toFill -= quality;
                freeTimeLeft -= 0.5f;
                Debug.Log("Get needs points");
            }
            //set default animation
            GetComponent<Animator>().runtimeAnimatorController = defaultAnimation;
            EndUsing();
        }
        else
        {
            Debug.Log("Furniture is occupied");
            Occupied();
        }
    }

    public void Occupied()
    {
        //move furniture from current position to the end of the array
        FurnitureData temp = furnitures[0];
        for (int i = 0; i < furnitures.Length - 1; i++)
        {
            furnitures[i] = furnitures[i + 1];
        }
        furnitures[furnitures.Length - 1] = temp;
        MoveToNext();
    }

    public void EndUsing()
    {
        //remove furniture from the array
        for (int i = 0; i < furnitures.Length - 1; i++)
        {
            furnitures[i] = furnitures[i + 1];
        }
        Array.Resize(ref furnitures, furnitures.Length - 1);
        MoveToNext();
    }
}