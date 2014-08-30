using System.Collections;
using UnityEngine;

public class DestroyMine : MonoBehaviour
{
    private DestroyMine()
    {
    }

    private void Awake()
    {
        if(true)
            Destroy(this);
    }
}