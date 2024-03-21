using System.Collections;
using System.Collections.Generic;
using System.Security;
using System.Threading.Tasks;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AI;
using static NPCui;

public class NPC : MonoBehaviour
{
    public NPCScriptable scriptable;
    public NavMeshAgent agent;

    public void SetValues(string name, int age, float freeTime, NPCStory story, string path, Vector3 exit)
    {
        scriptable.SetValues(name, age, freeTime, story, path, exit);
    }

    public async void StartNPC()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        scriptable.StartNPC();
        MoveToNext();
    }

    private async void MoveToNext()
    {
        if (scriptable.DestinationFurniture() != null)
        {
            //here there is no more furniture to use
        }
        agent.SetDestination(scriptable.GetDestination());
    }

    private async Task UseFurniture()
    {
        Debug.Log("Start using furniture");
        //here must get animation
        if (scriptable.DestinationFurniture().UseFurniture(out AnimatorController animatior, out RoomType furnitureType, out float quality, out Vector3 usingSlot))
        {
            agent.SetDestination(usingSlot);
            Debug.Log("Get data from furniture");
            //get animator controller from NPC transform
            AnimatorController defaultAnimation = GetComponent<Animator>().runtimeAnimatorController as AnimatorController;
            //set new animation
            GetComponent<Animator>().runtimeAnimatorController = animatior;
            //start getting needs points
            await scriptable.UseFurniture(quality, furnitureType);
            //set default animation
            GetComponent<Animator>().runtimeAnimatorController = defaultAnimation;
            scriptable.DestinationFurniture().LeaveFurniture(agent.destination);
            scriptable.EndUsing();
            MoveToNext();
        }
        else
        {
            Debug.Log("Furniture is occupied");
            scriptable.Occupied();
            MoveToNext();
        }
    }




    private void OnMouseDown()
    {
        FindObjectOfType<NPCui>().ShowNPC(scriptable);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<FurnitureData>() != null)
            if (other.GetComponent<FurnitureData>() == scriptable.DestinationFurniture())
            {
                //stop moving
                agent.SetDestination(transform.position);
                UseFurniture();
            }
    }

}
