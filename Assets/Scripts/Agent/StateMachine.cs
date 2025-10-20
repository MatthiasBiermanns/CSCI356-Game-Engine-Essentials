using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public IAgentState Current { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeState(IAgentState newState)
    {
        if (Current != null)
            Current.OnExit();
        Current = newState;
        if (Current != null)
            Current.OnEnter();
    }

    public void Tick()
    {
        if (Current != null)
            Current.Tick();
    }
}

public interface IAgentState
{
    void OnEnter();
    void OnExit();
    void Tick();
}