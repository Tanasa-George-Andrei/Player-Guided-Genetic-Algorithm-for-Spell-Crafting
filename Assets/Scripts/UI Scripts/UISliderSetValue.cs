using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class UISliderSetValue : MonoBehaviour
{
    [SerializeField]
    private ClampedFloatVariable value;
    [SerializeField]
    private Image image;
    [SerializeField]
    private TMP_Text textMesh;
    [SerializeField]
    [TextArea]
    private string textTemplate;
    [SerializeField]
    private bool disableTextAtZero = false;
    [SerializeField]
    private string displayFormat = "N0";

    private void UpdateSlider()
    {
        image.fillAmount = value.GetMinMaxRangeValue();
        if(disableTextAtZero && Mathf.Abs(value.Value - value.minValue) <= 0.0005)
        {
            textMesh.enabled = false;
        }
        else
        {
            if(!textMesh.enabled)
            {
                textMesh.enabled = true;
            }
            textMesh.text = textTemplate.Replace("%value", value.Value.ToString(displayFormat))
                .Replace("%min", value.minValue.ToString(displayFormat))
                .Replace("%max", value.maxValue.ToString(displayFormat));
        }
    }

    private void OnEnable()
    {
        value.OnValueChange += UpdateSlider;
    }
    private void OnDisable()
    {
        value.OnValueChange -= UpdateSlider;
    }
    private void Start()
    {
        UpdateSlider();
    }
}
