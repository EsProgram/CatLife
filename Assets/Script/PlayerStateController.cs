using System.Collections;
using UnityEngine;

/// <summary>
/// プレイヤーの状態を管理するシングルトンクラス
/// 状態遷移もコントロールする
/// </summary>
public class PlayerStateController
{
    /// <summary>
    /// プレイヤーの状態を表す列挙型
    /// </summary>
    public enum PlayerState
    {
        None,
        Idle,
        WalkForward,
        WarkBack,
        Run,
        Rotate,
        AimFish,
        //Climb,
    }

    private static PlayerStateController _singleton;//シングルトンオブジェクト

    private const float MOVE_SENSITIVITY = 0.1f;//ユーザーの移動入力がこの値以上なら移動状態に遷移できる
    private Vector3 inputHV = Vector3.zero;//プレイヤーの縦・横入力値をまとめたもの(y軸は常に0)
    private PlayerState ps;//プレイヤーの現在の状態
    private float inputVartical;//縦移動の入力値
    private float inputHorizontal;//横カメラ移動の入力値

    /// <summary>
    /// プライベートコンストラクタ
    /// </summary>
    private PlayerStateController()
    {
        ps = PlayerState.Idle;
    }

    /// <summary>
    /// シングルトンオブジェクトを返す
    /// </summary>
    /// <returns>プレイヤーの状態を管理するシングルトンオブジェクト</returns>
    public static PlayerStateController GetInstance()
    {
        if(_singleton == null)
            _singleton = new PlayerStateController();
        return _singleton;
    }

    /// <summary>
    /// プレイヤーの現在の状態を取得する
    /// </summary>
    /// <returns>プレイヤーの現在の状態</returns>
    public PlayerState GetState()
    {
        return ps;
    }

    /// <summary>
    /// AimFish状態での処理が終了したら呼び出す処理
    /// Idle状態に遷移する
    /// </summary>
    public void EndAimFishState()
    {
        //他の状態の時に誤って呼ばれても副作用を起こさないようにする
        if(ps == PlayerState.AimFish)
            ps = PlayerState.Idle;
    }

    /// <summary>
    /// ユーザーの入力によるプレイヤーの状態をアップデートする
    /// </summary>
    public void Update()
    {
        //ユーザーの入力を保存する
        inputHorizontal = inputHV.x = Input.GetAxis("Horizontal");
        inputVartical = inputHV.z = Input.GetAxis("Vertical");

        //ユーザーの入力から状態を決定する処理
        JudgeStateIdle();
        JudgeStateWalkForward();
        JudgeStateWalkBack();
        JudgeStateRun();
        JudgeStateRotate();
        JudgeStateAimFish();
    }

    /// <summary>
    /// 最後に呼ばれたUpdateメソッドで取得したユーザーの垂直方向の入力値を得る
    /// </summary>
    /// <returns>垂直方向の入力値[0 ~ 1]</returns>
    public float GetInputVertical()
    {
        return inputVartical;
    }

    /// <summary>
    /// 最後に呼ばれたUpdateメソッドで取得したユーザーの水平方向の入力値を得る
    /// </summary>
    /// <returns>水平方向の入力値[0 ~ 1]</returns>
    public float GetInputHorizontal()
    {
        return inputHorizontal;
    }

    /// <summary>
    /// ユーザーの入力からIdle状態かどうか判定する
    /// </summary>
    private void JudgeStateIdle()
    {
        //(ユーザによる縦入力がMOVE_SENSITIVITYより下かつAimFishでない)またはAimFishが終了しているならばIdle
        if(Mathf.Abs(inputVartical) < MOVE_SENSITIVITY && ps != PlayerState.AimFish)
            ps = PlayerState.Idle;
    }

    /// <summary>
    /// ユーザーの入力からWalkForward状態かどうか判定する
    /// </summary>
    private void JudgeStateWalkForward()
    {
        //ユーザーによる縦入力がMOVE_SENSITIVITY以上かつAimFishでないならばWalkForward
        if(inputVartical >= MOVE_SENSITIVITY && ps != PlayerState.AimFish)
            ps = PlayerState.WalkForward;
    }

    /// <summary>
    /// ユーザーの入力からWalkBack状態かどうか判定する
    /// </summary>
    private void JudgeStateWalkBack()
    {
        //ユーザーによる縦入力が-MOVE_SENSITIVITY以下かつAimFishでないならばWalkBack
        if(inputVartical <= -MOVE_SENSITIVITY && ps != PlayerState.AimFish)
            ps = PlayerState.WarkBack;
    }

    /// <summary>
    /// ユーザーの入力からRun状態かどうか判定する
    /// </summary>
    private void JudgeStateRun()
    {
        //"Run"ボタンが押されているかつユーザーによって縦入力がMOVE_SENSITIVITY以上かつAimFishでないならばRun
        if(Input.GetButton("Run") && inputVartical >= MOVE_SENSITIVITY && ps != PlayerState.AimFish)
            ps = PlayerState.Run;
    }

    /// <summary>
    /// ユーザーの入力からRotate状態かどうか判定する
    /// </summary>
    private void JudgeStateRotate()
    {
        //ユーザーによる横入力の絶対値がMOVE_SENSITIVITY以上かつAimFishでないならRotate
        if(Mathf.Abs(inputHorizontal) >= MOVE_SENSITIVITY && ps != PlayerState.AimFish)
            ps = PlayerState.Rotate;
    }

    /// <summary>
    /// ユーザーの入力からAimFish状態かどうか判定する
    /// </summary>
    private void JudgeStateAimFish()
    {
        //"AimFish"ボタンが押されたかつIdleならAimFish
        if(Input.GetButtonDown("AimFish") && ps == PlayerState.Idle)
        {
            ps = PlayerState.AimFish;
        }
    }
}