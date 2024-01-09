using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Flashlight : MonoBehaviour, IInteractable
{
    public InventoryManager inventoryManager;
    public GameObject promptCanvas;
    public GameObject lightBeam;
    public AudioClip useSFX, pickupSFX;
    public Item item;


    public void Start()
    {
        // Assign lightbeam
        lightBeam = transform.GetChild(0).gameObject;

        inventoryManager = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
        GameObject ui = GameObject.Find("UI");
        foreach (Transform t in ui.GetComponentInChildren<Transform>())
        {
            if (t.name == "PromptCanvas")
            {
                promptCanvas = t.gameObject;
            }
        }
    }

    public void Use()
    {
        SoundFXManager.instance.PlaySoundFXClip(useSFX, transform, 1f);
        lightBeam.SetActive(!lightBeam.activeSelf);
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
