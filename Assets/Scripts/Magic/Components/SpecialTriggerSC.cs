using UnityEngine;
using static UnityEngine.UI.Image;

public class ASpecialTrigger : ActiveSpellComponent
{

    public ASpecialTrigger()
    {
        
    }

    public ASpecialTrigger(OriginSpellComponent _origin, SpellHistoryNode _history, SpellCastData _castData) : base(_origin, _history, _castData)
    {
        
    }

    public override void Execue()
    {
        castData.spell.OnSpecialTrigger += ActivateTrigger;
        state = ActiveSpellStates.Waiting;
    }

    private void ActivateTrigger()
    {
        castData.spell.OnSpecialTrigger -= ActivateTrigger;
        origin.NextComponent(history, castData);
        state = ActiveSpellStates.Finished;
    }

    public override ActiveSpellComponent Clone(OriginSpellComponent _origin, SpellHistoryNode _history, SpellCastData _castData)
    {
        return new ASpecialTrigger(_origin, _history, _castData);
    }

    public override void Reset(OriginSpellComponent _origin, SpellHistoryNode _history, SpellCastData _castData, ActiveSpellComponent _active)
    {
        castData.spell.OnSpecialTrigger -= ActivateTrigger;
        origin = _origin;
        history = _history;
        castData = _castData;
        state = ActiveSpellStates.Started;
        ASpecialTrigger temp = (ASpecialTrigger)_active;
    }

    public override OriginSpellComponent GenerateOriginComponent()
    {
        return new OSpecialTriggerId(this);
    }

    public override string ToString()
    {
        return "SpecialTriggerId";
    }

    public override void EndComponent()
    {
        ActivateTrigger();
    }
}

public class OSpecialTriggerId : OriginSpellComponent
{
    public OSpecialTriggerId(ActiveSpellComponent _active) : base(_active, 25) {}

    public override bool isOfRightType(ActiveSpellComponent _active)
    {
        return _active.GetType() == typeof(ASpecialTrigger);
    }
}
