using UnityEngine;

public class ADestroyTrigger : ActiveSpellComponent
{

    public ADestroyTrigger()
    {
        
    }

    public ADestroyTrigger(OriginSpellComponent _origin, SpellHistoryNode _history, SpellCastData _castData) : base(_origin, _history, _castData)
    {
        
    }

    public override void Execue()
    {
        if(history.target.GetActualDirector() == null) 
        {
            state = ActiveSpellStates.Finished;
            return;
        }
        history.target.GetActualDirector().OnObjectDestroy += ActivateTrigger;
        state = ActiveSpellStates.Waiting;
    }

    private void ActivateTrigger()
    {
        history.target.GetActualDirector().OnObjectDestroy -= ActivateTrigger;
        origin.NextComponent(history, castData);
        state = ActiveSpellStates.Finished;
    }

    public override ActiveSpellComponent Clone(OriginSpellComponent _origin, SpellHistoryNode _history, SpellCastData _castData)
    {
        return new ADestroyTrigger(_origin, _history, _castData);
    }

    public override void Reset(OriginSpellComponent _origin, SpellHistoryNode _history, SpellCastData _castData, ActiveSpellComponent _active)
    {
        history.target.GetActualDirector().OnObjectDestroy -= ActivateTrigger;
        origin = _origin;
        history = _history;
        castData = _castData;
        state = ActiveSpellStates.Started;
        ADestroyTrigger temp = (ADestroyTrigger)_active;
    }

    public override OriginSpellComponent GenerateOriginComponent()
    {
        return new ODestroyTriggerId(this);
    }

    public override string ToString()
    {
        return "DestroyTriggerId";
    }

    public override void EndComponent()
    {
        //This might not be a good idea
        origin.NextComponent(history,castData);
        state = ActiveSpellStates.Finished;
    }
}

public class ODestroyTriggerId : OriginSpellComponent
{
    public ODestroyTriggerId(ActiveSpellComponent _active) : base(_active, 25) {}

    public override bool isOfRightType(ActiveSpellComponent _active)
    {
        return _active.GetType() == typeof(ADestroyTrigger);
    }
}
