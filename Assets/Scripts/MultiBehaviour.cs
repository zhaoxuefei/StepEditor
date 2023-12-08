using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZXFFrame
{
    public class MultiBehaviour : MonoBehaviour
    {

        private Step runStep;
        private List<Step> multiSteps;
        private bool isRun = true;
        void Start()
        {

        }


        void Update()
        {
            bool curStat = true;
            for (int i = 0; i < multiSteps.Count; i++)
            {
                if (!multiSteps[i].isTriggered)
                {
                    curStat = false;
                }
            }

            if (curStat != isRun)
            {
                isRun = curStat;

                if (isRun)
                {
                    AppRun.instence.RunStep(runStep);
                }
                else
                {
                    AppRun.instence.PauseStep(runStep);
                }
            }
        }


        public void Init(Step runStep, List<Step> multiSteps)
        {

            this.runStep = runStep;
            this.multiSteps = multiSteps;
        }
    }
}


