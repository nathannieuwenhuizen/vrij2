using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SomeStupidAnimal : MonoBehaviour
{
    private FSM fsm;
    // Start is called before the first frame update
    void Start()
    {
        fsm = new FSM(new EatState());
    }

    // Update is called once per frame
    void Update()
    {
        fsm.Update();
    }
}

public class EatState : IState
{
    public ILiveStateDelegate OnStateSwitch { get; set; }

    public void Exit()
    {
        Debug.Log("done eating...");
    }

    public void Run()
    {
        Debug.Log("eating...");
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnStateSwitch(new SleepState());
        }
    }

    public void Start()
    {
        Debug.Log("start eating...");
    }
}
public class SleepState : IState
{
    public ILiveStateDelegate OnStateSwitch { get; set; }

    public void Exit()
    {
        Debug.Log("done sleeping...");
    }

    public void Run()
    {
        Debug.Log("zzz...");
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnStateSwitch(new EatState());
        }
    }

    public void Start()
    {
        Debug.Log("start sleeping...");
    }
}
