using System;
using System.Collections;
using UnityEngine;

public class APropel : ActiveSpellComponent
{
    public float speed;

    public APropel(float _speed)
    {
        speed = _speed;
    }

    public APropel(OriginSpellComponent _origin, SpellHistoryNode _history, SpellCastData _castData, float _speed) : base(_origin, _history, _castData)
    {
        speed = _speed;
    }

    public override void Execue()
    {
        if (history.target.GetActualDirector() == null || !history.target.GetActualDirector().GetGameObject().activeSelf)
        {
            state = ActiveSpellStates.Finished;
            return;
        }
        if (state == ActiveSpellStates.Started)
        {
            history.target.GetActualDirector().Propel(history.target.GetPropelDir(), speed);
            origin.NextComponent(SpellHistoryNode.AddNode(
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
        return new APropel(_origin, _history, _castData, speed);
    }

    public override void Reset(OriginSpellComponent _origin, SpellHistoryNode _history, SpellCastData _castData, ActiveSpellComponent _active)
    {
        origin = _origin;
        history = _history;
        castData = _castData;
        APropel temp = (APropel)_active;
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
        return _active.GetType() == typeof(APropel);
    }
}

public class GPropel : GeneticSpellComponentInt
{
    public GPropel() : base(2, 60)
    {
    }

    public GPropel(int _id, int _value) : base(_id, _value, 2, 60)
    {
    }

    public float GetSpeed()
    {
        return (value * 0.5f);
    }

    public float GetDistance()
    {
        return GetSpeed();
    }

    public override GeneticSpellComponent Clone()
    {
        return new GPropel(id, value);
    }

    public override bool CompareComponent(in GeneticSpellComponent _other, in float genCMFraction, out double similarity)
    {
        if (_other.GetType() == typeof(GPropel))
        {
            GPropel temp = (GPropel)_other;
            similarity = DifFunc(MathF.Abs(GetSpeed() - temp.GetSpeed()) / ((upper - lower) * 0.5f));
            return true;
        }
        else if (_other.GetType() == typeof(GDashDir))
        {
            GDashDir temp = (GDashDir)_other;
            similarity = DropoffFunc(genCMFraction) * DifFunc(MathF.Abs(GetDistance() - temp.GetDistance()) / ((upper - lower) * 0.5f));
            return true;
        }
        else if (_other.GetType() == typeof(GTeleportDir))
        {
            GTeleportDir temp = (GTeleportDir)_other;
            similarity = 0.9f * DropoffFunc(genCMFraction) * DifFunc(MathF.Abs(GetDistance() - temp.GetDistance()) / ((upper - lower) * 0.5f));
            return true;
        }
        similarity = 0;
        return false;
    }

    public override GeneticSpellComponent Generate()
    {
        return new GPropel(id, Helpers.Range(lower, upper));
    }

    public override OriginSpellComponent GenerateOrigin(ElementData _element)
    {
        return (new APropel(GetSpeed())).GenerateOriginComponent();
    }

    public override string GetComponentName()
    {
        return "Propel";
    }

    public override string GetDisplayString()
    {
        return "Propel at " + GetSpeed() +"m/s";
    }
}