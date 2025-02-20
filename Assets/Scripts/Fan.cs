using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fan : MonoBehaviour
{
    private Animator animator;
    private AreaEffector2D areaEffector;

    public bool isOn = false;
    public bool fanRunning = false;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        areaEffector = GetComponent<AreaEffector2D>();
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
        if(fanRunning) return;

        animator.SetInteger("AnimationState", 1);
        areaEffector.enabled = true;
        fanRunning = true;
    }

    public void TurnOff()
    {
        if(!fanRunning) return;

        animator.SetInteger("AnimationState", 0);
        areaEffector.enabled = false;
        fanRunning = false;
    }
}
