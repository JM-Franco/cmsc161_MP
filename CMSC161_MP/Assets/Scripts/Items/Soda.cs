using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Soda : MonoBehaviour, IInteractable
{
    public InventoryManager inventoryManager;
    public GameObject promptCanvas;
    public Item item;

    public void Start()
    {
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
        GameObject.Find("FirstPersonPlayer").GetComponent<PlayerMovements>().StartCoroutine(UseSoda());
    }

    IEnumerator UseSoda()
    {
        PlayerMovements playerMovements = GameObject.Find("FirstPersonPlayer").GetComponent<PlayerMovements>();
        playerMovements.walkSpeed *= 2f;
        playerMovements.sprintSpeed *= 2f;
        playerMovements.crouchSpeed *= 2f;
        Debug.Log("Applying new speed values");
        yield return new WaitForSeconds(3f);
        Debug.Log("Returning previous speed values");
        playerMovements.walkSpeed /= 2f;
        playerMovements.sprintSpeed /= 2f;
        playerMovements.crouchSpeed /= 2f;
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
