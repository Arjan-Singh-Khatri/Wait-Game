using System.Collections.Generic;
using Unity;
using UnityEngine;
using UnityEngine.UI;



[System.Serializable]
[CreateAssetMenu(fileName = "ItemAttribute" , menuName = "ItemAttribute" )]
public class ItemAttributes : ScriptableObject
{
    public Image itemVisualPrefab;
    public int _sizeX;
    public int _sizeY;
    public string _name;
    public string _description;
    public string itemCode;
    public string itemInteractText;
    public ItemTypes itemType = ItemTypes.key;
}

public class ItemListHolder: MonoBehaviour {

    public static ItemListHolder Instance;

    public Dictionary<string, ItemAttributes> itemList;

    private void Awake()
    {
        

    }
}