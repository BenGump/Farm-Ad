using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class UpgradeZone : MonoBehaviour
{
    public TMP_Text amountText;
    public GameObject progressSlider;

    public int upgradeCost = 100;
    public int currentCash = 0;

    [SerializeField] Inventory playerInventory;

    public List<UnityEvent> eventList;
    Coroutine cash;

    private float sliderXScaleMaximum;
    private float sliderZScaleMaximum;

    // Start is called before the first frame update
    void Start()
    {
        sliderXScaleMaximum = progressSlider.transform.localScale.x;
        sliderZScaleMaximum = progressSlider.transform.localScale.z;

        progressSlider.transform.localScale = new Vector3(0, progressSlider.transform.localScale.y, 0);
    }

    public void Upgrade()
    {
        foreach (UnityEvent unityEvent in eventList)
        {
            unityEvent?.Invoke();
        }

        currentCash = 0;
    }

    void TakeCash()
    {
        //Debug.Log("Calling Take Cash");

        if (playerInventory.cash > 0)
        {
            //Debug.Log("Actually calling TakeCash");

            currentCash++;
            playerInventory.RemoveCash(1);

            amountText.text = $"{currentCash} / {upgradeCost}";

            float percentageReached = (float)currentCash / (float)upgradeCost;

            progressSlider.transform.localScale = new Vector3(sliderXScaleMaximum * percentageReached, progressSlider.transform.localScale.y, sliderZScaleMaximum * percentageReached);

            if(currentCash >= upgradeCost)
            {
                Upgrade();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            InvokeRepeating("TakeCash", .05f, .05f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CancelInvoke("TakeCash");
        }
    }

    void OnDisable()
    {
        // Safety first
        CancelInvoke("TakeCash");    
    }
}
