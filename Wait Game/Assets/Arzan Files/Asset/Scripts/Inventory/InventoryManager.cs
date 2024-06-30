
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryManager : MonoBehaviour {

    // Inventory and canvas
    [SerializeField] RectTransform _inventoryVisualCanvas;
    [SerializeField] RectTransform _inventoryVisualFront;
    [SerializeField] Canvas _inventoryCanvas;
    Vector2 _inventoryVisualOrigin;

    // Grid 
    private Grid inventoryGrid;
    private const int _inventoryCellSize = 200;

    // Keep track of different positions
    Vector3 currentPointerWorldPos;
    Vector2 originalPosition;
    Quaternion originalRotation;
    List<Vector2> currentPosArray = new();

    // Invenotry UI 
    [SerializeField] InventoryUI inventoryUI;

    private void Start() {

        _inventoryVisualOrigin.x = _inventoryVisualFront.anchoredPosition.x - _inventoryVisualFront.rect.width / 2;
        _inventoryVisualOrigin.y = _inventoryVisualFront.anchoredPosition.y + _inventoryVisualFront.rect.height / 2;
        
        inventoryGrid = new(Mathf.FloorToInt(_inventoryVisualFront.sizeDelta.x), Mathf.FloorToInt(_inventoryVisualFront.sizeDelta.y),
            _inventoryCellSize, _inventoryVisualOrigin);
        
        inventoryGrid.CreateGrid();
    }

    private void Update() {


        if (inventoryUI.gameObject.activeSelf)
            RectTransformUtility.ScreenPointToWorldPointInRectangle(_inventoryVisualCanvas, Input.mousePosition,
                null, out currentPointerWorldPos);
    }

    #region Inventory Item Functions

    public void LoadItem(RectTransform item, int sizeX, int sizeY) {
        
        Vector2? finalAnchorPos = inventoryGrid.GetFinalAnchorPositionToLoadItem(sizeX, sizeY);

        if(finalAnchorPos == null) {
            // Inventory Full !!
            Debug.Log("Inventory is Full !! Cannot store Item");
            return;
        }

        var instantiatedItem = Instantiate(item,finalAnchorPos.Value,Quaternion.identity, _inventoryCanvas.transform);
        instantiatedItem.anchoredPosition = finalAnchorPos.Value;
        instantiatedItem.gameObject.SetActive(false);

        var posArray = inventoryGrid.GetPosArrayUnderItem( instantiatedItem.anchoredPosition.x - (_inventoryCellSize / 2 * sizeX),
                                                           instantiatedItem.anchoredPosition.y - (_inventoryCellSize / 2 * sizeY),
                                                           instantiatedItem.anchoredPosition.x + (_inventoryCellSize / 2 * sizeX),
                                                           instantiatedItem.anchoredPosition.y + (_inventoryCellSize / 2 * sizeY) );
        
        foreach (var pos in posArray){
            inventoryGrid.AddInvalidPositionToSet(pos);
        }

    }

    // Drop when When OnDragEnd
    public void DropItem(RectTransform item, int sizeX, int sizeY)
    {
        Vector2? finalAnchorPosition = inventoryGrid.GetFinalAnchorPositionForItem( 
                                       item.anchoredPosition.x - (_inventoryCellSize / 2 * sizeX),
                                       item.anchoredPosition.y - (_inventoryCellSize / 2 * sizeY),
                                       item.anchoredPosition.x + (_inventoryCellSize / 2 * sizeX),
                                       item.anchoredPosition.y + (_inventoryCellSize / 2 * sizeY) );


        if (finalAnchorPosition == null) {

            item.rotation = originalRotation;
            item.anchoredPosition = originalPosition;

            foreach (var pos in currentPosArray)
            {
                inventoryGrid.AddInvalidPositionToSet(pos);
            }

            return;
        }

        item.position = finalAnchorPosition.Value;
    }

    public void PickUpItem(RectTransform item, int sizeX, int sizeY) {
        
        originalPosition = item.anchoredPosition;
        originalRotation = item.rotation;

        currentPosArray = inventoryGrid.GetPosArrayUnderItem( originalPosition.x - (_inventoryCellSize / 2 * sizeX),
                                                              originalPosition.y - (_inventoryCellSize / 2 * sizeY),
                                                              originalPosition.x + (_inventoryCellSize / 2 * sizeX),
                                                              originalPosition.y + (_inventoryCellSize / 2 * sizeY) );

        foreach (var pos in currentPosArray){

            inventoryGrid.RemovePositionFromSet(pos);
        }
    }

    // Start Moving When OnDrag
    public void ItemDrag(RectTransform item) {

        item.anchoredPosition = currentPointerWorldPos;
        item.anchoredPosition = new Vector2( 
            Mathf.Clamp(item.anchoredPosition.x, _inventoryVisualOrigin.x + item.rect.width/2 ,
                _inventoryVisualOrigin.x + _inventoryVisualFront.rect.width - item.rect.width / 2),

            Mathf.Clamp(item.anchoredPosition.y, _inventoryVisualOrigin.y - _inventoryVisualFront.rect.height + item.rect.height / 2,
                _inventoryVisualOrigin.y - item.rect.height / 2) );

    }

    #endregion

    // Discard item
    public void DiscardItem(RectTransform item, int sizeX, int sizeY)
    {

        var posArray = inventoryGrid.GetPosArrayUnderItem(originalPosition.x - (_inventoryCellSize / 2 * sizeX),
                                                              originalPosition.y - (_inventoryCellSize / 2 * sizeY),
                                                              originalPosition.x + (_inventoryCellSize / 2 * sizeX),
                                                              originalPosition.y + (_inventoryCellSize / 2 * sizeY) );

        foreach (var pos in posArray)
        {
            inventoryGrid.RemovePositionFromSet(pos);
        }

        Destroy(item.gameObject);
    }

}
