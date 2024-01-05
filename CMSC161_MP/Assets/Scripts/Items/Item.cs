using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Scriptable object/Item")]
public class Item : ScriptableObject
{
    //public TileBase tile;
    //public ActionType actionType;
    //public Vector2Int range = new Vector2Int(5, 4);

    [Header("Only gameplay")]
    public ItemType type;
    public bool destroyItemOnUse;

    //[Header("Only UI")]

    [Header("Both")]
    public Sprite image;
    public GameObject prefab;

    public enum ItemType
    { 
        Ball,
        Key,
        Soda,
        NoiseMaker,
        Flashlight
	}

    public enum ActionType
    { 
        Foo,
        Bar
	}
}
