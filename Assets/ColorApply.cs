using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public interface IColorApply
{
    public void Apply(List<Color> colors);
}

[Serializable]
public class ColorApplyObject
{ 
    public MonoBehaviour objectMonoRef;
    public List<Color> colors = new List<Color>();

    private IColorApply _colorApply = null;
    
    public void Apply()
    {
        if (_colorApply is null)
        {
            IColorApply temp = (IColorApply)objectMonoRef;
            if (temp is null)
            {
                ConsoleLogger.Log("No IColorApply found");
                return;
            }

            _colorApply = temp;
        }
        
        _colorApply.Apply(colors);
    }
}

public class ColorApply : MonoBehaviour
{
    [SerializeField] private List<ColorApplyObject> colorApplyObjects = new List<ColorApplyObject>();
    
    private void Start()
    {
        ApplyAll();
    }

    public void ApplyAll()
    {
        foreach (ColorApplyObject cao in colorApplyObjects)
        {
            cao.Apply();
        }
    }
}
