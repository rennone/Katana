using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

// 参考
// http://notargs.com/blog/?p=313

/// Rigidbodyの速度を保存しておくクラス
public class RigidbodyPauser
{
    public Vector3 velocity;
    public Vector3 angularVeloccity;
    public Rigidbody rigidbody;

    public RigidbodyPauser(Rigidbody rigid)
    {
        rigidbody = rigid;
        Sleep();
    }

    public void Sleep()
    {

        velocity = rigidbody.velocity;
        angularVeloccity = rigidbody.angularVelocity;
        rigidbody.Sleep();
    }

    public void WakeUp()
    {
        rigidbody.WakeUp();
        rigidbody.velocity = velocity;
        rigidbody.angularVelocity = angularVeloccity;
    }
}

public class Pausable : MonoBehaviour
{
    // ポーズの種類
    public enum PauseState
    {
        Active,       //ポーズ中でない
        PauseAll,     //完全に止まる
        PauseUpdate   //Updateだけ切れてる(アニメーションは動く)
    }

    // 現在のポーズ状態, ポーズさせたい(復帰させたい)場合はこの変数に代入
    public PauseState State = PauseState.Active;

    // 前回のポーズを保存しておく
    private PauseState _prevState;

    /// Rigidbodyのポーズ前の速度の配列
    RigidbodyPauser[] rigidbodyPauser;

    /// ポーズ中のMonoBehaviourの配列
    MonoBehaviour[] _pausingMonoBehaviours;

    // ポーズ中のAnimatorの配列
    private Animator[] _pausingAnimators;

    /// 更新処理
    void Update()
    {
        // ポーズ状態が変更されていたら、Pause/Resumeを呼び出す。
        if (_prevState != State)
        {
            // 新しく子が追加されたりした可能性もあるのでいったんResumeする.
            Resume();
            switch (State)
            {
                case PauseState.PauseAll:
                    PauseMonoBehavior();
                    PauseAnimator();
                    break;
                case PauseState.PauseUpdate:
                    PauseMonoBehavior();
                    break;
                case PauseState.Active:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _prevState = State;
        }
    }


    void SetAnimatorEnable(bool enable)
    {
        if (_pausingAnimators == null)
            return;

        foreach (var animator in _pausingAnimators)
        {
            animator.enabled = enable;
        }
    }

    void SetMonovehaviorEnable(bool enable)
    {
        if(_pausingMonoBehaviours == null)
            return;

        foreach (var behaviour in _pausingMonoBehaviours)
        {
            behaviour.enabled = enable;
        }
    }

    //! 子要素から取得可能条件
    bool Findable(Behaviour b)
    {
        // Pausableはdisableしない && すでにdisableのものは取得しない(Resumeでenableになってしまうので)
        return b != this && b.enabled;
    }

    private void PauseAnimator()
    {
        _pausingAnimators = Array.FindAll(transform.GetComponentsInChildren<Animator>(),Findable);
        SetAnimatorEnable(false);
    }


    private void PauseMonoBehavior()
    {
        // Rigidbodyの停止
        // 子要素から、スリープ中でなく、IgnoreGameObjectsに含まれていないRigidbodyを抽出
        Predicate<Rigidbody> rigidbodyPredicate = obj => !obj.IsSleeping();

        var pausingRigidbodies = Array.FindAll(transform.GetComponentsInChildren<Rigidbody>(), rigidbodyPredicate);
        rigidbodyPauser = new RigidbodyPauser[pausingRigidbodies.Length];
        for (int i = 0; i < pausingRigidbodies.Length; i++)
        {
            // 速度、角速度も保存しておく
            rigidbodyPauser[i] = new RigidbodyPauser(pausingRigidbodies[i]);
        }

        _pausingMonoBehaviours = Array.FindAll(transform.GetComponentsInChildren<MonoBehaviour>(), Findable);
        SetMonovehaviorEnable(false);
    }

    /// 再開
    private void Resume()
    {
        ResumeMonoBehavior();

        // PauseAllだったらAnimatorも再開
        if (_prevState == PauseState.PauseAll)
            ResumeAnimator();
    }

    private void ResumeMonoBehavior()
    {
        // Rigidbodyの再開
        if (rigidbodyPauser != null)
        {
            foreach (var pauser in rigidbodyPauser)
                pauser.WakeUp();
        }

        // MonoBehaviourの再開
        SetMonovehaviorEnable(true);
    }

    void ResumeAnimator()
    {
        SetAnimatorEnable(true);
    }
}