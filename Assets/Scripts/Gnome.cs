using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gnome : Walkable
{

    private Rigidbody rb;
    private CapsuleCollider myColl;
    private Vector3 lookRotation;

    [Header("gnome info")]

    [SerializeField]
    private int controllerIndex = 0;

    public bool canMove = false;
    public bool isOnTop = false;

    public Gnome playerAboveMe;
    public Gnome playerBelowMe;

    [SerializeField]
    private float turnSpeed = 10f;

    [SerializeField]
    private float normalSpeed = 100f;

    [SerializeField]
    private float stackedSpeed = 50f;

    [SerializeField]
    private float interactDistanceWithPlayer = 2f;

    [SerializeField]
    private AnimationCurve jumpCurve;
    private float jumpSpeed = 2f;

    [SerializeField]
    public Transform headPivot;
    [SerializeField]
    private float hatBounciness = 5f;

    [SerializeField]
    private bool relativeToCamera = true;

    [SerializeField]
    private InteractableObject hoverObject;

    [SerializeField]
    private TrenchCoat trenchCoat;

    private bool isMoving = false;

    public GameObject artWorkParent;
    private List<ArtWork> stolenArtWork;

    public TrenchCoat TrenchCoat
    {
        get { return trenchCoat; }
        set { trenchCoat = value; }
    }

    public List<ArtWork> StolenArtWork
    {
        get { 
            return stolenArtWork;
        }
        set {
            Debug.Log("Test");
            stolenArtWork = value;
        }
    }

    public override void Start()
    {
        base.Start();

        stolenArtWork = new List<ArtWork>();
        artWorkParent = new GameObject("Artworks");
        artWorkParent.transform.parent = transform;
        artWorkParent.transform.localPosition = new Vector2(0, 0);

        myColl = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
    }


    public Gnome GetClosestPlayer()
    {
        Gnome closest = null;
        float closestDistance = interactDistanceWithPlayer * 2;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactDistanceWithPlayer);
        int i = 0;
        while (i < hitColliders.Length)
        {
            if (hitColliders[i].GetComponent<Gnome>() != null)
            {
                Gnome otherPlayer = hitColliders[i].GetComponent<Gnome>();
                float distance = Vector3.Distance(transform.position, hitColliders[i].transform.position);
                if (distance < closestDistance && !otherPlayer.isOnTop && otherPlayer != this)
                {
                    closestDistance = distance;
                    closest = hitColliders[i].GetComponent<Gnome>();
                }
            }
            i++;
        }

        return closest;
    }

    public InteractableObject GetClosestInteractable()
    {
        InteractableObject closest = null;
        float closestDistance = Mathf.Infinity;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactDistanceWithPlayer);
        int i = 0;
        while (i < hitColliders.Length)
        {
            if (hitColliders[i].transform.parent != null) {
                if (hitColliders[i].transform.parent.GetComponent<InteractableObject>() != null)
                {
                    InteractableObject otherObject = hitColliders[i].transform.parent.GetComponent<InteractableObject>();

                    float distance = Vector3.Distance(transform.position, hitColliders[i].transform.position);
                    if (distance < closestDistance && otherObject.CanBeInteracted(this))
                    {
                        closestDistance = distance;
                        closest = otherObject;
                    }
                }
            }
            i++;
        }

        return closest;
    }


    public Vector2 TrackDirection()
    {
        return -Vector2.Perpendicular(new Vector2(transform.localPosition.x, transform.localPosition.z).normalized);
    }


    public void GoToTopOfStack()
    {
        Gnome closestPlayer = GetClosestPlayer();
        if (closestPlayer != null)
        {
            Gnome playerToAttachTo = closestPlayer;
            if (closestPlayer.playerAboveMe != null)
            {
                playerToAttachTo = closestPlayer.playerAboveMe;
            }
            
            if (playerToAttachTo.trenchCoat != null)
            {
                playerToAttachTo.trenchCoat.Wear(this);
                playerToAttachTo.trenchCoat = null;
            }

            playerBelowMe = playerToAttachTo;
            playerToAttachTo.playerAboveMe = this;
            rb.velocity = Vector3.zero;
            myColl.enabled = false;
            transform.parent = playerToAttachTo.headPivot;
            //transform.position = playerToAttachTo.transform.position + new Vector3(0, 2, 0);
            canMove = false;
            if (rb != null)
            {
                Destroy(rb);
                rb = null;
            }
            IsOnTop = true;
            StartCoroutine(Jump(playerBelowMe.transform));
        } else
        {
            //discards trechcoat to the ground (makes popupacitve again)
            if (trenchCoat != null)
            {
                trenchCoat.TakeOff(this, true);
            }

        }
    }
    IEnumerator Jump(Transform dest)
    {
        float index = 0;


        float startPos = transform.position.y;
        Vector2 currentPos = new Vector2();
        Vector2 desiredPos = new Vector2();


        while (index < 1)
        {
            yield return new WaitForFixedUpdate();
            index += Time.deltaTime * jumpSpeed;

            float y = startPos + ((dest.position.y + 2f) - startPos) * jumpCurve.Evaluate(index * 2f);

            desiredPos.x = dest.position.x;
            desiredPos.y = dest.position.z;
            currentPos = Vector2.Lerp(new Vector2(transform.position.x, transform.position.z), desiredPos, index);

            transform.position = new Vector3(currentPos.x, y, currentPos.y);
        }
    }
    public void GoAwayFromStack()
    {
        if (playerBelowMe != null)
        {
            //gives trenchcoat to player below me
            if (trenchCoat != null)
            {
                Debug.Log("transfer to other player");

                trenchCoat.Wear(playerBelowMe);
                trenchCoat = null;
            }

            playerBelowMe.playerAboveMe = null;
            canMove = true;
            IsOnTop = false;
            rb = gameObject.AddComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            myColl.enabled = true;
            playerBelowMe = null;
            transform.parent = null;

        } else
        {
            //discards trechcoat to the ground (makes popupacitve again)
            trenchCoat.TakeOff(this, true);
        }

    }

    protected override void WalkStep()
    {
        if (IsOnTop) { return; }
        base.WalkStep();
    }

    public bool IsOnTop
    {
        get { return isOnTop; }
        set
        {
            isOnTop = value;
            if (value)
            {
                walkParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            } 
        }
    }

    void FixedUpdate()
    {
        //getting input to aim
        lookRotation = new Vector3(Input.GetAxis("Horizontal_P" + controllerIndex), 0, -Input.GetAxis("Vertical_P" + controllerIndex));

        //play sound when gnome starts moving
        if (lookRotation.x != 0 || lookRotation.z != 0)
        {
            if (!isMoving)
            {
                //AudioManager.instance?.PlaySound(AudioEffect.normal_gibberish, .1f);
            }
        }
        isMoving = lookRotation.x != 0 || lookRotation.z != 0;



        //rotates the gnome relative to camera rotation
        if (relativeToCamera)
        {
            Vector3 forward = Camera.main.transform.forward;
            forward.y = 0;
            forward.Normalize();
            lookRotation = forward * -Input.GetAxis("Vertical_P" + controllerIndex);

            Vector3 right = Camera.main.transform.right;
            right.y = 0;
            right.Normalize();
            lookRotation += right * Input.GetAxis("Horizontal_P" + controllerIndex);

            lookRotation.y = 0;
        }

        //move function
        if (canMove && rb != null)
        {
            //look rotational speed
            if (playerAboveMe == null)
            {
                rb.velocity = new Vector3(lookRotation.x * normalSpeed, rb.velocity.y, lookRotation.z * normalSpeed);
            } else
            {
                rb.velocity = new Vector3(lookRotation.x * stackedSpeed, rb.velocity.y, lookRotation.z * stackedSpeed);
            }
        }



        //rotates
        if (Input.GetAxis("Horizontal_P" + controllerIndex) == 0 && Input.GetAxis("Vertical_P" + controllerIndex) == 0) {

        } else {
            //rb.rotation = Quaternion.Slerp(rb.rotation, Quaternion.LookRotation(lookRotation), Time.deltaTime * turnSpeed);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookRotation), Time.deltaTime * turnSpeed);
        }


    }
    private void Update()
    {
        //stack/unstack to other players
        if (Input.GetButtonDown("Fire_P" + controllerIndex))
        {
            if (isOnTop)
            {
                GoAwayFromStack();
                AudioManager.instance?.PlaySound(AudioEffect.gnome_yell, .4f);
            }
            else
            {
                GoToTopOfStack();
                AudioManager.instance?.PlaySound(AudioEffect.normal_gibberish, .4f);
            }
        }


        //head animation
        if (rb != null)
        {
            headPivot.transform.localRotation = Quaternion.Euler(-new Vector3(Vector3.Distance(Vector3.zero, rb.velocity) * hatBounciness, 0, 0));
        }

        UpdateWalkCycle();

        CheckInteraction();
    }

    public void UpdateWalkCycle()
    {
        //walk particle
        if (!isOnTop)
        {
            base.WalkCycle();
        }

    }


    public void CheckInteraction()
    {
        //check object that is closest to player to interact with
        InteractableObject closestObject = GetClosestInteractable();
        if (closestObject != hoverObject)
        {
            if (hoverObject != null) hoverObject.HideUI();
            hoverObject = closestObject;
            if (hoverObject != null) hoverObject.ShowUI(controllerIndex);
        }

        //interact with enviroment
        if (Input.GetButtonDown("Interact_P" + controllerIndex))
        {
            Debug.Log("interact" + controllerIndex);
            if (hoverObject != null)
            {
                AudioManager.instance?.PlaySound(AudioEffect.normal_gibberish, .4f);
                hoverObject.Interact(this);
            }
        }

    }

    void OnDrawGizmosSelected()
    {
        // Display the detection radius
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, interactDistanceWithPlayer);
    }
}
