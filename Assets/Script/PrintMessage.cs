using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PrintMessage : MonoBehaviour
{
    private const int HEIGHT = 20;
    private int stayCount;
    private int count;
    private bool upSwitch;//上から下までfalse/下から上までtrue
    private bool showFlag;//メッセージ表示フラグ
    private static Queue<string> messageQueue = new Queue<string>();
    private string message;

    [SerializeField]
    private int speed = 2;

    public static void Add(string message)
    {
        messageQueue.Enqueue(message);
    }

    private PrintMessage()
    {
    }

    private void Awake()
    {
        Add("Welcome to CatLife!!");
    }

    private void FixedUpdate()
    {
        //今メッセージ処理をしていなければキューを処理
        if(!showFlag && messageQueue.Count > 0)
        {
            message = messageQueue.Dequeue();
            showFlag = true;
            count = 0;
            stayCount = 0;
            upSwitch = false;
        }
        if(showFlag)
        {
            //下に下がる処理
            if(count < 100 && !upSwitch)
                count += speed;
            //下がりきったらupSwitchを入れる
            else if(count >= 100)
                upSwitch = true;

            //しばらく停止する処理
            if(upSwitch)
                ++stayCount;

            //上に上がる処理
            if(upSwitch && count > -HEIGHT && stayCount > 60)
                count -= speed;
            //上がりきったら
            if(upSwitch && count <= -HEIGHT)
                showFlag = false;
        }
    }

    private void OnGUI()
    {
        if(showFlag)
            GUI.TextField(new Rect(Screen.width / 4, count - HEIGHT, Screen.width * 2 / 4, HEIGHT), message);
    }
}