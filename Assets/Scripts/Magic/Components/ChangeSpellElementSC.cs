using UnityEngine;

public class AChangeSpellElement : ActiveSpellComponent
{
    public AChangeSpellElement()
    {

    }

    public AChangeSpellElement(OriginSpellComponent _origin, SpellHistoryNode _history, SpellCastData _castData) : base(_origin, _history, _castData)
    {
    }

    public override void Execue()
    {
        origin.NextComponent(history, new SpellCastData(castData.owner,castData.spell,castData.element.oppositeElement));
        state = ActiveSpellStates.Finished;
    }

    public override ActiveSpellComponent Clone(OriginSpellComponent _origin, SpellHistoryNode _history, SpellCastData _castData)
    {
        return new AChangeSpellElement(_origin, _history, _castData);
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
        return new OChangeSpellElement(this);
    }

    public override string ToString()
    {
        return "ChangeSpellElement";
    }

}

public class OChangeSpellElement : OriginSpellComponent
{
    public OChangeSpellElement(ActiveSpellComponent _active) : base(_active, 25) {}

    public override bool isOfRightType(ActiveSpellComponent _active)
    {
        return _active.GetType() == typeof(AChangeSpellElement);
    }
}

public class GChangeSpellElement : GeneticSpellComponent
{
    public GChangeSpellElement()
    {
    }

    public GChangeSpellElement(int _id) : base(_id)
    {
    }

    public override GeneticSpellComponent Clone()
    {
        return new GChangeSpellElement(id);
    }

    public override bool CompareComponent(in GeneticSpellComponent _other, in float genCMFraction, out double similarity)
    {
        if (_other.GetType() == typeof(GChangeSpellElement))
        {
            similarity = 1;
            return true;
        }
        similarity = 0;
        return false;
    }

    public override GeneticSpellComponent Generate()
    {
        return new GChangeSpellElement(id);
    }

    public override OriginSpellComponent GenerateOrigin(ElementData _element)
    {
        return (new AChangeSpellElement()).GenerateOriginComponent();
    }

    public override string GetComponentName()
    {
        return "Change Spell Element";
    }

    public override string GetDisplayString()
    {
        return "Change Spell Element";
    }

    public override void ParamMutation(in float genCMFraction)
    {
    }
}