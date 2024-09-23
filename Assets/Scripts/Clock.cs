using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Clock : MonoBehaviour
{
    [SerializeField] private GameObject _secondHand;
    [SerializeField] private GameObject _minuteHand;
    [SerializeField] private GameObject _hourHand;
    [SerializeField] private TextMeshProUGUI _timeText;

    [SerializeField] private WorldTimeAPIClient _worldTimeAPIClient;
    [SerializeField] private TimeZoneDB _timeZoneDB;

    private string _oldSeconds;
    private bool _isTimeUpdated = false;

    public void Initialize() => StartCoroutine(CheckTimeHourly());

    private void Update()
    {
        UpdateClockValue();
    }

    private void ClockFace(int valueHours, int valueMinutes, int valueSeconds)
    {
        iTween.RotateTo(_secondHand, iTween.Hash("z", valueSeconds * 6 * -1, "time", 1, "easetype", "easeOutQuint"));
        iTween.RotateTo(_minuteHand, iTween.Hash("z", valueMinutes * 6 * -1, "time", 1, "easetype", "easeOutElastic"));
        float hourDistance = (float)valueMinutes / 60f;
        float hourAngle = ((valueHours % 12) + hourDistance) * 360f / 12f * -1f;
        iTween.RotateTo(_hourHand, iTween.Hash("z", hourAngle, "time", 1, "easetype", "easeOutElastic"));

        int hourFormat = valueHours % 12 + (System.DateTime.Now.Hour >= 12 ? 12 : 0);
        _timeText.text = $"{hourFormat:D2}:{valueMinutes:D2}:{valueSeconds:D2}";
    }

    private void UpdateClockValue()
    {
        string seconds = System.DateTime.UtcNow.ToString("ss");

        if (seconds != _oldSeconds)
        {
            if (!_isTimeUpdated)
            {
                int valueSeconds = int.Parse(seconds);
                int valueMinutes = int.Parse(System.DateTime.UtcNow.ToString("mm"));
                int valueHours = int.Parse(System.DateTime.UtcNow.ToLocalTime().ToString("hh"));
                ClockFace(valueHours, valueMinutes, valueSeconds);
            }           
        }

        _oldSeconds = seconds;
    }

    private IEnumerator CheckTimeHourly()
    {
        while (true)
        {
            yield return new WaitForSeconds(3600);

            _worldTimeAPIClient.GetTime((newHours, newMinutes, newSeconds) =>
            {
                UpdateClock(newHours, newMinutes, newSeconds);
            });

            _timeZoneDB.GetTime((newHours, newMinutes, newSeconds) =>
            {
                UpdateClock(newHours, newMinutes, newSeconds);
            });
        }
    }

    private void UpdateClock(int hours, int minutes, int seconds)
    {
        _isTimeUpdated = true;
        ClockFace(hours, minutes, seconds);
        _isTimeUpdated = false;
    }

}
