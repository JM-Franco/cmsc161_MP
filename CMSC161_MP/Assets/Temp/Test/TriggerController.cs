using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TriggeController : MonoBehaviour
{
    [SerializeField] private Animator myDoor = null;
    [SerializeField] private bool openTrigger = false;
    [SerializeField] private bool closeTrigger = false;

    [SerializeField] private string doorOpen = "DoorOpen";
    [SerializeField] private string doorClose = "DoorClose";
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (openTrigger)
            {
                myDoor.Play("Door Open", 0, 0.0f);
                gameObject.SetActive(false);
            }
            else if (closeTrigger)
{
                myDoor.Play("Door Close", 0, 0.0f);
                gameObject.SetActive(false);
            }
        }
    }
}