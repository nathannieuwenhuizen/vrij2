using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Human : MonoBehaviour
{

    [Range(0,180)]
    [SerializeField]
    private float detectionAngle = 90f;
    [Range(1,10)]
    [SerializeField]
    private float detectionRange = 2f;

    public Light spotLight;

    [HideInInspector]
    public HumanMovement movement;

    private Gnome[] gnomes;

    [SerializeField]
    private FSM stateMachine;

    public ChaseState chaseState;
    public PatrolState patrolState;

    public float gnomeAttackDistance = 2f;

    [HideInInspector]
    public Gnome foundGnome;

    private void Start()
    {
        movement = GetComponent<HumanMovement>();
        gnomes = FindObjectsOfType<Gnome>();


        patrolState = new PatrolState();
        patrolState.human = this;
        chaseState = new ChaseState();
        chaseState.human = this;

        stateMachine = new FSM(patrolState);

    }

    private void Update()
    {
        stateMachine.Update();
        //spotLight.color = detectedGnome() != null ? Color.red : Color.yellow;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Vector3 rightLine = Vector3.Lerp(transform.forward, -transform.right, detectionAngle / 180).normalized * detectionRange;
        Vector3 leftLine = Vector3.Lerp(transform.forward, transform.right, detectionAngle / 180).normalized * detectionRange;

        #if (UNITY_EDITOR)
        UnityEditor.Handles.DrawWireArc(transform.position, transform.up, rightLine, detectionAngle, detectionRange);
        UnityEditor.Handles.DrawLine(transform.position, transform.position + rightLine);
        UnityEditor.Handles.DrawLine(transform.position, transform.position + leftLine);
        #endif  
    }

    public Gnome detectedGnome()
    {
        Gnome result = null;

        foreach(Gnome gnome in gnomes)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, gnome.transform.position - transform.position, out hit, detectionRange))
            {
                if (Vector3.Angle(transform.forward, hit.transform.position - transform.position) < detectionAngle / 2)
                {
                    if (hit.transform.GetComponent<Gnome>() != null)
                    {
                        if (gnome.TrenchCoat != null && gnome.playerBelowMe != null)
                        {
                            continue;
                        }
                        if (gnome.playerAboveMe != null)
                        {
                            if (gnome.playerAboveMe.TrenchCoat != null)
                            {
                                continue;
                            }
                        }
                        result = gnome;
                    }
                }
            }
        } 
        return result;
    }


    //editor only
    public void UpdateLight()
    {
        if (spotLight == null) return;
        spotLight.spotAngle = detectionAngle;
        spotLight.range = detectionRange;
    }
}

public class PatrolState : IState
{
    public ILiveStateDelegate OnStateSwitch { get; set; }
    public Human human;
    public Gnome lastDetectedGnome;
    public void Start()
    {
        human.spotLight.color = Color.yellow;

        human.movement.StartPatrolling();
    }

    public void Run()
    {

        human.foundGnome = human.detectedGnome();
        if (human.foundGnome != null && lastDetectedGnome == null)
        {
            OnStateSwitch(human.chaseState);
        }
        lastDetectedGnome = human.foundGnome;
    }
    public void Exit()
    {
        human.movement.StopMovement();
    }

}

public class ChaseState : IState
{
    public ILiveStateDelegate OnStateSwitch { get; set; }
    public Human human;

    public void Start()
    {
        human.spotLight.color = Color.red;
        human.movement.StartChase(human.foundGnome.transform);

        AudioManager.instance?.FadeMusic(Music.guardSeesYou, 1f);
    }

    public void Run()
    {
        human.foundGnome = human.detectedGnome();
        if (human.foundGnome == null)
        {
            OnStateSwitch(human.patrolState);
            return;
        }

        if (Vector3.Distance(human.transform.position, human.foundGnome.transform.position) < human.gnomeAttackDistance)
        {
            Debug.Log("I'm taking yourt stuff away!");
            AudioManager.instance?.PlaySound(AudioEffect.guard_catches_you, .3f);
            OnStateSwitch(human.patrolState);
        }
    }

    public void Exit()
    {
        AudioManager.instance?.FadeMusic(Music.museum, 1f);
        human.movement.StopMovement();
    }


}
