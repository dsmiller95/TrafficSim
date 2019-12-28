using UnityEngine;
using System.Collections;

/// <summary>
/// Any object/command which takes up space in the execution chain, and has potential to trigger another command
/// </summary>
public interface ILinkedScriptingObject
{
    ILinkedScriptingObject GetParent();
    ILinkedScriptingObject GetChild();
}
