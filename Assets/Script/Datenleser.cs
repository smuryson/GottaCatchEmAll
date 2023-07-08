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
        if (intensity <= 0.25f)
        {
            color = new Color(0.82f, 0.62f, 0.64f); //hex: D19EA2
        }
        else if (intensity <= 0.5f)
        {
            color = new Color(0.82f, 0.41f, 0.46f); //hex: D16976
        }
        else if (intensity <= 0.75f)
        {
            color = new Color(0.81f, 0.2f, 0.28f); //hex: D13447
        }
        else
        {
            color = new Color(0.73f, 0f, 0.08f); //hex: BA0015
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
        if (intensity <= 0.25f)
        {
            color = new Color(0.66f, 0.77f, 0.89f); //hex: AAC6E4
        }
        else if (intensity <= 0.5f)
        {
            color = new Color(0.44f, 0.65f, 0.9f); //hex: 70A6E4
        }
        else if (intensity <= 0.75f)
        {
            color = new Color(0.22f, 0.53f, 0.9f); //hex: 3989E4
        }
        else
        {
            color = new Color(0f, 0.41f, 0.9f); //hex: 0059BF
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