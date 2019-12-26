using UnityEngine;

public interface ICarFloatAction : ICarAction
{
    void Execute(float acceleration);
}