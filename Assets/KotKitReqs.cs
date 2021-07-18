using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Helpers.Json;
using System.IO;
using System;

public class KotKitReqs : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(TimetableCoroutine());
    }

    IEnumerator TimetableCoroutine()
    {
        UnityWebRequest www = UnityWebRequest.Get("https://fpch.deprimus.men/api/v1/timetable/" + LocalData.facultyId + "/" + LocalData.classRoomId);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            string text = www.downloadHandler.text;

            Json json = Json.Parse(text);

            Json timetable = Json.Parse(json.Entries["data"].AsObject().Entries["data"].AsString().Replace("\\", ""));

            List<WeekDay> weekDays = new List<WeekDay>();

            try
            {
                JsonValue[] arr = timetable.Entries["monday"].AsArray();

                for(int i=0;i<arr.Length;i++)
                {
                    Json obj = arr[i].AsObject();

                    WeekDay day = new WeekDay();
                    day.name = obj.Entries["name"].AsString();
                    day.weekDayName = "Monday";
                    day.start = obj.Entries["start"].AsNumber();

                    weekDays.Add(day);
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex.ToString());
            }

            try
            {
                JsonValue[] arr = timetable.Entries["tuesday"].AsArray();

                for (int i = 0; i < arr.Length; i++)
                {
                    Json obj = arr[i].AsObject();

                    WeekDay day = new WeekDay();
                    day.name = obj.Entries["name"].AsString();
                    day.weekDayName = "Tuesday";
                    day.start = obj.Entries["start"].AsNumber();

                    weekDays.Add(day);
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex.ToString());
            }

            try
            {
                JsonValue[] arr = timetable.Entries["wednesday"].AsArray();

                for (int i = 0; i < arr.Length; i++)
                {
                    Json obj = arr[i].AsObject();

                    WeekDay day = new WeekDay();
                    day.name = obj.Entries["name"].AsString();
                    day.weekDayName = "Wednesday";
                    day.start = obj.Entries["start"].AsNumber();

                    weekDays.Add(day);
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex.ToString());
            }

            try
            {
                JsonValue[] arr = timetable.Entries["thursday"].AsArray();

                for (int i = 0; i < arr.Length; i++)
                {
                    Json obj = arr[i].AsObject();

                    WeekDay day = new WeekDay();
                    day.name = obj.Entries["name"].AsString();
                    day.weekDayName = "Thursday";
                    day.start = obj.Entries["start"].AsNumber();

                    weekDays.Add(day);
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex.ToString());
            }

            try
            {
                JsonValue[] arr = timetable.Entries["friday"].AsArray();

                for (int i = 0; i < arr.Length; i++)
                {
                    Json obj = arr[i].AsObject();

                    WeekDay day = new WeekDay();
                    day.name = obj.Entries["name"].AsString();
                    day.weekDayName = "Friday";
                    day.start = obj.Entries["start"].AsNumber();

                    weekDays.Add(day);
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex.ToString());
            }

            Debug.Log(weekDays.Count);

            FindObjectOfType<CardSwipe>().days = weekDays;
            FindObjectOfType<CardSwipe>().UpdateDay();
         }
    }
}

public class WeekDay
{
    public string name, weekDayName;
    public double start;
}
