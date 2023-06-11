using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class UITextSetValue : MonoBehaviour
{
    [SerializeField]
    private FloatVariable value;
    [SerializeField]
    private TMP_Text textMesh;
    [SerializeField]
    [TextArea]
    private string textTemplate;
    [SerializeField]
    private string displayFormat = "N0";

    private void UpdateSlider()
    {
        if(!textMesh.enabled)
        {
            textMesh.enabled = true;
        }
        textMesh.text = textTemplate.Replace("%value", value.Value.ToString(displayFormat));
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
