using System.Collections;
using UnityEngine;

public class ShowText : MonoBehaviour
{
    private string fname = "太刀魚";
    private const int H = 20;
    private int i;
    private bool compFlag = false;
    private int stayCount = 0;

    private void OnGUI()
    {
        GUI.TextField(new Rect(Screen.width / 4, i, Screen.width * 2 / 4, H), fname + "が取れました");
    }

    private void Update()
    {
        if(i < 100 && !compFlag)
            ++i;
        else if(i >= 100)
            compFlag = true;
        if(compFlag)
            ++stayCount;
        if(compFlag && i > -H && stayCount > 60)
            --i;
        if(compFlag && i <= -H)
            Destroy(this);
    }
}