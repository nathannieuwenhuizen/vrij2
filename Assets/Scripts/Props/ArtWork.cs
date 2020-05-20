using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtWork : InteractableObject
{
    private float jumpSpeed = 2f;
    private float jumpHeight = 3f;

    private Vector3 rotateSpeed = new Vector3(5f, 5f, 5f);

    [SerializeField]
    private AudioEffect effectSound;

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
            GameManager.instance.UpdateScoreUI();
            yield return StartCoroutine(AnimateTo(gnome.artWorkParent.transform));
        }
    }

    public void CollectByHuman(Human human)
    {
        StartCoroutine(AnimateTo(human.transform));
    }

    IEnumerator AnimateTo(Transform dest)
    {

        transform.parent = null;

        float index = 0;
        float startPos = transform.position.y;

        Vector2 currentPos = new Vector2();
        Vector2 desiredPos = new Vector2();
        Vector3 startScale = new Vector3(1, 1, 1);
        Vector3 endScale = Vector3.zero;

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

        transform.rotation = Quaternion.Euler(Vector3.zero); //set roation back
        transform.parent = dest;
        gameObject.SetActive(false);
    }
}
