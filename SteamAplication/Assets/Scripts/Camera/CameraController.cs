using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using Steamworks;

public class CameraController : NetworkBehaviour
{
    public GameObject cameraHolder;
    public Camera Camera;
    public float senX, senY;
    public Transform oriantattion;
    public GameObject playerModel;
    float xRotation;
    float yRotation;
    private bool isGameScene = false;

    private bool escape = false;

    public override void OnStartAuthority()
    {
        if (cameraHolder == null || isLocalPlayer == false)
        {
            return;
        }

        cameraHolder.SetActive(true);

        if (SceneManager.GetActiveScene().name == "Game" || SceneManager.GetActiveScene().name == "Lobby")
        {
            isGameScene = true;
        }
    }

    void Update()
    {
        if (!isLocalPlayer || !isGameScene)
            return;

        if (cameraHolder == null)
            return;

        // added
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            escape = !escape;
        }

        if (escape)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        Rotate();
    }

    public void Rotate()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * senX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * senY;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80, 80);

        playerModel.transform.rotation = Quaternion.Euler(0, yRotation, 0);
        oriantattion.rotation = Quaternion.Euler(xRotation, yRotation, 0);
    }
}