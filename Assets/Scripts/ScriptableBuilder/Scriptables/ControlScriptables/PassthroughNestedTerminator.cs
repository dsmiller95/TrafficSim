﻿using UnityEngine;
using System.Collections;
using Assets.Scripts.ScriptableBuilder.ScriptLinking;

public class PassthroughNestedTerminator : ScriptableNestedBlockTerminator
{
    public override IScriptableEntry Execute(ICarActionable reciever)
    {
        return this.child;
    }
}
