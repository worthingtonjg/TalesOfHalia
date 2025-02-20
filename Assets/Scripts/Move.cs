using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public bool isOn = false;
    public Vector2 MoveDirection;
    public float duration;
    public float pauseBetweenMoves;

    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private  bool running = false;
    private Vector2 startPoint;
    private Vector2 endPoint;
    

    // Start is called before the first frame update
    void Start()
    {
        startPoint = transform.position;
        endPoint = new Vector2(transform.position.x + MoveDirection.x, transform.position.y + MoveDirection.y);
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();    
    }

    void Update() {
        if (isOn) {
            TurnOn();
        }
        else {
            TurnOff();
        }
    }

    // Update is called once per frame
    public void TurnOn()
    {
        if(running) return;
        StartCoroutine(LerpBetweenPoints(startPoint, endPoint));
        animator.SetInteger("AnimationState", 1);
        running = true;
    }

    public void TurnOff()
    {
        if(!running) return;

        animator.SetInteger("AnimationState", 0);
        StopAllCoroutines();
        running = false;
    }

    IEnumerator LerpBetweenPoints(Vector2 start, Vector2 end)
    {
        animator.SetInteger("AnimationState", 1);
        animator.SetTrigger("Activate");

        spriteRenderer.flipX = start.x < end.x;

        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            transform.position = Vector3.Lerp(start, end, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = end; // ensure we end at the exact end point

        animator.SetInteger("AnimationState", 0);
        yield return new WaitForSeconds(pauseBetweenMoves);

        StartCoroutine(LerpBetweenPoints(end, start));
        
    }

}
