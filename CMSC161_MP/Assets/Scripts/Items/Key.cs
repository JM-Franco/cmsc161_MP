using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Key : MonoBehaviour, IInteractable
{
    public InventoryManager inventoryManager;
    public GameObject promptCanvas;
    public AudioClip useSFX, pickupSFX;
    public Item item;

    public void Start()
    {
        inventoryManager = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
        GameObject ui = GameObject.Find("UI");
        foreach(Transform t in ui.GetComponentInChildren<Transform>())
        {
            if (t.name == "PromptCanvas")
            {
                promptCanvas = t.gameObject;
            }
        }
    }

    public void Use()
    {
        throw new System.NotImplementedException();
    }

    public void Interact()
    {
        if (!inventoryManager.CheckFreeSlot())
        {
            promptCanvas.SetActive(true);
            promptCanvas.GetComponentInChildren<TextMeshProUGUI>().text = "Inventory Slot Full";
            return;
        }
        SoundFXManager.instance.PlaySoundFXClip(pickupSFX, transform, 1f);
        inventoryManager.AddItem(item);
        //transform.SetParent(inventoryManager.hand.transform);
        //transform.localPosition = Vector3.zero;
        //transform.localRotation = Quaternion.Euler(Vector3.zero);
        //transform.localScale = Vector3.one;

        Destroy(gameObject);
    }

    public void ShowPrompt()
    {
        if (!promptCanvas.activeSelf)
        {
            promptCanvas.SetActive(true);
            promptCanvas.GetComponentInChildren<TextMeshProUGUI>().text = "Press E to pick up";
        }

    }
}
