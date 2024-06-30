using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyInteractable : MonoBehaviour, IInteractable
{

    [SerializeField]private ItemAttributes itemScriptableObject;

    private void Start(){

        itemScriptableObject._name = gameObject.name;
    }

    public string GetText(){
        return itemScriptableObject.itemInteractText;
    }

    public void Interact(){
        LoadItem();
    }

    public Transform GetTransform() {
        return transform;
    }

    public void LoadItem(){
        InventoryManager inventoryManager = FindObjectOfType<InventoryManager>();
        inventoryManager.LoadItem(itemScriptableObject.itemVisualPrefab.rectTransform, itemScriptableObject._sizeX, itemScriptableObject._sizeY);
        Destroy(gameObject);
    }
}
