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

    private void Update(){

        if (playerInteract.GetInteractable() != null){
            Show(playerInteract.GetInteractable());
        }
        else
            Hide();
    }

    void Show(IInteractable interactable) { 

        interactUIPanel.SetActive(true);

        ////Interaction Panel position and rotation 
        //interactUIPanel.transform.SetPositionAndRotation(playerCamera.transform.position + offset, 
        //    Quaternion.LookRotation(-interactable.GetTransform().forward));
        InfoText.instance.SetContent( interactable.GetText() );
        playerInteract.canInteract = true;
    }

    void Hide() {
        interactUIPanel.SetActive(false);
        playerInteract.canInteract = false;
    }
}
