using UnityEngine;
using Mirror;

public class PlayerMovmentController : NetworkBehaviour
{
    public float Speed = 0.1f;
    public GameObject PlayerModel;
    public Transform orientation;
    public Animator animator; 

    private void Start()
    {
        PlayerModel.SetActive(false);
    }

    private void Update()
    {
        if (PlayerModel.activeSelf == false)
        {
            SetPosition();
            PlayerModel.SetActive(true);
        }

        if (isLocalPlayer == true)
        {
            Movement();
        }
    }

    void SetPosition()
    {
        transform.position = new Vector3(Random.Range(-5, 5), 2, Random.Range(-15, 7));
    }

    void Movement()
    {
        float xDirection = Input.GetAxis("Horizontal");
        float zDirection = Input.GetAxis("Vertical");

        Vector3 forwardDirection = new Vector3(orientation.forward.x, 0, orientation.forward.z).normalized;
        Vector3 rightDirection = new Vector3(orientation.right.x, 0, orientation.right.z).normalized;

        Vector3 moveDirection = forwardDirection * zDirection + rightDirection * xDirection;

        transform.Translate(moveDirection * Speed);

        float moveSpeed = moveDirection.magnitude;
        animator.SetFloat("Speed", moveSpeed); 
    }
}
