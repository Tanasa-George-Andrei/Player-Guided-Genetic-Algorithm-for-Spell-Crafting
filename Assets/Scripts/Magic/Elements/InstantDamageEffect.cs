using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantDamageEffect : ElementEffect
{

    public float damage = 10f;

    public override void ApplyEffect(SpellHistoryNode _history, SpellCastData _castData)
    {
        _history.target.Damage(damage, _castData.owner);
    }
}
