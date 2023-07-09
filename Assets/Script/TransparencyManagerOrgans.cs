using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparencyManagerOrgans : MonoBehaviour
{
    [SerializeField]
    new Renderer renderer;
    float alphaValueOrgans = 0.6f;
    bool isTransparent = false;
    bool setTransparent = true;
    bool organIsGettingUsed = false;

    public void SetTransparentOn()
    {
        setTransparent = true;
    }
    public void SetTransparentOff()
    {
        setTransparent = false;
    }
    public void UseOrgan()
    {
        organIsGettingUsed = true;
    }
    public void ReleaseOrgan()
    {
        organIsGettingUsed = false;
    }

    private void Start()
    {
        Color color = renderer.material.color;
        color.a = alphaValueOrgans;
        renderer.material.color = color;
        isTransparent = true;
    }

    private void Update()
    {
        if (setTransparent == true)
        {
            if(organIsGettingUsed == false)
            {
                if (isTransparent == false)
                {
                    Color color = renderer.material.color;
                    color.a = alphaValueOrgans;
                    renderer.material.color = color;
                    isTransparent = true;
                }
            }
        }
        if (setTransparent == false)
        {
            if (isTransparent == true)
            {
                Color color = renderer.material.color;
                color.a = 1;
                renderer.material.color = color;
                isTransparent = true;
            }
        }
    }
}
