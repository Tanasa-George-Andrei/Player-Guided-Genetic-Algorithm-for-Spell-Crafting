using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AExplode : ActiveSpellComponent
{
    public float radius;
    private LayerMask entityLayerMask = (1 << 9);
    public Coroutine graphicsWait;

    public AExplode(float _radius)
    {
        radius = _radius;
    }

    public AExplode(OriginSpellComponent _origin, SpellHistoryNode _history, SpellCastData _castData, float _radius) : base(_origin, _history, _castData)
    {
        radius = _radius;
        appliedTo = new List<IMagicObjectDirector>();
    }

    Collider[] collisions = new Collider[256];
    int hits;
    List<IMagicObjectDirector> appliedTo;
    public override void Execue()
    {
        if(state == ActiveSpellStates.Started)
        {
            hits = UnityEngine.Physics.OverlapSphereNonAlloc(history.target.GetPosition(), radius, collisions, entityLayerMask);
            IMagicObjectDirector tempDirector;
            appliedTo.Clear();
            for (int i = 0; i < hits; i++)
            {
                if (Helpers.CheckExplosionObstruction(collisions[i], history.target.GetPosition(), entityLayerMask) || collisions[i].bounds.Contains(history.target.GetPosition()))
                {
                    tempDirector = collisions[i].gameObject.GetComponent<IMagicObjectDirector>();
                    if(tempDirector != null)
                    {
                        bool isInList = false;
                        foreach (IMagicObjectDirector director in appliedTo)
                        {
                            if (director == tempDirector)
                            {
                                isInList = true;
                                break;
                            }
                        }
                        if (!isInList)
                        {
                            //Debug.Log(tempDirector.GetName());
                            appliedTo.Add(tempDirector);
                            origin.NextComponent(SpellHistoryNode.AddNode(
                                new MagicPlaceholderDirector(
                                tempDirector,
                                tempDirector.GetCollider(),
                                tempDirector.GetGameObject(),
                                tempDirector.GetName(),
                                (collisions[i].bounds.center - history.target.GetPosition()).normalized,
                                Quaternion.FromToRotation(Vector3.forward, (collisions[i].bounds.center - history.target.GetPosition()).normalized
                                    )),
                                history), castData);
                        }
                    }
                }
            }
            graphicsWait = MagicManager.Instance.StartCoroutine(GraphicsWait());
        }
        if (state == ActiveSpellStates.Running)
        {
            Graphics.DrawMesh(castData.element.mesh, Matrix4x4.TRS(history.target.GetPosition(), history.target.GetRotation(), 2 * radius * Vector3.one), castData.element.material, 0);
        }
        
    }
    private IEnumerator GraphicsWait()
    {
        state = ActiveSpellStates.Running;
        yield return Helpers.GetWait(1f);
        state = ActiveSpellStates.Finished;
    }

    public override ActiveSpellComponent Clone(OriginSpellComponent _origin, SpellHistoryNode _history, SpellCastData _castData)
    {
        return new AExplode(_origin, _history, _castData, radius);
    }

    public override void Reset(OriginSpellComponent _origin, SpellHistoryNode _history, SpellCastData _castData, ActiveSpellComponent _active)
    {
        origin = _origin;
        history = _history;
        castData = _castData;
        state = ActiveSpellStates.Started;
        MagicManager.Instance.StopCoroutine(graphicsWait);
        AExplode temp = (AExplode)_active;
        radius = temp.radius;
        appliedTo.Clear();
    }

    public override OriginSpellComponent GenerateOriginComponent()
    {
        return new OExplode(this);
    }

    public override string ToString()
    {
        return "Explode";
    }

}

public class OExplode : OriginSpellComponent
{
    public OExplode(ActiveSpellComponent _active) : base(_active, 25) {}

    public override bool isOfRightType(ActiveSpellComponent _active)
    {
        return _active.GetType() == typeof(AExplode);
    }
}

public class GExplode : GeneticSpellComponentInt
{
    public GExplode() : base(1, 30)
    {
    }

    public GExplode(int _id, int _value) : base(_id, _value, 1, 30)
    {
    }

    public float GetDistance()
    {
        return value * 0.5f;
    }

    public override GeneticSpellComponent Clone()
    {
        return new GExplode(id, value);
    }

    public override bool CompareComponent(in GeneticSpellComponent _other, in float genCMFraction, out double similarity)
    {
        if (_other.GetType() == typeof(GExplode))
        {
            GExplode temp = (GExplode)_other;
            similarity = DifFunc(MathF.Abs(value - temp.value) / (float)(upper - lower));
            return true;
        }
        similarity = 0;
        return false;
    }

    public override GeneticSpellComponent Generate()
    {
        return new GExplode(id, Helpers.Range(lower, upper));
    }

    public override OriginSpellComponent GenerateOrigin(ElementData _element)
    {
        return (new AExplode(GetDistance())).GenerateOriginComponent();
    }

    public override string GetDisplayString()
    {
        return "Explode for " + GetDistance() + "m";
    }
}