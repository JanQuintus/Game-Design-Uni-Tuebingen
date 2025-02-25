﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AudioClipBundle
{
    [SerializeField] private AudioClip[] clips;

    public AudioClip GetRandomClip()
    {
        if (clips.Length == 0)
            return null;
        return clips[Random.Range(0, clips.Length)];
    }
}
