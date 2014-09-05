using System;
using System.Collections;
using UnityEngine;

public sealed class GaugeController
{
    private static GaugeController _singleton;
    private GUITexture gauge;
    private GUITexture frame;
    private GUITexture permit;
    private float gaugeWidth;

    public const float MAX = 244f;//ゲージの最大値
    public const float HALF = MAX / 2;//ゲージの半数
    private const float FRAME_SIZE = 6f;//枠の幅

    /// <summary>
    /// ゲージコントローラーのインスタンスを生成する
    /// </summary>
    /// <param name="gauge">ゲージ</param>
    /// <param name="frame">フレーム</param>
    /// <param name="permit">許可範囲</param>
    /// <returns></returns>
    public static GaugeController GetInstance(GUITexture gauge, GUITexture frame, GUITexture permit)
    {
        if(_singleton == null)
            _singleton = new GaugeController(gauge, frame, permit);
        return _singleton;
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
    /// <param name="permit">許可範囲</param>
    private GaugeController(GUITexture gauge, GUITexture frame, GUITexture permit)
    {
        this.gauge = gauge;
        this.frame = frame;
        this.permit = permit;
    }

    /// <summary>
    /// ゲージの表示非表示を設定する
    /// </summary>
    /// <param name="d">ゲージを表示するならtrue,非表示にするならfalse</param>
    public void SetEnabled(bool d)
    {
        gauge.enabled = d;
        frame.enabled = d;
        permit.enabled = d;
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
    /// ゲージが許可範囲内ならtrue
    /// </summary>
    /// <returns>成否</returns>
    public bool IsPermit()
    {
        if(permit.pixelInset.x + HALF <= gaugeWidth && gaugeWidth <= permit.pixelInset.x + HALF + permit.pixelInset.width)
            return true;
        return false;
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
    /// 許可範囲のxオフセットと幅を設定する
    /// x + widh の値が不正な値を取る場合は何もしない
    /// </summary>
    /// <param name="x">0 ~ 244</param>
    /// <param name="width">x + width が 0 ~ 244 になる値</param>
    public void SetPermitXAndWidth(float x, float width)
    {
        if(x < 0f || width < 0f || MAX < x + width)
        {
            Debug.LogError(string.Format("Permitに不正な値が設定されました\nx = {0}, width = {1}", x, width));
            return;
        }
        permit.pixelInset = new Rect(x - HALF, -10f, width, 20f);
    }

    /// <summary>
    /// ゲージが表示されているかどうかの真偽値を返す
    /// </summary>
    /// <returns>表示されていればtrue、それ以外ならfalse</returns>
    public bool IsEnabled()
    {
        if(gauge.enabled && frame.enabled && permit.enabled)
            return true;
        else
            return false;
    }
}