using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GnomeMovement : MonoBehaviour
{

    private Rigidbody rb;
    private BoxCollider myColl;
    private Vector3 lookRotation;


    [SerializeField]
    private int playerIndex = 0;

    public bool canMove = false;
    public bool isOnTop = false;

    public GnomeMovement playerAboveMe;
    public GnomeMovement playerBelowMe;

    [SerializeField]
    private float turnSpeed = 10f;

    [SerializeField]
    private float normalSpeed = 100f;

    [SerializeField]
    private float maxVelocity = 200f;

    [SerializeField]
    private float interactDistanceWithPlayer = 2f;

    [SerializeField]
    private AnimationCurve jumpCurve;
    private float jumpSpeed = 2f;

    void Start()
    {
        myColl = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
    }


    public GnomeMovement CheckClosestPlayer()
    {
        GnomeMovement closest = null;
        float closestDistance = interactDistanceWithPlayer * 2;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactDistanceWithPlayer);
        int i = 0;
        while (i < hitColliders.Length)
        {
            if (hitColliders[i].GetComponent<GnomeMovement>() != null)
            {
                GnomeMovement otherPlayer = hitColliders[i].GetComponent<GnomeMovement>();
                float distance = Vector3.Distance(transform.position, hitColliders[i].transform.position);
                if ( distance < closestDistance && !otherPlayer.isOnTop && otherPlayer != this)
                {
                    closestDistance = distance;
                    closest = hitColliders[i].GetComponent<GnomeMovement>();
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
        GnomeMovement closestPlayer = CheckClosestPlayer();
        if (closestPlayer != null)
        {
            GnomeMovement playerToAttachTo = closestPlayer;
            if (closestPlayer.playerAboveMe != null)
            {
                playerToAttachTo = closestPlayer.playerAboveMe;
            }
            
            playerBelowMe = playerToAttachTo;
            playerToAttachTo.playerAboveMe = this;
            rb.velocity = Vector3.zero;
            myColl.enabled = false;
            transform.parent = playerToAttachTo.transform;
            //transform.position = playerToAttachTo.transform.position + new Vector3(0, 2, 0);
            canMove = false;
            Destroy(rb);
            isOnTop = true;
            StartCoroutine(Jump(playerBelowMe.transform));
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

            float y = startPos + ((dest.position.y + 1f) - startPos) * jumpCurve.Evaluate(index * 2f);

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
            playerBelowMe.playerAboveMe = null;
            canMove = true;
            isOnTop = false;
            rb = gameObject.AddComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            myColl.enabled = true;
            playerBelowMe = null;
            transform.parent = null;

        }

    }

    void FixedUpdate()
    {
        //getting input to aim
        lookRotation = new Vector3(Input.GetAxis("Horizontal_P" + playerIndex), 0, -Input.GetAxis("Vertical_P" + playerIndex));

        //move function
        if (canMove && rb != null)
        {
            //look rotational speed
            rb.AddForce(lookRotation * normalSpeed);
            if (Vector3.Distance(Vector3.zero, rb.velocity) > maxVelocity)
            {
                rb.velocity = rb.velocity.normalized * maxVelocity;
            }
        }

        //rotates
        if (Input.GetAxis("Horizontal_P" + playerIndex) == 0 && Input.GetAxis("Vertical_P" + playerIndex) == 0) {

        } else {
            //rb.rotation = Quaternion.Slerp(rb.rotation, Quaternion.LookRotation(lookRotation), Time.deltaTime * turnSpeed);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookRotation), Time.deltaTime * turnSpeed);
        }


    }
    private void Update()
    {
        //stack/unstack to other players
        if (Input.GetButtonDown("Fire_P" + playerIndex))
        {
            if (isOnTop)
            {
                GoAwayFromStack();
            }
            else
            {
                GoToTopOfStack();
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
