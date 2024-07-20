using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class CameraController : NetworkBehaviour
{
    public GameObject cameraHolder;
    public Camera camera;
    public Vector3 offset;
    private bool isGameScene = false;

    public override void OnStartLocalPlayer()
    {
        if (cameraHolder == null)
        {
            Debug.LogError("CameraHolder is not assigned in the inspector.");
            return;
        }

        cameraHolder.SetActive(true);

        if (SceneManager.GetActiveScene().name == "Game")
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

        camera.transform.position = transform.position + offset;

        camera.transform.LookAt(transform.position);
    }
   
}
