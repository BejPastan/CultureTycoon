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
        //here must get animation
        scriptable.DestinationFurniture().UseFurniture(out AnimatorController animatior, out RoomType furnitureType, out float quality);
        //get animator controller from NPC transform
        AnimatorController defaultAnimation = GetComponent<Animator>().runtimeAnimatorController as AnimatorController;
        //set new animation
        GetComponent<Animator>().runtimeAnimatorController = animatior;

        //start getting needs points
        await scriptable.UseFurniture(quality, furnitureType);

        //set default animation
        GetComponent<Animator>().runtimeAnimatorController = defaultAnimation;

        scriptable.EndUsing();
        MoveToNext();
    }

    public void RemoveNPC()
    {
        scriptable.active = false;
        Destroy(gameObject);
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
