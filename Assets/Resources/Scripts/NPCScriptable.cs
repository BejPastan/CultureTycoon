using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class NPCScriptable : ScriptableObject
{
    public bool active = false;
    public string name;
    public int age;
    //public Needs[] needs;
    public float freeTime;
    public float freeTimeLeft;
    public NPCStory story;
    public string prefPath;

    int slot;

    public float avarageHappines;
    public int numberOfVisits;

    private FurnitureData[] furnitures;
    
    public FurnitureData DestinationFurniture()
    {
        if(furnitures.Length > 0)
            return furnitures[0];
        return null;
    }

    public Dictionary<NeedsType, RoomType> furnitureForNeeds = new Dictionary<NeedsType, RoomType>();
    public Vector3 exit;

    public void SetValues(string name, int age, float freeTime, NPCStory story, string path, Vector3 exit)
    {
        furnitureForNeeds.Add(NeedsType.art, RoomType.artWorkshop);
        furnitureForNeeds.Add(NeedsType.DIY, RoomType.Workshop);
        furnitureForNeeds.Add(NeedsType.music, RoomType.scene);
        furnitureForNeeds.Add(NeedsType.play, RoomType.playground);
        furnitureForNeeds.Add(NeedsType.science, RoomType.scienceLab);
        this.name = name;
        this.age = age;
        //needs = story.GetNeeds();
        this.freeTime = freeTime;
        this.story = story;
        prefPath = path;
        this.exit = exit;
    }

    public Vector3 GetDestination()
    {
        if (furnitures.Length > 0)
            if(furnitures[0].CanBeUsed(out Vector3 slot, out int slotId))
            {
                this.slot = slotId;
                return slot;
            }
        CalcHappines();
        CheckStory();
        Debug.Log("Move to exit");
        return exit;
    }

    public async void StartNPC()
    {
        active = true;
        for (int i = 0; i < story.needs.Length; i++)
        {
            story.needs[i].toFill = story.needs[i].value;
        }
        freeTimeLeft = freeTime;

        furnitures = FindObjectsOfType<FurnitureData>();
        SortFurnitures();
    }

    private void SortFurnitures()
    {
        Debug.Log("Sort furnitures");
        //sort needs by value
        for (int i = 0; i < story.needs.Length; i++)
        {
            for (int j = i + 1; j < story.needs.Length; j++)
            {
                if (story.needs[i].toFill > story.needs[j].toFill)
                {
                    Needs temp = story.needs[i];
                    story.needs[i] = story.needs[j];
                    story.needs[j] = temp;
                }
            }
        }

        int apropriatePos = 0;
        //sort furniture to fit needs using dictionary
        for (int i = 0; i < story.needs.Length; i++)
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
                if (furnitures[j].destinationType == furnitureForNeeds[story.needs[i].type])
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

    public void CalcHappines()
    {
        //calc happines
        float happines = 0;
        for (int i = 0; i < story.needs.Length; i++)
        {
            happines += story.needs[i].toFill / story.needs[i].value;
        }
        happines*=20;
        avarageHappines = (avarageHappines * numberOfVisits + happines) / (numberOfVisits + 1);
        numberOfVisits++;
    }

    private void CheckStory()
    {
        story.CheckCompletion();
    }

    public async Task UseFurniture(float quality, RoomType furnitureType)
    {
        int needsId = 0;
        for (int i = 0; i < story.needs.Length; i++)
        {
            if (furnitureForNeeds[story.needs[i].type] == furnitureType)
            {
                needsId = i;
                break;
            }
        }

        float timeOnFuriture = furnitures[0].usageTime;
        while (freeTimeLeft > 0 && story.needs[needsId].toFill > 0 && timeOnFuriture > 0)
        {
            await Task.Delay(500);
            story.needs[needsId].toFill -= quality;
            freeTimeLeft -= 0.5f;
            timeOnFuriture -= 0.5f;
            //Debug.Log("Get needs points");
            if (freeTimeLeft <= 0)
            {
                CalcHappines();
                return;
            }
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
    }

    public void EndUsing()
    {
        //remove furniture from the array
        furnitures[0].LeaveFurniture(slot);
        for (int i = 0; i < furnitures.Length - 1; i++)
        {
            furnitures[i] = furnitures[i + 1];
        }
        Array.Resize(ref furnitures, furnitures.Length - 1);
    }

    public void GetValues(out Needs[] needs)
    {
        needs = story.needs;
    }
}