using UnityEngine;

public class PatrolState : IState
{
    public ILiveStateDelegate OnStateSwitch { get; set; }
    public Human human;
    public Gnome lastDetectedGnome;

    public Gnome noticingGnome;
    public Gnome closeGnome;

    public void Start()
    {

        human.thoughtBubble.HideUI();

        human.spotLight.color = Color.green;
        human.movement.StartPatrolling();
    }

    public void Run()
    {
        noticingGnome = human.noticingGnome();
        closeGnome = human.closeGnome();

        if (noticingGnome != null && lastDetectedGnome == null)
        {
            human.foundGnome = noticingGnome;
            OnStateSwitch(human.noticingState);
        }

        if (closeGnome != null && lastDetectedGnome == null)
        {
            human.foundGnome = closeGnome;
            OnStateSwitch(human.chaseState);
        }

        if (noticingGnome == null && closeGnome == null)
        {
            lastDetectedGnome = null;
        } else if (noticingGnome != null)
        {
            lastDetectedGnome = noticingGnome;
        } else if (closeGnome != null)
        {
            lastDetectedGnome = closeGnome;
        }
    }
    public void Exit()
    {
        human.thoughtBubble.ShowUI();
    }

}
public class NoticingState : IState
{
    public ILiveStateDelegate OnStateSwitch { get; set; }
    public Human human;
    private float noticingPrecentage = 0;
    public float noticeSpeed = .5f;

    public void Start()
    {
        noticingPrecentage = 0;

        human.thoughtBubble.ShowUI();
        human.thoughtBubble.SetSymbol(Thought.noticing);
        human.thoughtBubble.BackColor = Color.yellow;
        human.thoughtBubble.FillColor = Color.red;

        human.spotLight.color = Color.yellow;
    }

    public void Run()
    {

        human.foundGnome = human.noticingGnome();

        if (human.foundGnome != null)
        {
            float distancePrecentage = 1 - Mathf.Min(.99f, Vector3.Distance(human.foundGnome.transform.position, human.transform.position) / human.detectionRange);
            //Debug.Log(distancePrecentage);
            noticingPrecentage += Time.deltaTime * (noticeSpeed * (1 + distancePrecentage * 2f));
        } else
        {
            noticingPrecentage -= Time.deltaTime * noticeSpeed;
        }

        if (human.closeGnome() != null)
        {
            human.foundGnome = human.closeGnome();
            OnStateSwitch(human.chaseState);
        }

        human.thoughtBubble.FillAmount = noticingPrecentage;

        if (noticingPrecentage > 1)
        {
            OnStateSwitch(human.chaseState);
        } else if (noticingPrecentage < 0)
        {
            OnStateSwitch(human.patrolState);
        }

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
    public Vector3 lastSeenPos;
    public void Start()
    {
        human.movement.StopMovement();

        AudioManager.instance?.PlaySound(AudioEffect.guard_catches_you, .1f);
        human.spotLight.color = Color.red;
        human.movement.StartChase(human.foundGnome.transform);

        human.thoughtBubble.SetSymbol(Thought.alert);
        human.thoughtBubble.FillColor = Color.red;

        GameManager.instance.HumanIsAlerted(human);
    }

    public void Run()
    {
        human.foundGnome = human.noticingGnome();
        if (human.foundGnome == null && human.closeGnome() == null)
        {
            human.searchState.searchPos = lastSeenPos;
            OnStateSwitch(human.searchState);
            return;
        }
        else
        {
            if (human.foundGnome == null)
            {
                human.foundGnome = human.closeGnome();
            }
            if (human.foundGnome != null)
            {
                lastSeenPos = human.foundGnome.transform.position;
            }
        }

        if (Vector3.Distance(human.transform.position, human.foundGnome.transform.position) < human.gnomeAttackDistance)
        {
            human.RetrieveArtWorkFrom(human.foundGnome);
            AudioManager.instance?.PlaySound(AudioEffect.guard_catches_you, .3f);
            GameManager.instance.HumanIsNormal(human);
            OnStateSwitch(human.patrolState);
        }
    }

    public void Exit()
    {
        human.movement.StopMovement();
    }
}
public class SearchState : IState
{
    public ILiveStateDelegate OnStateSwitch { get; set; }
    public Human human;
    public Vector3 searchPos;
    public void Start()
    {
        human.thoughtBubble.FillColor = Color.yellow;

        human.spotLight.color = Color.yellow;
        //Debug.Log("search pos: " + searchPos);
        //Debug.Log("my pos: " + human.transform.position);
        human.movement.Search(searchPos, 90, 5f);//starts searching
    }

    public void Run()
    {
        if (!human.movement.IsMoving)
        {
            GameManager.instance.HumanIsNormal(human);
            OnStateSwitch(human.patrolState);
        }

        human.foundGnome = human.noticingGnome();
        if (human.foundGnome != null)
        {
            OnStateSwitch(human.chaseState);
        }
        if (human.closeGnome() != null)
        {
            human.foundGnome = human.closeGnome();
            OnStateSwitch(human.chaseState);
        }
    }

    public void Exit()
    {
        human.movement.StopMovement();
    }
}
