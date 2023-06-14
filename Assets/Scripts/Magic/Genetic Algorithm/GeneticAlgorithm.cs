using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Unity.VisualScripting;

public class GTeleport : GeneticSpellComponent
{
    private float distance;
    private const float lower = 4;
    private const float upper = 50;

    public GTeleport()
    {
    }

    public GTeleport(float _distance, byte _id) : base(_id)
    {
        distance = _distance;
    }

    public float GetDistance()
    {
        return distance;
    }

    public override bool CompareComponent(in GeneticSpellComponent _other, in float genCMFraction, out double similarity)
    {
        if (_other.GetType() == typeof(GTeleport))
        {
            GTeleport temp = (GTeleport)_other;
            similarity = DifFunc(MathF.Abs(GetDistance() - temp.GetDistance()) / (upper - lower));
            return true;
        }
        else if (_other.GetType() == typeof(GDash))
        {
            GDash temp = (GDash)_other;
            similarity = 0.9f * DropoffFunc(genCMFraction) * DifFunc(MathF.Abs(GetDistance() - temp.GetDistance()) / (upper - lower));
            return true;
        }
        else if (_other.GetType() == typeof(GPropel))
        {
            GPropel temp = (GPropel)_other;
            similarity = 0.9f * DropoffFunc(genCMFraction) * DifFunc(MathF.Abs(GetDistance() - temp.GetDistance()) / (upper - lower));
            return true;
        }
        similarity = 0;
        return false;
    }

    public override GeneticSpellComponent Generate()
    {
        return new GTeleport(Helpers.Range(lower, upper), id);
    }

    public override string GetDisplayString()
    {
        return "Teleport for" + distance;
    }

    public override void Mutation(in float genCMFraction)
    {
        float range = (upper - lower) * 0.5f * (float)DropoffFunc(genCMFraction);
        distance = Helpers.Range(MathF.Max(lower, distance - range / 2), MathF.Min(upper, distance + range / 2));
    }

    public override GeneticSpellComponent Clone()
    {
        return new GTeleport(distance, id);
    }

    public override OriginSpellComponent GenerateOrigin(ElementData _element)
    {
        throw new NotImplementedException();
    }
}

public class GDash : GeneticSpellComponent
{
    private float distance;
    private const float lower = 4;
    private const float upper = 50;

    public GDash()
    {
    }
    public GDash(float _distance, byte _id) : base(_id)
    {
        distance = _distance;
    }

    public float GetDistance()
    {
        return distance;
    }

    public override bool CompareComponent(in GeneticSpellComponent _other, in float genCMFraction, out double similarity)
    {
        if (_other.GetType() == typeof(GDash))
        {
            GDash temp = (GDash)_other;
            similarity = DifFunc(MathF.Abs(GetDistance() - temp.GetDistance()) / (upper - lower));
            return true;
        }
        else if (_other.GetType() == typeof(GTeleport))
        {
            GTeleport temp = (GTeleport)_other;
            similarity = 0.9f * DropoffFunc(genCMFraction) * DifFunc(MathF.Abs(GetDistance() - temp.GetDistance()) / (upper - lower));
            return true;
        }
        else if (_other.GetType() == typeof(GPropel))
        {
            GPropel temp = (GPropel)_other;
            similarity = DropoffFunc(genCMFraction) * DifFunc(MathF.Abs(GetDistance() - temp.GetDistance()) / (upper - lower));
            return true;
        }
        similarity = 0;
        return false;
    }

    public override GeneticSpellComponent Generate()
    {
        return new GDash(Helpers.Range(lower, upper), id);
    }

    public override string GetDisplayString()
    {
        return "Dash for" + distance;
    }

    public override void Mutation(in float genCMFraction)
    {
        float range = (upper - lower) * 0.5f * (float)DropoffFunc(genCMFraction);
        distance = Helpers.Range(MathF.Max(lower, distance - range / 2), MathF.Min(upper, distance + range / 2));
    }

    public override GeneticSpellComponent Clone()
    {
        return new GDash(distance, id);
    }

    public override OriginSpellComponent GenerateOrigin(ElementData _element)
    {
        throw new NotImplementedException();
    }
}

public class GPropel : GeneticSpellComponent
{
    private float speed;
    private const float time = 1f;
    private const float lower = 4;
    private const float upper = 50;

    public GPropel()
    {
    }
    public GPropel(float _speed, byte _id) : base(_id)
    {
        speed = _speed;
    }

    public float GetDistance()
    {
        return speed * time;
    }


    public override bool CompareComponent(in GeneticSpellComponent _other, in float genCMFraction, out double similarity)
    {
        if (_other.GetType() == typeof(GPropel))
        {
            GPropel temp = (GPropel)_other;
            similarity = DifFunc(MathF.Abs(GetDistance() - temp.GetDistance()) / (upper - lower));
            return true;
        }
        else if (_other.GetType() == typeof(GDash))
        {
            GDash temp = (GDash)_other;
            similarity = DropoffFunc(genCMFraction) * DifFunc(MathF.Abs(GetDistance() - temp.GetDistance()) / (upper - lower));
            return true;
        }
        else if (_other.GetType() == typeof(GTeleport))
        {
            GTeleport temp = (GTeleport)_other;
            similarity = 0.9f * DropoffFunc(genCMFraction) * DifFunc(MathF.Abs(GetDistance() - temp.GetDistance()) / (upper - lower));
            return true;
        }
        similarity = 0;
        return false;
    }

    public override GeneticSpellComponent Generate()
    {
        return new GPropel(Helpers.Range(lower / time, upper / time), id);
    }

    public override string GetDisplayString()
    {
        return "Propel with speed: " + speed + "m/s";
    }

    public override void Mutation(in float genCMFraction)
    {
        float range = (upper - lower) * 0.5f * (float)DropoffFunc(genCMFraction);
        speed = Helpers.Range(MathF.Max(lower / time, speed / time - range / 2), MathF.Min(upper / time, speed / time + range / 2));
    }

    public override GeneticSpellComponent Clone()
    {
        return new GPropel(speed, id);
    }

    public override OriginSpellComponent GenerateOrigin(ElementData _element)
    {
        throw new NotImplementedException();
    }
}

public class GCreateMProjectile : GeneticSpellComponent
{
    private int noProjectiles;
    private const int lower = 1;
    private const int upper = 5;

    public GCreateMProjectile()
    {
    }
    public GCreateMProjectile(int _noProjectiles, byte _id) : base(_id)
    {
        noProjectiles = _noProjectiles;
    }

    public int GetNoProjectiles()
    {
        return noProjectiles;
    }

    public override bool CompareComponent(in GeneticSpellComponent _other, in float genCMFraction, out double similarity)
    {
        if (_other.GetType() == typeof(GCreateMProjectile))
        {
            GCreateMProjectile temp = (GCreateMProjectile)_other;
            similarity = DifFunc(MathF.Abs(GetNoProjectiles() - temp.GetNoProjectiles()) / (float)(upper - lower));
            return true;
        }
        similarity = 0;
        return false;
    }

    public override GeneticSpellComponent Generate()
    {
        return new GCreateMProjectile(Helpers.Range(lower, upper), id);
    }

    public override string GetDisplayString()
    {
        return "Create Magic Projectile " + noProjectiles;
    }

    public override void Mutation(in float genCMFraction)
    {
        float range = MathF.Ceiling((upper - lower) * 0.5f * (1 - genCMFraction));
        noProjectiles = (int)MathF.Ceiling(Helpers.Range(MathF.Max(lower, noProjectiles - range / 2), MathF.Min(upper, noProjectiles + range / 2)));
    }

    public override GeneticSpellComponent Clone()
    {
        return new GCreateMProjectile(noProjectiles, id);
    }

    public override OriginSpellComponent GenerateOrigin(ElementData _element)
    {
        throw new NotImplementedException();
    }
}

public class GExplode : GeneticSpellComponent
{
    private float radius;
    private const float lower = 2;
    private const float upper = 15;

    public GExplode()
    {
    }
    public GExplode(float _radius, byte _id) : base(_id)
    {
        radius = _radius;
    }

    public float GetExplosionRadius()
    {
        return radius;
    }
    public override bool CompareComponent(in GeneticSpellComponent _other, in float genCMFraction, out double similarity)
    {
        if (_other.GetType() == typeof(GExplode))
        {
            GExplode temp = (GExplode)_other;
            similarity = DifFunc(MathF.Abs(GetExplosionRadius() - temp.GetExplosionRadius()) / (upper - lower));
            return true;
        }
        similarity = 0;
        return false;
    }

    public override GeneticSpellComponent Generate()
    {
        return new GExplode(Helpers.Range(lower, upper), id);
    }

    public override string GetDisplayString()
    {
        return "Explode " + radius;
    }

    public override void Mutation(in float genCMFraction)
    {
        float range = (upper - lower) * 0.5f * (float)DropoffFunc(genCMFraction);
        radius = Helpers.Range(MathF.Max(lower, radius - range / 2), MathF.Min(upper, radius + range / 2));
    }

    public override GeneticSpellComponent Clone()
    {
        return new GExplode(radius, id);
    }

    public override OriginSpellComponent GenerateOrigin(ElementData _element)
    {
        throw new NotImplementedException();
    }
}

public class GBounces : GeneticSpellComponent
{
    private int noBounces;
    private const int lower = 1;
    private const int upper = 5;

    public GBounces()
    {
    }
    public GBounces(int _noBounces, byte _id) : base(_id)
    {
        noBounces = _noBounces;
    }

    public float GetNoBounces()
    {
        return noBounces;
    }
    public override bool CompareComponent(in GeneticSpellComponent _other, in float genCMFraction, out double similarity)
    {
        if (_other.GetType() == typeof(GBounces))
        {
            GBounces temp = (GBounces)_other;
            similarity = DifFunc(MathF.Abs(GetNoBounces() - temp.GetNoBounces()) / (float)(upper - lower));
            return true;
        }
        similarity = 0;
        return false;
    }

    public override GeneticSpellComponent Generate()
    {
        return new GBounces(Helpers.Range(lower, upper), id);
    }

    public override string GetDisplayString()
    {
        return "Bounces for " + noBounces;
    }

    public override void Mutation(in float genCMFraction)
    {
        float range = MathF.Ceiling((upper - lower) * 0.5f * (1 - genCMFraction));
        noBounces = (int)MathF.Ceiling(Helpers.Range(MathF.Max(lower, noBounces - range / 2), MathF.Min(upper, noBounces + range / 2)));
    }

    public override GeneticSpellComponent Clone()
    {
        return new GBounces(noBounces, id);
    }

    public override OriginSpellComponent GenerateOrigin(ElementData _element)
    {
        throw new NotImplementedException();
    }
}

public class GFork : GeneticSpellComponent
{
    private int noForks;
    private const int lower = 1;
    private const int upper = 5;

    public GFork()
    {
    }
    public GFork(int _noForks, byte _id) : base(_id)
    {
        noForks = _noForks;
    }

    public float GetNoForks()
    {
        return noForks;
    }
    public override bool CompareComponent(in GeneticSpellComponent _other, in float genCMFraction, out double similarity)
    {
        if (_other.GetType() == typeof(GFork))
        {
            GFork temp = (GFork)_other;
            similarity = DifFunc(MathF.Abs(GetNoForks() - temp.GetNoForks()) / (float)(upper - lower));
            return true;
        }
        similarity = 0;
        return false;
    }

    public override GeneticSpellComponent Generate()
    {
        return new GFork(Helpers.Range(lower, upper), id);
    }

    public override string GetDisplayString()
    {
        return "Fork " + noForks;
    }

    public override void Mutation(in float genCMFraction)
    {
        float range = MathF.Ceiling((upper - lower) * 0.5f * (1 - genCMFraction));
        noForks = (int)MathF.Ceiling(Helpers.Range(MathF.Max(lower, noForks - range / 2), MathF.Min(upper, noForks + range / 2)));
    }

    public override GeneticSpellComponent Clone()
    {
        return new GFork(noForks, id);
    }

    public override OriginSpellComponent GenerateOrigin(ElementData _element)
    {
        throw new NotImplementedException();
    }
}

public class GPierce : GeneticSpellComponent
{
    private int noPierce;
    private const int lower = 1;
    private const int upper = 5;

    public GPierce()
    {
    }
    public GPierce(int _noPierce, byte _id) : base(_id)
    {
        noPierce = _noPierce;
    }

    public float GetNoPierce()
    {
        return noPierce;
    }
    public override bool CompareComponent(in GeneticSpellComponent _other, in float genCMFraction, out double similarity)
    {
        if (_other.GetType() == typeof(GPierce))
        {
            GPierce temp = (GPierce)_other;
            similarity = DifFunc(MathF.Abs(GetNoPierce() - temp.GetNoPierce()) / (float)(upper - lower));
            return true;
        }
        similarity = 0;
        return false;
    }

    public override GeneticSpellComponent Generate()
    {
        return new GPierce(Helpers.Range(lower, upper), id);
    }

    public override string GetDisplayString()
    {
        return "Pierce " + noPierce;
    }

    public override void Mutation(in float genCMFraction)
    {
        float range = MathF.Ceiling((upper - lower) * 0.5f * (1 - genCMFraction));
        noPierce = (int)MathF.Ceiling(Helpers.Range(MathF.Max(lower, noPierce - range / 2), MathF.Min(upper, noPierce + range / 2)));
    }

    public override GeneticSpellComponent Clone()
    {
        return new GPierce(noPierce, id);
    }

    public override OriginSpellComponent GenerateOrigin(ElementData _element)
    {
        throw new NotImplementedException();
    }
}

public class GSticky : GeneticSpellComponent
{
    public GSticky()
    {
    }
    public GSticky(byte _id) : base(_id)
    {
    }

    public override GeneticSpellComponent Clone()
    {
        return new GSticky(id);
    }

    public override bool CompareComponent(in GeneticSpellComponent _other, in float genCMFraction, out double similarity)
    {
        if (_other.GetType() == typeof(GSticky))
        {
            similarity = 1;
            return true;
        }
        similarity = 0;
        return false;
    }

    public override GeneticSpellComponent Generate()
    {
        return new GSticky(id);
    }

    public override OriginSpellComponent GenerateOrigin(ElementData _element)
    {
        throw new NotImplementedException();
    }

    public override string GetDisplayString()
    {
        return "Sticky";
    }
}

public class GRightClickTrigger : GeneticSpellComponent
{
    public GRightClickTrigger()
    {
    }

    public GRightClickTrigger(byte _id) : base(_id)
    {
    }

    public override GeneticSpellComponent Clone()
    {
        return new GRightClickTrigger(id);
    }

    public override bool CompareComponent(in GeneticSpellComponent _other, in float genCMFraction, out double similarity)
    {
        if (_other.GetType() == typeof(GRightClickTrigger))
        {
            similarity = 1;
            return true;
        }
        similarity = 0;
        return false;
    }

    public override GeneticSpellComponent Generate()
    {
        return new GRightClickTrigger(id);
    }

    public override OriginSpellComponent GenerateOrigin(ElementData _element)
    {
        throw new NotImplementedException();
    }

    public override string GetDisplayString()
    {
        return "Right Click Trigger";
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
        if(p < 30)
        {
            foreach (GeneticSpellComponent comp in components)
            {
                comp.Mutation(genCMFraction);
            }
        }
        else
        {
            int halfCompSize = components.Count / 2;
            int noPermutations = Helpers.Range(0, halfCompSize + 1);
            int interlaceModifier;
            int c1,c2;
            for (int i = 0; i < noPermutations; i++)
            {
                c1 = Helpers.Range(0, halfCompSize);
                c2 = Helpers.Range(0, halfCompSize);
                for (int j = 0;c1 == c2 && j < 10; j++)
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
    public List<double> f1;
    public bool isChosen;

    public SpellCandidate(int _genId, string _displayString)
    {
        genId = _genId;
        displayString = _displayString;
        f1 = new List<double>();
        isChosen = false;
    }
}

public class GNoTrigger : GeneticSpellComponent
{
    public GNoTrigger()
    {
    }

    public GNoTrigger(byte _id) : base(_id)
    {
    }

    public override GeneticSpellComponent Clone()
    {
        return new GRightClickTrigger(id);
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
        throw new NotImplementedException();
    }

    public override string GetDisplayString()
    {
        return "No Trigger";
    }
}

public class GeneticComponentBag
{

    private readonly GeneticSpellComponent[] triggerList = new GeneticSpellComponent[] { new GRightClickTrigger(), new GNoTrigger() };
    private readonly int triggerLen;
    private List<GeneticSpellComponent> componentList;
    private int compEndIndex;
    private int genCompLen;

    public GeneticComponentBag(List<GeneticSpellComponent> comps)
    {
        triggerLen = triggerList.Length;

        compEndIndex = comps.Count;
        genCompLen = compEndIndex * 2;
        componentList = new List<GeneticSpellComponent>();
        for (int i = 0; i < compEndIndex; i++)
        {
            componentList.Add(comps[i].Clone());
            componentList[i].id = (byte)i;
        }
    }

    public GenPop GenerateGenPop()
    {
        GenPop temp = new GenPop();
        for (int i = 0; i < genCompLen; i++)
        {
            temp.components.Add(null);
        }
        for (int i = 0; i < compEndIndex; i++)
        {
            temp.components[i * 2] = componentList[i].Generate();
        }
        for (int k = 0; k < compEndIndex; k++)
        {
            int j = Helpers.Range(k, compEndIndex);
            (temp.components[j * 2], temp.components[k * 2]) = (temp.components[k * 2], temp.components[j * 2]);
        }
        for (int i = 0; i < compEndIndex; i++)
        {
            temp.components[i * 2 + 1] = triggerList[Helpers.Range(0, triggerLen)].Generate();
            temp.components[i * 2 + 1].id = (byte)(compEndIndex + i);
        }

        return temp;

    }

    public int GetSizeOfBag()
    {
        return genCompLen;
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
    private float displayGenPercentage;
    private int displayGenIndex;
    private int maxCandidateNo;
    private double[,] lcs;
    private int popLen;
    private System.Random random;
    private bool isDone;

    public bool IsDone { get => isDone; set => isDone = value; }
    public int ID { get => id; private set => id = value; }

    public GeneticAlgorithm(in List<GeneticSpellComponent> _components, int _maxPopSize, int _maxGenIndex, int _candidateNumber, float _mutationPercentage, float _popCutoffPercentage, float _nextGenParentPercentage, float _displayGenPercentage, double _survivalPressure)
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
        displayGenPercentage = _displayGenPercentage;
        displayGenIndex = (int)MathF.Floor(maxGenIndex * displayGenPercentage);
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
            currentSpellCandidates.Add(new SpellCandidate(popIndex, currentGen[popIndex].GetDisplayString()));
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
                currentSpellCandidates[candidateCount].f1.Add(f1Score);
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
                savedCandidates.Add(new GenPop(currentGen[c.genId].components));
            }
        }
        for (int i = 0; i < displayGenIndex && currentGenIndex < maxGenIndex; i++, currentGenIndex++)
        {
            UnityEngine.Debug.Log(currentGenIndex);
            GenerateNextGeneration();
        }
        isDone = (currentGenIndex == maxGenIndex);

        CalculateFitness();
        currentGen.Sort(CompareFitness);
        return GetDisplayCandiadates((int)MathF.Ceiling((1 -  ((float)currentGenIndex / maxGenIndex) / 2) * maxPopSize), false);
    }

    public MagicSpell GetResult(SpellCandidate candidate)
    {
        throw new NotImplementedException();
    }

    public List<SpellCandidate> GetFirstRoundOfCandidates()
    {
        return GetDisplayCandiadates(maxPopSize, true);
    }
}
