using System.Collections;
using UnityEngine;

[System.Obsolete("使用しないでください")]
public class DestroyMine : MonoBehaviour
{
    private static GameObject allowAttachedObject;

    private DestroyMine()
    {
    }

    private void Awake()
    {
        allowAttachedObject = GameObject.FindGameObjectWithTag("Player");
    }

    private void Start()
    {
        if(allowAttachedObject != this.gameObject)
            Destroy(this);
    }
}