using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Beacons")]
    public GameObject activeBeacon;
    public GameObject inactiveBeacon;

    // Start is called before the first frame update
    void Start()
    {
        activeBeacon.SetActive(false);
        inactiveBeacon.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ActivateBeacon();
        }
    }

    public void ActivateBeacon()
    {
        activeBeacon.SetActive(true);
        inactiveBeacon.SetActive(false);
    }

    public void DeactivateBeacon()
    {
        activeBeacon.SetActive(false);
        inactiveBeacon.SetActive(true);
    }
}