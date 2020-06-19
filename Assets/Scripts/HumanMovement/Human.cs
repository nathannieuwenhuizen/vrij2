using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Human : Walkable
{

    [Range(0,180)]
    [SerializeField]
    private float detectionAngle = 90f;
    [Range(1,10)]
    [SerializeField]
    public float detectionRange = 2f;

    public ViewInfo noticingView;
    public ViewInfo closeView;
    public Light spotLight;

    [HideInInspector]
    public HumanMovement movement;

    private Gnome[] gnomes;

    [HideInInspector]
    public FSM stateMachine;

    public ChaseState chaseState;
    public SearchState searchState;
    public PatrolState patrolState;
    public NoticingState noticingState;
    public AlwaysChaseState alwaysChaseState;

    public ThoughtBubble thoughtBubble;

    public float gnomeAttackDistance = 2f;


    [HideInInspector]
    public Gnome foundGnome;

    public override void Start()
    {
        base.Start();

        movement = GetComponent<HumanMovement>();
        gnomes = FindObjectsOfType<Gnome>();


        patrolState = new PatrolState();
        patrolState.human = this;
        noticingState = new NoticingState();
        noticingState.human = this;
        chaseState = new ChaseState();
        chaseState.human = this;
        searchState = new SearchState();
        searchState.human = this;
        alwaysChaseState = new AlwaysChaseState();
        alwaysChaseState.human = this;

        stateMachine = new FSM(patrolState);


     }

    public void RetrieveArtWorkFrom(Gnome gnome)
    {
        CameraShake.instance?.Shake(.5f);
        if (gnome.StolenArtWork.Count > 0)
        {
            StartCoroutine(RetrievingArt(gnome));
        }
    }
    IEnumerator RetrievingArt(Gnome gnome)
    {
        for (int i = 0; i < Mathf.Max(1, Mathf.RoundToInt(gnome.StolenArtWork.Count / 2f)); i++)
        {
            gnome.StolenArtWork[i].gameObject.SetActive(true);
            gnome.StolenArtWork[i].CollectByHuman(this);
            gnome.StolenArtWork.RemoveAt(i);
            GameManager.instance.UpdateScoreUI();
            yield return new WaitForSeconds(.3f);
        }
    }


    private void Update()
    {
        stateMachine?.Update();
        WalkCycle();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        DrawFieldLine(noticingView);
        Gizmos.color = Color.red;
        DrawFieldLine(closeView);
    }
    public void DrawFieldLine(ViewInfo viewInfo)
    {

        Vector3 rightLine = Vector3.Lerp(transform.forward, -transform.right, viewInfo.angle / 180).normalized * viewInfo.range;
        Vector3 leftLine = Vector3.Lerp(transform.forward, transform.right, viewInfo.angle / 180).normalized * viewInfo.range;

#if (UNITY_EDITOR)
        UnityEditor.Handles.DrawWireArc(transform.position, transform.up, rightLine, viewInfo.angle, viewInfo.range);
        UnityEditor.Handles.DrawLine(transform.position, transform.position + rightLine);
        UnityEditor.Handles.DrawLine(transform.position, transform.position + leftLine);
#endif

    }

    public Gnome detectedGnome(ViewInfo viewInfo)
    {
        Gnome result = null;

        foreach(Gnome gnome in gnomes)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, gnome.transform.position - transform.position, out hit, viewInfo.range))
            {
                if (Vector3.Angle(transform.forward, hit.transform.position - transform.position) < viewInfo.angle / 2)
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
    public Gnome noticingGnome()
    {
        return detectedGnome(noticingView);
    }
    public Gnome closeGnome()
    {
        return detectedGnome(closeView);
    }


    //editor only
    public void UpdateLight()
    {
        if (spotLight == null) return;
        spotLight.spotAngle = noticingView.angle;
        spotLight.range = noticingView.range;
    }
}

[System.Serializable]
public class ViewInfo
{
    [Range(1, 20)]
    public float range = 5f;
    [Range(0, 360)]
    public float angle = 90f;
}