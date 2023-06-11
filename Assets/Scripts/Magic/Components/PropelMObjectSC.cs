using System.Collections;
using UnityEngine;

public class APropelMObject : ActiveSpellComponent
{
    public float speed;

    public APropelMObject(float _speed)
    {
        speed = _speed;
    }

    public APropelMObject(OriginSpellComponent _origin, SpellHistoryNode _history, SpellCastData _castData, float _speed) : base(_origin, _history, _castData)
    {
        speed = _speed;
    }

    public override void Execue()
    {
        if (history.target.GetActualDirector() == null)
        {
            state = ActiveSpellStates.Finished;
            return;
        }
        if (state == ActiveSpellStates.Started)
        {
            history.target.GetActualDirector().Propel(history.target.GetPropelDir(), speed);
            origin.NextComponent(new SpellHistoryNode(
                new MagicPlaceholderDirector(
                    history.target.GetActualDirector(), 
                    history.target.GetActualDirector().GetCollider(),
                    history.target.GetActualDirector().GetGameObject(),
                    history.target.GetActualDirector().GetName(), 
                    history.target.GetActualDirector().GetPosition(),
                    Quaternion.FromToRotation(Vector3.forward, 
                        -history.target.GetActualDirector().GetPropelDir())),
                history), castData);
            state = ActiveSpellStates.Finished;
        }
    }

    public override ActiveSpellComponent Clone(OriginSpellComponent _origin, SpellHistoryNode _history, SpellCastData _castData)
    {
        return new APropelMObject(_origin, _history, _castData, speed);
    }

    public override void Reset(OriginSpellComponent _origin, SpellHistoryNode _history, SpellCastData _castData, ActiveSpellComponent _active)
    {
        origin = _origin;
        history = _history;
        castData = _castData;
        APropelMObject temp = (APropelMObject)_active;
        state = ActiveSpellStates.Started;
        speed = temp.speed;
    }

    public override OriginSpellComponent GenerateOriginComponent()
    {
        return new OPropelMObject(this);
    }

    public override string ToString()
    {
        return "PropelMObject: " + speed;
    }

}

public class OPropelMObject : OriginSpellComponent
{
    public OPropelMObject(ActiveSpellComponent _active) : base(_active, 25) {}

    public override bool isOfRightType(ActiveSpellComponent _active)
    {
        return _active.GetType() == typeof(APropelMObject);
    }
}
