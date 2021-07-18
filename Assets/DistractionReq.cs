using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Helpers.Json;
using UnityEngine.UI;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

public class DistractionReq : MonoBehaviour
{
    [SerializeField] Text placeName;
    [SerializeField] Text placeAddress;

    // Start is called before the first frame update
    void Start()
    {
#if PLATFORM_ANDROID
                if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
                {
                    var callbacks = new PermissionCallbacks();
                    callbacks.PermissionGranted += PermissionCallbacks_PermissionGranted;
                    Permission.RequestUserPermission(Permission.FineLocation);
                    }
#endif
        StartCoroutine(DistractionCoroutine());
    }

    void PermissionCallbacks_PermissionGranted(string permissionName)
    {
        StartCoroutine(DistractionCoroutine());
    }

    IEnumerator DistractionCoroutine()
    {
        // First, check if user has location service enabled
        if (!Input.location.isEnabledByUser)
            yield break;

        // Start service before querying location
        Input.location.Start();

        // Wait until service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Service didn't initialize in 20 seconds
        if (maxWait < 1)
        {
            print("Timed out");
            yield break;
        }

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            print("Unable to determine device location");
            yield break;
        }

        LocationInfo info = Input.location.lastData;

        // Stop service if there is no need to query location updates continuously
        Input.location.Stop();

        string url = "https://api.geoapify.com/v2/places?categories=accommodation&filter=circle:"+info.longitude+ "," + info.latitude + ",4000&limit=20&apiKey=f8c086721f9541d6adbedc5d893fb159";
        
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            string text = www.downloadHandler.text;

            JsonValue[] features = Json.Parse(text).Entries["features"].AsArray();

            JsonValue val = features[Random.Range(1, 999999) % features.Length];

            Json properties = val.AsObject().Entries["properties"].AsObject();

            string name = properties.Entries["name"].AsString();

            string address = properties.Entries["address_line2"].AsString();

            placeName.text = name;

            placeAddress.text = address;
        }
    }
}
