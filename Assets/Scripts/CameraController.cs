using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private BallController target;
    private float offset;
    // Start is called before the first frame update
    void Awake()
    {
        //get the default camera position difference from the ball
        offset = transform.position.y - target.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        var currentPos = transform.position;
        currentPos.y = target.transform.position.y + offset;
        transform.position = currentPos;
    }
}
