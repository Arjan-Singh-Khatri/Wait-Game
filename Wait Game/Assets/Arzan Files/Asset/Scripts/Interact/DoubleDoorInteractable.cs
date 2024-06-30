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
    const string interactDoorText = " Door name ";
    const string requiredKeyID = "default"; 


    private NavMeshModifierVolume navMeshModifier;
    private GameObject leftDoorContainer;
    private GameObject rightDoorContainer;

    public bool isLocked;
    public bool isOpen = false;

    private bool openingDoor = false; 
    private bool closingDoor = false;


    private void Start()
    {
        navMeshModifier = GetComponent<NavMeshModifierVolume>();
        leftDoorContainer = transform.GetChild(0).gameObject;
        rightDoorContainer = transform.GetChild(1).gameObject;

        if (isLocked) navMeshModifier.area = 1;
        else navMeshModifier.area = 0;
    }
    public string GetText(){
        return interactDoorText;
    }

    public Transform GetTransform() {
        return transform;
    }

    private void Update(){
        
        if (openingDoor )
            OpenDoor();
        else if (closingDoor)
            CloseDoor();
            
    }

    public void Interact(){

        if (isLocked) return;

        if (isOpen) 
            openingDoor = true;

        else
            closingDoor = true;
    }

    void CloseDoor() {
        leftDoorContainer.transform.rotation = Quaternion.Slerp(Quaternion.identity,
            Quaternion.Euler(0, 0, 0), 1 * Time.deltaTime);

        rightDoorContainer.transform.rotation = Quaternion.Slerp(Quaternion.identity,
            Quaternion.Euler(0, 180, 0), 1 * Time.deltaTime);

        // Checking when to stop 
        if (leftDoorContainer.transform.rotation.eulerAngles.y <= -90){
            closingDoor = false;
            isOpen = false;
        }

    }

    void OpenDoor(){

        leftDoorContainer.transform.rotation = Quaternion.Slerp(Quaternion.identity,
            Quaternion.Euler(0, -90, 0), 1 * Time.deltaTime);

        rightDoorContainer.transform.rotation = Quaternion.Slerp(Quaternion.identity,
            Quaternion.Euler(0, 270, 0), 1 * Time.deltaTime);

        // Checking when to stop 
        if (leftDoorContainer.transform.rotation.eulerAngles.y <= -90)
        {
            openingDoor = false;
            isOpen = true;
        }

    }

    public void TryKey(string key) { 
        if(requiredKeyID == key)
        {
            // Some Ui
            // Sound maybe then call this with some delay
            OpenDoor();
        }else
        {
            // Some Ui
            Debug.Log("Key doesn't match the door!");
        }
    }

    public void LoadItem(){
        // Do not need to load
    }
}
