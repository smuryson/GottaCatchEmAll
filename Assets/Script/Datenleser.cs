using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEditor.VersionControl;
using UnityEngine;

public class Datenleser : MonoBehaviour
{
    [SerializeField]
    private TextAsset dataset;
    [SerializeField]
    private float timeStepsInSeconds;
    [SerializeField]
    GameObject broker;
    bool isReverse;
    int maxHour;
    int minHour = 0;
    int hour = 0;
    int savedHour = 0;
    Entry[] entries;
    bool isStopped;
    bool isReset;
    GameObject[] organBrokers;
    AddIDToOrganMesh[] addIDList;

    public int GetMaxHour()
    {
        return maxHour;
    }
    public int GetHour()
    {
        return hour;
    }
    public float GetTimesteps()
    {
        return timeStepsInSeconds;
    }
    public bool GetIsStopped()
    {
        return isStopped;
    }
    public bool GetIsReset()
    {
        return isReset;
    }
    // Start is called before the first frame updater
    void Start()
    {
        //Zeitstrahl bzw. Buttonvariablen initieren
        isReverse = false;
        isStopped = true;
        isReset = true;

        //Array mit allen Kindern(1.Grad) vom Brokerbody erstellen
        int brokerChildCount = broker.transform.childCount;
        organBrokers = new GameObject[brokerChildCount];
        for(int i = 0 ; i < brokerChildCount; i++)
        {
            organBrokers[i] = broker.transform.GetChild(i).gameObject;
        }
        //Array mit allen AddIdToMesh Skripten
        addIDList = new AddIDToOrganMesh[organBrokers.Length];
        for(int i = 0 ; i < organBrokers.Length ; i++)
        {
            addIDList[i] = organBrokers[i].GetComponent<AddIDToOrganMesh>();
        }

        //added Path des Nutzers, der bis /Assets geht mit dem Path bis zur Textdatei
        string pathToAssets = Application.dataPath;
        string pathRest = "/Datasets/" + dataset.name + ".txt";
        string path = pathToAssets + pathRest;

        //liest Datenset
        StreamReader DatenBankReader = new StreamReader(path);

        string content = DatenBankReader.ReadToEnd();

        //splittet Eintr�ge
        string[] data = content.Split('\n');
        string[] values = new string[6];
        entries = new Entry[data.Length];
        for (int i = 0; i < data.Length; i++)
        {
            values = data[i].Split('.');
            entries[i] = new Entry(values);
        }
        maxHour = 0;
        for (int i = 0; i < entries.Length; i++)
        {
            if (entries[i].Stunde > maxHour)
            {
                maxHour = entries[i].Stunde;
            }
        }
    }
    IEnumerator UpdateModelEveryTimestep(Entry[] entries)
    {
        for (hour = savedHour; hour < maxHour + 2; hour++)
        {
            //check, ob Reverse an ist
            if (isReverse == true)
            {
                hour--;
                hour--;
            }
            //checke,ob Stunde unter 0
            if(hour < minHour)
            {
                hour = minHour;
            }
            //checke, ob Stunde unter Max
            if(hour > maxHour)
            {
                hour = maxHour;
            }
            //mache Zeug fuer alle betroffenen Objekte
            for (int i = 0; i < entries.Length; i++)
            {
                if (hour == entries[i].Stunde)
                {
                    
                    for (int j = 0; j < organBrokers.Length; j++)
                    {
                        int brokerId = addIDList[j].GetID();
                        if (brokerId == entries[i].OrganID)
                        {
                            GameObject mesh = addIDList[j].GetMesh();
                            if (broker.name == "PainBroker")
                            {
                                ChangeColorPain(mesh, entries[i].Schmerz);
                            }
                            if (broker.name == "LimitationBroker")
                            {
                                ChangeColorLimitation(mesh, entries[i].Kondition);
                            }
                        }
                    }
                        UnityEngine.Debug.Log("Stunde " + entries[i].Stunde + ": Organ " + entries[i].OrganID + " | Betroffenheit: " + entries[i].Kondition + " | Schmerz: " + entries[i].Schmerz);
                }
            }
        yield return new WaitForSeconds(timeStepsInSeconds);
        }
    }

    public class OriginalMeshColor  : MonoBehaviour
    {
        public Color Color { get; set; }
    }

    public void ChangeColorPain(GameObject mesh, int schmerz)
    {
        Renderer renderer = mesh.GetComponent<Renderer>();

        // checkt im mesh nach original material color
        if (!mesh.TryGetComponent(out OriginalMeshColor originalColor))
        {
            // stored og color wenn nicht schon stored
            originalColor = mesh.gameObject.AddComponent<OriginalMeshColor>();
            originalColor.Color = renderer.material.color;
        }

        if (schmerz == 0)
        {
            // gibt og color zurück wenn wert 0
            renderer.material.color = originalColor.Color;
            return;
        }

        float intensity = Mathf.Clamp01(schmerz / 101f);

        Color color;
        if (intensity <= 0.1f)
        {
            color = new Color(0.8823529411764706f, 0.6470588235294118f, 0.13725490196078433f); //hex: E1A523
        }
        else if (intensity <= 0.2f)
        {
            color = new Color(0.8588235294117647f, 0.5882352941176471f, 0.12941176470588237f); //hex: DB9621
        }
        else if (intensity <= 0.3f)
        {
            color = new Color(0.8313725490196079f, 0.5411764705882353f, 0.12941176470588237f); //hex: D48A21
        }
        else if (intensity <= 0.4f)
        {
            color = new Color(0.8156862745098039f, 0.48627450980392156f, 0.13333333333333333f); //hex: D07C22
        }
        else if (intensity <= 0.5f)
        {
            color = new Color(0.792156862745098f, 0.42745098039215684f, 0.12156862745098039f); //hex: CA6D1F
        }
        else if (intensity <= 0.6f)
        {
            color = new Color(0.7647058823529411f, 0.3843137254901961f, 0.3843137254901961f); //hex: C3621D
        }
        else if (intensity <= 0.7f)
        {
            color = new Color(0.7372549019607844f, 0.3215686274509804f, 0.11764705882352941f); //hex: BC521E
        }
        else if (intensity <= 0.8f)
        {
            color = new Color(0.7176470588235294f, 0.27058823529411763f, 0.12549019607843137f); //hex: B74520
        }
        else if (intensity <= 0.9f)
        {
            color = new Color(0.6823529411764706f, 0.21176470588235294f, 0.11764705882352941f); //hex: AE361E
        }
        else
        {
            color = new Color(0.6549019607843137f, 0.15294117647058825f, 0.11764705882352941f); //hex: A7271E
        }

        // assignt farbe zum material
        renderer.material.color = color;

        UnityEngine.Debug.Log(mesh.name + ", Pain: " + schmerz + "%");
    }
    public void ChangeColorLimitation(GameObject mesh, int kondition)
    {
        Renderer renderer = mesh.GetComponent<Renderer>();

        // checkt im mesh nach original material color
        if (!mesh.TryGetComponent(out OriginalMeshColor originalColor))
        {
            // stored og color wenn nicht schon stored
            originalColor = mesh.gameObject.AddComponent<OriginalMeshColor>();
            originalColor.Color = renderer.material.color;
        }

        if (kondition == 0)
        {
            // gibt og color zurück wenn wert 0
            renderer.material.color = originalColor.Color;
            return;
        }

        float intensity = Mathf.Clamp01(kondition / 101f);

        Color color;
        if (intensity <= 0.1f)
        {
            color = new Color(0.4666666666666667f, 0.9411764705882353f, 0.4980392156862745f); //hex: 77F07F
        }
        else if (intensity <= 0.2f)
        {
            color = new Color(0.4117647058823529f, 0.8313725490196079f, 0.4392156862745098f); //hex: 69D470
        }
        else if (intensity <= 0.3f)
        {
            color = new Color(0.24313725490196078f, 0.788235294117647f, 0.5843137254901961f); //hex: 3EC995
        }
        else if (intensity <= 0.4f)
        {
            color = new Color(0.2980392156862745f, 0.2980392156862745f, 0.8f); //hex: 4CBCCC
        }
        else if (intensity <= 0.5f)
        {
            color = new Color(0.2549019607843137f, 0.6274509803921569f, 0.6823529411764706f); //hex: 41A0AE
        }
        else if (intensity <= 0.6f)
        {
            color = new Color(0.27058823529411763f, 0.5098039215686274f, 0.7803921568627451f); //hex: 4582C7
        }
        else if (intensity <= 0.7f)
        {
            color = new Color(0.21176470588235294f, 0.4f, 0.611764705882353f); //hex: 36669C
        }
        else if (intensity <= 0.8f)
        {
            color = new Color(0.4117647058823529f, 0.3333333333333333f, 0.7607843137254902f); //hex: 6955C2
        }
        else if (intensity <= 0.9f)
        {
            color = new Color(0.2980392156862745f, 0.2980392156862745f, 0.5490196078431373f); //hex: 4C3E8C
        }
        else
        {
            color = new Color(0.09411764705882353f, 0.11372549019607843f, 0.11372549019607843f); //hex: 181D6B
        }

        // assignt farbe zum material
        renderer.material.color = color;

        UnityEngine.Debug.Log(mesh.name + ", Limitation: " + kondition + "%");
    }

    public void Starting()
    {
        StartCoroutine(UpdateModelEveryTimestep(entries));
        isStopped = false;
        isReset = false;
    }
    public void Stop()
    {
        savedHour = hour;
        StopAllCoroutines();
        if(isReset == true)
        {
            isStopped = false;
        }else isStopped = true;
    }
    public void InverseTime()
    {
        isReverse = true;
    }
    public void UndoInverse()
    {
        isReverse = false;
    }
    public void Reset()
    {
        hour = 0;
        savedHour = 0;
        isReset = true;
        Stop();
        isStopped = true;
        UndoInverse();
    }
}
public class Entry
{
    int krankheitID;
    int organID;
    int stunde;
    int kondition;
    int schmerz;
    int final;
    public string output()
    {
        return Convert.ToString(stunde + " " + kondition + " " + schmerz);
    }


    public Entry(string[] input)
    {
        this.KrankheitID = Convert.ToInt16(input[0]);
        this.OrganID = Convert.ToInt16(input[1]);
        this.Stunde = Convert.ToInt16(input[2]);
        this.Kondition = Convert.ToInt16(input[3]);
        this.Schmerz = Convert.ToInt16(input[4]);
        this.Final = Convert.ToInt16(input[5]);
    }

    public int KrankheitID { get => krankheitID; set => krankheitID = value; }
    public int OrganID { get => organID; set => organID = value; }
    public int Stunde { get => stunde; set => stunde = value; }
    public int Kondition { get => kondition; set => kondition = value; }
    public int Schmerz { get => schmerz; set => schmerz = value; }
    public int Final { get => final; set => final = value; }
}