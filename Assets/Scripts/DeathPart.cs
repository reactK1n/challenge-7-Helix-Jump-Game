using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathPart : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Renderer>().material.color = Color.red;
    }

    public void HitDeathPath()
    {
        GameManager.Instance.RestartFromPreviousJump();
    }
}
