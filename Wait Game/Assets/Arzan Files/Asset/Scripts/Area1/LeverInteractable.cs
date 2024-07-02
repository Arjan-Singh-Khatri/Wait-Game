using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class LeverInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] DoubleDoorInteractable[] doorsOpenOnToggle;
    [SerializeField] DoubleDoorInteractable[] doorsCloseOnToggle;
    [SerializeField] string _interactText;

    private bool leverToggled = false;
    private Animator _animator;

    private void Start(){
        _animator = GetComponent<Animator>();
    }

    public string GetText(){
        return _interactText;   
    }

    public Transform GetTransform(){
        return transform;
    }

    public void Interact(){
        LeverInteractable[] levers = FindObjectsOfType<LeverInteractable>();

        ToggleDoors();

        foreach(var lever in levers) {
            if (lever.gameObject == this.gameObject)
                continue;
            lever.ToggleInteract();
        }
    }

    public void ToggleInteract(){

        if (!leverToggled) return;
        
        _animator.SetTrigger("ToggleLeverBack");

        foreach(var _door in doorsOpenOnToggle)
        {
            _door.CloseDoor();
        }

        foreach(var _door in doorsCloseOnToggle) { 
            _door.OpenDoor();
        }
    }

    void ToggleDoors() {
        if (leverToggled)
        {
            _animator.SetTrigger("ToggleLeverBack");
            foreach (var _door in doorsOpenOnToggle)
            {
                _door.CloseDoor();
            }

            foreach (var _door in doorsCloseOnToggle)
            {
                _door.OpenDoor();
            }
        }
        else
        {
            _animator.SetTrigger("ToggleLever");
            foreach (var _door in doorsOpenOnToggle)
            {
                _door.OpenDoor();
            }

            foreach (var _door in doorsCloseOnToggle)
            {
                _door.CloseDoor();
            }
        }

    }
    
    public void LoadItem(){
        // NO item load needed for this 
    }
}
