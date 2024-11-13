using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public bool quest1Completed = false;
    public bool quest2Completed = false;
    public bool quest3Completed = false;
    public bool quest4Completed = false;

    public GameObject quest1UIElement;
    public GameObject quest2UIElement;
    public GameObject quest3UIElement;
    public GameObject quest4UIElement;

    public int totalCorn = 0;
    public int quest1CornNeeded = 100;

    public int quest4CashNeeded = 300;

    public GameObject finishScreen;

    public void IncreaseTotalCorn(int amount)
    {
        totalCorn += amount;

        if(totalCorn >= quest1CornNeeded)
        {
            CompleteQuest(3);
        }
    }

    public void CompleteQuest(int number)
    {
        switch (number)
        {
            case 1:
                if (quest1Completed) return;
                quest1Completed = true;
                quest1UIElement.SetActive(false);
                break;

            case 2:
                if (quest2Completed) return;
                quest2Completed = true;
                quest2UIElement.SetActive(false);
                break;

            case 3:
                if (quest3Completed) return;
                quest3Completed = true;
                quest3UIElement.SetActive(false);
                break;

            default:
                Debug.Log($"Trying to complete a quest that is not listed in 'CompleteQuest'. ID: ${number}");
                break;
        }

        if(quest1Completed && quest2Completed && quest3Completed && quest4Completed)
        {
            // Finish game!
            finishScreen.SetActive(true);
        }
    }
}
