using System.Collections;
using System.Threading;
using UnityEngine;

/// <summary>
/// プレイヤーの状態を管理するシングルトンクラス
/// 状態遷移もコントロールする
/// </summary>
public sealed class PlayerStateController
{
    /// <summary>
    /// プレイヤーの状態を表す列挙型
    /// </summary>
    public enum PlayerState
    {
        None,
        Idle,
        WalkForward,
        WalkBack,
        Run,
        Rotate,
        AimFish,
        Hunt,
        //Climb,
    }

    private static PlayerStateController _singleton;//シングルトンオブジェクト

    private const float MOVE_SENSITIVITY = 0.1f;//ユーザーの移動入力がこの値以上なら移動状態に遷移できる
    private const uint WAIT_TIME_FOR_AIM_TO_AIM = 90U;//AimFish状態から次のAimFish状態に遷移可能になるまでのUpdate呼び出し回数
    private Vector3 inputHV = Vector3.zero;//プレイヤーの縦・横入力値をまとめたもの(y軸は常に0)
    private PlayerState ps;//プレイヤーの現在の状態
    private float inputVartical;//縦移動の入力値
    private float inputHorizontal;//横カメラ移動の入力値
    //Updateが呼び出される度にカウントする(状態遷移管理のためにリセットや取得を行う)
    //Updateはキャラクタースクリプト内のFixedUpdateで呼び出されるため一定の時間でカウントされる
    private uint updateCounterForAimToAim = 0;

    /// <summary>
    /// プライベートコンストラクタ
    private PlayerStateController()
    /// </summary>
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
    /// 引数に指定したステートが現在のステートと等しいかどうか判定する
    /// </summary>
    /// <param name="ps">判定したいステート</param>
    /// <returns></returns>
    public bool IsState(PlayerState ps)
    {
        return GetState() == ps;
    }

    /// <summary>
    /// Idle状態に遷移する
    /// </summary>
    public void SetStateIdle()
    {
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
        JudgeStateHunt();

        ++updateCounterForAimToAim;
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
    /// アップデート呼び出し数カウンタの値をリセットする
    /// このメソッドを呼び出すことでAimFish状態の連続遷移を防げる
    /// </summary>
    public void DisabledAimCertainTime()
    {
        updateCounterForAimToAim = 0;
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
            ps = PlayerState.WalkBack;
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
        //"AimFish"ボタンが押されたかつアップデートカウンタの値が一定値より上ならAimFish
        if(Input.GetButton("AimFish") && updateCounterForAimToAim > WAIT_TIME_FOR_AIM_TO_AIM)
        {
            ps = PlayerState.AimFish;
        }
    }

    private void JudgeStateHunt()
    {
        if(Input.GetButton("Hunt") && IsState(PlayerState.AimFish))
            ps = PlayerState.Hunt;
    }
}