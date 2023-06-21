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
        else
        {
            if (alg.IsDone)
            {
                PlayerUIManager.Instance.ChangeToSpellFinish();
            }
            else
            {
                PlayerUIManager.Instance.ChangeToCandidateSelect();
            }
            isUIOpen = true;
        }
    }

    public void FinishSpellCrafting(SpellCandidate _candidate)
    {
        spells[1] = SpellBuilder.GenerateSpell(element, _candidate.spellChromosome.components);
        spellTimers[1].maxValue = 1;
        spells[1].Equip(this, spellTimers[1]);
        isUIOpen = false;
        creatingSpell = false;
    }

    private GeneticAlgorithm alg;
    private List<SpellCandidate> candidates;
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
        alg = new GeneticAlgorithm(componentBag, 5000, 100, 20, 0.3f, 0.1f, 0.2f, 10, 1.4f);
        candidates = alg.GetFirstRoundOfCandidates();
        PlayerUIManager.Instance.SetSpellCandidates(candidates, alg.ID, alg.IsDone);
    }

    public void NextIteration(List<SpellCandidate> _candidates , int _ID)
    {
        isUIOpen = false;
        if (alg.ID == _ID && !alg.IsDone) 
        {
            PlayerUIManager.Instance.SetSpellCandidates(alg.NextIterations(_candidates), alg.ID, alg.IsDone);
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
        spells[2] = SpellBuilder.GenerateSpell(element, new List<ActiveSpellComponent>() {new AFork(5), new ACreateMProjectile(), new APropel(30), new ACollisionTrigger(), new AChangeSpellElement(), new AExplode(1), new APrintId("%id") });
        spellTimers[2].maxValue = 1;
        spells[2].Equip(this, spellTimers[2]);

    }
}
