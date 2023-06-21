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
        if (history.target.GetActualDirector() == null || !history.target.GetActualDirector().GetGameObject().activeSelf)
        {
            state = ActiveSpellStates.Finished;
            return;
        }
        history.target.GetActualDirector().OnObjectDestroy += ActivateTrigger;
        state = ActiveSpellStates.Waiting;
    }

    //Modify so it takes a placeholder
    private void ActivateTrigger(IMagicObjectDirector _director)
    {
        history.target.GetActualDirector().OnObjectDestroy -= ActivateTrigger;
        origin.NextComponent(SpellHistoryNode.AddNode(_director, history), castData);
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
    }

    public override OriginSpellComponent GenerateOriginComponent()
    {
        return new ODestroyTrigger(this);
    }

    public override string ToString()
    {
        return "DestroyTrigger";
    }

    public override void EndComponent()
    {
        //This might not be a good idea
        origin.NextComponent(history,castData);
        state = ActiveSpellStates.Finished;
    }
}

public class ODestroyTrigger : OriginSpellComponent
{
    public ODestroyTrigger(ActiveSpellComponent _active) : base(_active, 25) {}

    public override bool isOfRightType(ActiveSpellComponent _active)
    {
        return _active.GetType() == typeof(ADestroyTrigger);
    }
}

public class GDestroyTrigger : GeneticSpellComponent
{
    public GDestroyTrigger()
    {
    }

    public GDestroyTrigger(int _id) : base(_id)
    {
    }

    public override GeneticSpellComponent Clone()
    {
        return new GDestroyTrigger(id);
    }

    public override bool CompareComponent(in GeneticSpellComponent _other, in float genCMFraction, out double similarity)
    {
        if (_other.GetType() == typeof(GDestroyTrigger))
        {
            similarity = 1;
            return true;
        }
        similarity = 0;
        return false;
    }

    public override GeneticSpellComponent Generate()
    {
        return new GDestroyTrigger(id);
    }

    public override OriginSpellComponent GenerateOrigin(ElementData _element)
    {
        return (new ADestroyTrigger()).GenerateOriginComponent();
    }

    public override string GetDisplayString()
    {
        return "On Destroy";
    }

    public override void ParamMutation(in float genCMFraction)
    {
    }

    public override GeneticSpellComponent CompMutation()
    {
        return GeneticComponentBag.triggerList[Helpers.Range(0, GeneticComponentBag.triggerList.Length)].Generate();
    }
}