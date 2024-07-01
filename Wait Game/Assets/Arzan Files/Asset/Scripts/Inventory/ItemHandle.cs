
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ItemHandle : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
{
    private InventoryManager inventoryManager;
    private RectTransform itemRectTransform;
    private InventoryUI inventoryUi;
    public ItemAttributes itemScriptableObject;

    private void Awake(){

        itemRectTransform = GetComponent<RectTransform>();
        inventoryUi = FindObjectOfType<InventoryUI>();    
        inventoryManager = FindFirstObjectByType<InventoryManager>();
    }

    private void Start()
    {
        inventoryUi.onDiscardItem += DiscardItem;
    }

    #region Drag and Drop   
    public void OnBeginDrag(PointerEventData eventData){
        inventoryManager.PickUpItem(itemRectTransform, itemScriptableObject._sizeX, itemScriptableObject._sizeY );
    }

    public void OnDrag(PointerEventData eventData){
        inventoryManager.ItemDrag(itemRectTransform);
    }


    public void OnEndDrag(PointerEventData eventData){
        inventoryManager.DropItem(itemRectTransform, itemScriptableObject._sizeX, itemScriptableObject._sizeY);
    }
    #endregion

    #region Inventory UI
    public void ShowItem()
    {
        inventoryUi.ShowItemDetail(itemScriptableObject._description, itemScriptableObject._name, this,
            itemScriptableObject.itemVisualPrefab);

        inventoryUi.currentItemType = itemScriptableObject.itemType;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ShowItem();
    }

    public void DiscardItem()
    {
        inventoryUi.currentItemType = ItemTypes.none;
        inventoryManager.DiscardItem(gameObject.GetComponent<RectTransform>(), itemScriptableObject._sizeX, itemScriptableObject._sizeY);
        inventoryUi.HideItemDetail();
    }

    private void OnDisable()
    {
        inventoryUi.onDiscardItem -= DiscardItem;
    }

    #endregion
}