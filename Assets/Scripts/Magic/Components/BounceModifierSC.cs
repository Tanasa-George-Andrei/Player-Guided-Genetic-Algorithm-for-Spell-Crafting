using System;
using System.Collections.Generic;
using UnityEngine;

public class ABounceModifier : ActiveSpellComponent
{

    public int noBounces;

    public ABounceModifier(int _noBounces)
    {
        noBounces = _noBounces;
    }

    public ABounceModifier(OriginSpellComponent _origin, SpellHistoryNode _history, SpellCastData _castData, int _noBounces) : base(_origin, _history, _castData)
    {
        noBounces = _noBounces;
    }

    public override void Execue()
    {
        if (history.target.GetActualDirector() == null || !history.target.GetActualDirector().GetGameObject().activeSelf)
        {
            state = ActiveSpellStates.Finished;
            return;
        }
        history.target.GetActualDirector().SetBounce(noBounces);
        history.target.GetActualDirector().OnBounce += ActivateTrigger;
        history.target.GetActualDirector().OnObjectDestroy += BypassTrigger;
        state = ActiveSpellStates.Waiting;
    }

    private void ActivateTrigger(IMagicObjectDirector _director, bool _isLastBounce)
    {
        origin.NextComponent(SpellHistoryNode.AddNode(_director, history), castData);
        if(_isLastBounce)
        {
            history.target.GetActualDirector().OnObjectDestroy -= BypassTrigger;
            history.target.GetActualDirector().OnBounce -= ActivateTrigger;
            state = ActiveSpellStates.Finished;
        }
    }
    private void BypassTrigger(IMagicObjectDirector _director)
    {
        history.target.GetActualDirector().OnObjectDestroy -= BypassTrigger;
        history.target.GetActualDirector().OnBounce -= ActivateTrigger;
        state = ActiveSpellStates.Finished;
    }

    public override ActiveSpellComponent Clone(OriginSpellComponent _origin, SpellHistoryNode _history, SpellCastData _castData)
    {
        return new ABounceModifier(_origin, _history, _castData, noBounces);
    }

    public override void Reset(OriginSpellComponent _origin, SpellHistoryNode _history, SpellCastData _castData, ActiveSpellComponent _active)
    {
        history.target.GetActualDirector().OnBounce -= ActivateTrigger;
        history.target.GetActualDirector().OnObjectDestroy -= BypassTrigger;
        origin = _origin;
        history = _history;
        castData = _castData;
        state = ActiveSpellStates.Started;
        ABounceModifier temp = (ABounceModifier)_active;
        temp.noBounces = noBounces;
    }

    public override OriginSpellComponent GenerateOriginComponent()
    {
        return new OGBounceModifier(this);
    }

    public override string ToString()
    {
        return "GBounceModifier";
    }

    public override void EndComponent()
    {
        BypassTrigger(null);
    }
}

public class OGBounceModifier : OriginSpellComponent
{
    public OGBounceModifier(ActiveSpellComponent _active) : base(_active, 25) {}

    public override bool isOfRightType(ActiveSpellComponent _active)
    {
        return _active.GetType() == typeof(ABounceModifier);
    }
}

public class GBounceModifier : GeneticSpellComponentInt
{
    public GBounceModifier() : base(1, 10)
    {
    }

    public GBounceModifier(int _id, int _value) : base(_id, _value, 1, 10)
    {
    }

    public override GeneticSpellComponent Clone()
    {
        return new GBounceModifier(id, value);
    }

    public override bool CompareComponent(in GeneticSpellComponent _other, in float genCMFraction, out double similarity)
    {
        if (_other.GetType() == typeof(GBounceModifier))
        {
            GBounceModifier temp = (GBounceModifier)_other;
            similarity = DifFunc(MathF.Abs(value - temp.value) / (float)(upper - lower));
            return true;
        }
        similarity = 0;
        return false;
    }

    public override GeneticSpellComponent Generate()
    {
        return new GBounceModifier(id, Helpers.Range(lower, upper));
    }

    public override OriginSpellComponent GenerateOrigin(ElementData _element)
    {
        return (new ABounceModifier(value)).GenerateOriginComponent();
    }

    public override string GetDisplayString()
    {
        return "Bounce for " + value + " times";
    }
}
