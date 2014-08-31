using System.Collections;
using UnityEngine;

public class FadeoutAndDelete : MonoBehaviour
{
    private Color fadeout = new Color(0, 0, 0, 0.01f);

    private void Update()
    {
        if(this.renderer.material.color.a > 0)
            this.renderer.material.color -= fadeout;
        else
            Destroy(this.gameObject);
    }
}