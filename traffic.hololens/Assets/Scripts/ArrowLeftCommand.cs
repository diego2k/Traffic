using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowLeftCommand : MonoBehaviour
{
    void OnShowLeft()
    {
        var meshRenderer = this.gameObject.GetComponentInChildren<MeshRenderer>();
        meshRenderer.enabled = true;
        var boxCollider = this.gameObject.GetComponentInChildren<BoxCollider>();
        boxCollider.enabled = true;
    }
}
