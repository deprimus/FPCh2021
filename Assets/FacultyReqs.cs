using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Helpers.Json;
using System.IO;

public static class AdminSession
{
    public static string session, facultyId;
}

public class FacultyReqs : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Dropdown dropdown;
    [SerializeField] Text facultyText;
    [SerializeField] Dropdown classRoomDropdown;
    [SerializeField] Text classroomText;
    [SerializeField] public bool admin = false;

    int facultyId, classroomId;

    Dictionary<string, int> facultyIds = new Dictionary<string, int>();
    Dictionary<string, int> classRoomIds = new Dictionary<string, int>();

    void Start()
    {
        if (admin)
            return;

        try
        {

            string data = File.ReadAllText(Application.persistentDataPath + "/faculty.igd");

            string[] parameters = data.Split('~');

            LocalData.facultyId = parameters[0];
            LocalData.classRoomId = parameters[1];
            LocalData.facultyName = parameters[2];
            LocalData.classRoomName = parameters[3];
            LocalData.authorName = parameters[4];
            LocalData.authorSecret = parameters[5];            LocalData.authorId = parameters[6];

            FindObjectOfType<UIButtons>().PlayTransition();
        }
        catch
        {
            StartCoroutine(FacultyCoroutine());
        } 
    }

    public void SendAdminForm()
    {
        StartCoroutine(AdminCoroutine());
    }

    IEnumerator AdminCoroutine()
    {
        WWWForm form = new WWWForm();
        form.AddField("username", facultyText.text);
        form.AddField("password", classroomText.GetComponent<InputField>().text);

        UnityWebRequest www = UnityWebRequest.Post("https://fpch.deprimus.men/api/v1/admin/auth", form);
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

            AdminSession.session = json.Entries["data"].AsObject().Entries["session"].AsString();
            AdminSession.facultyId = json.Entries["data"].AsObject().Entries["facultyId"].AsNumber().ToString();

            FindObjectOfType<UIButtons>().PlayTransition();
        }
    }

    public void GetSecret()
    {
        classroomId = classRoomIds[classroomText.text];

        if (string.IsNullOrWhiteSpace(nameText.text))
            return;

        StartCoroutine(SecretCoroutine());
    }

    IEnumerator SecretCoroutine()
    {
        UnityWebRequest www = UnityWebRequest.Get("https://fpch.deprimus.men/api/v1/author/new?name="+nameText.text);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            string text = www.downloadHandler.text;

            Json json = Json.Parse(text);

            JsonValue value = json.Entries["secret"];

            string data = value.AsString();

            JsonValue value2 = json.Entries["id"];

            double id = value2.AsNumber();

            LocalData.facultyId = facultyId.ToString();
            LocalData.classRoomId = classroomId.ToString();
            LocalData.facultyName = facultyText.text;
            LocalData.classRoomName = classroomText.text;
            LocalData.authorName = nameText.text;
            LocalData.authorSecret = data; LocalData.authorId = id.ToString();

            File.WriteAllText(Application.persistentDataPath + "/faculty.igd", facultyId + "~" + classroomId + "~" + facultyText.text + "~" + classroomText.text + "~" + nameText.text + "~" + data + "~" + id);

            FindObjectOfType<UIButtons>().PlayTransition();
        }
    }

    public void ChooseClassRoom()
    {
        facultyId = facultyIds[facultyText.text];

        StartCoroutine(ClassRoomCoroutine());
    }

    IEnumerator ClassRoomCoroutine()
    {
        UnityWebRequest www = UnityWebRequest.Get("https://fpch.deprimus.men/api/v1/timetable/"+facultyId);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            string text = www.downloadHandler.text;

            classRoomDropdown.ClearOptions();

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

            classRoomDropdown.AddOptions(options);
        }
    }

    IEnumerator FacultyCoroutine()
    {
        UnityWebRequest www = UnityWebRequest.Get("https://fpch.deprimus.men/api/v1/faculty");
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

            for(int i=0;i<dataArr.Length;i++)
            {
                Json entry = dataArr[i].AsObject();

                double id = entry.Entries["id"].AsNumber();

                string name = entry.Entries["name"].AsString();

                options.Add(name);

                facultyIds[name] = (int)id;
            }

            dropdown.AddOptions(options);

            ChooseClassRoom();
        }
    }
}

public static class LocalData
{
    public static string facultyId, classRoomId, authorId;
    public static string facultyName, classRoomName, authorName, authorSecret;
}
