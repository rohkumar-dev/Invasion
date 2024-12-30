using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using TMPro;

public class SliderController : MonoBehaviour
{
    [HideInInspector] public UnityEvent<float> OnPlayerPrefChange;

    public enum ValueType { Percentage, Decimal };

    [SerializeField] private Image fillImage;
    [SerializeField] private TextMeshProUGUI prefValueText;
    [SerializeField] private Button decrementButton;
    [SerializeField] private Button incrementButton;
    [SerializeField] private ValueType valueType;

    [SerializeField] private string prefKeyName;
    [SerializeField] private float minParameterAmount;
    [SerializeField] private float maxParameterAmount;
    [SerializeField] private float incrementAmount;
    [SerializeField] private float defaultValue;

    private float prefValue, fillAmount;

    private void Start() {
        incrementButton.onClick.AddListener(IncrementPrefValue);
        decrementButton.onClick.AddListener(DecrementPrefValue);

        prefValue = PlayerPrefs.GetFloat(prefKeyName);
        if (prefValue == 0f)
            prefValue = defaultValue;

        UpdateFillAmount();
    }

    private void IncrementPrefValue() {
        prefValue += incrementAmount;
        prefValue = Mathf.Min(prefValue, maxParameterAmount);
        UpdateFillAmount();
    }

    private void DecrementPrefValue() {
        prefValue -= incrementAmount;
        prefValue = Mathf.Max(prefValue, minParameterAmount);
        UpdateFillAmount();
    }

    private void UpdateFillAmount() {
        fillAmount = (prefValue - minParameterAmount) / (maxParameterAmount - minParameterAmount);
        fillImage.fillAmount = fillAmount;
        prefValueText.SetText(valueType == ValueType.Percentage ? GetPercentage() : GetDecimal());
        PlayerPrefs.SetFloat(prefKeyName, prefValue);
        OnPlayerPrefChange.Invoke(prefValue);
    }

    private string GetPercentage() {
        return (prefValue * 100f).ToString("0.00") + "%";
    }

    private string GetDecimal() {
        return prefValue.ToString("0.00");
    }
}