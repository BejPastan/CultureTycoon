using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AI;

public class NPCScriptable : ScriptableObject
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


    public float avarageHappines;
    public int numberOfVisits;


    private FurnitureData[] furnitures;
    public Dictionary<NeedsType, RoomType> furnitureForNeeds = new Dictionary<NeedsType, RoomType>();
    public Vector3 exit;

    public void SetValues(string name, int age, float freeTime, NPCStory story, string path, Vector3 exit)
    {
        Debug.Log("Set values");
        this.name = name;
        this.age = age;
        needs = story.GetNeeds();
        this.freeTime = freeTime;
        this.story = story;
        prefPath = path;
        this.exit = exit;
    }

    public void PasteComponent(NPCScriptable original)
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
        if(furnitures.Length == 0)
        {
            MoveToExit();
            return;
        }
        agent.SetDestination(furnitures[0].transform.position);
    }

    public void MoveToExit()
    {
        Debug.Log("Move to exit");
        agent.SetDestination(exit);
        //calc happines
        float happines = 0;
        for (int i = 0; i < needs.Length; i++)
        {
            happines += needs[i].toFill / needs[i].value;
        }
        happines*=20;
        avarageHappines = (avarageHappines * numberOfVisits + happines) / (numberOfVisits + 1);
        numberOfVisits++;
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
            float timeOnFuriture = furnitures[0].usageTime;
            while (freeTimeLeft > 0 && needs[needsId].toFill > 0 && timeOnFuriture>0)
            {
                await Task.Delay(500);
                needs[needsId].toFill -= quality;
                freeTimeLeft -= 0.5f;
                timeOnFuriture -= 0.5f;
                Debug.Log("Get needs points");
                if(freeTimeLeft <= 0)
                {
                    MoveToExit();
                    return;
                }
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
        furnitures[0].LeaveFurniture(agent.destination);
        //remove furniture from the array
        for (int i = 0; i < furnitures.Length - 1; i++)
        {
            furnitures[i] = furnitures[i + 1];
        }
        Array.Resize(ref furnitures, furnitures.Length - 1);
        MoveToNext();
    }

    private void OnMouseDown()
    {
        Debug.Log("Click");
        if (active)
        {
            Debug.Log("Active");
            FindObjectOfType<NPCui>().ShowNPC(this);
        }
    }

    public void GetValues(out Needs[] needs)
    {
        needs = this.needs;
    }
}