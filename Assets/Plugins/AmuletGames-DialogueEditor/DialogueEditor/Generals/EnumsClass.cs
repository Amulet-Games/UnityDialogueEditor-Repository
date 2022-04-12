using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AG
{
    public class EnumsClass : MonoBehaviour
    {
    }

    public enum G_LanguageTypeEnum
    {
        English,
        German,
        Danish,
        Spanish,
        Japanese,
        Latin
    }

    public enum N_EndNodeTypeEnum
    {
        End,
        Repeat,
        Goback,
        ReturnToStart
    }

    public enum N_AvatarDirectionTypeEnum
    {
        Left,
        Right
    }

    public enum N_NodeTypeEnum
    {
        Start,          // Green        Color(0.29f, 1, 0.34f)
        Dialogue,       // Red          Color(1, 0.29f, 0.29f)
        Event,          // Purple       Color(0.89f, 0.29f, 1)
        End             // Green        Color(0.29f, 1, 0.34f)
    }
}