using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Helpers.Json;
using System.IO;
using UnityEngine.SceneManagement;
public class MarketReqs : MonoBehaviour
{
    [SerializeField] string section;

    void Start()
    {
        StartCoroutine(MarketCoroutine());
    }

    IEnumerator MarketCoroutine()
    {
        UnityWebRequest www = UnityWebRequest.Get("https://fpch.deprimus.men/api/v1/marketplace/" + LocalData.facultyId + "?section="+section);

        Debug.Log("https://fpch.deprimus.men/api/v1/marketplace/" + LocalData.facultyId + "?section=" + section);

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            string text = www.downloadHandler.text;

            Debug.Log(text);

            Json json = Json.Parse(text);

            JsonValue[] values = json.Entries["data"].AsArray();

            List<MarketItem> marketItems = new List<MarketItem>();

                for (int i = 0; i < values.Length; i++)
                {
                    Json obj = values[i].AsObject();

                MarketItem day = new MarketItem();
                    day.title = obj.Entries["title"].AsString();
                day.content = obj.Entries["content"].AsString();
                day.date = obj.Entries["created_on"].AsString().Split('T')[0];
                day.authorName = obj.Entries["author"].AsObject().Entries["name"].AsString();

                marketItems.Add(day);
                }

            Debug.Log(marketItems.Count);

            if (FindObjectOfType<LeftRightSwipe>() != null)
            {
                FindObjectOfType<LeftRightSwipe>().marketItems = marketItems;
                FindObjectOfType<LeftRightSwipe>().UpdateDay();
            }
            if (FindObjectOfType<CardSwipe>() != null)
            {
                FindObjectOfType<CardSwipe>().items = marketItems;
                FindObjectOfType<CardSwipe>().UpdateCard();
            }
        }
    }
}

public class MarketItem
{
    public string title, content, date, authorName;
}
