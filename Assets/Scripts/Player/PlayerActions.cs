using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(PlayerDirector))]
public class PlayerAction : MonoBehaviour
{
    [HideInInspector]
    public PlayerDirector director;

    //Make these scriptable objects
    private MagicSpell[] spells = new MagicSpell[4];
    public ClampedFloatVariable[] spellTimers = new ClampedFloatVariable[4];

    private bool canCast = true;
    private float castDowntime = 0.1f;
    private Coroutine castDisable;
    private int lastCastSpellIndex = -1;

    private bool creatingSpell = false;
    private bool finishedStep = false;
    private bool isUIOpen = false;

    //Note It might run even when exiting play mode
    public void HandleSpellMaker()
    {

        if (!creatingSpell)
        {
            StartSpellMaker();
        }
        else if(isUIOpen)
        {
            PlayerUIManager.Instance.ExitCandidateSelect();
            isUIOpen = false;
        }
        else if (finishedStep) 
        {
            PlayerUIManager.Instance.ChangeToCandidateSelect();
            isUIOpen = true;
        }
    }

    private GeneticAlgorithm alg;
    private void StartSpellMaker()
    {
        creatingSpell = true;
        List<GeneticSpellComponent> componentBag = new List<GeneticSpellComponent>();
        componentBag.Add(new GPropel());
        componentBag.Add(new GExplode());
        componentBag.Add(new GCreateMProjectile());
        //componentBag.Add(new GPierce());
        //componentBag.Add(new GFork());
        //componentBag.Add(new GSticky());
        alg = new GeneticAlgorithm(componentBag, 100, 200, 12, 0.3f, 0.1f, 0.2f, 0.3f, 1.4f);
        PlayerUIManager.Instance.SetSpellCandidates(alg.GetFirstRoundOfCandidates(), alg.ID);
        PlayerUIManager.Instance.SetSMInteractNotification(true);
        finishedStep = true;
    }

    public void NextIteration(List<SpellCandidate> _candidates , int _ID)
    {
        isUIOpen = false;
        if (alg.ID == _ID && !alg.IsDone) 
        {
            PlayerUIManager.Instance.SetSpellCandidates(alg.NextIterations(_candidates), alg.ID);
            PlayerUIManager.Instance.SetSMInteractNotification(true);
            finishedStep = true;
        }
        else if (alg.ID == _ID)
        {
            creatingSpell = false;
        }
    }

    private IEnumerator DisableCasting()
    {
        canCast = false;
        yield return Helpers.GetWait(castDowntime);
        canCast = true;
    }

    public void OnAction(List<bool> _input, bool _special)
    {
        if(_input.Count != 4)
        {
            throw new IndexOutOfRangeException();
        }
        if(!canCast)
        {
            return;
        }
        for (int i = 0; i < 4; i++)
        {
            //TODO: Add the health decreaseing thing for forced activation
            if (spells[i] != null && _input[i])
            {
                if (!_special)
                {
                    if(spells[i].canBeCast)
                    {
                        spells[i].Cast();
                        castDisable = StartCoroutine(DisableCasting());
                        lastCastSpellIndex = -1;
                        break;
                    }
                    else 
                    {
                        if(lastCastSpellIndex != i)
                        {
                            lastCastSpellIndex = i;
                        }
                        else
                        {
                            if (director.HasEnoughHealth(spellTimers[i].Value))
                            {
                                director.Damage(spellTimers[i].Value, director);
                                spells[i].Cast();
                                castDisable = StartCoroutine(DisableCasting());
                            }
                                lastCastSpellIndex = -1;
                                break;  
                        }
                    }
                }
                else if(_special)
                {
                    spells[i].SpecialTrigger();
                    break;
                }
            }
        }
    }


    public ElementData element;
    private void Awake()
    {
        director = GetComponent<PlayerDirector>();
        spells[0] = SpellBuilder.GenerateSpell(element, new List<ActiveSpellComponent>() { new ADashDir(20), new ACreateMProjectile(), new ATimeTrigger(10), new AExplode(10) });
        spellTimers[0].maxValue = 10;
        spells[0].Equip(this, spellTimers[0]);
        spells[1] = SpellBuilder.GenerateSpell(element, new List<ActiveSpellComponent>() { new ACreateMProjectile(), new APropelMObject(30), new ACollisionTrigger() });
        spellTimers[1].maxValue = 5;
        spells[1].Equip(this, spellTimers[1]);
        spells[2] = SpellBuilder.GenerateSpell(element, new List<ActiveSpellComponent>() { new ACreateMProjectile(), new APropelMObject(30), new ACollisionTrigger(), new AExplode(25) });
        spellTimers[2].maxValue = 25;
        spells[2].Equip(this, spellTimers[2]);
        spells[3] = SpellBuilder.GenerateSpell(element, new List<ActiveSpellComponent>() { new ATeleportDir(10), new ACreateMProjectile(), new ATimeTrigger(10), new AExplode(25) });
        spellTimers[3].maxValue = 30;
        spells[3].Equip(this, spellTimers[3]);
    }
}
