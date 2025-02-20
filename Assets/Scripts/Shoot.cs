using System.Collections;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    public GameObject bullet;
    public GameObject spawnPoint;
    public int bulletGroup = 3;
    public float pauseBetweenGroups = 1;
    public float waitBeforeShot = 1f;
    public float waitAfterShot = .3f;

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(Fire());
    }

    // Update is called once per frame
    IEnumerator Fire()
    {
        for (int i = 0; i < bulletGroup; i++)
        {
            animator.SetTrigger("Attack");
            yield return new WaitForSeconds(waitBeforeShot);
            Instantiate(bullet, spawnPoint.transform.position, Quaternion.identity);
            yield return new WaitForSeconds(waitAfterShot);
        }
        yield return new WaitForSeconds(pauseBetweenGroups);
        StartCoroutine(Fire());
    }
}
