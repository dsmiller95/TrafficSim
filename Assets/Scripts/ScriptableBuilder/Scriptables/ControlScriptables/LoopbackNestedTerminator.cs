using UnityEngine;
using System.Collections;
using Assets.Scripts.ScriptableBuilder.ScriptLinking;

public class LoopbackNestedTerminator : ScriptableNestedBlockTerminator
{
    public override IScriptableEntry Execute(ICarActionable reciever)
    {
        return this.pairedNestedParent;
    }
}
