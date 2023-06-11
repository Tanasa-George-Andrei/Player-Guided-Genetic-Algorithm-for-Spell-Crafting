using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellBuilder
{
    private List<OriginSpellComponent> originSpellComponents = new List<OriginSpellComponent>();
    private ElementData element;

    public SpellBuilder(ElementData _element)
    {
        element = _element;
    }

    public void ChangeElement(ElementData _element)
    {
        element = _element;
    }

    public void Append(ActiveSpellComponent _component)
    {
        originSpellComponents.Add(_component.GenerateOriginComponent());
    }

    public void Append(GeneticSpellComponent _component)
    {
        OriginSpellComponent temp = _component.GenerateOrigin(element);
        if(temp != null)
        {
            originSpellComponents.Add(temp);
        }
    }

    public MagicSpell Build()
    {
        int count = originSpellComponents.Count;
        for (int i = 0; i < count; i++)
        {
            originSpellComponents[i].Index = i;
        }
        return new MagicSpell(originSpellComponents, element);
    }

    public static MagicSpell GenerateSpell(ElementData _element, List<ActiveSpellComponent> _components)
    {
        SpellBuilder sb = new SpellBuilder(_element);
        foreach (ActiveSpellComponent _component in _components)
        {
            sb.Append(_component);
        }
        return sb.Build();
    }
    public static MagicSpell GenerateSpell(ElementData _element, List<GeneticSpellComponent> _components)
    {
        SpellBuilder sb = new SpellBuilder(_element);
        foreach (GeneticSpellComponent _component in _components)
        {
            sb.Append(_component);
        }
        return sb.Build();
    }
}
