using System;
using UnityEngine;

namespace Level.Tutorial
{
    [Serializable]
    public struct ZoneTasks
    {
        [field: SerializeField] public TutorialTask[] tasks { get; private set; }
    }
}
