using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public class ACollisionTrigger : ActiveSpellComponent
{

    public ACollisionTrigger()
    {
        
    }

    public ACollisionTrigger(OriginSpellComponent _origin, SpellHistoryNode _history, SpellCastData _castData) : base(_origin, _history, _castData)
    {
        
    }

    public override void Execue()
    {
        if(history.target.GetActualDirector() == null || !history.target.GetActualDirector().GetGameObject().activeSelf) 
        {
            state = ActiveSpellStates.Finished;
            return;
        }
        history.target.GetActualDirector().OnHit += ActivateTrigger;
        history.target.GetActualDirector().OnObjectDestroy += BypassTrigger;
        state = ActiveSpellStates.Waiting;
    }

    private void ActivateTrigger(Collision _collision)
    {
        history.target.GetActualDirector().OnHit -= ActivateTrigger;
        history.target.GetActualDirector().OnObjectDestroy -= BypassTrigger;
        //List<Collider> temp = new List<Collider>();
        //For the moment the damage will scale with the amount of hits
        IMagicObjectDirector tempDirector;
        for (int i = 0; i < _collision.contactCount; i++)
        {
            tempDirector = _collision.GetContact(i).otherCollider.gameObject.GetComponent<IMagicObjectDirector>();
            origin.NextComponent(SpellHistoryNode.AddNode(
                new MagicPlaceholderDirector(
                tempDirector,
                _collision.GetContact(i).otherCollider,
                _collision.GetContact(i).otherCollider.gameObject,
                _collision.GetContact(i).otherCollider.gameObject.name,
                _collision.GetContact(i).point,
                Quaternion.FromToRotation(Vector3.forward, _collision.GetContact(i).normal
                    )),
                history), castData);
        }
        state = ActiveSpellStates.Finished;
    }
    private void BypassTrigger(IMagicObjectDirector _director)
    {
        history.target.GetActualDirector().OnHit -= ActivateTrigger;
        history.target.GetActualDirector().OnObjectDestroy -= BypassTrigger;
        origin.NextComponent(SpellHistoryNode.AddNode(_director, history), castData);
        state = ActiveSpellStates.Finished;
    }

    public override ActiveSpellComponent Clone(OriginSpellComponent _origin, SpellHistoryNode _history, SpellCastData _castData)
    {
        return new ACollisionTrigger(_origin, _history, _castData);
    }

    public override void Reset(OriginSpellComponent _origin, SpellHistoryNode _history, SpellCastData _castData, ActiveSpellComponent _active)
    {
        if(history.target.GetActualDirector() != null)
        {
            history.target.GetActualDirector().OnHit -= ActivateTrigger;
            history.target.GetActualDirector().OnObjectDestroy -= BypassTrigger;
        }
        origin = _origin;
        history = _history;
        castData = _castData;
        state = ActiveSpellStates.Started;
    }

    public override OriginSpellComponent GenerateOriginComponent()
    {
        return new OCollisionTrigger(this);
    }

    public override string ToString()
    {
        return "CollisionTrigger";
    }

    public override void EndComponent()
    {
        BypassTrigger(history.target.GetPlaceholder(true));
    }
}

public class OCollisionTrigger : OriginSpellComponent
{
    public OCollisionTrigger(ActiveSpellComponent _active) : base(_active, 25) {}

    public override bool isOfRightType(ActiveSpellComponent _active)
    {
        return _active.GetType() == typeof(ACollisionTrigger);
    }
}

public class GCollisionTrigger : GeneticSpellComponent
{
    public GCollisionTrigger()
    {
    }

    public GCollisionTrigger(int _id) : base(_id)
    {
    }

    public override GeneticSpellComponent Clone()
    {
        return new GCollisionTrigger(id);
    }

    public override bool CompareComponent(in GeneticSpellComponent _other, in float genCMFraction, out double similarity)
    {
        if (_other.GetType() == typeof(GCollisionTrigger))
        {
            similarity = 1;
            return true;
        }
        similarity = 0;
        return false;
    }

    public override GeneticSpellComponent Generate()
    {
        return new GCollisionTrigger(id);
    }

    public override OriginSpellComponent GenerateOrigin(ElementData _element)
    {
        return (new ACollisionTrigger()).GenerateOriginComponent();
    }

    public override string GetComponentName()
    {
        return "Collision Trigger";
    }

    public override string GetDisplayString()
    {
        return "On Collision";
    }

    public override void ParamMutation(in float genCMFraction)
    {
    }

    public override GeneticSpellComponent CompMutation()
    {
        return GeneticComponentBag.triggerList[Helpers.Range(0, GeneticComponentBag.triggerList.Length)].Generate();
    }
}