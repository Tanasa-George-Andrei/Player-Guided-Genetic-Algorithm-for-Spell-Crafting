using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicManager : MonoBehaviour
{
    public static MagicManager Instance { get; private set; }

    private List<ActiveSpellComponent> activeSpellComponents = new List<ActiveSpellComponent>();

    public void AddActiveSpellComponent(ActiveSpellComponent _activeSpellComponent)
    {
        activeSpellComponents.Add(_activeSpellComponent);
    }

    public void Reset()
    {
        activeSpellComponents.Clear();
        StopAllCoroutines();
    }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
    }

    ActiveSpellComponent temp;
    private void Update()
    {
        //TODO: See about order/priority of active spell componentsor other data strucutres
        //TODO: Also see redistributing load over multiole frames or limiting the max number of components
        for (int i = 0; i < activeSpellComponents.Count; i++)
        {
            if (activeSpellComponents[i].state == ActiveSpellStates.Running || activeSpellComponents[i].state == ActiveSpellStates.Started)
            {
                    activeSpellComponents[i].Execue();
            }     
        }
        activeSpellComponents.RemoveAll(asc => { 
            if(asc.state == ActiveSpellStates.Finished)
            {
                asc.Release();
                return true;
            }
            return false; });
    }
}
