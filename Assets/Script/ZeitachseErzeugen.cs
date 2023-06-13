using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class ZeitachseErzeugen : MonoBehaviour
{
    float maxValue;
    bool hourToDay = false;
    [SerializeField]
    Datenleser datenleser;
    private RectTransform graphContainer;
    private RectTransform labelTemplateX;
    private RectTransform dashTemplate;
    private RectTransform iconRect;
    bool checkedMax = false;
    [SerializeField]
    Canvas canvas;
    [SerializeField]
    Text skala;
    [SerializeField]
    Text ifHour;
    [SerializeField]
    Text ifDay;
    int hour;

    void Start()
    {
        hour = 0;
    }
    private void Awake()
    {
        graphContainer = transform.Find("GraphContainer").GetComponent<RectTransform>();
        labelTemplateX = graphContainer.Find("TimeStampsTemplate").GetComponent<RectTransform>();
        dashTemplate = graphContainer.Find("DashTemplate").GetComponent<RectTransform>();
        iconRect = graphContainer.Find("Icon").GetComponent<RectTransform>();
    }
    private void Update()
    {
        //RectTransform newIcon = null;
        if(checkedMax == false)
        {
            int maxValue = datenleser.GetMaxHour();
            MakeTimeStamps(maxValue);
            checkedMax = true;
            //newIcon = Instantiate(iconRect);
            //CreateIcon(0,newIcon);
        }
        //int previousHour = hour;
        hour = datenleser.GetHour();
        //if(hour != previousHour)
        //{
        //    MoveIcon(hour, newIcon);
        //}
    }
    private void CreateIcon(float hour, RectTransform newIcon)
    {
        maxValue = datenleser.GetMaxHour();
        newIcon = Instantiate(iconRect);
        newIcon.SetParent(graphContainer);
        float graphSizeY = graphContainer.sizeDelta.y;
        float xPos = (hour / maxValue) * 1050;
        if(xPos == float.NaN)
        {
            xPos = 0;
        }
        newIcon.gameObject.SetActive(true);
        newIcon.anchoredPosition = new Vector2(xPos, 50);

        iconRect.gameObject.SetActive(false);
    }
    //private void MoveIcon(float hour, RectTransform newIcon)
    //{
    //    float graphSizeY = graphContainer.sizeDelta.y;
    //    float xPos = (hour / maxValue) * 1050;
    //    if (xPos == float.NaN)
    //    {
    //        xPos = 0;
    //    }
    //    newIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(xPos, 50);
    //}
    private void MakeTimeStamps(int maxValue)
    {
        float graphSizeY = graphContainer.sizeDelta.y;
        float formerMaxValue = 0;
        //Schriftgroesse und Tag/Stunde anpassen
        if (maxValue < 73)
        {
            skala.fontSize = 10 * (60 / maxValue);
        }
        else
        {
            formerMaxValue = maxValue / 6;
            maxValue = maxValue / 24;
            hourToDay = true;
        }
        //Skala zu Tag umschalten und den jeweiligen Schriftzug deaktivieren
        if(hourToDay == false)
        {
            ifDay.gameObject.SetActive(false);
        }
        if(hourToDay == true)
        {
            ifHour.gameObject.SetActive(false);
        }
        //Zahlen generieren
        for (float i = 0; i <= maxValue; i++)
        {
            float xPos = (i / maxValue) *1050 + (1 / maxValue) * 1050;
            RectTransform labelX = Instantiate(labelTemplateX);
            labelX.SetParent(graphContainer);
            labelX.gameObject.SetActive(true);
            labelX.anchoredPosition = new Vector2(xPos, graphSizeY-20);
            labelX.GetComponent<Text>().text = i.ToString();

            RectTransform dash = Instantiate(dashTemplate);
            dash.SetParent(graphContainer);
            dash.gameObject.SetActive(true);
            dash.anchoredPosition = new Vector2(xPos, graphSizeY-50);
        }
        //falls Einheit = Tag, wollen Punkte, die die Stunden symbolisieren
        if(hourToDay == true)
        {
            for (float i = 0; i <= formerMaxValue; i++)
            {
                float xPos = (i / formerMaxValue) * 1050 + (1 / formerMaxValue) * 1050;
                RectTransform labelDot = Instantiate(labelTemplateX);
                labelDot.SetParent(graphContainer);
                labelDot.gameObject.SetActive(true);
                labelDot.anchoredPosition = new Vector2(xPos, graphSizeY - 20);
                labelDot.GetComponent<Text>().text = ".";
            }
        }
        labelTemplateX.gameObject.SetActive(false);
        dashTemplate.gameObject.SetActive(false);
    }
}
