using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntryPoint : MonoBehaviour
{
    [SerializeField] private Clock _clock;
    [SerializeField] private WorldTimeAPIClient _worldTimeAPIClient;
    [SerializeField] private TimeZoneDB _timeZoneDB;

    private void Start()
    {
        _worldTimeAPIClient.Initialize();
        _timeZoneDB.Initialize();
        _clock.Initialize();
    }
}
