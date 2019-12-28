using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;

public class ScriptableObjectContainerController : MonoBehaviour
{
    //This is only for debug purposes: when the cars are being spawned this should not be used
    public GameObject initialScriptHolder;

    public void AttachCarBehaviorFromScriptField(GameObject target)
    {
        var scriptable = target.AddComponent<ScriptConfiguredCarBehavior>();

        var allActions = new List<ICarAction>(this.GetComponentsInChildren<ICarAction>());
        Debug.Log($"actions: {allActions.Count()}");
        var startTriggers = allActions.Where(x => x.GetActionType() == CarActionTypes.Start);
        Debug.Log($"startTrigs: {startTriggers.Count()}");

        scriptable.RegisterStartTriggers(startTriggers);
    }

    // Use this for initialization
    void Start()
    {

    }

    private bool hasBeenTriggered = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!hasBeenTriggered)
            {
                Debug.Log("TRIggErEd");
                this.hasBeenTriggered = true;
                this.AttachCarBehaviorFromScriptField(this.initialScriptHolder);
            }
        }
    }
}
