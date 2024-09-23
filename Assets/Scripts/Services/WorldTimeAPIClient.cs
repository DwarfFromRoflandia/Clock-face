using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WorldTimeAPIClient : MonoBehaviour
{
    private string apiUrl = "https://worldtimeapi.org/api/timezone/Europe/Moscow";

    private Action<int, int, int> _callback;
    
    public void Initialize()
    {
        if (_callback != null) StartCoroutine(GetTimeFromAPI(_callback));
    }

    public void GetTime(Action<int, int, int> callback)
    {
        _callback = callback; // Сохраняем коллбэк для вызова в Start
        StartCoroutine(GetTimeFromAPI(callback));
    }

    private IEnumerator GetTimeFromAPI(Action<int, int, int> callback)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(apiUrl))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(webRequest.error);
            }
            else
            {
                string jsonResponse = webRequest.downloadHandler.text;
                Debug.Log(jsonResponse);

                TimeData timeData = JsonUtility.FromJson<TimeData>(jsonResponse);

                DateTime dateTime;
                if (DateTime.TryParse(timeData.datetime, out dateTime))
                {
                    string timeOnly = dateTime.ToString("HH:mm:ss");
                    Debug.Log("Current time in Moscow: " + timeOnly);

                    int hours = dateTime.Hour;
                    int minutes = dateTime.Minute;
                    int seconds = dateTime.Second;

                    callback?.Invoke(hours, minutes, seconds);

                }
                else
                {
                    Debug.LogError("Failed to convert datetime string to DateTime object.");
                }
            }
        }
    }

    [Serializable]
    public class TimeData
    {
        public string datetime;
        public string utc_offset;
        public string timezone;
        public int day_of_week;
        public int day_of_year;
        public string utc_datetime;
        public long unixtime;
        public string abbreviation;
        public bool dst;
        public int raw_offset;
        public int week_number;
    }
}
