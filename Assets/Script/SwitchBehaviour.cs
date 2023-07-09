using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchBehaviour : MonoBehaviour
{
    [SerializeField]
    RectTransform handleRect;
    [SerializeField]
    Color backgroundColorOn;
    [SerializeField]
    Color handleColorOn;
    [SerializeField]
    Image backgroundImage;
    [SerializeField]
    Image handleImage;

    Color backgroundColorOff;
    Color handleColorOff;

    Toggle toggle;

    Vector2 handlePos;

    private void Start()
    {
        toggle = GetComponent<Toggle>();
        handlePos = handleRect.anchoredPosition;
        backgroundColorOff = backgroundImage.color;
        handleColorOff = handleImage.color;
        toggle.onValueChanged.AddListener(OnSwitch);

        if(toggle.isOn)
        {
            OnSwitch(true);
        }
    }

    void OnSwitch(bool isOn)
    {
        if(isOn == true)
        {
            handleRect.anchoredPosition = handlePos * -1;
            backgroundImage.color = backgroundColorOn;
            handleImage.color = handleColorOn;
        }
        else
        {
            handleRect.anchoredPosition = handlePos;
            backgroundImage.color = backgroundColorOff;
            handleImage.color = handleColorOff;
        }
    }
    private void OnDestroy()
    {
        toggle.onValueChanged.RemoveListener(OnSwitch);
    }
}
