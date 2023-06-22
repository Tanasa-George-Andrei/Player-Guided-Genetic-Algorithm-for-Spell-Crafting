using System;
using System.Collections;
using UnityEngine;

public class ATimeTrigger : ActiveSpellComponent
{
    public float time;
    private Coroutine timerCoroutine;

    public ATimeTrigger(float _time)
    {
        time = _time;
    }

    public ATimeTrigger(OriginSpellComponent _origin, SpellHistoryNode _history, SpellCastData _castData, float _time) : base(_origin, _history, _castData)
    {
        time = _time;
    }

    public override void Execue()
    {
        timerCoroutine = MagicManager.Instance.StartCoroutine(Timer());
        history.target.GetActualDirector().OnObjectDestroy += FinishTimer;
        state = ActiveSpellStates.Waiting;
    }

    private IEnumerator Timer()
    {
        yield return Helpers.GetWait(time);
        FinishTimer(null);
    }

    private void FinishTimer(IMagicObjectDirector _director)
    {
        MagicManager.Instance.StopCoroutine(timerCoroutine);
        history.target.GetActualDirector().OnObjectDestroy -= FinishTimer;
        origin.NextComponent(history, castData);
        state = ActiveSpellStates.Finished;
    }

    public override ActiveSpellComponent Clone(OriginSpellComponent _origin, SpellHistoryNode _history, SpellCastData _castData)
    {
        return new ATimeTrigger(_origin, _history, _castData,time);
    }

    public override void Reset(OriginSpellComponent _origin, SpellHistoryNode _history, SpellCastData _castData, ActiveSpellComponent _active)
    {
        MagicManager.Instance.StopCoroutine(timerCoroutine);
        history.target.GetActualDirector().OnObjectDestroy -= FinishTimer;
        origin = _origin;
        history = _history;
        castData = _castData;
        state = ActiveSpellStates.Started;
        ATimeTrigger temp = (ATimeTrigger)_active;
        time = temp.time;
    }

    public override OriginSpellComponent GenerateOriginComponent()
    {
        return new OTimeTrigger(this);
    }

    public override string ToString()
    {
        return "TimeTrigger";
    }

    public override void EndComponent()
    {
        FinishTimer(null);
    }

}

public class OTimeTrigger : OriginSpellComponent
{
    public OTimeTrigger(ActiveSpellComponent _active) : base(_active, 25) {}

    public override bool isOfRightType(ActiveSpellComponent _active)
    {
        return _active.GetType() == typeof(ATimeTrigger);
    }
}

public class GTimeTrigger : GeneticSpellComponentInt
{
    public GTimeTrigger() : base(1, 60)
    {
    }

    public GTimeTrigger(int _id, int _value) : base(_id, _value, 1, 60)
    {
    }

    public float GetWaitTime()
    {
        return value * 0.5f;
    }

    public override GeneticSpellComponent Clone()
    {
        return new GTimeTrigger(id, value);
    }

    public override bool CompareComponent(in GeneticSpellComponent _other, in float genCMFraction, out double similarity)
    {
        if (_other.GetType() == typeof(GTimeTrigger))
        {
            GTimeTrigger temp = (GTimeTrigger)_other;
            similarity = DifFunc(MathF.Abs(value - temp.value) / (float)(upper - lower));
            return true;
        }
        similarity = 0;
        return false;
    }

    public override GeneticSpellComponent Generate()
    {
        return new GTimeTrigger(id, Helpers.Range(lower, upper));
    }

    public override OriginSpellComponent GenerateOrigin(ElementData _element)
    {
        return (new ATimeTrigger(GetWaitTime())).GenerateOriginComponent();
    }

    public override string GetComponentName()
    {
        return "Wait";
    }

    public override string GetDisplayString()
    {
        return "Wait for " + GetWaitTime() + "s";
    }

    public override void ParamMutation(in float genCMFraction)
    {
    }

    public override GeneticSpellComponent CompMutation()
    {
        return GeneticComponentBag.triggerList[Helpers.Range(0, GeneticComponentBag.triggerList.Length)].Generate();
    }
}
