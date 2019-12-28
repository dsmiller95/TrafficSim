using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScriptConfiguredCarBehavior: MonoBehaviour, ICarBehavior
{
    private ISet<ICarAction> currentPointers;

    public void Start()
    {
        //this.scriptableContainerComponent.GetComponent<ScriptableObjectContainerController>().AttachCarBehaviorFromScriptField(this.gameObject);
    }

    public void RegisterStartTriggers(IEnumerable<ICarAction> triggers)
    {
        this.currentPointers = new HashSet<ICarAction>(triggers);
        Debug.Log($"Registered {currentPointers.Count} pointers");
    }
    
    public void ExecuteBehavior(ICarActionable target)
    {
        if (this.currentPointers == null || this.currentPointers.Count <= 0)
        {
            return;
        }
        Debug.Log($"Executing {currentPointers.Count} pointers");
        var nextPointers = new HashSet<ICarAction>();
        foreach (var pointer in this.currentPointers)
        {
            if (pointer != null)
            {
                var mono = pointer as MonoBehaviour;
                Debug.Log($"Executing {mono.name} pointer");
                nextPointers.Add(pointer.Execute(target) as ICarAction);
            }
        }
        this.currentPointers = nextPointers;
    }
}