using UnityEngine;

public enum PickupEnum { growth, speed, jump, weapon}

public class Pickup : MonoBehaviour
{
    
    
    private PlayerMovementRigidBody playerController;
    private Animator animator;
    private Collider2D playerCollider;
    private Vector3 initialPosition;

    public float bounceHeight = 0.5f; // adjust this value to change the bounce height
    public float bounceSpeed = 1.0f; // adjust this value to change the bounce speed

    [SerializeField]
    public PickupEnum PickupType;

    // Start is called before the first frame update
    void Start()
    {
        var player = GameObject.Find("Player");
        
        playerController = player.GetComponent<PlayerMovementRigidBody>();
        animator = GetComponent<Animator>();
        playerCollider = GetComponent<Collider2D>();

        initialPosition = transform.position;
    }

    void Update()
    {
        float offset = Mathf.Sin(Time.time * bounceSpeed) * bounceHeight;
        transform.position = initialPosition + new Vector3(0, offset, 0);
    }

    private void OnTriggerEnter2D(Collider2D other){
        if(other.tag == "Player"){
            playerController.Pickup(PickupType);
            animator.SetTrigger("Activate");
            playerCollider.enabled = false;
        }
    }
}
