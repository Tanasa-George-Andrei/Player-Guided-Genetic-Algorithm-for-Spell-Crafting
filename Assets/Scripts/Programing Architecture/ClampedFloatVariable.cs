using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ClampedFloatVariable : FloatVariable
{
    public float maxValue = 1;
    public float minValue = 0;

    public float GetMinMaxRangeValue()
    {
        return Value/(maxValue-minValue);
    }

}
