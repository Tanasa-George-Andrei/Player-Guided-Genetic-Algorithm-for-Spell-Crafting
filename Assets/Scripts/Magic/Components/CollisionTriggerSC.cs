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
        if(history.target.GetActualDirector() == null) 
        {
            state = ActiveSpellStates.Finished;
            return;
        }
        history.target.GetActualDirector().HitTrigger += ActivateTrigger;
        history.target.GetActualDirector().OnObjectDestroy += BypassTrigger;
        state = ActiveSpellStates.Waiting;
    }

    private void ActivateTrigger(Collision _collision)
    {
        history.target.GetActualDirector().HitTrigger -= ActivateTrigger;
        //List<Collider> temp = new List<Collider>();
        //For the moment the damage will scale with the amount of hits
        IMagicObjectDirector tempDirector;
        for (int i = 0; i < _collision.contactCount; i++)
        {
            tempDirector = _collision.GetContact(i).otherCollider.gameObject.GetComponent<IMagicObjectDirector>();
            origin.NextComponent(new SpellHistoryNode(
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
    private void BypassTrigger()
    {
        history.target.GetActualDirector().OnObjectDestroy -= BypassTrigger;
        origin.NextComponent(history, castData);
        state = ActiveSpellStates.Finished;
    }

    public override ActiveSpellComponent Clone(OriginSpellComponent _origin, SpellHistoryNode _history, SpellCastData _castData)
    {
        return new ACollisionTrigger(_origin, _history, _castData);
    }

    public override void Reset(OriginSpellComponent _origin, SpellHistoryNode _history, SpellCastData _castData, ActiveSpellComponent _active)
    {
        history.target.GetActualDirector().HitTrigger -= ActivateTrigger;
        history.target.GetActualDirector().OnObjectDestroy -= BypassTrigger;
        origin = _origin;
        history = _history;
        castData = _castData;
        state = ActiveSpellStates.Started;
        ACollisionTrigger temp = (ACollisionTrigger)_active;
    }

    public override OriginSpellComponent GenerateOriginComponent()
    {
        return new OCollisionTriggerId(this);
    }

    public override string ToString()
    {
        return "CollisionTriggerId";
    }

    public override void EndComponent()
    {
        BypassTrigger();
    }
}

public class OCollisionTriggerId : OriginSpellComponent
{
    public OCollisionTriggerId(ActiveSpellComponent _active) : base(_active, 25) {}

    public override bool isOfRightType(ActiveSpellComponent _active)
    {
        return _active.GetType() == typeof(ACollisionTrigger);
    }
}
