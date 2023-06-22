using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Unity.VisualScripting;

public class GNoTrigger : GeneticSpellComponent
{
    public GNoTrigger()
    {
    }

    public GNoTrigger(int _id) : base(_id)
    {
    }

    public override GeneticSpellComponent Clone()
    {
        return new GNoTrigger(id);
    }

    public override bool CompareComponent(in GeneticSpellComponent _other, in float genCMFraction, out double similarity)
    {
        if (_other.GetType() == typeof(GNoTrigger))
        {
            similarity = 1;
            return true;
        }
        similarity = 0;
        return false;
    }

    public override GeneticSpellComponent Generate()
    {
        return new GNoTrigger(id);
    }

    public override OriginSpellComponent GenerateOrigin(ElementData _element)
    {
        return null;
    }

    public override string GetComponentName()
    {
        return "No Trigger";
    }

    public override string GetDisplayString()
    {
        return "Continue";
    }

    public override void ParamMutation(in float genCMFraction)
    {
    }

    public override GeneticSpellComponent CompMutation()
    {
        return GeneticComponentBag.triggerList[Helpers.Range(0, GeneticComponentBag.triggerList.Length)].Generate();
    }
}

public class GenPop
{
    public List<GeneticSpellComponent> components;

    public double fitness;
    public double rank;

    public GenPop()
    {
        fitness = 0;
        rank = 0;
        components = new List<GeneticSpellComponent>();
    }

    public GenPop(List<GeneticSpellComponent> _components)
    {
        fitness = 0;
        rank = 0;
        components = new List<GeneticSpellComponent>();
        int compCount = _components.Count;
        for (int i = 0; i < compCount; i++)
        {
            components.Add(_components[i].Clone());
        }
    }

    //Change to all compoennt value mutation
    //Change the permutation so with works with perserving structure
    public void Mutate(in int _popLen, in float genCMFraction)
    {
        int p = Helpers.Range(0, 100);
        if (p < 10)
        {
            int r = Helpers.Range(0, components.Count / 2);
            GeneticSpellComponent temp = components[r * 2 + 1].CompMutation();
            temp.id = components[r * 2 + 1].id;
            components[r * 2 + 1] = temp;
        }
        else if (p < 70)
        {
            foreach (GeneticSpellComponent comp in components)
            {
                comp.ParamMutation(genCMFraction);
            }
        }
        else
        {
            int halfCompSize = components.Count / 2;
            int noPermutations = Helpers.Range(0, halfCompSize + 1);
            int interlaceModifier;
            int c1, c2;
            for (int i = 0; i < noPermutations; i++)
            {
                c1 = Helpers.Range(0, halfCompSize);
                c2 = Helpers.Range(0, halfCompSize);
                for (int j = 0; c1 == c2 && j < 10; j++)
                {
                    c2 = Helpers.Range(0, halfCompSize);
                }
                interlaceModifier = Helpers.Range(0, 2);
                (components[c1 * 2 + interlaceModifier], components[c2 * 2 + interlaceModifier]) = (components[c2 * 2 + interlaceModifier], components[c1 * 2 + interlaceModifier]);
            }
        }
    }

    public double ComputeRougeLF1(in GenPop _reference, float _genCMFraction, ref double[,] _lcs, in int _popLen)
    {
        double similarity;
        for (int i = 0; i <= _popLen; i++)
        {
            for (int j = 0; j <= _popLen; j++)
            {
                if (i == 0 || j == 0)
                    _lcs[i, j] = 0;
                else if (_reference.components[i - 1].CompareComponent(components[j - 1], _genCMFraction, out similarity))
                    _lcs[i, j] = _lcs[i - 1, j - 1] + similarity;
                else
                    _lcs[i, j] = Math.Max(_lcs[i - 1, j], _lcs[i, j - 1]);
            }
        }

        double lcsLength = _lcs[_popLen, _popLen];
        double precision = lcsLength / (double)_popLen;
        double recall = lcsLength / (double)_popLen;

        return 2 * (precision * recall) / (precision + recall - 1e-10);
    }

    public String GetDisplayString()
    {
        StringBuilder sb = new StringBuilder();
        int compCount = components.Count;
        for (int i = 0; i < compCount; i++)
        {
            sb.AppendLine(components[i].GetDisplayString());
        }
        return sb.ToString();
    }
}

public class SpellCandidate
{
    public int genId;
    public string displayString;
    public GenPop spellChromosome;
    public bool isChosen;

    public SpellCandidate(int _genId, string _displayString, GenPop _spellChromosome)
    {
        genId = _genId;
        displayString = _displayString;
        spellChromosome = new GenPop(_spellChromosome.components);
        isChosen = false;
    }
}

public class GeneticComponentBag
{

    public static readonly GeneticSpellComponent[] triggerList = new GeneticSpellComponent[] { new GNoTrigger(), new GCollisionTrigger(), new GDestroyTrigger(), new GSpecialTrigger(), new GTimeTrigger()};
    private readonly int triggerLen;
    private List<GeneticSpellComponent> componentList;
    private int halfCompCount;
    private int fullCompCount;

    public GeneticComponentBag(List<GeneticSpellComponent> comps)
    {
        triggerLen = triggerList.Length;

        halfCompCount = comps.Count;
        fullCompCount = halfCompCount * 2;
        componentList = new List<GeneticSpellComponent>();
        for (int i = 0; i < halfCompCount; i++)
        {
            componentList.Add(comps[i].Clone());
            componentList[i].id = i + 1;
        }
    }

    public GenPop GenerateGenPop()
    {
        GenPop temp = new GenPop();
        for (int i = 0; i < fullCompCount; i++)
        {
            temp.components.Add(null);
        }
        for (int i = 0; i < halfCompCount; i++)
        {
            temp.components[i * 2] = componentList[i].Generate();
        }
        for (int k = 0; k < halfCompCount; k++)
        {
            int j = Helpers.Range(k, halfCompCount);
            (temp.components[j * 2], temp.components[k * 2]) = (temp.components[k * 2], temp.components[j * 2]);
        }
        for (int i = 0; i < halfCompCount; i++)
        {
            temp.components[i * 2 + 1] = triggerList[Helpers.Range(0, triggerLen)].Generate();
            temp.components[i * 2 + 1].id = halfCompCount + i + 1;
        }

        return temp;

    }

    public int GetSizeOfBag()
    {
        return fullCompCount;
    }

}

public class GeneticAlgorithm
{
    private int id;
    private List<GenPop> currentGen;
    private List<GenPop> nextGenParents;
    private List<SpellCandidate> currentSpellCandidates;
    private List<GenPop> savedCandidates;
    private double survivalPressure;
    private int maxPopSize;
    private int aftercutPopSize;
    private float mutationPercentage;
    private int mutationNo;
    private float popCutoffPercentage;
    private float nextGenParentPercentage;
    private int noParents;
    private int maxGenIndex;
    private int currentGenIndex;
    private int displayCanidiateInterval;
    private int maxCandidateNo;
    private double[,] lcs;
    private int popLen;
    private System.Random random;
    private bool isDone;

    public bool IsDone { get => isDone; set => isDone = value; }
    public int ID { get => id; private set => id = value; }

    public GeneticAlgorithm(in List<GeneticSpellComponent> _components, int _maxPopSize, int _maxGenIndex, int _candidateNumber, float _mutationPercentage, float _popCutoffPercentage, float _nextGenParentPercentage, int _displayCanidiateInterval, double _survivalPressure)
    {
        id = Helpers.Range(1, int.MaxValue);
        currentGen = new List<GenPop>();
        nextGenParents = new List<GenPop>();
        currentSpellCandidates = new List<SpellCandidate>();
        savedCandidates = new List<GenPop>();
        currentGenIndex = 0;
        maxPopSize = _maxPopSize;
        maxGenIndex = _maxGenIndex;
        currentGenIndex = 0;
        maxCandidateNo = _candidateNumber;
        mutationPercentage = _mutationPercentage;
        mutationNo = (int)MathF.Ceiling(maxPopSize * mutationPercentage);
        popCutoffPercentage = _popCutoffPercentage;
        aftercutPopSize = maxPopSize - (int)MathF.Floor(maxPopSize * popCutoffPercentage);
        nextGenParentPercentage = _nextGenParentPercentage;
        noParents = (int)MathF.Ceiling(maxPopSize * nextGenParentPercentage);
        survivalPressure = _survivalPressure;
        random = new System.Random();
        displayCanidiateInterval = _displayCanidiateInterval;
        isDone = false;

        //Add the trigger pattern thing
        GeneticComponentBag bag = new GeneticComponentBag(_components);
        popLen = bag.GetSizeOfBag();
        for (int i = 0; i < maxPopSize; i++)
        {
            currentGen.Add(bag.GenerateGenPop());
        }
        lcs = new double[popLen + 1, popLen + 1];
        //For testing purpoises
        //Debug.Log(currentGen[0].ComputeRougeLF1(currentGen[0], 0));
    }

    private int GetIndexOfMin(in List<double> list)
    {
        int minIndex = 0, count = list.Count;
        double minValue = double.MaxValue;
        for (int i = 0; i < count; i++)
        {
            if (list[i] < minValue)
            {
                minValue = list[i];
                minIndex = i;
            }
        }
        return minIndex;
    }

    public List<SpellCandidate> GetDisplayCandiadates(int _cutoffCandidateIndex, bool _getRandomFirstCandidate)
    {
        currentSpellCandidates.Clear();
        List<double> f1Sum = new List<double>();
        for (int i = 0; i < _cutoffCandidateIndex; i++)
        {
            f1Sum.Add(0);
        }
        int candidateCount = currentSpellCandidates.Count;
        int candidateIndex;
        if (_getRandomFirstCandidate)
        {
            candidateIndex = Helpers.Range(0, _cutoffCandidateIndex);
        }
        else
        {
            candidateIndex = 0;
        }
        AddCandidate(ref f1Sum, candidateIndex, _cutoffCandidateIndex, ref candidateCount);
        
        for (int i = 0; candidateCount < maxCandidateNo && i < 100;)
        {
            candidateIndex = GetIndexOfMin(f1Sum);
            if (IsRepeatingCandidate(candidateIndex))
            {
                f1Sum[candidateIndex] += 10000;
                i++;
            }
            else
            {
                AddCandidate(ref f1Sum, candidateIndex, _cutoffCandidateIndex, ref candidateCount);
            }
        }
        return currentSpellCandidates;

        bool IsRepeatingCandidate(int index)
        {
            int candCount = currentSpellCandidates.Count;
            for (int i = 0; i < candCount; i++)
            {
                if (currentSpellCandidates[i].genId == index)
                {
                    return true;
                }
            }
            return false;
        }

        void AddCandidate(ref List<double> f1Sum, int popIndex, in int genCount, ref int candidateCount)
        {
            double f1Score = 0;
            currentSpellCandidates.Add(new SpellCandidate(popIndex, currentGen[popIndex].GetDisplayString(), currentGen[popIndex]));
            for (int i = 0; i < genCount; i++)
            {
                if (i == popIndex)
                {
                    f1Score = 1;
                }
                else
                {
                    f1Score = currentGen[i].ComputeRougeLF1(currentGen[popIndex], currentGenIndex / maxGenIndex, ref lcs, popLen);
                }
                f1Sum[i] += f1Score;
            }
            candidateCount += 1;
        }
    }

    private void Crossover(in GenPop _parent1, in GenPop _parent2)
    {
        int p1Count = _parent1.components.Count, p2Count = _parent2.components.Count;
        int minCount = (p1Count < p2Count) ? p1Count : p2Count;
        int startCut = Helpers.Range(0, minCount - 1), cutLength = Helpers.Range(0, minCount - startCut);
        int ngenCount = currentGen.Count;
        currentGen.Add(new GenPop(_parent1.components));
        currentGen.Add(new GenPop(_parent2.components));
        for (int i = startCut; i <= startCut + cutLength; i++)
        {
            (currentGen[ngenCount].components[i], currentGen[ngenCount + 1].components[i]) = (currentGen[ngenCount + 1].components[i], currentGen[ngenCount].components[i]);
        }
        PMXFill(p1Count, startCut, cutLength, ngenCount, ngenCount + 1);
        PMXFill(p2Count, startCut, cutLength, ngenCount + 1, ngenCount);

        void PMXFill(in int _pCount, in int _startCut, in int _cutLength, in int _searcIndex, in int _replacementIndex)
        {
            int temp;
            for (int i = 0; i < _pCount; i++)
            {
                if (i >= _startCut && i <= _startCut + _cutLength)
                {
                    continue;
                }
                temp = GetCReplacementIndex(currentGen[_searcIndex].components, currentGen[_replacementIndex].components, _startCut, _cutLength, currentGen[_searcIndex].components[i].id);
                if (temp == -1)
                {
                    continue;
                }
                else
                {
                    currentGen[_searcIndex].components[i] = currentGen[_replacementIndex].components[temp];
                }
            }

            int GetCReplacementIndex(in List<GeneticSpellComponent> _searchTarget, in List<GeneticSpellComponent> _replacementTarget, in int _startCut, in int _cutLength, int _searchId)
            {
                int result = -1;
                for (int i = _startCut; i <= _startCut + _cutLength; i++)
                {
                    if (_searchTarget[i].id == _searchId)
                    {
                        result = GetCReplacementIndex(_searchTarget, _replacementTarget, _startCut, _cutLength, _replacementTarget[i].id);
                        if (result == -1)
                        {
                            return i;
                        }
                        else
                        {
                            return result;
                        }
                    }
                }
                return -1;
            }
        }
    }

    private void MutatePopulation()
    {
        int[] mutationIndexes = new int[mutationNo];
        for (int i = 0; i < mutationNo; i++)
        {
            mutationIndexes[i] = Helpers.Range(0, maxPopSize);
        }
        for (int i = 0; i < mutationNo; i++)
        {
            currentGen[i].Mutate(popLen, currentGenIndex / maxGenIndex);
        }
    }

    private void CalculateFitness()
    {
        int candCount = savedCandidates.Count;
        foreach (GenPop pop in currentGen)
        {
            pop.fitness = 0;
            for (int i = 0; i < candCount; i++)
            {
                pop.fitness += pop.ComputeRougeLF1(savedCandidates[i], currentGenIndex / maxGenIndex, ref lcs, popLen);
            }
        }
    }

    private static int CompareFitness(GenPop y, GenPop x)
    {
        if (x == null)
        {
            if (y == null)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }
        else
        {
            if (y == null)
            {
                return 1;
            }
            else
            {
                return x.fitness.CompareTo(y.fitness);
            }
        }
    }

    private void CalculateRanking()
    {
        currentGen.Sort(CompareFitness);
        for (int i = 0; i < aftercutPopSize; i++)
        {
            currentGen[i].rank = (survivalPressure - (2 * survivalPressure - 2) * ((double)i / (aftercutPopSize - 1))) / aftercutPopSize;
        }
    }

    private void GenerateNextGeneration()
    {
        nextGenParents.Clear();

        CalculateFitness();
        CalculateRanking();

        double randomValue;
        double cumulativeProbability;
        for (int i = 0; i < noParents; i++)
        {
            randomValue = Helpers.random.NextDouble();
            cumulativeProbability = 0.0;

            foreach (GenPop pop in currentGen)
            {
                cumulativeProbability += pop.rank;
                if (randomValue <= cumulativeProbability)
                {
                    nextGenParents.Add(pop);
                    break;
                }
            }
        }
        currentGen.Clear();

        int genCount = 0;
        foreach (GenPop pop in savedCandidates)
        {
            nextGenParents.Add(new GenPop(pop.components));
            currentGen.Add(new GenPop(pop.components));
            genCount++;
        }

        int parentCount = nextGenParents.Count;
        while (genCount < maxPopSize)
        {
            Crossover(nextGenParents[Helpers.Range(0, parentCount)], nextGenParents[Helpers.Range(0, parentCount)]);
            genCount += 2;
        }
        while (genCount > maxPopSize)
        {
            currentGen.RemoveAt(genCount - 1);
            genCount--;
        }

        MutatePopulation();
    }

    public List<SpellCandidate> NextIterations(List<SpellCandidate> _candidates)
    {
        savedCandidates.Clear();
        foreach (SpellCandidate c in _candidates)
        {
            if (c.isChosen)
            {
                savedCandidates.Add(new GenPop(c.spellChromosome.components));
            }
        }
        for (int i = 0; i < displayCanidiateInterval && currentGenIndex < maxGenIndex; i++, currentGenIndex++)
        {
            GenerateNextGeneration();
        }
        isDone = (currentGenIndex == maxGenIndex);

        CalculateFitness();
        currentGen.Sort(CompareFitness);
        int candidateSearchSize;
        if(isDone)
        {
            candidateSearchSize = maxPopSize;
        }
        else 
        {
            candidateSearchSize = (int)MathF.Ceiling(MathF.Max((1 - ((float)currentGenIndex / maxGenIndex) / 3) * maxPopSize, maxCandidateNo));
        }
        return GetDisplayCandiadates(candidateSearchSize, false);
    }

    public List<SpellCandidate> GetFirstRoundOfCandidates()
    {
        return GetDisplayCandiadates(maxPopSize, true);
    }

    public List<SpellCandidate> ForceFinish()
    {
        isDone = true;
        return GetDisplayCandiadates(maxPopSize, false);
    }

}
