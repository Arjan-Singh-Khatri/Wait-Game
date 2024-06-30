using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] float overlapSphereRadius;
    [SerializeField] LayerMask interactableLayer;
    public bool canInteract = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    public IInteractable GetInteractable() {
        Collider[] colliders = Physics.OverlapSphere(transform.position,overlapSphereRadius,interactableLayer);
        
        Collider closestInteractable = null;

        foreach(Collider collider in colliders) {
            if(closestInteractable == null) closestInteractable = collider; 
            if(Vector3.Distance(collider.transform.position, transform.position) <
                Vector3.Distance(closestInteractable.transform.position, transform.position)) {
                closestInteractable = collider;
            }
        }

        if(closestInteractable != null) { 
            if (closestInteractable.TryGetComponent(out IInteractable interactable))
                return interactable;
        }

        return null;
    }

    private void Interact() { 

        if(canInteract){
            IInteractable interactable = GetInteractable();
            interactable.Interact();
        }
    }

    private void OnDrawGizmos(){
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position,overlapSphereRadius);
    }

}
