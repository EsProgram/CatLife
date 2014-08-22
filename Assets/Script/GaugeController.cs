using System;
using System.Collections;
using UnityEngine;

public class GaugeController
{
    private static GaugeController _singleton;
    private GUITexture gauge;
    private GUITexture frame;
    private float gaugeWidth;

    private const float MAX = 255f;//ゲージの最大値

    /// <summary>
    /// ゲージコントローラーのインスタンスを生成する
    /// </summary>
    /// <param name="gauge">ゲージ</param>
    /// <param name="frame">フレーム</param>
    /// <returns></returns>
    public static GaugeController CreateInstance(GUITexture gauge, GUITexture frame)
    {
        if(_singleton == null)
            _singleton = new GaugeController(gauge, frame);
        return _singleton;
    }

    /// <summary>
    /// CreateInstanceによって生成されたシングルトンオブジェクトへの参照を返す
    /// CreateInstanceによってインスタンスが生成されていない場合Nullを返す
    /// </summary>
    /// <returns>シングルトンオブジェクト</returns>
    public static GaugeController GetInstance()
    {
        return _singleton ?? null;
    }

    /// <summary>
    /// 使用しない
    /// </summary>
    [Obsolete("使用できません")]
    private GaugeController()
    {
    }

    /// <summary>
    /// プライベートコンストラクタ
    /// </summary>
    /// <param name="gauge">ゲージ</param>
    /// <param name="frame">ゲージの枠</param>
    private GaugeController(GUITexture gauge, GUITexture frame)
    {
        this.gauge = gauge;
        this.frame = frame;
    }

    /// <summary>
    /// ゲージの表示非表示を設定する
    /// </summary>
    /// <param name="d">ゲージを表示するならtrue,非表示にするならfalse</param>
    public void GaugeEnabled(bool d)
    {
        gauge.enabled = d;
        frame.enabled = d;
    }

    /// <summary>
    /// ゲージを動かす
    /// </summary>
    /// <param name="gaugeSpeed">
    /// ゲージ速度
    /// </param>
    public void GaugeMove(float gaugeSpeed)
    {
        gaugeWidth = Mathf.PingPong(Time.time * MAX * gaugeSpeed, MAX);
        gauge.pixelInset = new Rect(gauge.pixelInset.xMin,
                                    gauge.pixelInset.yMin,
                                    gaugeWidth,
                                    gauge.pixelInset.height);
    }

    /// <summary>
    /// このメソッド呼び出し事のゲージの値を返す
    /// </summary>
    /// <returns>ゲージの停止した幅</returns>
    public float GetGaugeValue()
    {
        return gaugeWidth;
    }

    /// <summary>
    /// ゲージが表示されているかどうかの真偽値を返す
    /// </summary>
    /// <returns>表示されていればtrue、それ以外ならfalse</returns>
    public bool IsGaugeEnabled()
    {
        if(gauge.enabled && frame.enabled)
            return true;
        else
            return false;
    }
}