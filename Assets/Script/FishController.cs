using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FishController : MonoBehaviour
{
    private AimControl ac;
    private CharacterController cc;

    /*ゲージ関連*/
    [SerializeField]
    private float gaugeSpeed;
    [SerializeField]
    private float permitWidth;

    /// <summary>
    /// この魚のゲージ速度を返す
    /// </summary>
    public float GaugeSpeed { get { return gaugeSpeed; } }

    /// <summary>
    /// 許可範囲を返す
    /// </summary>
    public float PermitWidth { get { return permitWidth; } }

    private void Awake()
    {
        ac = AimControl.GetInstance();
    }

    private void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    private void Update()
    {
        //Aim範囲内なら停止
        //それ以外なら数秒動いて数秒停止
        if(ac.IsAim()) { Debug.Log("お魚が狙われてるよ");/*何もしない(停止)*/ }
        else
        {
            Debug.Log("お魚は狙われてないよ");
            //moveFlagがtrueなら動く
            //falseなら何もしない
            //moveFlagはUpdate回数で管理？
        }
    }
}