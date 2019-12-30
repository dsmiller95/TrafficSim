using UnityEngine;
using System.Collections;
using Assets.Scripts.ScriptableBuilder.ScriptLinking;

public class PassthroughNestedTerminator : ScriptableNestedBlockTerminator
{
    public PassthroughNestedTerminator()
    {
        Debug.Log("instantiating instance of Passthrough");
    }

    public override IScriptableEntry Execute(ICarActionable reciever)
    {
        Debug.Log($"passthrough behavior executing. Next child: {this.child}");
        return this.child;
    }
}
