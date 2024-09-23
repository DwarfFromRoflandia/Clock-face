using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class TimeZoneDB : MonoBehaviour
{
    private string apiKey = "15KYYF5JY2CO";
    private string apiUrl = "https://api.timezonedb.com/v2.1/get-time-zone";

    private Action<int, int, int> _callback;
    
    public void Initialize()
    {
        if (_callback != null) StartCoroutine(GetMoscowTime(_callback));
    }

    public void GetTime(Action<int, int, int> callback)
    {
        _callback = callback;
        StartCoroutine(GetMoscowTime(callback));
    }

    private IEnumerator GetMoscowTime(Action<int, int, int> callback)
    {
        string url = $"{apiUrl}?key={apiKey}&format=json&by=zone&zone=Europe/Moscow";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError) Debug.LogError("Request execution error: " + webRequest.error);
            else
            {
                string jsonResponse = webRequest.downloadHandler.text;

                TimeZoneResponse response = JsonUtility.FromJson<TimeZoneResponse>(jsonResponse);

                if (response != null && response.status == "OK")
                {
                    Debug.Log($"Current time in Moscow: {response.formatted}");

                    string[] timeParts = response.formatted.Split(' ')[1].Split(':');
                    int hours = int.Parse(timeParts[0]);
                    int minutes = int.Parse(timeParts[1]);
                    int seconds = int.Parse(timeParts[2]);

                    // Вызов коллбэка с текущим временем
                    callback?.Invoke(hours, minutes, seconds);
                }
                else
                {
                    Debug.LogError("The correct time data could not be obtained.");
                }
            }
        }
    }

    [Serializable]
    public class TimeZoneResponse
    {
        public string status;
        public string message;
        public string countryCode;
        public string countryName;
        public string zoneName;
        public string formatted;
    }
}
