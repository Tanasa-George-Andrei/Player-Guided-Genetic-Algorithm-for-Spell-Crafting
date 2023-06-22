using System;
using System.Collections;
using UnityEngine;

public class ATeleportDir : ActiveSpellComponent
{
    public float distance;

    //TODO make a variant similar to dishonored 1's blink and 2's tentacle thingy
    public ATeleportDir(float _distance)
    {
        distance = _distance;
    }

    public ATeleportDir(OriginSpellComponent _origin, SpellHistoryNode _history, SpellCastData _castData, float _distance) : base(_origin, _history, _castData)
    {
        distance = _distance;
    }

    public override void Execue()
    {
        if(state == ActiveSpellStates.Started)
        {
            if (history.target.GetActualDirector() == null || !history.target.GetActualDirector().GetGameObject().activeSelf)
            {
                state = ActiveSpellStates.Finished;
                return;
            }
            history.target.GetActualDirector().TeleportInDirection(history.target.GetFlatDir(), distance);
            origin.NextComponent(SpellHistoryNode.AddNode(
                new MagicPlaceholderDirector(
                    history.target.GetActualDirector(), 
                    history.target.GetActualDirector().GetCollider(),
                    history.target.GetActualDirector().GetGameObject(),
                    history.target.GetActualDirector().GetName(), 
                    history.target.GetActualDirector().GetPosition(),
                    Quaternion.FromToRotation(Vector3.forward, 
                        -history.target.GetActualDirector().GetFlatDir())),
                history), castData);
            state = ActiveSpellStates.Finished;
        }
    }

    public override ActiveSpellComponent Clone(OriginSpellComponent _origin, SpellHistoryNode _history, SpellCastData _castData)
    {
        return new ATeleportDir(_origin, _history, _castData, distance);
    }

    public override void Reset(OriginSpellComponent _origin, SpellHistoryNode _history, SpellCastData _castData, ActiveSpellComponent _active)
    {
        origin = _origin;
        history = _history;
        castData = _castData;
        ATeleportDir temp = (ATeleportDir)_active;
        state = ActiveSpellStates.Started;
        distance = temp.distance;
    }

    public override OriginSpellComponent GenerateOriginComponent()
    {
        return new OTeleportDir(this);
    }

    public override string ToString()
    {
        return "TeleportDir: " + distance;
    }

}

public class OTeleportDir : OriginSpellComponent
{
    public OTeleportDir(ActiveSpellComponent _active) : base(_active, 25) {}

    public override bool isOfRightType(ActiveSpellComponent _active)
    {
        return _active.GetType() == typeof(ATeleportDir);
    }
}

public class GTeleportDir : GeneticSpellComponentInt
{
    public GTeleportDir() : base(2, 60)
    {
    }

    public GTeleportDir(int _id, int _value) : base(_id, _value, 2, 60)
    {
    }

    public float GetDistance()
    {
        return value * 0.5f;
    }

    public override GeneticSpellComponent Clone()
    {
        return new GTeleportDir(id, value);
    }

    public override bool CompareComponent(in GeneticSpellComponent _other, in float genCMFraction, out double similarity)
    {
        if (_other.GetType() == typeof(GTeleportDir))
        {
            GTeleportDir temp = (GTeleportDir)_other;
            similarity = DifFunc(MathF.Abs(GetDistance() - temp.GetDistance()) / ((upper - lower) * 0.5f));
            return true;
        }
        else if (_other.GetType() == typeof(GDashDir))
        {
            GDashDir temp = (GDashDir)_other;
            similarity = 0.9f * DropoffFunc(genCMFraction) * DifFunc(MathF.Abs(GetDistance() - temp.GetDistance()) / ((upper - lower) * 0.5f));
            return true;
        }
        else if (_other.GetType() == typeof(GPropel))
        {
            GPropel temp = (GPropel)_other;
            similarity = 0.9f * DropoffFunc(genCMFraction) * DifFunc(MathF.Abs(GetDistance() - temp.GetDistance()) / ((upper - lower) * 0.5f));
            return true;
        }
        similarity = 0;
        return false;
    }

    public override GeneticSpellComponent Generate()
    {
        return new GTeleportDir(id, Helpers.Range(lower, upper));
    }

    public override OriginSpellComponent GenerateOrigin(ElementData _element)
    {
        return (new ATeleportDir(GetDistance())).GenerateOriginComponent();
    }

    public override string GetComponentName()
    {
        return "Teleport";
    }
    public override string GetDisplayString()
    {
        return "Teleport for " + GetDistance() + "m";
    }
}