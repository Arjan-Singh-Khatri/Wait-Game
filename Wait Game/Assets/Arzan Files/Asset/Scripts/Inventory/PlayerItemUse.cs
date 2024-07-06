using SinglePlayer;
using System;
using UnityEngine;


public class PlayerItemUse: MonoBehaviour 
{
    [SerializeField] InventoryUI inventoryUI;

    [SerializeField] InfoText warningText; 

    private void Start(){
        inventoryUI.onUseItem += UseItem;
    }

    private void UseItem(){

        switch(inventoryUI.currentItemType){
            case ItemTypes.key:
                UseKey();
                break;

            default:
                break;
        }
    }

    // Key Item Use function 
    void UseKey() {

        Debug.Log(" Key Used ! Item Code : " + inventoryUI.currentItem.itemScriptableObject.itemCode);

        Collider closestInteractable = CheckInteractable();

        if (closestInteractable != null)
        {
            if (closestInteractable.TryGetComponent(out DoubleDoorInteractable doubleDoor))
                doubleDoor.TryKey(inventoryUI.currentItem.itemScriptableObject.itemCode);
        }
        else
        {
            warningText.SetContent(" Cannot use item in current Context! ");
            Debug.Log(" Cannot use item in current Context! ");
        }

    }
    
    Collider CheckInteractable() {

        Collider[] colliders = Physics.OverlapSphere(transform.position, 3.4f, 7);

        Collider closestInteractable = null;

        foreach (Collider collider in colliders)
        {
            if (closestInteractable == null) closestInteractable = collider;
            if (Vector3.Distance(collider.transform.position, transform.position) <
                Vector3.Distance(closestInteractable.transform.position, transform.position))
            {
                closestInteractable = collider;
            }
        }

        return closestInteractable;
    }
    
    private void OnDisable()
    {
        inventoryUI.onUseItem -= UseItem;
    }
}
