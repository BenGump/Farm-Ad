using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class Quest : MonoBehaviour
{
    public enum GOAL_TYPE
    {
        COLLECT,
        TRIGGER
    }

    [Header("Settings")]
    public string title;
    public string description;
    public bool isActive;
    public bool isCompleted;
    public List<UnityEvent> startingEvents;
    public List<UnityEvent> finishEvents;

    [Header("Finish Terms")]
    public GOAL_TYPE type;
    public int currentAmount;
    public int neededAmount;
    public string neededType;

    [Header("References")]
    public GameObject questUIPrefab;
    public Transform questUIParent;
    public GameObject questUIElement;

    void OnEnable()
    {
        isActive = true;
        BuildUI();

        foreach (UnityEvent startingEvent in startingEvents)
        {
            startingEvent?.Invoke();
        }
    }

    void Start()
    {
        // Already active quests do not trigger OnEnable. That's why their startingEvents never get fired without the following code
        if(gameObject.activeSelf)
        {
            foreach (UnityEvent startingEvent in startingEvents)
            {
                startingEvent?.Invoke();
            }
        }
    }

    public void BuildUI()
    {
        if (questUIElement != null) return;

        questUIElement = Instantiate(questUIPrefab, questUIParent);

        TMP_Text title = questUIElement.transform.GetChild(0).GetComponent<TMP_Text>();
        TMP_Text description = questUIElement.transform.GetChild(1).GetComponent<TMP_Text>();

        if(title != null && description != null)
        {
            title.text = this.title;
            description.text = this.description;
        }
    }

    public void IncreaseCollectGoal(string collectedItem, int amount)
    {
        if (type == GOAL_TYPE.COLLECT)
        {
            if (neededType == collectedItem)
            {
                currentAmount += amount;

                if (currentAmount >= neededAmount)
                {
                    CompleteQuest();
                }
            }
        }
    }

    public void SetCollectGoal(string collectedItem, int amount)
    {
        if (type == GOAL_TYPE.COLLECT)
        {
            if (neededType == collectedItem)
            {
                currentAmount = amount;

                if (currentAmount >= neededAmount)
                {
                    CompleteQuest();
                }
            }
        }
    }

    public void CompleteQuest()
    {
        if (isCompleted || !isActive) return;

        foreach (UnityEvent finishEvent in finishEvents)
        {
            finishEvent?.Invoke();
        }

        isCompleted = true;
        StartDeletingQuestElement();
    }



    public void StartDeletingQuestElement()
    {
        TMP_Text textComponent = questUIElement.transform.GetChild(1).GetComponent<TMP_Text>();

        StartCoroutine(DeletingQuestElement(textComponent, .01f));
    }

    private IEnumerator DeletingQuestElement(TMP_Text textComponent, float duration)
    {
        string originalText = textComponent.text;
        Debug.Log($"Starting at {textComponent.text.Length}");

        for (int i = 0; i < originalText.Length; i++)
        {
            string tempString = originalText.Insert(i + 1, "</s>");
            tempString = tempString.Insert(0, "<s>");
            textComponent.text = tempString;
            yield return new WaitForSeconds(duration);

        }

        questUIElement.GetComponent<Animator>()?.SetTrigger("FadeOut");
        Destroy(questUIElement, .75f);
    }

}
