using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour
{
    public bool active;
    public string name;
    public int age;
    public Needs[] needs;
    public float freeTime;
    public float freeTimeLeft;
    public NPCStory story;
    public string prefPath;

    private FurnitureData[] furnitures;
    public Dictionary<NeedsType, RoomType> furnitureForNeeds = new Dictionary<NeedsType, RoomType>();

    public void SetValues(string name, int age, float freeTime, NPCStory story, string path)
    {
        this.name = name;
        this.age = age;
        needs = story.GetNeeds();
        this.freeTime = freeTime;
        this.story = story;
        prefPath = path;
        active = true;
    }

    private void Awake()
    {
        furnitureForNeeds.Add(NeedsType.art, RoomType.artWorkshop);
        furnitureForNeeds.Add(NeedsType.DIY, RoomType.Workshop);
        furnitureForNeeds.Add(NeedsType.music, RoomType.scene);
        furnitureForNeeds.Add(NeedsType.play, RoomType.playground);
        furnitureForNeeds.Add(NeedsType.science, RoomType.scienceLab);
        
        for (int i = 0; i < needs.Length; i++)
        {
            needs[i].toFill = needs[i].value;
        }
        freeTimeLeft = freeTime;

        //here NPC must find place to go and start moving
        furnitures = FindObjectsOfType<FurnitureData>();
        SortFurnitures();
        MoveToNext();
    }

    private void SortFurnitures()
    {
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
        GetComponent<NavMeshAgent>().SetDestination(furnitures[0].transform.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<FurnitureData>() == furnitures[0])
        {
            //start using furniture
            UseFurniture();
        }
    }

    private void UseFurniture()
    {
        //here must get animation

        //start getting needs points
    }

    private async Task Using()
    {
        //wait 0.5 sec
        //get points from furniture type
        //remove energy from freeTimeLeft
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