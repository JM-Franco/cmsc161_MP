using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public InventorySlot[] inventorySlots;
    public GameObject inventoryItemPrefab;
    public GameObject holdPosition;

    public int selectedSlot = -1;

    private void Start()
    {
        ChangeSelectedSlot(0);
    }

    private void Update()
    {
        int numSlots = inventorySlots.Length;
        if (Input.mouseScrollDelta.y > 0)
        {
            ChangeSelectedSlot((selectedSlot + 1) % numSlots);
        }
        else if (Input.mouseScrollDelta.y < 0)
        {
            ChangeSelectedSlot((((selectedSlot - 1) % numSlots) + numSlots) % numSlots);
        }

        if (!CheckFreeSlot() && holdPosition.transform.childCount < 1) ReplaceHeldItemModel();

        if (Input.GetKeyDown(KeyCode.Q))
        {
            DropItem(); 
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && inventorySlots[selectedSlot].transform.childCount != 0)
        {
            Debug.Log("Item Used");
            UseItem();
        }
    }

    public void AddItem(Item item)
    {
        SpawnNewItem(item, inventorySlots[selectedSlot]);
	}

    void SpawnNewItem(Item item, InventorySlot slot)
    {
        GameObject newItemGo = Instantiate(inventoryItemPrefab, slot.transform);
        InventoryItem inventoryItem = newItemGo.GetComponent<InventoryItem>();
        inventoryItem.InitializeItem(item);
	}

    public void DropItem()
    {
        // Release item model from held position
        GameObject heldItem = holdPosition.transform.GetChild(0).gameObject;
        heldItem.GetComponent<Rigidbody>().isKinematic = false;
        heldItem.GetComponent<Collider>().isTrigger = false;
        heldItem.transform.SetParent(null);

        // Remove item from inventory
        InventoryItem inventoryItem = inventorySlots[selectedSlot].transform.GetComponentInChildren<InventoryItem>();
        Destroy(inventoryItem.gameObject);
    }

    public void UseItem()
    {
        GameObject heldItem = holdPosition.transform.GetChild(0).gameObject; // Reference to the object in the player's viewmodel

        // Call Use function of item
        IInteractable heldItemScript = heldItem.GetComponent<IInteractable>();
        heldItemScript.Use();

        InventoryItem activeInventoryItem = inventorySlots[selectedSlot].GetComponentInChildren<InventoryItem>();
        if (activeInventoryItem.item.destroyItemOnUse)
        {
            Destroy(heldItem); // Destroy item in viewmodel
            Destroy(activeInventoryItem.gameObject); // Destroy item in inventory
        }
    }

    void ChangeSelectedSlot(int newValue)
    {
        // Change selected slot
        if (selectedSlot >= 0)
        {
            inventorySlots[selectedSlot].Deselect();
        }

        inventorySlots[newValue].Select();
        selectedSlot = newValue;

        // Remove view model of held item
        if (holdPosition.transform.childCount > 0)
        {
            Destroy(holdPosition.transform.GetChild(0).gameObject);
        }

    }

    void ReplaceHeldItemModel()
    {
        InventoryItem inventoryItem = inventorySlots[selectedSlot].GetComponentInChildren<InventoryItem>();
        GameObject itemOnholdPosition = Instantiate(inventoryItem.item.prefab);
        itemOnholdPosition.transform.SetParent(holdPosition.transform);
        itemOnholdPosition.transform.localPosition = Vector3.zero;
        itemOnholdPosition.transform.localRotation = Quaternion.Euler(Vector3.zero);
        //itemOnholdPosition.transform.localScale = Vector3.one;

        if (inventoryItem.item.type == Item.ItemType.Flashlight)
        {
            itemOnholdPosition.transform.localRotation = Quaternion.Euler(new Vector3(356.871399f, 356.936035f, 358.72583f));
        }

        itemOnholdPosition.GetComponent<Rigidbody>().isKinematic = true;
        itemOnholdPosition.GetComponent<Collider>().isTrigger = true;
    }

    public bool CheckFreeSlot()
    {
        return inventorySlots[selectedSlot].transform.childCount == 0;
    }
}
