using System.Threading.Tasks;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AI;
using static NPCui;

public class NPC : MonoBehaviour
{
    public NPCScriptable scriptable;
    public NavMeshAgent agent;
    private Vector3 destination;

    bool onUI;

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
        destination = scriptable.GetDestination();
        Debug.Log("Move to " + destination);
        agent.SetDestination(destination);
        agent.isStopped = false;
    }

    private async Task UseFurniture()
    {
        transform.position = destination;
        transform.LookAt(scriptable.DestinationFurniture().transform.position);
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
        if (onUI)
            FindObjectOfType<NPCui>().HideNPC();
        onUI = false;
        Destroy(gameObject);
    }


    private void OnMouseDown()
    {
        FindObjectOfType<NPCui>().ShowNPC(this);
        onUI = true;
    }

    public void RemoveFromUI()
    {
        onUI = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<FurnitureData>() != null)
            if (other.GetComponent<FurnitureData>() == scriptable.DestinationFurniture())
            {

                //stop moving
                agent.Stop();
                UseFurniture();
            }
    }

}
