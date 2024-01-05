 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IInteractable
{
    public void Interact();
	public void Use();
	public void ShowPrompt();
}

public class Interactor : MonoBehaviour
{
    public Transform InteractorSource;
    public float InteractorRange;
	public GameObject messageCanvas;
    // Update is called once per frame
    void Update()
    {
		Ray r = new Ray(InteractorSource.position, InteractorSource.forward);
		if (Physics.Raycast(r, out RaycastHit hitInfo, InteractorRange))
		{ 
			if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj))
			{
				interactObj.ShowPrompt();

				if (Input.GetKeyDown(KeyCode.E))
				{
					interactObj.Interact();
				}
			}
		}
		else
		{ 
			if (messageCanvas.activeSelf)
			{
				messageCanvas.SetActive(false);
			}
		}
    }
}
