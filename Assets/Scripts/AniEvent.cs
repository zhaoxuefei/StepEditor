using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZXFFrame
{
    public class AniEvent : MonoBehaviour
    {
        public void RunStepIndex(string stepId)
        {
            AppRun.instence.RunStepIndex(long.Parse(stepId));
        }
    }
}