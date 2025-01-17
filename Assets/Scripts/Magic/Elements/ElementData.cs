using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

[CreateAssetMenu]
public class ElementData : ScriptableObject
{
    public GameObject projectile;

    public float dashDurationInSeconds;

    public Material material;
    public Mesh mesh;

    public ElementData oppositeElement;

    [SerializeReference]
    private List<ElementEffect> endEffects;

    public void AddEffect(ElementEffect _effect)
    {
        if(endEffects == null)
        {
            return;
        }
        foreach (ElementEffect effect in endEffects) 
        {
            if(effect.GetType() == _effect.GetType())
            {
                return;
            }
        }

        endEffects.Add(_effect);
    }

    public void ApplyEffects(SpellHistoryNode _history, SpellCastData _castData)
    {
        if (_history == null || _history.target == null || _history.target.GetActualDirector() == null || _castData == null)
        {
            return;
        }
        foreach (ElementEffect effect in endEffects)
        {
            effect.ApplyEffect(_history, _castData);
        }
    }

}
