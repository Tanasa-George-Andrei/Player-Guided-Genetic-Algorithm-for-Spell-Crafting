using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public enum ActiveSpellStates
{
    Started,
    Running,
    Waiting,
    Finished
}

public abstract class ActiveSpellComponent
{
    public ActiveSpellStates state = ActiveSpellStates.Started;
    protected OriginSpellComponent origin;
    protected SpellHistoryNode history;
    protected SpellCastData castData;

    protected ActiveSpellComponent()
    { 
    }

    protected ActiveSpellComponent(OriginSpellComponent _origin, SpellHistoryNode _history, SpellCastData _castData)
    {
        origin = _origin;
        history = _history;
        castData = _castData;
    }

    public abstract void Execue();

    public abstract ActiveSpellComponent Clone(OriginSpellComponent _origin, SpellHistoryNode _history, SpellCastData _castData);

    public abstract void Reset(OriginSpellComponent _origin, SpellHistoryNode _history, SpellCastData _castData, ActiveSpellComponent _active);

    public abstract OriginSpellComponent GenerateOriginComponent();

    public virtual void EndComponent()
    {
        state = ActiveSpellStates.Finished;
    }

    public abstract override String ToString();

    public virtual void Release()
    {

        origin.ReleaseObject(this);

    }
}

//Change it so Active generates the origin
public abstract class OriginSpellComponent
{
    private int index;
    protected ActiveSpellComponent active;
    //Look into changing this into a unity object pool
    protected List<ActiveSpellComponent> availableObjects = new List<ActiveSpellComponent>();
    protected List<ActiveSpellComponent> inUseObjects = new List<ActiveSpellComponent>();

    protected readonly int maxInUseObjects;

    public int Index { get => index; set => index = value; }

    public abstract bool isOfRightType(ActiveSpellComponent _active);

    protected OriginSpellComponent(ActiveSpellComponent _active, int _maxInUseObjects)
    {
        if (isOfRightType(_active))
        {
            active = _active;
        }
        else
        {
            throw new NotSupportedException();
        }
        maxInUseObjects = _maxInUseObjects;
    }

    public virtual void CreateActiveInstance(SpellHistoryNode _history, SpellCastData _castData)
    {

        ActiveSpellComponent temp;
        if (availableObjects.Count != 0)
        {
            temp = availableObjects[0];
            temp.Reset(this, _history, _castData, active);
            inUseObjects.Add(temp);
            availableObjects.RemoveAt(0);
        }
        else
        {
            temp = active.Clone(this, _history, _castData);
            inUseObjects.Add(temp);
        }
        int componentsToBeEnded = inUseObjects.Count - maxInUseObjects;
        for (int i = 0; i <= componentsToBeEnded; i++)
        {
            inUseObjects[i].EndComponent();
        }

        //Debug.Log(this.GetType() + " | " + (inUseObjects.Count + availableObjects.Count));
        MagicManager.Instance.AddActiveSpellComponent(temp);
    }

    public virtual void ReleaseObject(ActiveSpellComponent _active)
    {
        availableObjects.Add(_active);
        inUseObjects.Remove(_active);
    }

    public void NextComponent(SpellHistoryNode _history, SpellCastData _castData)
    {
        _castData.spell.NextComponent(index, _history, _castData);
    }
}

public class GeneticAlgObjectPool<T> where T : new()
{
    private List<T> availableObjects = new List<T>();
    private List<T> inUseObjects = new List<T>();

    public T GetObject()
    {
        lock (availableObjects)
        {
            T temp;
            if (availableObjects.Count != 0)
            {
                temp = availableObjects[0];
                inUseObjects.Add(temp);
                availableObjects.RemoveAt(0);
            }
            else
            {
                temp = new T();
                inUseObjects.Add(temp);
            }
            return temp;
        }
    }
    public void ReleaseObject(T _active)
    {
        lock (availableObjects)
        {
            availableObjects.Add(_active);
            inUseObjects.Remove(_active);
        }
    }

}

public abstract class GeneticSpellComponent
{
    public int id;

    public GeneticSpellComponent()
    {
        id = 0;
    }

    protected GeneticSpellComponent(int _id)
    {
        id = _id;
    }

    public abstract GeneticSpellComponent Generate();

    public abstract bool CompareComponent(in GeneticSpellComponent _other, in float genCMFraction, out double similarity);

    public abstract void ParamMutation(in float genCMFraction);
    public virtual GeneticSpellComponent CompMutation()
    {
        return this;
    }

    public abstract OriginSpellComponent GenerateOrigin(ElementData _element);

    public abstract String GetDisplayString();

    protected static double DropoffFunc(in float genCMFraction)
    {
        return MathF.Pow(4, -genCMFraction);
    }

    protected static double DifFunc(float x)
    {
        //return MathF.Pow(2, -x);
        return -0.2f * x + 1;
    }

    public abstract GeneticSpellComponent Clone();
}

public abstract class GenericOPGeneticSpellComponent<T> : GeneticSpellComponent where T : new()
{

    protected static GeneticAlgObjectPool<T> geneticAlgObjectPool = new GeneticAlgObjectPool<T>();

}

public abstract class GeneticSpellComponentInt : GeneticSpellComponent
{
    protected readonly int lower;
    protected readonly int upper;
    public int value;

    public GeneticSpellComponentInt(int _lower, int _upper)
    {
        id = 0;
        lower = _lower;
        upper = _upper;
    }

    protected GeneticSpellComponentInt(int _id, int _value, int _lower, int _upper)
    {
        id = _id;
        lower = _lower;
        upper = _upper;
        value = _value;
    }

    public override void ParamMutation(in float genCMFraction)
    {
        float range = MathF.Ceiling((upper - lower) * 0.75f * (1 - genCMFraction));
        value = (int)MathF.Ceiling(Helpers.Range(MathF.Max(lower, value - range / 2), MathF.Min(upper, value + range / 2)));
    }

}