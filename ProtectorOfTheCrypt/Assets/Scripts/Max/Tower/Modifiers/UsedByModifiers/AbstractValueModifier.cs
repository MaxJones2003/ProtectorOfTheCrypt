using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public abstract class AbstractValueModifier<T> : IModifier
{
    public string AttributeName;
    public T Amount;
    public abstract void Apply(TowerScriptableObject Tower);

    protected FieldType GetAttribute<FieldType>(TowerScriptableObject Tower,
    out object TargetObject, 
    out FieldInfo Field)
    {
        string[] paths = AttributeName.Split("/");
        string attribute = paths[paths.Length - 1];

        Type type = Tower.GetType();
        object target = Tower;

        for (int i = 0; i < paths.Length - 1; i++)
        {
            FieldInfo field = type.GetField(paths[i]);
            if(field == null)
            {
                UnityEngine.Debug.LogError($"Unable to apply modifier to attribute {AttributeName} becuase its file name was not found");
                throw new InvalidPathSpecifiedException(AttributeName);
            }
            else
            {
                target = field.GetValue(target);
                type = target.GetType();
            }
        }

        FieldInfo attributeField =  type.GetField(attribute);
        if(attributeField == null)
        {
            UnityEngine.Debug.LogError($"Unable to apply modifier to attribute {AttributeName} becuase its file name was not found");
            throw new InvalidPathSpecifiedException(AttributeName);
        }

        Field = attributeField;
        TargetObject = target;
        return (FieldType)attributeField.GetValue(target);
    }
}
