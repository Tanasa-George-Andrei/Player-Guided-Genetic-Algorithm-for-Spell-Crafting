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
        history.target.GetActualDirector().OnObjectDisable += FinishTimer;
        state = ActiveSpellStates.Waiting;
    }

    private IEnumerator Timer()
    {
        yield return Helpers.GetWait(time);
        FinishTimer();
    }

    private void FinishTimer()
    {
        MagicManager.Instance.StopCoroutine(timerCoroutine);
        history.target.GetActualDirector().OnObjectDisable -= FinishTimer;
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
        history.target.GetActualDirector().OnObjectDisable -= FinishTimer;
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
        return "PrintId";
    }

    public override void EndComponent()
    {
        FinishTimer();
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
