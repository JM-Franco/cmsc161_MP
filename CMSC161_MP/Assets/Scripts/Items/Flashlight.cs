using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Flashlight : MonoBehaviour, IInteractable
{
    public InventoryManager inventoryManager;
    public GameObject promptCanvas;
    public GameObject lightBeam;
    public Item item;

    public void Start()
    {
        lightBeam = transform.GetChild(0).gameObject;
        inventoryManager = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
        GameObject hud = GameObject.Find("HUD");
        foreach (Transform t in hud.GetComponentInChildren<Transform>())
        {
            if (t.name == "PromptCanvas")
            {
                promptCanvas = t.gameObject;
            }
        }
    }

    public void Use()
    {
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
