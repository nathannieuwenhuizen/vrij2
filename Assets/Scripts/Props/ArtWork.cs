using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtWork : InteractableObject
{
    private float jumpSpeed = 1f;
    private float jumpHeight = 3f;

    private Vector3 rotateSpeed = new Vector3(5f, 5f, 5f);

    [SerializeField]
    private AudioEffect effectSound;

    protected bool goesToHead = false;

    public override void Interact(Gnome gnome = null)
    {
        base.Interact(gnome);
        StartCoroutine(Stealing(gnome));
    }

    public IEnumerator Stealing(Gnome gnome)
    {
        if (!gnome.StolenArtWork.Contains(this))
        {
            AudioManager.instance?.PlaySound(effectSound, .4f);

            gnome.StolenArtWork.Add(this);
            BaseManager.instance?.UpdateScoreUI();
            if (goesToHead)
            {
                yield return StartCoroutine(AnimateTo(gnome.headPivot.transform, 1f));
            } else
            {
                yield return StartCoroutine(AnimateTo(gnome.pulledObject.transform, .5f));
            }
        }
    }

    public void CollectByHuman(Human human)
    {
        StartCoroutine(AnimateTo(human.transform, 0));
    }

    IEnumerator AnimateTo(Transform dest, float scale = 0)
    {

        transform.parent = null;

        float index = 0;
        float startPos = transform.position.y;

        Vector2 currentPos = new Vector2();
        Vector2 desiredPos = new Vector2();
        Vector3 startScale = new Vector3(1, 1, 1);
        Vector3 endScale = new Vector3(scale, scale, scale);

        SetCollider(false);
        while (index < 1)
        {
            yield return new WaitForFixedUpdate();

            index += Time.deltaTime * jumpSpeed;

            float y = startPos + ((dest.position.y) - startPos) + Mathf.Sin(Mathf.PI * index) * jumpHeight;

            desiredPos.x = dest.position.x;
            desiredPos.y = dest.position.z;
            currentPos = Vector2.Lerp(new Vector2(transform.position.x, transform.position.z), desiredPos, index);

            transform.position = new Vector3(currentPos.x, y, currentPos.y);
            transform.localScale = Vector3.Lerp(startScale, endScale, index);
            transform.Rotate(rotateSpeed);
        }
        transform.position = dest.position;
        transform.parent = dest;
        if (scale == 0)
        {
            transform.rotation = Quaternion.Euler(Vector3.zero); //set roation back
            gameObject.SetActive(false);
        } else
        {
            SetCollider(true);
        }
        if (goesToHead) { transform.localRotation = Quaternion.Euler(0, 0, 0); }
    }

    public void SetCollider(bool val)
    {
        foreach(Collider child in GetComponentsInChildren<Collider>())
        {
            child.enabled = val;
        }
    }
}
