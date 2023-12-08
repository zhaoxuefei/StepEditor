using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.Video;

namespace ZXFFrame
{
    public class AppRun : MonoBehaviour
    {
        public static AppRun instence;


        private Courseware curCour;
        private bool isFree = false;//自由模式
        public List<GameObject> hideObjs;

        public Text courText;
        public Button FreeBtn;
        public Button OrderBtn;
        public Text stepText;
        public GameObject videoPanel;

        private string stepExp;

        private Dictionary<long, Step> dicSteps = new Dictionary<long, Step>();

        private void Awake()
        {
            instence = this;
        }

        private void Start()
        {
            string configPath = "Assets/Resources/path.txt";
            string xmlPath = File.ReadAllText(configPath);
            Debug.Log(xmlPath);
            //string path = Path.Combine(Application.dataPath, "XmlFiles", "爆炸实验_11_24_09_30_01.xml");
            //string path = Path.Combine(Application.dataPath, "XmlFiles", "炸炸炸炸炸炸炸炸炸炸炸炸_11_24_10_15_24.xml");
            //string path = Path.Combine(Application.dataPath, "XmlFiles", "testcube_11_28_11_04_45.xml");
            //string path = Path.Combine(Application.dataPath, "XmlFiles", "隔离场景_11_23_08_34_28.xml");

            curCour = SerializeHelper.DeserializeWithXmlFile<Courseware>(xmlPath);
            Debug.Log("加载课件成功");

            FreeBtn.onClick.AddListener(FreedStep);
            OrderBtn.onClick.AddListener(OrderStep);

            hideObjs = new List<GameObject>();

            InitCourseware();
        }

        private void OrderStep()
        {
            isFree = false;
            InitStep(curCour.firstStep);
        }

        private void FreedStep()
        {
            isFree = true;

            //foreach (var step in curCour.dicSteps)
            //{
            //    InitStep(step.Value);
            //}
        }


        /// <summary>
        /// Inits the courseware.
        /// </summary>
        private void InitCourseware()
        {

            string text = "课件名称:" + curCour.name + "\n";
            courText.text = text;
            for (int i = 0; i < curCour.steps.Count; i++)
            {
                dicSteps.Add(curCour.steps[i].id, curCour.steps[i]);
                bool isfirst = true;
                for (int h = 0; h < curCour.connects.Count; h++)
                {
                    if (curCour.connects[h].to == curCour.steps[i].id)
                    {
                        isfirst = false;
                    }
                }
                if (isfirst)
                {
                    curCour.firstStep = curCour.steps[i];
                }
            }
        }


        /// <summary>
        /// Inits the step.
        /// </summary>
        private void InitStep(Step step)
        {

            stepExp = "步骤名称：" + step.name + "\n";
            stepExp += "初始化完成，等待执行" + "\n";
            stepText.text = stepExp;

            if (step.isHaveAA)
            {
                AnimatorAction aa = step.aa;
                GameObject go = GameObject.Find(aa.aaObjName);
                go.AddComponent<AniEvent>();
                Animation anim = go.GetComponent<Animation>();

                AnimationClip clip = null;

                foreach (AnimationState state in anim)
                {
                    if (state.clip.name == aa.clipName)
                    {
                        clip = state.clip;
                        break;
                    }
                }

                if (clip)
                {
                    AnimationEvent evt = new AnimationEvent();
                    float clipTime = clip.length;
                    float frameRate = clip.frameRate;
                    int totalFrame = (int)(clipTime / (1f / frameRate));//总帧数
                    evt.time = clipTime * aa.actionIndex * (1f / totalFrame);
                    evt.stringParameter = aa.stepId.ToString();
                    evt.functionName = "RunStepIndex";
                    clip.AddEvent(evt);
                }
            }

            if (step.triggerMode == TriggerMode.Multi)
            {
                MultiBehaviour mb = GameObject.Find(step.executors[0].name).GetOrCreatComponent<MultiBehaviour>();
                List<Step> multiSteps = new List<Step>();
                for (int i = 0; i < step.preStepIndexs.Count; i++)
                {
                    //multiSteps.Add(curCour.dicSteps[step.preStepIndexs[i]]);
                }
                mb.Init(step, multiSteps);
                return;
            }

            if (step.triggerMode == TriggerMode.DragHover)
            {
                List<TriggerBehaviour> tbs = new List<TriggerBehaviour>();

                for (int i = 0; i < step.hoverObjNames.Count; i++)
                {
                    GameObject dragHoverObj = GameObject.Find(step.hoverObjNames[i]);
                    TriggerBehaviour tb1 = dragHoverObj.GetOrCreatComponent<TriggerBehaviour>();
                    tb1.Init(step);
                    tbs.Add(tb1);
                }

                DragHoverBehaviour dhb = GameObject.Find(step.executors[0].name).GetOrCreatComponent<DragHoverBehaviour>();
                dhb.Init(step, tbs);

                return;
            }



            GameObject dragObj = GameObject.Find(step.triggerObjNames[0]);

            if (!dragObj)
            {
                for (int i = 0; i < hideObjs.Count; i++)
                {
                    if (hideObjs[i].name == step.triggerObjNames[0])
                    {
                        dragObj = hideObjs[i];
                    }
                }
            }



            TriggerBehaviour tb = dragObj.GetOrCreatComponent<TriggerBehaviour>();
            tb.Init(step);
            tb.triggerAction = RunStep;

            for (int i = 0; i < step.showObjs.Count; i++)
            {
                var showObj = GameObject.Find(step.showObjs[i]);
                if (showObj)
                {
                    showObj.SetActive(true);
                }
                else
                {
                    for (int j = 0; j < hideObjs.Count; j++)
                    {
                        if (hideObjs[j].name == step.showObjs[i])
                        {
                            hideObjs[j].SetActive(true);
                            hideObjs.RemoveAt(j);
                        }
                    }
                }
            }

            for (int i = 0; i < step.hideObjs.Count; i++)
            {
                var hideObj = GameObject.Find(step.hideObjs[i]);
                if (hideObj)
                {
                    hideObjs.Add(hideObj);
                    hideObj.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Runs the step.
        /// </summary>
        public void RunStep(Step step)
        {


            float waitTime = 0f;

            for (int i = 0; i < step.executors.Count; i++)
            {
                Executor executor = step.executors[i];//粒子播放------同动画一样，检索组件------

                GameObject obj = GameObject.Find(executor.name);
                if (!obj)
                {
                    for (int j = 0; j < hideObjs.Count; j++)
                    {
                        if (hideObjs[j].name == executor.name)
                        {
                            obj = hideObjs[j];
                            obj.SetActive(true);
                            hideObjs.Remove(obj);
                        }
                    }
                }

                if (obj)
                {
                    if (executor.isHide)
                    {
                        hideObjs.Add(obj);
                        obj.SetActive(false);
                    }

                    //粒子
                    //if (obj.GetComponent<ParticleSystem>())
                    //{
                    //    obj.GetComponent<ParticleSystem>().Play();
                    //}

                    //path
                    float moveTime = 0f;

                    if (executor.path.Count > 0)
                    {
                        Sequence sequence = DOTween.Sequence();
                        for (int j = 0; j < executor.path.Count; j++)
                        {
                            sequence.Append(obj.transform.DOMove(executor.path[j].tran.pos.GetVector3(), executor.path[j].localTime));
                            sequence.Join(obj.transform.DORotate(executor.path[j].tran.rot.GetVector3(), executor.path[j].localTime));
                            moveTime += executor.path[j].localTime;
                        }
                        sequence.Play();
                        waitTime = Mathf.Max(waitTime, moveTime);
                    }


                    //视频
                    //VideoPlayer vp = GetComponent<VideoPlayer>();
                    //http://101.201.211.154:8080/%E6%8E%98%E8%BF%9B%E6%95%B0%E5%AD%97%E5%AD%AA%E7%94%9F.mp4
                    //vp.url = executor.videoUrl = "http://101.201.211.154:8080/%E6%8E%98%E8%BF%9B%E6%95%B0%E5%AD%97%E5%AD%AA%E7%94%9F.mp4";
                    //vp.Play();
                    //if (vp.isPlaying)
                    //{
                    //    videoPanel.SetActive(true);
                    //}

                    //shader
                    GameObject shaderObj = GameObject.Find(executor.shaderCheck.objName);
                    if (shaderObj)
                    {
                        Material material = shaderObj.GetComponent<MeshRenderer>().sharedMaterial;
                        int index = material.shader.FindPropertyIndex(executor.shaderCheck.variableName);
                        var sType = material.shader.GetPropertyType(index);
                        switch (sType)
                        {
                            case UnityEngine.Rendering.ShaderPropertyType.Color:
                                material.SetColor(executor.shaderCheck.variableName, executor.shaderCheck.cValue.GetColor());
                                break;
                            case UnityEngine.Rendering.ShaderPropertyType.Float:
                            case UnityEngine.Rendering.ShaderPropertyType.Range:
                                material.SetFloat(executor.shaderCheck.variableName, executor.shaderCheck.fValue);
                                break;
                            default:
                                break;
                        }
                    }

                    //跟随，lookat
                    var bm = executor.ob.behaviourMode;
                    GameObject go = GameObject.Find(executor.ob.objName);
                    GameObject ta = GameObject.Find(executor.ob.target);
                    if (go)
                    {
                        switch (bm)
                        {
                            case BehaviourMode.Follow:
                                go.transform.position = ta.transform.position + executor.ob.offset.GetVector3();
                                go.transform.SetParent(ta.transform);
                                break;
                            case BehaviourMode.LookAt:
                                go.transform.LookAt(ta.transform);
                                break;
                            case BehaviourMode.Stillness:
                                break;
                            default:
                                break;
                        }
                    }


                    //动画播放
                    GameObject animObj = GameObject.Find(executor.name);
                    if (executor.isUseAnim)
                    {
                        var anim = animObj.GetComponent<Animation>();
                        anim.enabled = true;
                        if (animObj && anim)
                        {
                            //----------求播放时间点-----------------//

                            AnimationClip ac = null;
                            foreach (AnimationState state in anim)
                            {
                                if (state.clip.name == executor.anima.animationClip)
                                {
                                    ac = state.clip;
                                    break;
                                }
                            }


                            float playtime = 0f;
                            float clipTime = 0f;
                            if (ac)
                            {
                                clipTime = ac.length;
                                float frameRate = ac.frameRate;
                                int totalFrame = (int)(clipTime / (1f / frameRate));//总帧数
                                playtime = clipTime * executor.anima.playFrame * (1f / totalFrame);
                                //----------求播放时间点-----------------//

                                var a = anim[executor.anima.animationClip];
                                a.speed = executor.anima.speed;
                                anim[executor.anima.animationClip].time = playtime;
                                if (executor.isLoop)
                                {
                                    anim.wrapMode = WrapMode.Loop;
                                }
                                else
                                {
                                    anim.wrapMode = WrapMode.Once;
                                }
                                anim.Play(executor.anima.animationClip);
                                waitTime = Mathf.Max(waitTime, clipTime - playtime);
                            }
                        }
                    }
                }
            }
            if (step.triggerMode == TriggerMode.Wait)
            {
                waitTime = step.waitTime;
            }

            stepExp += "步骤正在执行\n";
            stepExp += "步骤长度：" + waitTime + "\n";
            stepText.text = stepExp;

            gameObject.transform.DOMove(Vector3.zero, waitTime).OnComplete(() => { EndStep(step); });

            //executor.isFold
            //播放动画---------14号完成--------===================================
            //动画播放事件，根据帧触发-------18号完成---------===================================
            //物体显示隐藏---------14号完成---------============================================
            //可选步骤-------14号完成------==================================================
            //分割动画---10000----0-234---235-627---628---1000---要单独做，0.5天
            //倍速执行------单动画倍速播放---18号------========================================
            //播放视频----------14号完成-----------------------===========================================
            //相机动作----切换相机---跟随---注视--18号完成---------==================================
            //触发方式，等待------14号完成---------------===================================================
            //path路径点移动，dotweeen来做就行----------18号完成---------=======================================
            //切换sahder方法。物体---材质---属性名称--（属性类型）--和值------16号完成---=============================
            //自由模式下都所有步骤初始化-----------==========================================
            //可选步骤-----------==========================================
            //添加步骤可以拖动位置-----------
            //执行文件暴露在编辑器修改-------==========================================
            //多点交互，提供素材制作--------2个，脚本制作----------==========================================
            //心脏按压，诊断学----------脚本制作----------------==========================================
            //粒子播放--------------------------14号完成---------=============================================
            //完成后回调EndStep()；
        }


        public void PauseStep(Step step)
        {
            for (int i = 0; i < step.executors.Count; i++)
            {
                Executor executor = step.executors[i];//粒子播放------同动画一样，检索组件------

                //动画播放
                GameObject animObj = GameObject.Find(executor.name);
                if (executor.isUseAnim)
                {
                    var anim = animObj.GetComponent<Animation>();
                    anim.enabled = false;
                }
            }
        }

        /// <summary>
        /// Ends the step.
        /// </summary>
        private void EndStep(Step step)
        {
            if (step.triggerObjNames.Count > 0)
            {
                if (step.triggerMode != TriggerMode.Multi)
                {
                    GameObject obj = GameObject.Find(step.triggerObjNames[0]);
                    if (!obj)
                    {
                        for (int j = 0; j < hideObjs.Count; j++)
                        {
                            if (hideObjs[j].name == step.triggerObjNames[0])
                            {
                                obj = hideObjs[j];

                            }
                        }
                    }
                    DestroyImmediate(obj.GetComponent<TriggerBehaviour>());
                }

            }


            if (step.isHaveAA)
            {
                AnimatorAction aa = step.aa;
                GameObject go = GameObject.Find(aa.aaObjName);
                DestroyImmediate(go.GetComponent<AniEvent>());
                Animation anim = go.GetComponent<Animation>();
                AnimationClip clip = null;

                foreach (AnimationState state in anim)
                {
                    if (state.clip.name == aa.clipName)
                    {
                        clip = state.clip;
                        break;
                    }
                }

                if (clip)
                {
                    clip.events = null;
                }



            }
            //VideoPlayer vp = GetComponent<VideoPlayer>();
            //if (vp.isPlaying)
            //{
            //    vp.Stop();
            //    videoPanel.SetActive(false);
            //}

            for (int i = 0; i < step.executors.Count; i++)
            {
                Executor executor = step.executors[i];

                var bm = executor.ob.behaviourMode;
                GameObject go = GameObject.Find(executor.ob.objName);
                //GameObject ta = GameObject.Find(executor.ob.target);
                if (go)
                {
                    switch (bm)
                    {
                        case BehaviourMode.Follow:
                            go.transform.SetParent(null);
                            break;
                        case BehaviourMode.LookAt:
                            break;
                        case BehaviourMode.Stillness:
                            break;
                        default:
                            break;
                    }
                }
            }

            /////////////////////////////////////////////////////////////////////////////////////////

            if (!isFree)
            {
                int initCount = 0;
                for (int i = 0; i < curCour.connects.Count; i++)
                {
                    if (curCour.connects[i].form == step.id)
                    {
                        InitStep(dicSteps[curCour.connects[i].to]);
                        initCount++;
                    }
                }
                if(initCount==0)
                {
                    EndCourseware();
                }
            }
            else
            {
                //最后一步，执行结束课件方法
                //EndCourseware();
            }
        }


        private void EndCourseware()
        {
            stepText.text = "所有步骤播放完成";
        }

        public void RunStepIndex(long stepIndex)
        {
            Step step = dicSteps[stepIndex];
            RunStep(step);
        }

        private void Update()
        {

        }
    }
}
