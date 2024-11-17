using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimeTracker : MonoBehaviour
{
    public bool stopCounting = false;
    public TMP_Text timerText;
    // Start is called before the first frame update

    public int Minutes { get; private set; }
    public int Seconds { get; private set; }
    public float Milliseconds { get; private set; }

    private float elapsedTime; // Gesamtzeit in Sekunden

    void Update()
    {
        if (stopCounting) return;

        // Inkrementiere die vergangene Zeit
        elapsedTime += Time.deltaTime;

        // Berechne Minuten, Sekunden und Millisekunden
        Minutes = Mathf.FloorToInt(elapsedTime / 60f); // Minuten
        Seconds = Mathf.FloorToInt(elapsedTime % 60f); // Sekunden
        Milliseconds = Mathf.FloorToInt((elapsedTime % 1f) * 100);

        timerText.text = $"{Minutes:D2}:{Seconds:D2}:{Milliseconds}";
    }
}
