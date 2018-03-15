using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyDoor : MonoBehaviour {
    public Vector3 distance = new Vector3();
    public float time = 1.0f;
    public bool isOpen = false;

    private doorState state = doorState.close;
    private Vector3 closePosition;
    private bool[] moreOpen;

    enum doorState { opening, closing, open, close }

    private void Start()
    {
        closePosition = transform.position;
        if (isOpen)
        {
            transform.position += distance; 
            state = doorState.opening;
        }

        moreOpen = new bool[3] {closePosition.y + distance.y > closePosition.y, closePosition.y + distance.y > closePosition.y, closePosition.z + distance.z > closePosition.z};
    }

    private void Update()
    {
        if (state == doorState.closing)
        {
            transform.position += -distance / (1 / Time.deltaTime * time);
            if (MoreVector(closePosition, transform.position)) state = doorState.close;
        }
        else if (state == doorState.opening)
        {
            transform.position += distance / (1 / Time.deltaTime * time);
            if (MoreVector(transform.position, closePosition + distance)) state = doorState.open;
        }
    }

    public void OnActive ()
    {
        if (state == doorState.close || state == doorState.closing)
            state = doorState.opening;
        else
            state = doorState.closing;
    }


    private bool MoreVector (Vector3 vector1, Vector3 vector2)
    {
        return
            (moreOpen[0] ? vector1.x >= vector2.x : vector1.x <= vector2.x) &&
            (moreOpen[1] ? vector1.y >= vector2.y : vector1.y <= vector2.y) &&
            (moreOpen[2] ? vector1.z >= vector2.z : vector1.z <= vector2.z);
    }
}
