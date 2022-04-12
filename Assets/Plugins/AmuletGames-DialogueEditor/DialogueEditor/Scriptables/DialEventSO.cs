using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AG
{
    public abstract class DialEventSO : ScriptableObject
    {
        public abstract void Execute();
    }
}