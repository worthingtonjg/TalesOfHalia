using System.Collections;
using UnityEngine;

public class Trampoline : MonoBehaviour
{
    private Animator animator;
    private AreaEffector2D areaEffector;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        areaEffector = GetComponent<AreaEffector2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator Bounce()
    {
        animator.SetTrigger("Activate");
        areaEffector.enabled = true;
        yield return new WaitForSeconds(0.2f);
        areaEffector.enabled = false;
    }
}
