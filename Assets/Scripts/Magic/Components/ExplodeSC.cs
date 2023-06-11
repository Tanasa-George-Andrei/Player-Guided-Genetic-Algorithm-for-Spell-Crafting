using System.Collections.Generic;
using UnityEngine;

public class AExplode : ActiveSpellComponent
{
    public float radius;
    private LayerMask entityLayerMask = (1 << 9);

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
        //Change this to work off of game properties
        hits = UnityEngine.Physics.OverlapSphereNonAlloc(history.target.GetPosition(), radius, collisions, entityLayerMask);
        IMagicObjectDirector tempDirector;
        appliedTo.Clear();
        for (int i = 0; i < hits; i++)
        {
            if (Helpers.CheckExplosionObstruction(collisions[i], history.target.GetPosition(), entityLayerMask) || collisions[i].bounds.Contains(history.target.GetPosition()))
            {
                tempDirector = collisions[i].gameObject.GetComponent<IMagicObjectDirector>();
                bool isInList = false;
                foreach (IMagicObjectDirector director in appliedTo)
                {
                    if(director == tempDirector) 
                    {
                        isInList = true; 
                        break; 
                    }
                }
                if(!isInList)
                {
                    appliedTo.Add(tempDirector);
                    origin.NextComponent(new SpellHistoryNode(
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
        AExplode temp = (AExplode)_active;
        radius = temp.radius;
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
