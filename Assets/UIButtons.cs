using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Helpers.Json;
using System.IO;
using UnityEngine.Networking;

public class UIButtons : MonoBehaviour
{
    [SerializeField] Animator animator;

    public void SubmitMarket()
    {
        FindObjectOfType<AddMaketReq>().Submit();
    }

    public void SendClassroomRequest()
    {
        FindObjectOfType<FacultyReqs>().ChooseClassRoom();
    }

    public void SendRequest()
    {
        FindObjectOfType<FacultyReqs>().GetSecret();
    }

    public void Logout()
    {
        File.Delete(Application.persistentDataPath + "/faculty.igd");

        Application.Quit();
    }

    public void SendAdminForm()
    {
        FindObjectOfType<FacultyReqs>().SendAdminForm();
    }

    public void PlayTransition()
    {
        animator.Play("transition");
    }

    public void LoadAdmin()
    {
        SceneManager.LoadScene("Admin");
    }

    public void LoadMain()
    {
        if (FindObjectOfType<FacultyReqs>().admin)
            SceneManager.LoadScene("Timetable");
        else SceneManager.LoadScene("KotKit");
    }

    struct MingisKao
    {
        public string name;
        public string location;
        public string start;

        public MingisKao(string name, string location, string start) {
            this.name = name;
            this.location = location;
            this.start = start;
        }
    }

    public void SaveTimetable()
    {
        StartCoroutine(SaveCoroutine());
    }

    IEnumerator SaveCoroutine()
    {
        Dictionary<string, List<MingisKao>> stuff = new Dictionary<string, List<MingisKao>>();
        GameObject layoutGroup = GameObject.Find("LayoutGroup");

        bool first = true;

        foreach (Transform child in layoutGroup.transform)
        {
            if (first)
            {
                first = false;
                continue;
            }

            string dayName = child.GetComponent<Text>().text;

            stuff[dayName.ToLowerInvariant()] = new List<MingisKao>();

            bool second = true;

            foreach (Transform child2 in child)
            {
                if (second)
                {
                    second = false;

                    continue;
                }

                string name = child2.GetChild(0).GetComponent<InputField>().text;
                string location = child2.GetChild(2).GetComponent<InputField>().text;
                string start = child2.GetChild(3).GetComponent<InputField>().text;

                stuff[dayName.ToLowerInvariant()].Add(new MingisKao(name, location, start));
            }
        }

        Dictionary<string, JsonValue> finalStuff = new Dictionary<string, JsonValue>();

        foreach (var mingisKao in stuff)
        {
            JsonValue[] arr = new JsonValue[mingisKao.Value.Count];
            int indx = 0;

            foreach (var kao in mingisKao.Value)
            {
                Dictionary<string, JsonValue> kaoVals = new Dictionary<string, JsonValue>();

                kaoVals["name"] = new JsonValue(JsonValue.ValueType.STRING, kao.name);
                kaoVals["location"] = new JsonValue(JsonValue.ValueType.STRING, kao.location);
                kaoVals["start"] = new JsonValue(JsonValue.ValueType.NUMBER, (double)int.Parse(kao.start.Split(':')[0]));

                arr[indx++] = new JsonValue(JsonValue.ValueType.OBJECT, new Json(kaoVals));
            }

            JsonValue day = new JsonValue(JsonValue.ValueType.ARRAY, arr);

            finalStuff[mingisKao.Key.ToLowerInvariant()] = day;
        }

        Json donbok = new Json(finalStuff);

        WWWForm wwwForm = new WWWForm();
        wwwForm.AddField("data", donbok.ToString());

        UnityWebRequest www = UnityWebRequest.Post("https://fpch.deprimus.men/api/v1/timetable/" + AdminSession.facultyId + "/" + FindObjectOfType<TimetableReq>().GetID(), wwwForm);

        www.SetRequestHeader("X-Session", AdminSession.session);

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("wowzers");
        }
    }

    public void Home()
    {
        SceneManager.LoadScene("KotKit");
    }

    public void Marketplace()
    {
        SceneManager.LoadScene("Marketplace");
    }

    public void Profile()
    {
        SceneManager.LoadScene("Profile");
    }

    public void Distraction()
    {
        SceneManager.LoadScene("Distraction");
    }

    public void BlackMarket()
    {
        SceneManager.LoadScene("Black Market");
    }

    public void CaminMarket()
    {
        SceneManager.LoadScene("Camin Market");
    }

    public void AddMarket()
    {
        SceneManager.LoadScene("AddToMarket");
    }

    public void Timetable()
    {
        SceneManager.LoadScene("Timetable");
    }
}
