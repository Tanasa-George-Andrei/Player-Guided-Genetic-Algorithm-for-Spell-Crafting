using UnityEngine;

public class AApplyElementEffect : ActiveSpellComponent
{
    public AApplyElementEffect()
    {

    }

    public AApplyElementEffect(OriginSpellComponent _origin, SpellHistoryNode _history, SpellCastData _castData) : base(_origin, _history, _castData)
    {
    }

    public override void Execue()
    {
        castData.element.ApplyEffects(history,castData);
        origin.NextComponent(history, castData);
        state = ActiveSpellStates.Finished;
    }

    public override ActiveSpellComponent Clone(OriginSpellComponent _origin, SpellHistoryNode _history, SpellCastData _castData)
    {
        return new AApplyElementEffect(_origin, _history, _castData);
    }

    public override void Reset(OriginSpellComponent _origin, SpellHistoryNode _history, SpellCastData _castData, ActiveSpellComponent _active)
    {
        origin = _origin;
        history = _history;
        castData = _castData;
        state = ActiveSpellStates.Started;
    }

    public override OriginSpellComponent GenerateOriginComponent()
    {
        return new OApplyElementEffect(this);
    }

    public override string ToString()
    {
        return "ApplyElementEffect";
    }

}

public class OApplyElementEffect : OriginSpellComponent
{
    public OApplyElementEffect(ActiveSpellComponent _active) : base(_active, 25) {}

    public override bool isOfRightType(ActiveSpellComponent _active)
    {
        return _active.GetType() == typeof(AApplyElementEffect);
    }
}

public class GApplyElementEffect : GeneticSpellComponent
{
    public GApplyElementEffect()
    {
    }

    public GApplyElementEffect(int _id) : base(_id)
    {
    }

    public override GeneticSpellComponent Clone()
    {
        return new GApplyElementEffect(id);
    }

    public override bool CompareComponent(in GeneticSpellComponent _other, in float genCMFraction, out double similarity)
    {
        if (_other.GetType() == typeof(GApplyElementEffect))
        {
            similarity = 1;
            return true;
        }
        similarity = 0;
        return false;
    }

    public override GeneticSpellComponent Generate()
    {
        return new GApplyElementEffect(id);
    }

    public override OriginSpellComponent GenerateOrigin(ElementData _element)
    {
        return (new AApplyElementEffect()).GenerateOriginComponent();
    }

    public override string GetComponentName()
    {
        return "Apply Element Effect";
    }

    public override string GetDisplayString()
    {
        return "Apply Element Effect";
    }

    public override void ParamMutation(in float genCMFraction)
    {
    }
}