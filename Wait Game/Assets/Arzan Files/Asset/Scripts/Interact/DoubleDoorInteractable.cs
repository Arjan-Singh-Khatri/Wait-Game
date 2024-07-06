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
    const string interactDoorText = " Toggle Door";
    [SerializeField] string requiredKeyID ; 

    private GameObject doorObject;

    public bool isLocked;
    public bool isOpen = false;

    private bool openingDoor = false; 
    private bool closingDoor = false;

    [SerializeField] private float doorRotationAmountOpen;
    [SerializeField] private float doorRotationAmountClose;

    private NavMeshModifierVolume navMeshModifier;

    private void Start()
    {
        navMeshModifier = GetComponent<NavMeshModifierVolume>();  

        if (transform.childCount == 0)
            doorObject = this.gameObject;
        else
            doorObject = transform.GetChild(0).gameObject;

        navMeshModifier.enabled = true;

        if (isLocked)
            navMeshModifier.area = 1;
        else 
            navMeshModifier.area = 0;
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

        if (isLocked){
            InfoText.instance.SetContent("Door locked cannot open. !");
            return;
        }

        if (isOpen) 
            openingDoor = true;

        else
            closingDoor = true;
    }

    public void CloseDoor() {
        doorObject.transform.rotation = Quaternion.Slerp(Quaternion.identity,
            Quaternion.Euler(0, 0, 0), 1 * Time.deltaTime);

        // Checking when to stop 
        if (doorObject.transform.rotation.eulerAngles.y <= -90){
            closingDoor = false;
            isOpen = false;
        }
        navMeshModifier.area = 1;
    }

    public void OpenDoor(){

        doorObject.transform.rotation = Quaternion.Slerp(Quaternion.identity,
            Quaternion.Euler(0, doorRotationAmountOpen, 0), 1 * Time.deltaTime);

        // Checking when to stop 
        if (doorObject.transform.rotation.eulerAngles.y <= -90)
        {
            openingDoor = false;
            isOpen = true;
        }
        navMeshModifier.area = 0;
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
            InfoText.instance.SetContent("Wrong key used. !");
        }
    }

    public void LoadItem(){
        // Do not need to load
    }
}
