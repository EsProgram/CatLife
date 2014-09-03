using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PState = PlayerStateController.PlayerState;

public class PrintMessage : MonoBehaviour
{
    private const int HEIGHT = 20;//メッセージ枠の高さ
    private const int STAY_TIME = 60;//下で留まるフレーム数
    private int stayCount;
    private int count;
    private bool upSwitch;//上から下までfalse/下から上までtrue
    private bool showFlag;//メッセージ表示フラグ
    private static Queue<string> messageQueue = new Queue<string>();
    private static string curMessage;//現在表示中のメッセージ
    private PlayerStateController psc;

    [SerializeField]
    private int speed = 2;//メッセージの降下/上昇スピード
    [SerializeField]
    private int down = Screen.height / 5;//どこまで降下するか

    /// <summary>
    /// メッセージキューにメッセージを追加する
    /// 現在キューの中に同じメッセージが含まれていた場合は自動的に省略される
    /// </summary>
    /// <param name="message">追加するメッセージ</param>
    public static void Add(string message)
    {
        //現在表示中のメッセージと同じでないかつ同じメッセージがメッセージキューに追加されていなければ
        if(!messageQueue.Contains(message) && curMessage != message)
            messageQueue.Enqueue(message);
    }

    private PrintMessage()
    {
    }

    private void Awake()
    {
        psc = PlayerStateController.GetInstance();
        Add("Welcome to CatLife!!");
    }

    private void FixedUpdate()
    {
        //今メッセージ処理をしていなければキューを処理
        if(!showFlag && messageQueue.Count > 0)
        {
            curMessage = messageQueue.Dequeue();
            showFlag = true;
            count = 0;
            stayCount = 0;
            upSwitch = false;
        }
        if(showFlag)
        {
            //下に下がる処理
            if(count < down && !upSwitch)
                count += speed;
            //下がりきったらupSwitchを入れる
            else if(count >= down)
                upSwitch = true;

            //しばらく停止する処理
            if(upSwitch)
                ++stayCount;

            //上に上がる処理(Hunt時は上がらないようにする)
            if(upSwitch && count > -HEIGHT && stayCount > STAY_TIME && !psc.IsState(PState.HuntFish))
                count -= speed;
            //上がりきったら
            if(upSwitch && count <= -HEIGHT)
            {
                showFlag = false;
                curMessage = string.Empty;
            }
        }
    }

    private void OnGUI()
    {
        if(showFlag)
            GUI.TextField(new Rect(Screen.width / 4, count - HEIGHT, Screen.width * 2 / 4, HEIGHT), curMessage);
    }
}