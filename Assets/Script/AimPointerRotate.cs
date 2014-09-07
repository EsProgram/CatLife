using System.Collections;
using UnityEngine;

public class AimPointerRotate : MonoBehaviour
{
    [SerializeField]
    private float speed = 180f;

    private void Update()
    {
        transform.Rotate(Vector3.up, speed * Time.deltaTime);
    }
}