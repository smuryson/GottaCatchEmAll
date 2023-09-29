using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationManager : MonoBehaviour
{
    [SerializeField]
    GameObject body;
    [SerializeField]
    float yRotation;
    bool isRotating;
    bool isLeft;
    void Start()
    {
        isRotating = false;
        isLeft = false;

        yRotation = 0.2f;
    }

    // Update is called once per frame
    void Update()
    {
        if (isRotating)
        {
            if (!isLeft)
            {
                body.transform.Rotate(0, yRotation, 0, Space.Self);
            }
            if (isLeft)
            {
                body.transform.Rotate(0, - yRotation, 0, Space.Self);
            }
        }
    }
    public void rotateLeft()
    {
        isRotating = true;
        isLeft = false;
    }
    public void rotateRight()
    {
        isRotating = true;
        isLeft = true;
    }
    public void rotateOff()
    {
        isRotating = false;
    }
    public void Reset()
    {
        isRotating = false;
        isLeft = false;
        Vector3 defaultVec = new Vector3(0, 180, 0);
        body.transform.eulerAngles = defaultVec;
    }
}