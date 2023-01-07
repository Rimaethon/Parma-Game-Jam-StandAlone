using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ObjectOverallAudio : MonoBehaviour
{
    public bool PlayOnAwake;
    public SoundVariant[] SoundVariants;
    private AudioSource source;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        if (PlayOnAwake && SoundVariants.Length > 0) PlaySound(0);
    }
    public void PlaySound(int index)
    {
        source.pitch = Time.timeScale;
        source.PlayOneShot(SoundVariants[index].clips[Random.Range(0,SoundVariants[index].Length)]);
    }
    [System.Serializable]
    public class SoundVariant
    {
        public int Length { get => clips.Length; }
        public AudioClip[] clips;
    }
}
