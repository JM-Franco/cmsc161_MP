using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    public Item item;

    [Header("UI")]
    public Image image;

    public void InitializeItem(Item newItem)
    {
        item = newItem;
        image = GetComponent<Image>();
        image.sprite = newItem.image;

	}
}
