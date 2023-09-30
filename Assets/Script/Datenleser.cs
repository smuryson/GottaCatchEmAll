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
    private bool isSpedUp;
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
    public void SetTimeSteps()
    {
        if (!isSpedUp) { timeStepsInSeconds = 0.25f; isSpedUp = true; }
        else { timeStepsInSeconds = 1; isSpedUp = false; }
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
        //for (int i = 0; i < entries.Length; i++)
        //{
        //    if (entries[i].Stunde > maxHour)
        //    {
        //        maxHour = entries[i].Stunde;
        //    }
        //}
        for(int i = 0; i < entries.Length; i++)
        {
            if (entries[i].Final == 1)
            {
                maxHour = entries[i].Stunde;
            }
        }
        isSpedUp = false;
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
                                if(entries[i].Schmerz != 0) //so that no organ will be dialled back after double entries
                                {
                                    ChangeColorPain(mesh, entries[i].Schmerz);
                                }
                            }
                            if (broker.name == "LimitationBroker")
                            {
                                if (entries[i].Kondition != 0) //so that no organ will be dialled back after double entries
                                {
                                    ChangeColorLimitation(mesh, entries[i].Kondition);
                                }
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
        intensity = Mathf.Clamp01(schmerz / 101f);

        Color color;
        if (intensity <= 0.1f)
        {
            color = new Color(0.9490196078431372f, 0.38823529411764707f, 0.3137254901960784f); //hex: F26350
        }
        else if (intensity <= 0.2f)
        {
            color = new Color(0.788235294117647f, 0.25098039215686274f, 0.11372549019607843f); //hex: C9401D
        }
        else if (intensity <= 0.3f)
        {
            color = new Color(0.7215686274509804f, 0.1843137254901961f, 0.12549019607843137f); //hex: B82F20
        }
        else if (intensity <= 0.4f)
        {
            color = new Color(0.6901960784313725f, 0.15294117647058825f, 0.12156862745098039f); //hex: B0271F
        }
        else if (intensity <= 0.5f)
        {
            color = new Color(0.6588235294117647f, 0.11764705882352941f, 0.11372549019607843f); //hex: A81E1D
        }
        else if (intensity <= 0.6f)
        {
            color = new Color(0.596078431372549f, 0.10980392156862745f, 0.10588235294117647f); //hex: 981C1B
        }
        else if (intensity <= 0.7f)
        {
            color = new Color(0.5490196078431373f, 0.09803921568627451f, 0.09803921568627451f); //hex: 8C1919
        }
        else if (intensity <= 0.8f)
        {
            color = new Color(0.4196078431372549f, 0.08235294117647059f, 0.10588235294117647f); //hex: 6B151B
        }
        else if (intensity <= 0.9f)
        {
            color = new Color(0.3568627450980392f, 0.07450980392156863f, 0.10196078431372549f); //hex: 5B131A
        }
        else
        {
            color = new Color(0.2901960784313726f, 0.06666666666666667f, 0.09803921568627451f); //hex: 4A1119
        }


        // assignt farbe zum material
        renderer.material.color = color;

        UnityEngine.Debug.Log(mesh.name + ", Pain: " + schmerz + "%");
    }
    public void ChangeColorLimitation(GameObject mesh, int kondition)
    {
        Renderer renderer = mesh.GetComponent<Renderer>();

        // checkt im mesh nach original material color
        if (!mesh.TryGetComponent(out OriginalMeshData originalData))
        {
            // stored og color wenn nicht schon stored
            originalData = mesh.gameObject.AddComponent<OriginalMeshData>();
            originalData.Material = renderer.material;
        }

        if (kondition == 0)
        {
            // gibt og color zurück wenn wert 0
            renderer.material = originalData.Material;
            return;
        }

        float intensity = Mathf.Clamp01(kondition / 101f);

        Color color;
        if (intensity <= 0.1f)
        {
            color = new Color(0.7490196078431373f, 0.9372549019607843f, 1f); //hex: BFEFFF
        }
        else if (intensity <= 0.2f)
        {
            color = new Color(0.6588235294117647f, 0.9019607843137255f, 1f); //hex: A8E6FF
        }
        else if (intensity <= 0.3f)
        {
            color = new Color(0.47843137254901963f, 0.8274509803921568f, 1f); //hex: 7AD3FF
        }
        else if (intensity <= 0.4f)
        {
            color = new Color(0.38823529411764707f, 0.792156862745098f, 1f); //hex: 63CAFF
        }
        else if (intensity <= 0.5f)
        {
            color = new Color(0.20784313725490197f, 0.7215686274509804f, 1f); //hex: 35B8FF
        }
        else if (intensity <= 0.6f)
        {
            color = new Color(0f, 0.611764705882353f, 1f); //hex: 009CFF
        }
        else if (intensity <= 0.7f)
        {
            color = new Color(0.2980392156862745f, 0.33725490196078434f, 0.9607843137254902f); //hex: 4C56F5
        }
        else if (intensity <= 0.8f)
        {
            color = new Color(0.24705882352941178f, 0.2823529411764706f, 0.8f); //hex: 3F48CC
        }
        else if (intensity <= 0.9f)
        {
            color = new Color(0.20392156862745098f, 0.23137254901960785f, 0.6588235294117647f); //hex: 343BA8
        }
        else
        {
            color = new Color(0.1568627450980392f, 0.1803921568627451f, 0.5098039215686274f); //hex: 282E82
        }

        // Create a new material with the desired color
        Material newMaterial = new Material(originalData.Material);
        newMaterial.color = color;

        // assignt farbe zum material
        renderer.material = newMaterial;

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
        //isReverse = true;
        if(isReverse == true)
        {
            isReverse = false;
        }
        else
        {
            isReverse = true;
        }
    }
    //public void UndoInverse()
    //{
    //    isReverse = false;
    //}
    public void Reset()
    {
        hour = 0;
        savedHour = 0;
        isReset = true;
        Stop();
        isStopped = true;
        isReverse = false;

        for (int i = 0; i < entries.Length; i++)
        {
            for (int j = 0; j < organBrokers.Length; j++)
            {
                int brokerId = addIDList[j].GetID();
                if (brokerId == entries[i].OrganID)
                {
                    GameObject mesh = addIDList[j].GetMesh();
                    if (broker.name == "PainBroker")
                    {
                        ChangeColorPain(mesh, 0);
                    }
                    if (broker.name == "LimitationBroker")
                    {
                        ChangeColorLimitation(mesh, 0);
                    }
                }
            }
        }
    }
}
public class OriginalMeshData : MonoBehaviour
{
    public Material Material { get; set; }
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
        this.KrankheitID = Convert.ToInt32(input[0]);
        this.OrganID = Convert.ToInt32(input[1]);
        this.Stunde = Convert.ToInt32(input[2]);
        this.Kondition = Convert.ToInt32(input[3]);
        this.Schmerz = Convert.ToInt32(input[4]);
        this.Final = Convert.ToInt32(input[5]);
    }

    public int KrankheitID { get => krankheitID; set => krankheitID = value; }
    public int OrganID { get => organID; set => organID = value; }
    public int Stunde { get => stunde; set => stunde = value; }
    public int Kondition { get => kondition; set => kondition = value; }
    public int Schmerz { get => schmerz; set => schmerz = value; }
    public int Final { get => final; set => final = value; }
}