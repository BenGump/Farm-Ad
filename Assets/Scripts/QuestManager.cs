using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public bool hasCompletedAllQuests = false;
    public List<Quest> quests;

    public TimeTracker timeTracker;

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
        foreach (Quest quest in quests)
        {
            if(quest.isActive)
            {
                quest.IncreaseCollectGoal("corn", amount);
            }
        }

        if (CheckForWin())
        {
            finishScreen.SetActive(true);
            hasCompletedAllQuests = true;
            timeTracker.stopCounting = true;
        }
    }

    public void SetTotalCash(int amount)
    {
        foreach (Quest quest in quests)
        {
            if (quest.isActive)
            {
                quest.SetCollectGoal("cash", amount);
            }
        }

        if(CheckForWin())
        {
            finishScreen.SetActive(true);
            hasCompletedAllQuests = true;
            timeTracker.stopCounting = true;
        }
    }

    public bool CheckForWin()
    {
        if (hasCompletedAllQuests) return false;

        bool win = true;

        foreach (Quest quest in quests)
        {
            if(!quest.isCompleted)
            {
                win = false;
            }
        }

        return win;
    }
    
}
