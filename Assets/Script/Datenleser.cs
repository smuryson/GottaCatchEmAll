using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEditor.VersionControl;
using UnityEngine;

public class Datenleser : MonoBehaviour
{
    [SerializeField]
    private TextAsset dataset;
    [SerializeField]
    private float timeSteps;
    [SerializeField]
    private int duration;

    // Start is called before the first frame update
    void Start()
    {
        //added Path des Nutzers, der bis /Assets geht mit dem Path bis zur Textdatei
        string pathToAssets = Application.dataPath;
        string pathRest = "/Datasets/" + dataset.name + ".txt";
        string path = pathToAssets + pathRest;

        //liest Datenset
        StreamReader DatenBankReader = new StreamReader(path);

        string content = DatenBankReader.ReadToEnd();

        //splittet Einträge
        string[] data = content.Split('\n');
        string[] values = new string[6];
        Entry[] entries = new Entry[data.Length];
        for (int i = 0; i < data.Length; i++)
        {
            values = data[i].Split('.');
            entries[i] = new Entry(values);
        }
        StartCoroutine(UpdateModelEveryTimestep(entries));
    }
    IEnumerator UpdateModelEveryTimestep(Entry[] entries)
    {
        for(int hour = 0; hour < duration; hour++)
        {
            for (int i = 0; i < entries.Length; i++)
            {
                if (hour == entries[i].Stunde)
                {
                //DoColourStuff(entries[i].OrganID, entries[i].Kondition, entries[i].Schmerz);
                Debug.Log("Stunde " + entries[i].Stunde + ": Organ " + entries[i].OrganID + " | Betroffenheit: " + entries[i].Kondition + " | Schmerz: " + entries[i].Schmerz);
                }
            }
        yield return new WaitForSeconds(timeSteps);
        }
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