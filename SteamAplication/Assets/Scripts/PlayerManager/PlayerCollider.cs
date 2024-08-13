using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollider : MonoBehaviour
{
    public bool isConsolActiveted = false;
    public float radius = 3f;

    public GameObject ConsolIndicator;
    public LayerMask mask;
    private void Update()
    {
        Collider[] col = Physics.OverlapSphere(transform.position, radius, mask);

        if (col.Length > 0)
        {
            for (int i = 0; i < col.Length; i++)
            {
                ConsolIndicator.transform.position = new Vector3(col[0].transform.position.x, 2f, col[0].transform.position.z);
                ConsolIndicator.SetActive(true);
                isConsolActiveted = true;
            }
        }
        else
        {
            ConsolIndicator.SetActive(false);
            isConsolActiveted = false;
        }
    }

}
