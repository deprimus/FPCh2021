using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Helpers.Json;
using System.IO;
public class ProfileReqs : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] Transform parent;
    [SerializeField] Text nameText;

    void Start()
    {
        nameText.text = LocalData.authorName;
        StartCoroutine(UserCoroutine());
    }

    IEnumerator UserCoroutine()
    {
        UnityWebRequest www = UnityWebRequest.Get("https://fpch.deprimus.men/api/v1/marketplace/" + LocalData.facultyId + "/" + LocalData.authorId);
        www.SetRequestHeader("X-Author-ID", LocalData.authorId);
        www.SetRequestHeader("X-Author-Secret", LocalData.authorSecret);
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

            for (int i=0;i<5 && i < values.Length;i++)
            {
                Json json2 = values[i].AsObject();

                GameObject clone = Instantiate(prefab);
                clone.transform.SetParent(parent);


                clone.transform.GetChild(0).GetComponent<Text>().text = json2.Entries["title"].AsString();
            }
        }
    }
}
