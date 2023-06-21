using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellHistoryNode
{
    public IMagicObjectDirector target;
    public SpellHistoryNode prev;

    public SpellHistoryNode(IMagicObjectDirector _target, SpellHistoryNode _prev)
    {
        target = _target;
        prev = _prev;
        if(target.GetType() != typeof(MagicPlaceholderDirector))
        {
            target.OnObjectDestroy += ReplaceWithPlaceholder;
        }
    }

    public void ReplaceWithPlaceholder(IMagicObjectDirector _director)
    {
        target.OnObjectDestroy -= ReplaceWithPlaceholder;
        target = _director;
    }

    public static SpellHistoryNode AddNode(IMagicObjectDirector _target, SpellHistoryNode _prev)
    {
        if(_target == null)
        {
            return _prev;
        }
        return new SpellHistoryNode(_target, _prev);
    }

}

public class SpellCastData
{
    public IMagicObjectDirector owner;
    public MagicSpell spell;
    public ElementData element;
    //Use it store damage multipliers for explosions

    public SpellCastData(IMagicObjectDirector _owner, MagicSpell _spell, ElementData _element)
    {
        owner = _owner;
        spell = _spell;
        element = _element;
    }
}
