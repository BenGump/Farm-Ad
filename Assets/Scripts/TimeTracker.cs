using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimeTracker : MonoBehaviour
{
    public bool stopCounting = false;
    [SerializeField] TMP_Text timerText;
    // Start is called before the first frame update

    int minutes;
    int seconds;

    float elapsedTime; // Gesamtzeit in Sekunden

    void Update()
    {
        if (stopCounting) return;

        // Inkrementiere die vergangene Zeit
        elapsedTime += Time.deltaTime;

        // Berechne Minuten, Sekunden und Millisekunden
        minutes = Mathf.FloorToInt(elapsedTime / 60f); // Minuten
        seconds = Mathf.FloorToInt(elapsedTime % 60f); // Sekunden

        timerText.text = $"{minutes:D2}:{seconds:D2}";
    }
}
