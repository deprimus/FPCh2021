using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Helpers.Json;
using System.IO;
using System;
public class TimetableReq : MonoBehaviour
{
    [SerializeField] Dropdown dropdown;
    [SerializeField] Text classRoomText;

    [SerializeField] GameObject dayPrefab;
    [SerializeField] GameObject itemPrefab;

    [SerializeField] GameObject dayParent;

    Dictionary<string, int> classRoomIds = new Dictionary<string, int>();

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ClassRoomCoroutine());
    }

    public int GetID()
    {
        return classRoomIds[classRoomText.text];
    }

    IEnumerator ClassRoomCoroutine()
    {
        UnityWebRequest www = UnityWebRequest.Get("https://fpch.deprimus.men/api/v1/timetable/" + AdminSession.facultyId);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            string text = www.downloadHandler.text;

            dropdown.ClearOptions();

            Json json = Json.Parse(text);

            JsonValue value = json.Entries["data"];

            JsonValue[] dataArr = value.AsArray();

            List<string> options = new List<string>();

            for (int i = 0; i < dataArr.Length; i++)
            {
                Json entry = dataArr[i].AsObject();

                double id = entry.Entries["id"].AsNumber();

                string name = entry.Entries["name"].AsString();

                options.Add(name);

                classRoomIds[name] = (int)id;
            }

            dropdown.AddOptions(options);

            GetTimetable();
        }
    }

    void Clear()
    {
        foreach(Transform child in dayParent.transform)
        {
            if (child.name.Contains("Day"))
                Destroy(child.gameObject);
        }
    }

    public void GetTimetable()
    {
        Clear();

        StartCoroutine(TimetableCoroutine());
    }

    void GenerateDays()
    {
        string[] days = new string[] { "monday", "tuesday", "wednesday", "thursday", "friday" };

        for (int i = 0; i < days.Length; i++)
        {
            try
            {
                GameObject dayClone = Instantiate(dayPrefab);
                dayClone.transform.parent = dayParent.transform;

                dayClone.GetComponent<Text>().text = days[i];
            }
            catch (Exception ex)
            {
                Debug.Log(ex.ToString());
            }
        }
    }

    IEnumerator TimetableCoroutine()
    {
        UnityWebRequest www = UnityWebRequest.Get("https://fpch.deprimus.men/api/v1/timetable/" + AdminSession.facultyId + "/" + classRoomIds[classRoomText.text]);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            GenerateDays();

            Debug.Log(www.error);
        }
        else
        {
            string text = www.downloadHandler.text;

            Debug.Log(text);

            string[] days = new string[] { "monday", "tuesday", "wednesday", "thursday", "friday" };

            Json timetable;

            try
            {
                Json json = Json.Parse(text);

                timetable = Json.Parse(json.Entries["data"].AsObject().Entries["data"].AsString().Replace("\\", ""));
            }
            catch
            {
                GenerateDays();

                throw new Exception("rip");
            }

            for(int i=0;i<days.Length;i++)
            {
                try
                {
                    GameObject dayClone = Instantiate(dayPrefab);
                    dayClone.transform.parent = dayParent.transform;

                    dayClone.GetComponent<Text>().text = days[i];

                    JsonValue[] arr = timetable.Entries[days[i]].AsArray();

                    for (int j = 0; j < arr.Length; j++)
                    {
                        GameObject itemClone = Instantiate(itemPrefab);
                        itemClone.transform.parent = dayClone.transform;

                        Json obj = arr[j].AsObject();

                        itemClone.transform.GetChild(0).GetComponent<InputField>().text = obj.Entries["name"].AsString();
                        itemClone.transform.GetChild(2).GetComponent<InputField>().text = obj.Entries["location"].AsString();
                        itemClone.transform.GetChild(3).GetComponent<InputField>().text = obj.Entries["start"].AsNumber().ToString()+":00";
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.ToString());
                }
            }
        }
    }

    public void Save()
    {

    }
}
