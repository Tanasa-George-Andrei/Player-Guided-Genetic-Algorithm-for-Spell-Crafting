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
    }

    public override OriginSpellComponent GenerateOriginComponent()
    {
        return new OSpecialTrigger(this);
    }

    public override string ToString()
    {
        return "SpecialTrigger";
    }

    public override void EndComponent()
    {
        ActivateTrigger();
    }
}

public class OSpecialTrigger : OriginSpellComponent
{
    public OSpecialTrigger(ActiveSpellComponent _active) : base(_active, 25) {}

    public override bool isOfRightType(ActiveSpellComponent _active)
    {
        return _active.GetType() == typeof(ASpecialTrigger);
    }
}

public class GSpecialTrigger : GeneticSpellComponent
{
    public GSpecialTrigger()
    {
    }

    public GSpecialTrigger(int _id) : base(_id)
    {
    }

    public override GeneticSpellComponent Clone()
    {
        return new GSpecialTrigger(id);
    }

    public override bool CompareComponent(in GeneticSpellComponent _other, in float genCMFraction, out double similarity)
    {
        if (_other.GetType() == typeof(GSpecialTrigger))
        {
            similarity = 1;
            return true;
        }
        similarity = 0;
        return false;
    }

    public override GeneticSpellComponent Generate()
    {
        return new GSpecialTrigger(id);
    }

    public override OriginSpellComponent GenerateOrigin(ElementData _element)
    {
        return (new ASpecialTrigger()).GenerateOriginComponent();
    }

    public override string GetDisplayString()
    {
        return "On Special Key";
    }

    public override void ParamMutation(in float genCMFraction)
    {
    }

    public override GeneticSpellComponent CompMutation()
    {
        return GeneticComponentBag.triggerList[Helpers.Range(0, GeneticComponentBag.triggerList.Length)].Generate();
    }
}