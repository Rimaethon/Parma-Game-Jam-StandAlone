using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectTimerController : MonoBehaviour
{
    public bool PlayOnAwake;
    public UnityEvent OnReset;
    public float NormalizedTime { get
        {
            if (CurrentSection >= Sequence.Length || Sequence[CurrentSection].Time<=0) return 0;
            return Timer / Sequence[CurrentSection].Time;
        }
    }
    public float Timer { get; private set; }
    public int CurrentSection { get; private set; }
    public SequenceSection[] Sequence;


    void Awake()
    {
        if (PlayOnAwake) StartTimer();
    }
    public void ResetTimer()
    {
        StopAllCoroutines();
        Timer = 0;
        CurrentSection = 0;
        OnReset.Invoke();
    }
    public void StartTimer()
    {
        if (Timer==0 && CurrentSection==0) StartCoroutine(Countdown());
    }

    IEnumerator Countdown()
    {
        for (CurrentSection=0; CurrentSection<Sequence.Length; CurrentSection++)
        {
            for (Timer = Sequence[CurrentSection].Time; Timer > 0; Timer = Mathf.Clamp(Timer-Time.deltaTime, 0, Sequence[CurrentSection].Time)) yield return null;
            Sequence[CurrentSection].OnTimeElapsed.Invoke();
        }
        ResetTimer();
    }
    [System.Serializable]
    public class SequenceSection
    {
        public string name;
        public float Time = 1;
        public UnityEvent OnTimeElapsed;
    }
}
