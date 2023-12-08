using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZXFFrame
{
    public class DragHoverBehaviour : MonoBehaviour
    {


        private List<TriggerBehaviour> tbs;
        private Step runStep;
        private bool isRun = true;

        void Start()
        {

        }

        void Update()
        {
            bool curStat = true;

            if(tbs[0].hoverTime<=0)
            {
                curStat = false;
            }

            for (int i = 0; i < tbs.Count - 1; i++)
            {
                if (tbs[i].hoverTime >= tbs[i + 1].hoverTime)
                {
                    curStat = false;
                    break;
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


        public void Init(Step step, List<TriggerBehaviour> tbs)
        {
            this.runStep = step;
            this.tbs = tbs;
        }
    }
}

