using System.Collections;
using System.Collections.Generic;
using TMPro;
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
                StartDeletingQuestElement(quest1UIElement);
                //quest1UIElement.SetActive(false);
                break;

            case 2:
                if (quest2Completed) return;
                quest2Completed = true;
                StartDeletingQuestElement(quest2UIElement);
                //quest2UIElement.SetActive(false);
                break;

            case 3:
                if (quest3Completed) return;
                quest3Completed = true;
                StartDeletingQuestElement(quest3UIElement);
                //quest3UIElement.SetActive(false);
                break;
            case 4:
                if(quest4Completed) return;
                quest4Completed = true;
                StartDeletingQuestElement(quest4UIElement);
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

    
    public void StartDeletingQuestElement(GameObject parentObject)
    {
        TMP_Text textComponent = parentObject.transform.GetChild(1).GetComponent<TMP_Text>();

        StartCoroutine(DeletingQuestElement(textComponent, .01f, parentObject));
    }

    private IEnumerator DeletingQuestElement(TMP_Text textComponent, float duration, GameObject parent)
    {
        string originalText = textComponent.text;
        Debug.Log($"Starting at {textComponent.text.Length}");

        for (int i = 0; i < originalText.Length; i++)
        {            
            string tempString = originalText.Insert(i+1, "</s>");
            tempString = tempString.Insert(0, "<s>");
            textComponent.text = tempString;
            yield return new WaitForSeconds(duration);

        }

        parent.GetComponent<Animator>()?.SetTrigger("FadeOut");
        Destroy(parent, .75f);
    }
}
