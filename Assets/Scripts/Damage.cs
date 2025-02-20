using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    public int damage = 1;
    public string targetTag = "Player";
    private PlayerMovementRigidBody playerController;

    // Start is called before the first frame update
    void Start()
    {
        var player = GameObject.Find("Player");
        playerController = player.GetComponent<PlayerMovementRigidBody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other){
        if(other.tag == targetTag){
            print($"hit: {targetTag}");
            playerController.TakeDamage(damage);
        }
    }
}
