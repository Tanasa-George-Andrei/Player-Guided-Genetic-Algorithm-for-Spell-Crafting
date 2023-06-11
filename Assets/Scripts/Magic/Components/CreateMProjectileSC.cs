using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MObjectInstance 
{
    public GameObject obj;
    public IMagicObjectDirector director;
    public Collider col;

    public MObjectInstance(GameObject _obj, IMagicObjectDirector _director, Collider _col)
    {
        obj = _obj;
        director = _director;
        col = _col;
    }
}

public class ACreateMProjectile : ActiveSpellComponent
{
    //Implement a better object pool in the element data maybe
    private MObjectInstance instance;
    private Collider[] historyColArr = new Collider[32];
    public List<Collider> historyColList = new List<Collider>();
    public List<Coroutine> colReEnable = new List<Coroutine>();

    public ACreateMProjectile()
    {
       
    }

    public ACreateMProjectile(OriginSpellComponent _origin, SpellHistoryNode _history, SpellCastData _castData) : base(_origin, _history, _castData)
    {
        
    }


    public override void Execue()
    {
        if(instance == null) 
        {
            GameObject tempObj = UnityEngine.Object.Instantiate(castData.element.projectile, history.target.GetProjectilePosition(), Quaternion.FromToRotation(Vector3.forward, history.target.GetTargetDir(history.target.GetProjectilePosition())));
            instance = new MObjectInstance(tempObj, tempObj.GetComponent<IMagicObjectDirector>(), tempObj.GetComponent<Collider>());
        }
        else
        {
            instance.director.ResetObject(history.target.GetProjectilePosition(), Quaternion.FromToRotation(Vector3.forward, history.target.GetTargetDir(history.target.GetProjectilePosition())));
        }
        instance.obj.name = castData.element.projectile.name + " " + UnityEngine.Random.Range(0,1000);

        int hits = Physics.OverlapSphereNonAlloc(history.target.GetProjectilePosition(),instance.col.bounds.extents.y, historyColArr);
        for (int i = 0; i < hits; i++)
        {
            Physics.IgnoreCollision(instance.col, historyColArr[i], true);
            historyColList.Add(historyColArr[i]);
            colReEnable.Add(MagicManager.Instance.StartCoroutine(ReEnableCol(i, historyColArr[i])));
        }
        origin.NextComponent(new SpellHistoryNode(instance.director,history), castData);
        instance.director.OnObjectDisable += SetInactive;
        state = ActiveSpellStates.Waiting;
    }
    //TODO: Modify for different time scales or make a merhod respoisble for finding if colliders roughly colide
    //Also if you make a custom overlap function also add a timeout
    //Note Spheres are easier to calculate for intersection
    private IEnumerator ReEnableCol(int _index, Collider _col)
    {
        yield return Helpers.GetWait(1f);
        Physics.IgnoreCollision(instance.col, _col, false);
        historyColList.Remove(_col);
    }

    private void SetInactive()
    {
        state = ActiveSpellStates.Finished;
        instance.director.OnObjectDisable -= SetInactive;
    }

    public override ActiveSpellComponent Clone(OriginSpellComponent _origin, SpellHistoryNode _history, SpellCastData _castData)
    {
        return new ACreateMProjectile(_origin, _history, _castData);
    }

    public override void Reset(OriginSpellComponent _origin, SpellHistoryNode _history, SpellCastData _castData, ActiveSpellComponent _active)
    {
        instance.director.OnObjectDisable -= SetInactive;
        origin = _origin;
        history = _history;
        castData = _castData;
        ACreateMProjectile temp = (ACreateMProjectile)_active;
        state = ActiveSpellStates.Started;
        foreach(Coroutine co in colReEnable)
        {
            MagicManager.Instance.StopCoroutine(co);
        }
        colReEnable.Clear();
        foreach (Collider col in historyColList)
        {
            Physics.IgnoreCollision(instance.col, col, false);
        }
        historyColList.Clear();
    }

    public override OriginSpellComponent GenerateOriginComponent()
    {
        return new OCreateMProjectile(this);
    }

    public override string ToString()
    {
        return "CreateMProjectile: " + castData.element.projectile.name;
    }

    public override void EndComponent()
    {
        //SetInactive();
        instance.director.DisableObject();
    }
}

public class OCreateMProjectile : OriginSpellComponent
{
    public OCreateMProjectile(ActiveSpellComponent _active) : base(_active, 25) {}

    public override bool isOfRightType(ActiveSpellComponent _active)
    {
        return _active.GetType() == typeof(ACreateMProjectile);
    }
}