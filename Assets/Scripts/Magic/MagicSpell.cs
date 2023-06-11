using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicSpell
{
    //TODO: Add a coroutine for countdown

    private PlayerAction action;

    private ClampedFloatVariable timer;
    private const float timeIncrements = 0.1f;
    public bool canBeCast = true;
    private Coroutine castCoroutine;

    public List<OriginSpellComponent> spellComponents;
    public int compCount;

    public ElementData element;

    public MagicSpell()
    {
        spellComponents = new List<OriginSpellComponent>();
    }

    public MagicSpell(List<OriginSpellComponent> _spellComponents, ElementData _element)
    {
        spellComponents = _spellComponents;
        compCount = spellComponents.Count;
        element = _element;
    }

    public void NextComponent(int _index, SpellHistoryNode _history, SpellCastData _castData)
    {
        if(_index + 1 >= 0 && _index + 1 < compCount) 
        {
            spellComponents[_index + 1].CreateActiveInstance(_history, _castData);
        }
        else
        {
            element.ApplyEffects(_history, _castData);
        }
    }

    private IEnumerator castCountdown()
    {
        canBeCast = false;
        timer.Value = timer.maxValue;
        while(timer.Value > 0)
        {
            timer.Value = Mathf.Clamp(timer.Value - timeIncrements, timer.minValue, timer.maxValue);
            yield return Helpers.GetWait(timeIncrements);
        }
        canBeCast = true;
    }

    public void Cast()
    {
        //if(compCount > 0) 
        //{
        //    spellComponents[0].CreateActiveInstance(new SpellHistoryNode(action.director, null), new SpellCastData(action.director, this, element));
        //}
        NextComponent(-1, new SpellHistoryNode(action.director, null), new SpellCastData(action.director, this, element));
        if (!canBeCast && castCoroutine != null)
        {
            action.StopCoroutine(castCoroutine);
        }
        castCoroutine = action.StartCoroutine(castCountdown());
    }

    public void SpecialTrigger()
    { OnSpecialTrigger?.Invoke(); }

    public void Equip(PlayerAction _action, ClampedFloatVariable _timer)
    {
        action = _action;
        timer = _timer;
    }

    public event GenericTrigger OnSpecialTrigger;

}