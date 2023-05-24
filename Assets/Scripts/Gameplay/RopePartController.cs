using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopePartController : MonoBehaviour
{
    [SerializeField] private MeshRenderer mesh = default;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "RopeGear")
        {
            mesh.enabled = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "RopeGear")
        {
            mesh.enabled = true;
        }
    }
}
