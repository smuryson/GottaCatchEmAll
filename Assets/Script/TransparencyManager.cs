using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparencyManager : MonoBehaviour
{
    [SerializeField]
    new Renderer renderer;
    float alphaValueBones = 0.3f;
    bool isTransparent = false;
    bool setTransparent = true;
    public void SetTransparentOn()
    {
        setTransparent = true;
    }
    public void SetTransparentOff()
    {
        setTransparent = false;
    }

    private void Start()
    {
        Color color = renderer.material.color;
        color.a = alphaValueBones;
        renderer.material.color = color;
        isTransparent = true;
    }

    private void Update()
    {
        if(setTransparent == true)
        {
            if(isTransparent == false)
            {
                Color color = renderer.material.color;
                color.a = alphaValueBones;
                renderer.material.color = color;
                isTransparent = true;
            }
        }
        if(setTransparent == false)
        {
            if(isTransparent == true)
            {
                Color color = renderer.material.color;
                color.a = 1;
                renderer.material.color = color;
                isTransparent = true;
            }
        }
    }
}
