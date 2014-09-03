using System.Collections;
using System.Threading;
using UnityEngine;

/// <summary>
/// プレイヤーの状態を管理するシングルトンクラス
/// 状態遷移・入力情報もコントロールする
/// </summary>
public sealed class PlayerStateController
{
    /// <summary>
    /// プレイヤーの状態を表す列挙型
    /// </summary>
    [System.Flags, System.Serializable]
    public enum PlayerState
    {
        None = 0x0000,
        Idle = 0x0001,
        WalkForward = 0x0002,
        WalkBack = 0x0004,
        Run = 0x0008,
        Rotate = 0x0010,
        AimFish = 0x0020,
        HuntFish = 0x0040,
    }

    private static PlayerStateController _singleton;//シングルトンオブジェクト

    private const float MOVE_SENSITIVITY = 0.1f;//ユーザーの移動入力がこの値以上なら移動状態に遷移できる
    private const uint WAIT_TIME_FOR_AIM_TO_AIM = 90U;//AimFish状態から次のAimFish状態に遷移可能になるまでのUpdate呼び出し回数
    private Vector3 inputHV = Vector3.zero;//プレイヤーの縦・横入力値をまとめたもの(y軸は常に0)
    private PlayerState ps;//プレイヤーの現在の状態
    private float inputVartical;//縦移動の入力値
    private float inputHorizontal;//横カメラ移動の入力値
    private bool inputAimFish;
    private bool inputHunt;
    private bool inputOK;
    //Updateが呼び出される度にカウントする(状態遷移管理のためにリセットや取得を行う)
    //Updateはキャラクタースクリプト内のFixedUpdateで呼び出されるため一定の時間でカウントされる
    private uint updateCounterForAimToAim = 0;
    private AimControl ac;

    /// <summary>
    /// プライベートコンストラクタ
    private PlayerStateController()
    /// </summary>
    {
        ps = PlayerState.Idle;
        ac = GameObject.FindObjectOfType<AimControl>();
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
        if((GetState() & ps) != PlayerState.None)
            return true;
        return false;
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
    public void UpdateState()
    {
        //ユーザーの入力を更新
        GetUserInput();

        //現在の状態から遷移できる状態を判定する
        if(!IsState(PlayerState.HuntFish))
        {
            JudgeInputIdle();
            JudgeInputWalkForward();
            JudgeInputWalkBack();
            JudgeInputRun();
            JudgeInputRotate();
            JudgeInputAimFish();
        }
        if(IsState(PlayerState.AimFish))
            JudgeInputHuntFish();

        ++updateCounterForAimToAim;
    }

    /// <summary>
    /// ユーザーからの入力を更新する
    /// </summary>
    private void GetUserInput()
    {
        inputHorizontal = inputHV.x = Input.GetAxis("Horizontal");
        inputVartical = inputHV.z = Input.GetAxis("Vertical");
        inputAimFish = IsState(PlayerState.AimFish) ? true : Input.GetButton("AimFish");
        if(IsState(PlayerState.AimFish))
            inputHunt = Input.GetButton("Hunt");
        else
            inputHunt = false;
        inputOK = Input.GetButton("OK");
    }

    /// <summary>
    /// OKボタンが押されたかどうかを返す
    /// </summary>
    /// <returns></returns>
    public bool GetInputOK()
    {
        return inputOK;
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
    private void JudgeInputIdle()
    {
        //(ユーザによる縦入力がMOVE_SENSITIVITYより下
        if(Mathf.Abs(inputVartical) < MOVE_SENSITIVITY)
            ps = PlayerState.Idle;
    }

    /// <summary>
    /// ユーザーの入力からWalkForward状態かどうか判定する
    /// </summary>
    private void JudgeInputWalkForward()
    {
        //ユーザーによる縦入力がMOVE_SENSITIVITY以上
        if(inputVartical >= MOVE_SENSITIVITY)
            ps = PlayerState.WalkForward;
    }

    /// <summary>
    /// ユーザーの入力からWalkBack状態かどうか判定する
    /// </summary>
    private void JudgeInputWalkBack()
    {
        //ユーザーによる縦入力が-MOVE_SENSITIVITY以下
        if(inputVartical <= -MOVE_SENSITIVITY)
            ps = PlayerState.WalkBack;
    }

    /// <summary>
    /// ユーザーの入力からRun状態かどうか判定する
    /// </summary>
    private void JudgeInputRun()
    {
        //"Run"ボタンが押されているかつユーザーによって縦入力がMOVE_SENSITIVITY以上
        if(Input.GetButton("Run") && inputVartical >= MOVE_SENSITIVITY)
            ps = PlayerState.Run;
    }

    /// <summary>
    /// ユーザーの入力からRotate状態かどうか判定する
    /// </summary>
    private void JudgeInputRotate()
    {
        //ユーザーによる横入力の絶対値がMOVE_SENSITIVITY以上
        if(Mathf.Abs(inputHorizontal) >= MOVE_SENSITIVITY)
            ps = PlayerState.Rotate;
    }

    /// <summary>
    /// ユーザーの入力からAimFish状態かどうか判定する
    /// </summary>
    private void JudgeInputAimFish()
    {
        //"AimFish"ボタンが押されたかつアップデートカウンタの値が一定値より上かつ魚を狙ってる
        if(inputAimFish && updateCounterForAimToAim > WAIT_TIME_FOR_AIM_TO_AIM && ac.CompareAimObjectTag("Fish"))
        {
            ps = PlayerState.AimFish;
        }
    }

    /// <summary>
    /// ユーザーからの入力がHuntFish状態かどうか判定する
    /// </summary>
    private void JudgeInputHuntFish()
    {
        if(inputHunt)
            ps = PlayerState.HuntFish;
    }
}