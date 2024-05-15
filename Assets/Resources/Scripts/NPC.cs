using System.Threading.Tasks;
//using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour
{
    public NPCScriptable npcScriptable;
    public NavMeshAgent agent;
    private Vector3 destination;

    private Animator animator;

    bool onUI;

    public async void StartNPC()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        animator = gameObject.GetComponent<Animator>();
        npcScriptable.StartNPC();
        MoveToNext();
    }

    private async void MoveToNext()
    {
        if (npcScriptable.DestinationFurniture() != null)
        {
            
            //here there is no more furniture to use
        }
        destination = npcScriptable.GetDestination();
        animator.SetBool("walking", true);
        Debug.Log("Move to " + destination);
        agent.SetDestination(destination);
        agent.isStopped = false;
    }

    private async Task UseFurniture()
    {
        animator.SetBool("walking", false);
        transform.position = destination;
        transform.LookAt(npcScriptable.DestinationFurniture().transform.position);
        //here must get animation
        npcScriptable.DestinationFurniture().UseFurniture(out RuntimeAnimatorController animatorController, out RoomType furnitureType, out float quality);
        Debug.Log($"Start using furniture");
        //get animator controller from NPC transform
        RuntimeAnimatorController defaultAnimation = animator.runtimeAnimatorController as RuntimeAnimatorController;
        //set new animation
        animator.runtimeAnimatorController = animatorController;

        //start getting needs points
        await npcScriptable.UseFurniture(quality, furnitureType);

        //set default animation
        animator.runtimeAnimatorController = defaultAnimation;

        npcScriptable.EndUsing();
        MoveToNext();
    }

    public void RemoveNPC()
    {
        npcScriptable.active = false;
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
            if (other.GetComponent<FurnitureData>() == npcScriptable.DestinationFurniture())
            {
                //stop moving
                agent.Stop();
                UseFurniture();
            }
    }

}
