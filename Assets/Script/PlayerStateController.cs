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
        Walk = 0x0002,
        Run = 0x0004,
        Rotate = 0x0008,
        WalkRotate = Walk | Rotate,
        RunRotate = Run | Rotate,
        AimFish = 0x0010,
        HuntFish = 0x0020,
        AimMouse = 0x0040,
        HuntMouse = 0x0080,
        Aim = AimFish | AimMouse,
        Hunt = HuntFish | HuntMouse,
    }

    private static PlayerStateController _singleton;//シングルトンオブジェクト

    private const float MOVE_SENSITIVITY = 0.1f;//ユーザーの移動入力がこの値以上なら移動状態に遷移できる
    private const uint WAIT_TIME_FOR_AIM_TO_AIM = 90U;//AimFish状態から次のAimFish状態に遷移可能になるまでのUpdate呼び出し回数
    private PlayerState ps;//プレイヤーの現在の状態
    private bool setHuntMouseFlag;//HuntMouse状態に入れるかどうかを表すフラグ
    private float inputVartical;//縦移動の入力値
    private float inputHorizontal;//横カメラ移動の入力値
    private bool inputAim;
    private bool inputHunt;
    private bool inputOK;
    private bool inputRun;
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

    public void SetHuntMouse()
    {
        setHuntMouseFlag = true;
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
        if(!IsState(PlayerState.Hunt | PlayerState.Aim))
        {
            JudgeInputIdle();
            JudgeInputWalk();
            JudgeInputRun();
            JudgeInputRotate();
            JudgeInputWalkRotate();
            JudgeInputRunRotate();
            JudgeInputAimFish();
            JudgeInputAimMouse();
        }
        if(IsState(PlayerState.AimFish))
            JudgeInputHuntFish();
        if(IsState(PlayerState.AimMouse))
            JudgeInputHuntMouse();

        ++updateCounterForAimToAim;
    }

    /// <summary>
    /// ユーザーからの入力を更新する
    /// </summary>
    private void GetUserInput()
    {
        inputHorizontal = Input.GetAxis("Horizontal");
        inputVartical = Input.GetAxis("Vertical");
        inputRun = Input.GetButton("Run");
        inputAim = IsState(PlayerState.Aim) ? true : Input.GetButtonDown("Aim");
        if(IsState(PlayerState.Aim | PlayerState.Hunt))
            inputHunt = Input.GetButtonDown("Hunt");
        else
            inputHunt = false;
        inputOK = Input.GetButtonDown("OK");
    }

    /// <summary>
    /// Huntボタンが押されたかどうかを返す
    /// </summary>
    /// <returns></returns>
    public bool GetInputHunt()
    {
        return inputHunt;
    }

    /// <summary>
    /// Aimボタンが押されたかどうかを返す
    /// </summary>
    /// <returns></returns>
    public bool GetInputAim()
    {
        return inputAim;
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
    /// Runボタンが押されたかどうかを返す
    /// </summary>
    /// <returns></returns>
    public bool GetInputRun()
    {
        return inputRun;
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
    /// 指定したタグをもつオブジェクトがAim状態に推移できるかどうかを調べる
    /// </summary>
    /// <param name="tag">調べたいオブジェクトのタグ</param>
    /// <returns></returns>
    private bool IsAimCondition(string tag)
    {
        if(inputAim && updateCounterForAimToAim > WAIT_TIME_FOR_AIM_TO_AIM && ac.CompareAimObjectTag(tag))
            return true;
        return false;
    }

    /// <summary>
    /// ユーザーの入力からIdle状態かどうか判定する
    /// </summary>
    private void JudgeInputIdle()
    {
        //ユーザーによる入力がなければ
        if(Mathf.Abs(inputVartical) < MOVE_SENSITIVITY && Mathf.Abs(inputHorizontal) <= MOVE_SENSITIVITY
            && !inputRun && !inputAim && !inputHunt)
            ps = PlayerState.Idle;
    }

    /// <summary>
    /// ユーザーの入力からWalkForward状態かどうか判定する
    /// </summary>
    private void JudgeInputWalk()
    {
        //ユーザーによる縦入力がMOVE_SENSITIVITY以上
        if(Mathf.Abs(inputVartical) >= MOVE_SENSITIVITY)
            ps = PlayerState.Walk;
    }

    /// <summary>
    /// ユーザーの入力からRun状態かどうか判定する
    /// </summary>
    private void JudgeInputRun()
    {
        //"Run"ボタンが押されているかつユーザーによって縦入力がMOVE_SENSITIVITY以上
        if(inputRun && inputVartical >= MOVE_SENSITIVITY)
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
    /// ユーザーの入力からWalkRotate状態かどうか判定する
    /// </summary>
    private void JudgeInputWalkRotate()
    {
        if(Mathf.Abs(inputVartical) >= MOVE_SENSITIVITY && Mathf.Abs(inputHorizontal) >= MOVE_SENSITIVITY)
            ps = PlayerState.WalkRotate;
    }

    private void JudgeInputRunRotate()
    {
        if(inputRun && inputVartical >= MOVE_SENSITIVITY && Mathf.Abs(inputHorizontal) >= MOVE_SENSITIVITY)
            ps = PlayerState.RunRotate;
    }

    /// <summary>
    /// ユーザーの入力からAimFish状態かどうか判定する
    /// </summary>
    private void JudgeInputAimFish()
    {
        //"Aim"ボタンが押されたかつアップデートカウンタの値が一定値より上かつ魚を狙ってる
        if(IsAimCondition("Fish"))
            ps = PlayerState.AimFish;
    }

    /// <summary>
    /// ユーザーからの入力がHuntFish状態かどうか判定する
    /// </summary>
    private void JudgeInputHuntFish()
    {
        if(inputHunt)
            ps = PlayerState.HuntFish;
    }

    /// <summary>
    /// ユーザーからの入力がAimMouse状態かどうかを判定する
    /// </summary>
    private void JudgeInputAimMouse()
    {
        if(IsAimCondition("Mouse"))
            ps = PlayerState.AimMouse;
    }

    /// <summary>
    /// ユーザーからの入力がHuntMouse状態かどうかを判定する
    /// </summary>
    private void JudgeInputHuntMouse()
    {
        if(setHuntMouseFlag && inputHunt)
        {
            ps = PlayerState.HuntMouse;
            setHuntMouseFlag = false;
        }
    }
}