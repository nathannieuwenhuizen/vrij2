﻿using System.Collections;
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
    private List<ArtWork> stolenArtWork;//save just in case

    [SerializeField]
    private ParticleSystem LockParticle;

    [SerializeField]
    private BalanceUI balanceUI;

    [SerializeField]
    private LineRenderer rope;

    [HideInInspector]
    public Rigidbody pulledObject;
    private float maxDistance = 5f;

    [SerializeField]
    private Animator anim;

    [SerializeField]
    private BushEffect bushEffect;
    [SerializeField]
    private ParticleSystem destructionParticle;

    public TrenchCoat TrenchCoat
    {
        get { return trenchCoat; }
        set { trenchCoat = value; }
    }

    public bool HasCrown { get; set; } = false;

    public List<ArtWork> StolenArtWork
    {
        get { 
            return stolenArtWork;
        }
        set {
            stolenArtWork = value;
        }
    }

    private void Awake()
    {
        anim.SetFloat("Offset", Random.Range(0.0f, 1.0f));
    }
    public override void Start()
    {
        base.Start();


        pulledObject = new GameObject("Bag").AddComponent<Rigidbody>();
        pulledObject.mass = 0;
        pulledObject.gameObject.AddComponent<SphereCollider>().radius = 0.5f;
        pulledObject.transform.position = transform.position;

        stolenArtWork = new List<ArtWork>();
        //artWorkParent = new GameObject("Artworks");
        //artWorkParent.transform.parent = transform;
        //artWorkParent.transform.localPosition = new Vector2(0, 0);

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
                Debug.Log("trenchcoat should transfer");
                playerToAttachTo.trenchCoat.Wear(this);
                playerToAttachTo.trenchCoat = null;
            }

            balanceUI.ShowUI();

            playerBelowMe = playerToAttachTo;
            playerToAttachTo.playerAboveMe = this;
            rb.velocity = Vector3.zero;
            myColl.enabled = false;
            transform.parent = playerToAttachTo.headPivot;
            transform.localRotation = Quaternion.Euler(Vector3.zero);

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
            if (index > 0.5f)
            {
                if (!LockParticle.isPlaying)
                {
                    LockParticle.Play();
                }
            }
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

                //trenchCoat.Wear(playerBelowMe);
                //trenchCoat = null;
            }

            playerBelowMe.playerAboveMe = null;
            canMove = true;
            balanceUI.HideUI();

            IsOnTop = false;
            rb = gameObject.AddComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            myColl.enabled = true;
            playerBelowMe = null;
            transform.parent = null;
            transform.localRotation = Quaternion.Euler(Vector3.zero);


        }
        else
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
            ParkManager.instance?.FirstMovement();
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
            anim.SetFloat("Velocity", rb.velocity.magnitude);
            bushEffect.UpdateBush(rb.velocity.magnitude / normalSpeed);
        }
        else
        {
            anim.SetFloat("Velocity", 0);
        }

        if (isOnTop)
        {
            transform.localRotation = Quaternion.Euler(0, 0, balanceUI.Val * 20f);
            if (balanceUI.InBalance(Input.GetAxis("Horizontal_P" + controllerIndex)) == false)
            {
                GoAwayFromStack();
            }
        }


        //rotates
        if (canMove)
        {
            if (Input.GetAxis("Horizontal_P" + controllerIndex) == 0 && Input.GetAxis("Vertical_P" + controllerIndex) == 0)
            {

            }
            else
            {
                //rb.rotation = Quaternion.Slerp(rb.rotation, Quaternion.LookRotation(lookRotation), Time.deltaTime * turnSpeed);
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookRotation), Time.deltaTime * turnSpeed);
            }
        }

        CheckRopePull();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "DestroyAble" && HasCrown)
        {
            if (collision.gameObject.GetComponent<MeshDestroy>() == null)
            {
                destructionParticle.Emit(10);
                CameraShake.instance?.Shake(.5f);
                AudioManager.instance?.PlaySound(AudioEffect.destruction, .5f);
                MeshDestroy md = collision.gameObject.AddComponent<MeshDestroy>();
                md.sliced = true;
                md.DestroyMesh();
            }
        }
    }

        public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "End")
        {
            BaseManager.instance?.End(); 
        } else if (other.gameObject.tag == "Button")
        {
            other.GetComponent<FloorButton>()?.Push(this);
        }
        else if (other.gameObject.tag == "Bush")
        {
            bushEffect.EnterBush(other.gameObject);
        }

        if (other.GetComponent<FocusArea>() != null)
        {
            other.GetComponent<FocusArea>().Focus(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Button")
        {
            other.GetComponent<FloorButton>()?.Release(this);
        }
        else if (other.gameObject.tag == "Bush")
        {
            bushEffect.LeaveBush(other.gameObject);
        }


        if (other.GetComponent<FocusArea>() != null)
        {
            other.GetComponent<FocusArea>().Unfocus(this);
        }

    }

    public void CheckRopePull()
    {
        if (pulledObject == null) return;
        if (Vector3.Distance(pulledObject.transform.position, transform.position) > maxDistance)
        {
            Vector3 dist = transform.position - pulledObject.transform.position;
            pulledObject.velocity = dist * 0.8f;
            if (dist.magnitude > maxDistance * 2f)
            {
                pulledObject.transform.position = transform.position;
            }
        }
        if (stolenArtWork.Count > 0)
        {
            rope.enabled = true;
            rope.SetPosition(1, transform.InverseTransformDirection(stolenArtWork[0].transform.position - rope.transform.position ) );
        } else
        {
            rope.enabled = false;
        }
    }
    private void Update()
    {
        //stack/unstack to other players
        if (Input.GetButtonDown("Fire_P" + controllerIndex))
        {
            BaseManager.instance?.FirstJump();

            if (isOnTop)
            {
                GoAwayFromStack();
                AudioManager.instance?.PlaySound(AudioEffect.gnome_yell, .1f);
            }
            else
            {
                GoToTopOfStack();
                AudioManager.instance?.PlaySound(AudioEffect.normal_gibberish, .1f);
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
                //AudioManager.instance?.PlaySound(AudioEffect.normal_gibberish, .1f);
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
