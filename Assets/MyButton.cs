using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyButton : MonoBehaviour {
    public GameObject[] interactObjects;

    public void OnIterect ()
    {
        for (int i = 0; i < interactObjects.Length; i++)
        {
            if (interactObjects[i].GetComponent<MyDoor>() != null)
                interactObjects[i].GetComponent<MyDoor>().OnActive();
        }
    }
}
