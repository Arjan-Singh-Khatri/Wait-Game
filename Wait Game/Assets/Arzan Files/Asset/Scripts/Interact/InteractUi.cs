using SinglePlayer;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractUi : MonoBehaviour
{
    [SerializeField] PlayerInteract playerInteract;
    [SerializeField] GameObject interactUIPanel;
    [SerializeField] TextMeshProUGUI interactUiTextMesh;
    [SerializeField] Camera playerCamera;

    [SerializeField] Vector3 offset = new(0,0-1);

    [SerializeField] InfoText infoText;

    private void Start()
    {

    }
    private void Update(){

        if (playerInteract.GetInteractable() != null){
            Show(playerInteract.GetInteractable());
        }
        else
            Hide();
    }

    void Show(IInteractable interactable) {

        //interactUIPanel.SetActive(true);

        ////Interaction Panel position and rotation 
        //interactUIPanel.transform.SetPositionAndRotation(playerCamera.transform.position + offset, 
        //    Quaternion.LookRotation(-interactable.GetTransform().forward));

        //interactUiTextMesh.text = interactable.GetText();
        infoText.SetInteractText(interactable.GetText());

        playerInteract.canInteract = true;
    }

    void Hide() {
        infoText.SetInteractTextOff() ;
        playerInteract.canInteract = false;
    }
}
