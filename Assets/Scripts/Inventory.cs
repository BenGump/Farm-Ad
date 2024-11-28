using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("General Settings")]
    public UIManager uiManager;
    public QuestManager questManager;


        [Header("Cash things")]
    public int cash = 0;

    [Header("Corn things")]
    public int cornCounter = 0;
    public Transform cornBackpack;
    public GameObject cornPrefab;
    public List<GameObject> cornObjects;

    private void Start()
    {
        // Testing purpose
        //AddCash(100);
    }

    public void AddPlant(string plantName, int amount)
    {
        switch(plantName)
        {
            case "Corn":

                cornCounter += amount;

                // Initialize corn object
                for(int i = 0; i < amount; i++)
                {
                    InitializeCornObject();
                }
                

                // Change UI
                uiManager.ChangeCornText(cornCounter);

                break;
            default:
                Debug.Log($"Trying to add '{plantName}', but it's not found. Spelling error?");
                break;
        }
    }

    private void InitializeCornObject()
    {
        GameObject cornObject = Instantiate(cornPrefab, cornBackpack);

        cornObject.transform.localPosition = new Vector3(0, -0.25f + 0.15f * cornObjects.Count, 0);
        cornObject.transform.localRotation = Quaternion.Euler(0, -90f, 0);

        cornObjects.Add(cornObject);
    }

    public void RemovePlant(string plantName)
    {
        if(HasPlant(plantName))
        {
            switch (plantName)
            {
                case "Corn":
                    Destroy(cornObjects[cornObjects.Count-1]);
                    cornObjects.RemoveAt(cornObjects.Count-1);
                    cornCounter--;

                    // Change UI
                    uiManager.ChangeCornText(cornCounter);
                    break;
                default:
                    Debug.Log($"Trying to remove '{plantName}', but it's not found. Spelling error?");
                    break;
            }
        }
    }

    public bool HasPlant(string plantName)
    {
        switch (plantName)
        {
            case "Corn":
                if (cornCounter > 0) return true;
                break;
            default:
                Debug.Log($"There are no '{plantName}'. Spelling error?");
                return false;
        }

        return false;
    }
    
    public void AddCash(int amount)
    {
        cash += amount;

        questManager.SetTotalCash(cash);

        uiManager.ChangeCashText(cash);
    }

    public void RemoveCash(int amount)
    {
        cash -= amount;

        if(cash < 0)
        {
            cash = 0;
        }

        uiManager.ChangeCashText(cash);
    }

}
