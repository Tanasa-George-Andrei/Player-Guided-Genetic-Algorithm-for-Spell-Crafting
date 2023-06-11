using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class ElementEffect
{
    public abstract void ApplyEffect(SpellHistoryNode _history, SpellCastData _castData);
}
