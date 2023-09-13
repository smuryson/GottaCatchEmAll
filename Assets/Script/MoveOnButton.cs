using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class MoveOnButton : MonoBehaviour
{
    [SerializeField]
    Camera cam;
    static Vector3 posMid = Vector3.zero;
    static Vector3 posRight = Vector3.zero;
    static Vector3 posLeft = Vector3.zero;
    static int current;
    // Start is called before the first frame update
    void Start()
    {
        cam.orthographicSize = 1.8f;
        posLeft.x = -1;
        posLeft.y = 2.5f;
        posLeft.z = -1;
        current = 0;
        posMid.x = 0;
        posMid.y = 2.6f;
        posMid.z = -1.8f;
        posRight.x = 1.05f;
        posRight.y = 2.5f;
        posRight.z = -1;
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void DoMove(int i)
    {
        switch (i)
        {
            case 0:
                cam.orthographicSize = 1.8f;
                transform.position = posMid;
                break;
            case 1:
                cam.orthographicSize = 1f;
                transform.position = posRight;
                break;
            case -1:
                cam.orthographicSize = 1f;
                transform.position = posLeft;
                break;
        }
    }
    public void Move(int i)
    {
        switch (i)
        {
            case 1:
                if (current == 1)
                    break;
                current += 1;
                DoMove(current);
                break;
            case -1:
                if (current == -1)
                    break;
                current += -1;
                DoMove(current);
                break;
        }
    }
}