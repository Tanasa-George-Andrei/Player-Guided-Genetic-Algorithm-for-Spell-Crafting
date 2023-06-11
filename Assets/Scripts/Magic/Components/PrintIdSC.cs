using UnityEngine;

public class APrintId : ActiveSpellComponent
{
    public string text;

    public APrintId(string _text)
    {
        text = _text;
    }

    public APrintId(OriginSpellComponent _origin, SpellHistoryNode _history, SpellCastData _castData, string _text) : base(_origin, _history, _castData)
    {
        text = _text;
    }

    public override void Execue()
    {
        UnityEngine.Debug.Log(text.Replace("%id",history.target.GetName()));
        state = ActiveSpellStates.Finished;
    }

    public override ActiveSpellComponent Clone(OriginSpellComponent _origin, SpellHistoryNode _history, SpellCastData _castData)
    {
        return new APrintId(_origin, _history, _castData,text);
    }

    public override void Reset(OriginSpellComponent _origin, SpellHistoryNode _history, SpellCastData _castData, ActiveSpellComponent _active)
    {
        origin = _origin;
        history = _history;
        castData = _castData;
        state = ActiveSpellStates.Started;
        APrintId temp = (APrintId)_active;
        text = temp.text;
    }

    public override OriginSpellComponent GenerateOriginComponent()
    {
        return new OPrintId(this);
    }

    public override string ToString()
    {
        return "PrintId";
    }

}

public class OPrintId : OriginSpellComponent
{
    public OPrintId(ActiveSpellComponent _active) : base(_active, 25) {}

    public override bool isOfRightType(ActiveSpellComponent _active)
    {
        return _active.GetType() == typeof(APrintId);
    }
}
