using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Variable<T> : ScriptableObject
{
    [SerializeField]
    protected T defaultValue;
    [SerializeField]
    protected T currentValue;

    public delegate void ValueChanged();
    public event ValueChanged OnValueChange;

    public T Value { get => currentValue; set { this.currentValue = value; OnValueChange?.Invoke(); } }

    public void Reset()
    {
        currentValue = defaultValue;
    }

    private void OnEnable()
    {
        currentValue = defaultValue;
    }
}
