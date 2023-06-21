using System;
using System.Collections;
using UnityEngine;

public class AFork : ActiveSpellComponent
{
    public int branches;
    private Coroutine timerCoroutine;

    public AFork(int _branches)
    {
        branches = _branches;
    }

    public AFork(OriginSpellComponent _origin, SpellHistoryNode _history, SpellCastData _castData, int _branches) : base(_origin, _history, _castData)
    {
        branches = _branches;
    }

    public override void Execue()
    {
        timerCoroutine = MagicManager.Instance.StartCoroutine(Timer());
        history.target.GetActualDirector().OnObjectDestroy += FinishTimer;
        state = ActiveSpellStates.Waiting;
    }

    private IEnumerator Timer()
    {
        for (int i = 0; i < branches; i++)
        {
            origin.NextComponent(history, castData);
            yield return Helpers.GetWait(0.25f);
        }
        FinishTimer(null);
    }

    private void FinishTimer(IMagicObjectDirector _director)
    {
        MagicManager.Instance.StopCoroutine(timerCoroutine);
        history.target.GetActualDirector().OnObjectDestroy -= FinishTimer;
        state = ActiveSpellStates.Finished;
    }

    public override ActiveSpellComponent Clone(OriginSpellComponent _origin, SpellHistoryNode _history, SpellCastData _castData)
    {
        return new AFork(_origin, _history, _castData,branches);
    }

    public override void Reset(OriginSpellComponent _origin, SpellHistoryNode _history, SpellCastData _castData, ActiveSpellComponent _active)
    {
        MagicManager.Instance.StopCoroutine(timerCoroutine);
        history.target.GetActualDirector().OnObjectDestroy -= FinishTimer;
        origin = _origin;
        history = _history;
        castData = _castData;
        state = ActiveSpellStates.Started;
        AFork temp = (AFork)_active;
        branches = temp.branches;
    }

    public override OriginSpellComponent GenerateOriginComponent()
    {
        return new OFork(this);
    }

    public override string ToString()
    {
        return "Fork";
    }

    public override void EndComponent()
    {
        FinishTimer(null);
    }

}

public class OFork : OriginSpellComponent
{
    public OFork(ActiveSpellComponent _active) : base(_active, 25) {}

    public override bool isOfRightType(ActiveSpellComponent _active)
    {
        return _active.GetType() == typeof(AFork);
    }
}

public class GFork : GeneticSpellComponentInt
{
    public GFork() : base(1, 10)
    {
    }

    public GFork(int _id, int _value) : base(_id, _value, 1, 10)
    {
    }

    public override GeneticSpellComponent Clone()
    {
        return new GFork(id, value);
    }

    public override bool CompareComponent(in GeneticSpellComponent _other, in float genCMFraction, out double similarity)
    {
        if (_other.GetType() == typeof(GFork))
        {
            GFork temp = (GFork)_other;
            similarity = DifFunc(MathF.Abs(value - temp.value) / (float)(upper - lower));
            return true;
        }
        similarity = 0;
        return false;
    }

    public override GeneticSpellComponent Generate()
    {
        return new GFork(id, Helpers.Range(lower, upper));
    }

    public override OriginSpellComponent GenerateOrigin(ElementData _element)
    {
        return (new AFork(value)).GenerateOriginComponent();
    }

    public override string GetDisplayString()
    {
        return "Fork " + value + " times";
    }
}
