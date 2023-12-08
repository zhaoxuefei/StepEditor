using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using static XNode.Node;
using XNode;

namespace ZXFFrame
{

    [Serializable]
    public class Courseware
    {
        public string name;//课件名称
        public Step firstStep;
        public List<Step> steps;
        public List<Connect> connects;
        public Courseware()
        {
            this.name = "";
            this.firstStep = null;
            this.steps = new List<Step>();
            this.connects = new List<Connect>();
        }
    }

    [Serializable]
    public class Connect
    {
        public long form;
        public long to;

        public Connect()
        {
            this.form = 0;
            this.to = 0;
        }
    }



    [Serializable]
    public class Step
    {
        public long id;
        public string name;//步骤名字
        //public long parentID;//父级步骤
        //public List<long> childrenID;//孩子步骤
        public int levelX;//级别
        public int levelY;//级别
        public bool isShowDetail;//显示详情
        //public bool isNecessary;//该步骤是否是必须的，是否支持跳过
        public bool isHaveAA;//是否有动画关键帧
        public AnimatorAction aa;//关键帧事件
        public string explain;//说明
        public string music;//声音
        public List<string> hideObjs;//要隐藏的物体
        public List<string> showObjs;//要显示的物体
        public float waitTime;//等待时长
        public TriggerMode triggerMode;//触发模式
        public List<string> triggerObjNames;//触发物名称
        public List<int> preStepIndexs;//前置步骤
        public List<string> hoverObjNames;//划过物名称
        public bool isOldPos;//不触发是否还原位置
        public bool isTriggered;//是否满足触发
        public long dragHoverTime;//鼠标划过时间
        public Trans oldPos;//还原的位置
        public List<Executor> executors;//动作

        public Step()
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();
            this.id = BitConverter.ToInt64(buffer, 0);
            this.levelX = 0;
            this.levelY = 0;
            this.isShowDetail = false;
            this.name = "";
            //this.parentID = 0;
            //this.childrenID = new List<long>();
            //this.isNecessary = true;
            this.isHaveAA = false;
            this.aa = new AnimatorAction();
            this.explain = "";
            this.music = "";
            this.hideObjs = new List<string>();
            this.showObjs = new List<string>();
            this.waitTime = 0f;
            this.triggerMode = TriggerMode.Click;
            this.triggerObjNames = new List<string>();
            this.hoverObjNames = new List<string>();
            this.preStepIndexs = new List<int>();
            this.dragHoverTime = 0;
            this.isOldPos = false;
            this.isTriggered = false;

            this.oldPos = new Trans();
            this.executors = new List<Executor>();
        }
    }


    [Serializable]
    public class AnimatorAction
    {
        public string aaObjName;//带动画的物体
        public string clipName;//动画片段
        public int actionIndex;//事件帧
        public long stepId;//事件
        public AnimatorAction()
        {
            this.aaObjName = "";
            this.clipName = "";
            this.actionIndex = 0;
            this.stepId = 0;
        }
    }

    [Serializable]
    public enum TriggerMode
    {
        /// <summary>
        /// 点击
        /// </summary>
        Click = 0,
        /// <summary>
        /// 悬停
        /// </summary>
        Hover,
        /// <summary>
        /// 划过多个
        /// </summary>
        DragHover,
        /// <summary>
        /// 碰撞
        /// </summary>
        Collide,
        /// <summary>
        /// 等待
        /// </summary>
        Wait,
        /// <summary>
        /// 前置步骤触发
        /// </summary>
        Multi
    }


    [Serializable]
    public class Executor
    {
        public string name;//执行物名称
        public bool isHide;//是否隐藏物体
        public bool isLoop;//动作是否循环
        public string videoUrl;//视频
        public ShaderCheck shaderCheck;//shder
        public ObjBehaviour ob;//动作行为
        public bool isUseAnim;//是否使用动画
        public Anima anima;//动画片段名字
        public List<Point> path;//路径点
        public bool isFold;//是否折叠，此属性编辑器用


        public Executor()
        {
            this.name = "";
            this.isHide = false;
            this.isLoop = false;
            this.videoUrl = "";
            this.shaderCheck = new ShaderCheck();
            this.ob = new ObjBehaviour();
            this.isUseAnim = false;
            this.anima = new Anima();
            this.isFold = false;
            this.path = new List<Point>();
        }
    }

    /// <summary>
    /// shader
    /// </summary>
    [Serializable]
    public class ShaderCheck
    {
        public string objName;
        public string shaderName;
        public string variableName;//属性
        public float fValue;//f值
        public CourColor cValue;//值

        public ShaderCheck()
        {
            this.objName = "";
            this.shaderName = "";
            this.variableName = "";
            this.fValue = 0;
            this.cValue = new CourColor();
        }
    }

    [Serializable]
    public class ObjBehaviour
    {
        public string objName;
        public BehaviourMode behaviourMode;
        public string target;
        public CourVector3 offset;
        public ObjBehaviour()
        {
            this.objName = "";
            this.behaviourMode = BehaviourMode.Stillness;
            this.target = "";
            this.offset = new CourVector3();
        }
    }


    [Serializable]
    public enum BehaviourMode
    {
        /// <summary>
        /// 静止
        /// </summary>
        Stillness,
        /// <summary>
        /// 跟随
        /// </summary>
        Follow,
        /// <summary>
        /// 注视
        /// </summary>
        LookAt
    }

    [Serializable]
    public class Anima
    {
        public int playFrame;//开始播放的帧数
        public float speed;//播放速度
        public string animationClip;//动画片段名字
        public Anima()
        {
            this.playFrame = 0;
            this.speed = 1f;
            this.animationClip = "";
        }
    }

    [Serializable]
    public class Point
    {
        public float localTime;//时间点
        public Trans tran;//路径点

        public Point()
        {
            this.localTime = 0f;
            this.tran = new Trans();
        }

    }



    [Serializable]
    public class Trans
    {
        public CourVector3 pos;//坐标
        public CourVector3 rot;//旋转

        public Trans()
        {
            this.pos = new CourVector3();
            this.rot = new CourVector3();
        }
    }

    [Serializable]
    public class SoundEffect
    {
        public float playTime;
        public string audioName;
        public SoundEffect()
        {
            this.playTime = 0;
            this.audioName = "";
        }
    }


    [Serializable]
    public class CourVector3
    {
        public float x;
        public float y;
        public float z;

        public CourVector3()
        {
            this.x = 0;
            this.y = 0;
            this.z = 0;
        }

        public CourVector3(Vector3 value)
        {
            this.x = value.x;
            this.y = value.y;
            this.z = value.z;
        }

        public Vector3 GetVector3()
        {
            return new Vector3(x, y, z);
        }
    }


    [Serializable]
    public class CourColor
    {
        public float r;
        public float g;
        public float b;
        public float a;

        public CourColor()
        {
            this.r = 0;
            this.g = 0;
            this.b = 0;
            this.a = 0;
        }

        public CourColor(Color value)
        {
            this.r = value.r;
            this.g = value.g;
            this.b = value.b;
            this.a = value.a;
        }

        public Color GetColor()
        {
            return new Color(r, g, b, a);
        }
    }
}




