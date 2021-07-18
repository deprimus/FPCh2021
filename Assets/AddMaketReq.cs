using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Helpers.Json;
using System.IO;
using UnityEngine.SceneManagement;

public class AddMaketReq : MonoBehaviour
{
    [SerializeField] Text title;
    [SerializeField] Text categ;
    [SerializeField] Text name;
    [SerializeField] Text content;

    public void Submit()
    {
        StartCoroutine(SubmitCoroutine());
    }

    IEnumerator SubmitCoroutine()
    {
        string category = "";

        switch(categ.text)
        {
            case "General":
                category = "general";
                break;

            case "Black Market":
                category = "black_market";
                break;

            case "Camin Market":
                category = "camin_market";
                break;


        }    

        WWWForm form = new WWWForm();
        form.AddField("title", title.text);
        form.AddField("content", content.text);
        form.AddField("section", category);


        UnityWebRequest www = UnityWebRequest.Post("https://fpch.deprimus.men/api/v1/marketplace/" + LocalData.facultyId, form);
        www.SetRequestHeader("X-Author-ID", LocalData.authorId);
        www.SetRequestHeader("X-Author-Secret", LocalData.authorSecret);
        yield return www.SendWebRequest();

        SceneManager.LoadScene("Marketplace");
    }
}
