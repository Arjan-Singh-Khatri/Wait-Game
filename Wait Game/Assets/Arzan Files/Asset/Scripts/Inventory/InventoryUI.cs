using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ItemTypes {
    none,
    key,
}


public class InventoryUI : MonoBehaviour
{
    [SerializeField] GameObject inventoryPanel;
    [SerializeField] GameObject itemDetailPanel;
    [SerializeField] Canvas inventoryMainCanvas;
    [SerializeField] Button _closeButton;

    // Details
    [SerializeField] Button itemDetailUseButton;
    [SerializeField] Button itemDetailDiscardButton;
    [SerializeField] TextMeshProUGUI itemNameText;
    [SerializeField] TextMeshProUGUI itemDescriptionText;
    [SerializeField] Image detailPanelItemImage;

    private bool isInventoryOpen = false;

    // To Check Item
    public ItemTypes currentItemType;
    public ItemHandle currentItem;

    // Events
    public Action onDiscardItem;
    public Action onUseItem;

    private void Start(){
        _closeButton.onClick.AddListener(() =>
        {
            HideInventory();
            HideItemDetail();
        });
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            InventoryToggle();
        }
    }

    void InventoryToggle(){
        isInventoryOpen = !isInventoryOpen;
        inventoryPanel.SetActive(isInventoryOpen);

        foreach(Transform item in inventoryMainCanvas.transform)
        {
            item.gameObject.SetActive(true);
        }
    }

    void HideInventory() {
        inventoryPanel.SetActive(false);
    }

    public void ShowItemDetail(string itemDescripText, string itemName, ItemHandle item, Image itemImage) {

        detailPanelItemImage = itemImage;
        itemDetailPanel.SetActive(true);
        itemDescriptionText.text = itemDescripText;
        itemNameText.text = itemName;
        currentItem = item; 
    }

    public void HideItemDetail(){
        itemDetailPanel.SetActive(false) ;   
    }

    public void UseItem(){
        onUseItem.Invoke();
    }

    public void DiscardItem() { 
        onDiscardItem.Invoke();
    }
}




