﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowCommands : MonoBehaviour
{
    void OnLookAt()
    {
        var meshRenderer = this.gameObject.GetComponentInChildren<MeshRenderer>();
        meshRenderer.enabled = false;
        var boxCollider = this.gameObject.GetComponentInChildren<BoxCollider>();
        boxCollider.enabled = false;

        //this.gameObject.SetActive(false);
    }

}
