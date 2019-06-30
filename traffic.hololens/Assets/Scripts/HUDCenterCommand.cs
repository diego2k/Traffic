using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDCenterCommand : MonoBehaviour
{
    public UnityEngine.UI.Text center;

    void SetHUDTextCenter(string text)
    {
        center.text = text;
    }
}
