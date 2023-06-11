using System;

[Serializable]
public class VariableReference<T>
{
    public bool UseConstant = true;
    public T ConstantValue;
    public Variable<T> Variable;

    public VariableReference()
    { }

    public VariableReference(T _value)
    {
        UseConstant = true;
        ConstantValue = _value;
    }

    public T Value
    {
        get { return UseConstant ? ConstantValue : Variable.Value; }
    }

    public static implicit operator T(VariableReference<T> reference)
    {
        return reference.Value;
    }
}

[Serializable]
public class FloatReference : VariableReference<float>
{ }
