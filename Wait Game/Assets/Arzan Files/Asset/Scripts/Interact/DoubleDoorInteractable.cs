using SinglePlayer;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;

public class DoubleDoorInteractable : MonoBehaviour, IInteractable
{
    // Need to generate Some sort of ID or code for each door and keys with corresponding ID 
    const string interactDoorText = " Toggle Door ";
    [SerializeField] string requiredKeyID ; 

    private GameObject doorObject;

    public bool isLocked;
    public bool isOpen = false;

    private bool openingDoor = false; 
    private bool closingDoor = false;

    [SerializeField] private float doorRotationAmountOpen;
    [SerializeField] private float doorRotationAmountClose;

    NavMeshModifierVolume navMeshModifier;

    [SerializeField] InfoText interactiveText;

    private void Start()
    {
        doorObject = transform.GetChild(0).gameObject;
        
        navMeshModifier = GetComponent<NavMeshModifierVolume>();

        if (isLocked) navMeshModifier.area = 1; 
        else navMeshModifier.area = 0;
    }

    public string GetText(){
        return interactDoorText;
    }

    public Transform GetTransform() {
        return transform;
    }


    public void Interact(){

        if (isLocked){
            interactiveText.SetContent("Door is locked can't open!");

            return;
        }

        if (!isOpen) { 
            OpenDoor();
        }
        else
            CloseDoor();
    }

    public void CloseDoor() {
        doorObject.transform.rotation = Quaternion.Euler(0, 0, 0);

        navMeshModifier.area = 1;
    }

    public void OpenDoor(){

        doorObject.transform.rotation = Quaternion.Euler(0, doorRotationAmountOpen + doorObject.transform.rotation.y, 0);

        navMeshModifier.area = 0;
    }

    public void TryKey(string key) { 

        if(requiredKeyID == key)
        {
            OpenDoor();
        }else
        {
            interactiveText.SetContent(" Key doesn't match the door! ");
            Debug.Log("Key doesn't match the door!");
        }
    }

    public void LoadItem(){
        // Do not need to load
    }
}
