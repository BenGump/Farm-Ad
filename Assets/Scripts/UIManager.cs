using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TMP_Text cashText;
    public TMP_Text cornText;

    public void ChangeCornText(int amount)
    {
        cornText.text = $"Corn: {amount}";
    }

    public void ChangeCashText(int amount)
    {
        cashText.text = $"${amount}";
    }
}
