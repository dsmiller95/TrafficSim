using Assets.Scripts.ScriptableBuilder.ScriptLinking;
using UnityEngine;


namespace Assets.Scripts.ScriptableBuilder.SeriesScriptable.CarActions
{
    public interface ICarAction : IScriptableEntry
    {
        CarActionTypes GetActionType();
    }

    public enum CarActionTypes
    {
        SetVelocity,
        Noop,
        Start,
        SetAcceleration,
        End
    }
}