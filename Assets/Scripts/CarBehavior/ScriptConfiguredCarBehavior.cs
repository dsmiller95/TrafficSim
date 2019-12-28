using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.ScriptableBuilder.SeriesScriptable.CarActions;
using Assets.Scripts.ScriptableBuilder.ScriptLinking;

public class ScriptConfiguredCarBehavior: MonoBehaviour, ICarBehavior
{
    private ISet<IScriptableEntry> currentPointers;

    public void Start()
    {
        //this.scriptableContainerComponent.GetComponent<ScriptableObjectContainerController>().AttachCarBehaviorFromScriptField(this.gameObject);
    }

    public void RegisterStartTriggers(IEnumerable<IScriptableEntry> triggers)
    {
        this.currentPointers = new HashSet<IScriptableEntry>(triggers);
        Debug.Log($"Registered {currentPointers.Count} pointers");
    }
    
    public void ExecuteBehavior(ICarActionable target)
    {
        if (this.currentPointers == null || this.currentPointers.Count <= 0)
        {
            return;
        }
        Debug.Log($"Executing {currentPointers.Count} pointers");
        var nextPointers = new HashSet<IScriptableEntry>();
        foreach (var pointer in this.currentPointers)
        {
            if (pointer != null)
            {
                var mono = pointer as MonoBehaviour;
                Debug.Log($"Executing {mono.name} pointer");
                nextPointers.Add(pointer.Execute(target));
            }
        }
        this.currentPointers = nextPointers;
    }
}