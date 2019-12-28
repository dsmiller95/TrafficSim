using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface ICarBehavior
{
    void ExecuteBehavior(ICarActionable target);
}