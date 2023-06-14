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
    bool isReverse;
    int maxHour;
    int minHour = 0;
    int hour = 0;
    int savedHour = 0;
    Entry[] entries;
    bool isStopped;
    bool isReset;

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
    // Start is called before the first frame updater
    void Start()
    {
        isReverse = false;
        isStopped = true;
        isReset = true;
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
                //DoColourStuff(entries[i].OrganID, entries[i].Kondition, entries[i].Schmerz);
                UnityEngine.Debug.Log("Stunde " + entries[i].Stunde + ": Organ " + entries[i].OrganID + " | Betroffenheit: " + entries[i].Kondition + " | Schmerz: " + entries[i].Schmerz);
                }
            }
        yield return new WaitForSeconds(timeStepsInSeconds);
        }
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