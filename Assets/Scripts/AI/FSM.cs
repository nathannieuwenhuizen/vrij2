using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void ILiveStateDelegate(IState _state);

public interface IState
{
    ILiveStateDelegate OnStateSwitch { get; set; }
    void Start();
    void Run();
    void Exit();

}
public class FSM : MonoBehaviour
{
    private IState cState;
    public FSM (IState _startState)
    {
        SwitchState(_startState);
    }

    public void Update()
    {
        cState?.Run();
    }

    void SwitchState( IState _newState)
    {
        if (cState != null)
        {
            cState.OnStateSwitch -= SwitchState;
            cState.Exit();
        }
        cState = _newState;

        cState.Start();
        cState.OnStateSwitch += SwitchState;

    }
}
