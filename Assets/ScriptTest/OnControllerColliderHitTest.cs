using System.Collections;
using UnityEngine;

public class OnControllerColliderHitTest : MonoBehaviour
{
    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit.gameObject.tag != "Ground")
            print(hit.gameObject.name);
    }
}