using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using ZXFFrame;
using System.IO;

public class StepEditor : EditorWindow
{

    private bool isCreatNewCourseware = false;
    private static GUIStyle style15;
    private Courseware curCour;
    private string tittle = "奥医虚拟仿真步骤编辑器";

    private Vector2 stepPos = Vector2.zero;
    private Dictionary<Step, Vector2> hideObjPos = new Dictionary<Step, Vector2>();
    private Dictionary<Step, Vector2> showObjPos = new Dictionary<Step, Vector2>();


    private Dictionary<long, Step> dicSteps = new Dictionary<long, Step>();


    [MenuItem("StepEditorTool/CreatXml")]
    public static void Creat()
    {
        GetWindow(typeof(StepEditor));
        style15 = new GUIStyle(EditorStyles.label);
        style15.fontSize = 15;
    }

    void OnGUI()
    {
        this.titleContent = new GUIContent(tittle);
        EditorGUILayout.BeginHorizontal();

        GUILayout.Space(5);

        if (GUILayout.Button("创建课件", GUILayout.Width(100)))
        {
            isCreatNewCourseware = true;
            Debug.Log("创建新课件");
        }

        GUILayout.Space(20);

        if (GUILayout.Button("导入课件XML", GUILayout.Width(100)))
        {
            string path = EditorUtility.OpenFilePanel("导入XML", "", "xml");
            curCour = SerializeHelper.DeserializeWithXmlFile<Courseware>(path);
            var names = path.Split('/');
            tittle += names[names.Length - 1];

            string configPath = "Assets/Resources/path.txt";
            File.WriteAllText(configPath, path);

            dicSteps = new Dictionary<long, Step>();
            for (int i = 0; i < curCour.steps.Count; i++)
            {
                dicSteps.Add(curCour.steps[i].id, curCour.steps[i]);
            }


        }

        GUILayout.Space(20);

        if (GUILayout.Button("保存课件XML", GUILayout.Width(100)))
        {
            string path = EditorUtility.SaveFilePanel("保存xml", "", curCour.name + "_" + DateTime.Now.ToString("MM_dd_hh_mm_ss"), "xml");
            SerializeHelper.SerializeToXmlFile(curCour, path);

            string configPath = "Assets/Resources/path.txt";
            File.WriteAllText(configPath, path);
        }

        GUILayout.Space(20);
        EditorGUILayout.LabelField("当前运行xml地址:", style15, GUILayout.Width(120), GUILayout.Height(20));
        string configPath1 = "Assets/Resources/path.txt";
        string xmlPath = File.ReadAllText(configPath1);
        EditorGUILayout.TextField(xmlPath);


        GUILayout.Space(5);
        EditorGUILayout.EndHorizontal();

        if (isCreatNewCourseware)
        {
            isCreatNewCourseware = false;
            curCour = new Courseware();

            dicSteps = new Dictionary<long, Step>();
            for (int i = 0; i < curCour.steps.Count; i++)
            {
                dicSteps.Add(curCour.steps[i].id, curCour.steps[i]);
            }
        }

        if (curCour != null)
        {
            ShowCoursewareEditor(curCour);
        }
    }

    /// <summary>
    /// 显示课件
    /// </summary>
    /// <param name="cour"></param>
    private void ShowCoursewareEditor(Courseware cour)
    {

        EditorGUILayout.BeginHorizontal("Box");
        EditorGUILayout.LabelField("课件名字:", style15, GUILayout.Width(100), GUILayout.Height(20));
        cour.name = EditorGUILayout.TextField(cour.name);
        EditorGUILayout.EndHorizontal();

        GUI.contentColor = Color.red;
        if (GUILayout.Button("添加步骤", GUILayout.Width(100)))
        {
            Step step = new Step();
            curCour.steps.Add(step);
            dicSteps.Add(step.id ,step);
        }
        GUI.contentColor = Color.white;


        stepPos = EditorGUILayout.BeginScrollView(stepPos);

        for (int i = 0; i < curCour.steps.Count; i++)
        {
            ShowStepEditor(curCour.steps[i]);
        }


        EditorGUILayout.EndScrollView();
    }

    private void ShowStepEditor(Step step)
    {
        EditorGUILayout.BeginVertical("Box");

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20);
        EditorGUILayout.LabelField(step.id.ToString(), GUILayout.Width(150));
        EditorGUILayout.LabelField("步骤名称：", GUILayout.Width(60));
        step.name = EditorGUILayout.TextField(step.name);

        GUI.contentColor = Color.red;
        if (GUILayout.Button("删除步骤", GUILayout.Width(100)))
        {
            dicSteps.Remove(step.id);
            curCour.steps.Remove(step);
        }

        string isShowDetail = step.isShowDetail ? "展示详情" : "收起详情";

        if (GUILayout.Button(isShowDetail, GUILayout.Width(100)))
        {
            step.isShowDetail = !step.isShowDetail;
        }
        GUI.contentColor = Color.white;
        EditorGUILayout.EndHorizontal();

        if (step.isShowDetail)
        {
            ShowStepDetailEditor(step);
        }

        EditorGUILayout.EndVertical();
    }


    /*
    /// <summary>
    /// 显示步骤
    /// </summary>
    /// <param name="cour"></param>
    private void ShowStepEditor(Step step)
    {
        EditorGUILayout.BeginVertical();


        for (int i = 0; i < cour.steps.Count; i++)
        {
            Step step = cour.steps[i];
            EditorGUILayout.BeginVertical("Box");


            EditorGUILayout.BeginHorizontal();

            string isFold = step.isFold ? "→" : "↓";
            if (GUILayout.Button(isFold, GUILayout.Width(50)))
            {
                step.isFold = !step.isFold;
            }

            EditorGUILayout.LabelField("步骤名称：", GUILayout.Width(60));
            step.name = EditorGUILayout.TextField(step.name);

            //GUILayout.Space(10);
            //EditorGUILayout.LabelField("是否必须步骤：", GUILayout.Width(80));
            //step.isNecessary = EditorGUILayout.Toggle(step.isNecessary, GUILayout.Width(20), GUILayout.Height(20));

            GUILayout.Space(10);

            //GUI.contentColor = Color.red;
            //if (GUILayout.Button("删除", GUILayout.Width(50)))
            //{
            //    if (EditorUtility.DisplayDialog("删除步骤", "删除步骤:" + step.name + "-----------------------若删除错误，不要保存，重新导入xml", "确认", "取消"))
            //    {
            //        cour.steps.RemoveAt(i);
            //    }
            //}
            //GUI.contentColor = Color.white;

            //if (GUILayout.Button("↑", GUILayout.Width(30)))
            //{
            //    if (i > 0)
            //    {
            //        Step tmpStep = cour.steps[i];
            //        cour.steps[i] = cour.steps[i - 1];
            //        cour.steps[i - 1] = tmpStep;
            //    }
            //}
            //if (GUILayout.Button("↓", GUILayout.Width(30)))
            //{
            //    if (i < cour.steps.Count - 1)
            //    {
            //        Step tmpStep = cour.steps[i];
            //        cour.steps[i] = cour.steps[i + 1];
            //        cour.steps[i + 1] = tmpStep;
            //    }
            //}

            GUI.contentColor = Color.yellow;
            EditorGUILayout.LabelField((i).ToString(), style15, GUILayout.Width(20));
            GUI.contentColor = Color.white;

            EditorGUILayout.EndHorizontal();

            ShowStepDetailEditor(step);

            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndVertical();
    }
    */

    /// <summary>
    /// 显示步骤细节
    /// </summary>
    /// <param name="step"></param>
    private void ShowStepDetailEditor(Step step)
    {

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20);
        EditorGUILayout.LabelField("步骤说明：", GUILayout.Width(60));
        step.explain = EditorGUILayout.TextField(step.explain);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20);
        EditorGUILayout.LabelField("步骤声音：", GUILayout.Width(60));
        step.music = EditorGUILayout.TextField(step.music);
        EditorGUILayout.EndHorizontal();
        ///////////////////////////////////////////////////////////////////////////////
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20);


        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("隐藏物体：", GUILayout.Width(60));

        EditorGUILayout.BeginVertical();
        if (GUILayout.Button("添加隐藏物体", GUILayout.Width(100)))
        {
            step.hideObjs.Add("");
        }

        if (GUILayout.Button("批量拾取隐藏物体", GUILayout.Width(100)))
        {
            for (int i = 0; i < Selection.gameObjects.Length; i++)
            {
                step.hideObjs.Add(FullPath(Selection.gameObjects[i].transform));
            }
        }

        EditorGUILayout.EndVertical();

        GUILayout.Space(20);

        if (!hideObjPos.ContainsKey(step))
        {
            hideObjPos.Add(step, Vector2.zero);
        }
        hideObjPos[step] = EditorGUILayout.BeginScrollView(hideObjPos[step], GUILayout.Width(320), GUILayout.Height(100));

        for (int i = 0; i < step.hideObjs.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GameObject hideIObj = GameObject.Find(step.hideObjs[i]);
            UnityEngine.Object tmp = EditorGUILayout.ObjectField(hideIObj, typeof(GameObject), true, GUILayout.Width(200));

            if (tmp)
            {
                step.hideObjs[i] = FullPath((tmp as GameObject).transform);
            }

            GUILayout.Space(10);
            GUI.contentColor = Color.red;
            if (GUILayout.Button("删除", GUILayout.Width(50)))
            {
                step.hideObjs.RemoveAt(i);
                hideObjPos.Remove(step);
            }
            GUI.contentColor = Color.white;
            EditorGUILayout.EndHorizontal();

        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndHorizontal();


        GUILayout.Space(50);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("显示物体：", GUILayout.Width(60));
        if (GUILayout.Button("添加显示物体", GUILayout.Width(100)))
        {
            step.showObjs.Add("");
        }

        GUILayout.Space(20);

        if (!showObjPos.ContainsKey(step))
        {
            showObjPos.Add(step, Vector2.zero);
        }
        showObjPos[step] = EditorGUILayout.BeginScrollView(showObjPos[step], GUILayout.Width(320), GUILayout.Height(100));

        for (int i = 0; i < step.showObjs.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GameObject showObj = GameObject.Find(step.showObjs[i]);
            UnityEngine.Object tmp = EditorGUILayout.ObjectField(showObj, typeof(GameObject), true, GUILayout.Width(200));

            if (tmp)
            {
                step.showObjs[i] = tmp.name;
            }

            GUILayout.Space(10);
            GUI.contentColor = Color.red;
            if (GUILayout.Button("删除", GUILayout.Width(50)))
            {
                step.showObjs.RemoveAt(i);
                showObjPos.Remove(step);
            }
            GUI.contentColor = Color.white;
            EditorGUILayout.EndHorizontal();

        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20);
        EditorGUILayout.LabelField("是否有关键帧：", GUILayout.Width(120));

        step.isHaveAA = EditorGUILayout.Toggle(step.isHaveAA, GUILayout.Width(20), GUILayout.Height(20));
        if (step.isHaveAA)
        {
            GameObject aaObj = GameObject.Find(step.aa.aaObjName);
            EditorGUILayout.LabelField("动画物体：", GUILayout.Width(60));
            UnityEngine.Object tmp = EditorGUILayout.ObjectField(aaObj, typeof(GameObject), true, GUILayout.Width(150));
            if (tmp && (tmp as GameObject).GetComponent<Animation>())
            {
                step.aa.aaObjName = FullPath((tmp as GameObject).transform);
                GUILayout.Space(10);
                EditorGUILayout.LabelField("动画选择：", GUILayout.Width(60));

                AnimationClip clip = null;

                List<string> listName = new List<string>();


                var anim = (tmp as GameObject).GetComponent<Animation>();
                foreach (AnimationState state in anim)
                {
                    listName.Add(state.clip.name);
                    if (state.clip.name == step.aa.clipName)
                    {
                        clip = state.clip;
                    }
                }

                int index = 0;
                for (int j = 0; j < listName.Count; j++)
                {
                    if (listName[j] == step.aa.clipName)
                    {
                        index = j;
                    }
                }
                if (listName.Count > 0)
                {
                    step.aa.clipName = listName[EditorGUILayout.Popup(index, listName.ToArray(), GUILayout.Width(120))];
                }

                if (clip)
                {
                    GUILayout.Space(10);
                    float clipTime = clip.length;
                    float frameRate = clip.frameRate;
                    int totalFrame = (int)(clipTime / (1f / frameRate));//总帧数
                    EditorGUILayout.LabelField("触发帧：", GUILayout.Width(60));
                    step.aa.actionIndex = EditorGUILayout.IntSlider(step.aa.actionIndex, 0, totalFrame, GUILayout.Width(120));
                }

                GUILayout.Space(20);
                EditorGUILayout.LabelField("关联步骤id：", GUILayout.Width(100));
                step.aa.stepId = EditorGUILayout.LongField(step.aa.stepId, GUILayout.Width(200));
                //if (cour.steps.Count > 0)
                //{
                //    animatorAction.stepIndex = EditorGUILayout.IntSlider(animatorAction.stepIndex, 0, cour.steps.Count - 1, GUILayout.Width(150));
                //    cour.steps[animatorAction.stepIndex].isAA = true;
                //}
            }
            else
            {

            }
        }

        EditorGUILayout.EndHorizontal();





        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20);
        EditorGUILayout.LabelField("触发类型：", GUILayout.Width(100));
        step.triggerMode = (TriggerMode)EditorGUILayout.Popup((int)step.triggerMode, Enum.GetNames(typeof(TriggerMode)), GUILayout.Width(60));

        GUILayout.Space(20);

        //GUI.contentColor = Color.green;
        //EditorGUILayout.LabelField("是否关键帧触发：", GUILayout.Width(150));
        //EditorGUILayout.LabelField(step.isAA ? "是" : "否", GUILayout.Width(60));
        //GUI.contentColor = Color.white;

        EditorGUILayout.EndHorizontal();

        switch (step.triggerMode)
        {
            case TriggerMode.Hover:
            case TriggerMode.Click:
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GameObject triggerObj = step.triggerObjNames.Count <= 0 ? null : GameObject.Find(step.triggerObjNames[0]);
                EditorGUILayout.LabelField("触发物体：", GUILayout.Width(60));
                UnityEngine.Object tmp = EditorGUILayout.ObjectField(triggerObj, typeof(GameObject), true, GUILayout.Width(200));
                if (tmp)
                {
                    if (step.triggerObjNames.Count <= 0)
                    {
                        step.triggerObjNames.Add(FullPath((tmp as GameObject).transform));
                    }
                    else
                    {
                        step.triggerObjNames[0] = FullPath((tmp as GameObject).transform);
                    }
                }
                EditorGUILayout.EndHorizontal();
                break;
            case TriggerMode.Collide:
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GameObject triggerObj1 = step.triggerObjNames.Count <= 0 ? null : GameObject.Find(step.triggerObjNames[0]);
                EditorGUILayout.LabelField("拖拽物体：", GUILayout.Width(60));
                UnityEngine.Object tmp1 = EditorGUILayout.ObjectField(triggerObj1, typeof(GameObject), true, GUILayout.Width(200));
                if (tmp1)
                {
                    if (step.triggerObjNames.Count <= 0)
                    {
                        step.triggerObjNames.Add(FullPath((tmp1 as GameObject).transform));
                    }
                    else
                    {
                        step.triggerObjNames[0] = FullPath((tmp1 as GameObject).transform);
                    }
                }


                GUILayout.Space(20);

                GameObject triggerObj2 = step.triggerObjNames.Count <= 1 ? null : GameObject.Find(step.triggerObjNames[1]);
                EditorGUILayout.LabelField("触发物体：", GUILayout.Width(60));
                UnityEngine.Object tmp2 = EditorGUILayout.ObjectField(triggerObj2, typeof(GameObject), true, GUILayout.Width(200));
                if (tmp2)
                {
                    if (step.triggerObjNames.Count <= 1)
                    {
                        step.triggerObjNames.Add(FullPath((tmp2 as GameObject).transform));
                    }
                    else
                    {
                        step.triggerObjNames[1] = FullPath((tmp2 as GameObject).transform);
                    }
                }
                GUILayout.Space(20);
                EditorGUILayout.LabelField("未触发是否还原位置：", GUILayout.Width(120));
                step.isOldPos = EditorGUILayout.Toggle(step.isOldPos, GUILayout.Width(20), GUILayout.Height(20));
                EditorGUILayout.EndHorizontal();

                if (step.isOldPos)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    EditorGUILayout.LabelField("还原位置：", GUILayout.Width(120));
                    step.oldPos.pos = new CourVector3(EditorGUILayout.Vector3Field("x", step.oldPos.pos.GetVector3(), GUILayout.Width(350)));
                    step.oldPos.rot = new CourVector3(EditorGUILayout.Vector3Field("x", step.oldPos.rot.GetVector3(), GUILayout.Width(350)));
                    if (GUILayout.Button("提取", GUILayout.Width(80), GUILayout.Height(20)))
                    {
                        step.oldPos.pos = new CourVector3(Selection.activeGameObject.transform.position);
                        step.oldPos.rot = new CourVector3(Selection.activeGameObject.transform.rotation.eulerAngles);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                break;
            case TriggerMode.Wait:
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(20);
                EditorGUILayout.LabelField("等待时长(秒)：", GUILayout.Width(100));
                step.waitTime = EditorGUILayout.FloatField(step.waitTime, GUILayout.Width(60));
                EditorGUILayout.EndHorizontal();
                break;
            case TriggerMode.Multi:
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("前置步骤：", GUILayout.Width(100));
                if (GUILayout.Button("添加", GUILayout.Width(60)))
                {
                    step.preStepIndexs.Add(0);
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                for (int i = 0; i < step.preStepIndexs.Count; i++)
                {
                    step.preStepIndexs[i] = EditorGUILayout.IntField(step.preStepIndexs[i], GUILayout.Width(60));
                    GUI.contentColor = Color.red;
                    if (GUILayout.Button("删除", GUILayout.Width(50)))
                    {
                        step.preStepIndexs.RemoveAt(i);
                    }
                    GUILayout.Space(20);
                    GUI.contentColor = Color.white;
                }
                EditorGUILayout.EndHorizontal();
                break;
            case TriggerMode.DragHover:
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("划过物体：", GUILayout.Width(100));
                if (GUILayout.Button("添加", GUILayout.Width(60)))
                {
                    step.hoverObjNames.Add("");
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                for (int i = 0; i < step.hoverObjNames.Count; i++)
                {
                    GameObject hoverObj = GameObject.Find(step.hoverObjNames[i]);
                    UnityEngine.Object tmpHover = EditorGUILayout.ObjectField(hoverObj, typeof(GameObject), true, GUILayout.Width(150));
                    if (tmpHover)
                    {
                        step.hoverObjNames[i] = FullPath((tmpHover as GameObject).transform);
                    }
                    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    GUI.contentColor = Color.red;
                    if (GUILayout.Button("删除", GUILayout.Width(50)))
                    {
                        step.hoverObjNames.RemoveAt(i);
                    }
                    GUILayout.Space(20);
                    GUI.contentColor = Color.white;
                }
                EditorGUILayout.EndHorizontal();
                break;
        }
        ShowExecutorEditor(step);
    }

    /// <summary>
    /// 显示执行物
    /// </summary>
    /// <param name="step"></param>
    private void ShowExecutorEditor(Step step)
    {

        EditorGUILayout.BeginVertical("Box");

        EditorGUILayout.BeginHorizontal();
        GUI.contentColor = Color.red;
        GUILayout.Space(20);
        if (GUILayout.Button("添加执行物", GUILayout.Width(100)))
        {
            step.executors.Add(new Executor());
        }
        GUI.contentColor = Color.white;
        EditorGUILayout.EndHorizontal();

        for (int i = 0; i < step.executors.Count; i++)
        {
            EditorGUILayout.BeginVertical("Box");


            EditorGUILayout.BeginHorizontal();

            Executor executor = step.executors[i];
            GameObject executorObj = GameObject.Find(executor.name);
            GUILayout.Space(40);
            EditorGUILayout.LabelField("执行物体" + (i + 1).ToString() + ":", GUILayout.Width(60));
            UnityEngine.Object tmp = EditorGUILayout.ObjectField(executorObj, typeof(GameObject), true, GUILayout.Width(150));
            if (tmp)
            {
                executor.name = FullPath((tmp as GameObject).transform);
            }
            GUILayout.Space(20);
            EditorGUILayout.LabelField("是否隐藏：", GUILayout.Width(80));
            executor.isHide = EditorGUILayout.Toggle(executor.isHide, GUILayout.Width(20), GUILayout.Height(20));

            GUILayout.Space(20);
            EditorGUILayout.LabelField("是否用动画：", GUILayout.Width(80));
            executor.isUseAnim = EditorGUILayout.Toggle(executor.isUseAnim, GUILayout.Width(20), GUILayout.Height(20));


            if (executor.isUseAnim)
            {
                GUILayout.Space(20);
                EditorGUILayout.LabelField("是否循环：", GUILayout.Width(60));
                executor.isLoop = EditorGUILayout.Toggle(executor.isLoop, GUILayout.Width(20), GUILayout.Height(20));
                GUILayout.Space(20);

                EditorGUILayout.LabelField("播放速率：", GUILayout.Width(60));
                executor.anima.speed = EditorGUILayout.FloatField(executor.anima.speed, GUILayout.Width(60));

                GUILayout.Space(20);
                EditorGUILayout.LabelField("动画选择：", GUILayout.Width(60));

                AnimationClip clip = null;

                if (tmp)
                {
                    Animation anim = (tmp as GameObject).GetComponent<Animation>();
                    if (anim)
                    {
                        List<string> listName = new List<string>();

                        foreach (AnimationState state in anim)
                        {
                            listName.Add(state.clip.name);
                            if (executor.anima.animationClip == state.clip.name)
                            {
                                clip = state.clip;
                            }
                        }

                        if (listName.Count > 0)
                        {
                            int indexClip = 0;
                            for (int j = 0; j < listName.Count; j++)
                            {
                                if (executor.anima.animationClip == listName[j])
                                {
                                    indexClip = j;
                                }
                            }

                            executor.anima.animationClip = listName[EditorGUILayout.Popup(indexClip, listName.ToArray(), GUILayout.Width(120))];

                        }
                    }

                    if (clip)
                    {
                        GUILayout.Space(10);
                        float clipTime = clip.length;
                        float frameRate = clip.frameRate;
                        int totalFrame = (int)(clipTime / (1f / frameRate));//总帧数
                        executor.anima.playFrame = EditorGUILayout.IntSlider(executor.anima.playFrame, 0, totalFrame, GUILayout.Width(120));
                    }
                }

            }

            GUILayout.Space(10);
            GUI.contentColor = Color.red;
            if (GUILayout.Button("删除", GUILayout.Width(50)))
            {
                if (EditorUtility.DisplayDialog("删除执行物", "删除执行物:" + executor.name + "-----------------------若删除错误，不要保存，重新导入xml", "确认", "取消"))
                {
                    step.executors.RemoveAt(i);
                }
            }

            GUI.contentColor = Color.white;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(40);
            EditorGUILayout.LabelField("视频地址：", GUILayout.Width(60));
            executor.videoUrl = EditorGUILayout.TextField(executor.videoUrl);
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(40);
            EditorGUILayout.LabelField("材质设置：", GUILayout.Width(60));
            GameObject shaderObj = GameObject.Find(executor.shaderCheck.objName);
            UnityEngine.Object shaderObject = EditorGUILayout.ObjectField(shaderObj, typeof(GameObject), true, GUILayout.Width(150));
            if (shaderObject)
            {
                executor.shaderCheck.objName = FullPath((shaderObject as GameObject).transform);
                Shader shader = (shaderObject as GameObject).GetComponent<MeshRenderer>() ? (shaderObject as GameObject).GetComponent<MeshRenderer>().sharedMaterial.shader : null;
                if (shader)
                {
                    executor.shaderCheck.shaderName = EditorGUILayout.TextField(shader.name, GUILayout.Width(60));
                    int count = shader.GetPropertyCount();
                    List<string> pNames = new List<string>();
                    for (int j = 0; j < count; j++)
                    {
                        pNames.Add(shader.GetPropertyName(j));
                    }
                    int index = Mathf.Clamp(pNames.BinarySearch(executor.shaderCheck.variableName), 0, pNames.Count - 1);
                    var pop = EditorGUILayout.Popup(index, pNames.ToArray(), GUILayout.Width(160));
                    executor.shaderCheck.variableName = pNames[pop];
                    var sType = shader.GetPropertyType(pop);
                    switch (sType)
                    {
                        case UnityEngine.Rendering.ShaderPropertyType.Color:
                            executor.shaderCheck.cValue = new CourColor(EditorGUILayout.ColorField(executor.shaderCheck.cValue.GetColor(), GUILayout.Width(160)));
                            break;
                        case UnityEngine.Rendering.ShaderPropertyType.Float:
                            executor.shaderCheck.fValue = EditorGUILayout.FloatField(executor.shaderCheck.fValue, GUILayout.Width(60));
                            break;
                        case UnityEngine.Rendering.ShaderPropertyType.Range:
                            Vector2 xy = shader.GetPropertyRangeLimits(pop);
                            executor.shaderCheck.fValue = EditorGUILayout.Slider(executor.shaderCheck.fValue, xy.x, xy.y, GUILayout.Width(160));
                            break;
                        default:
                            break;
                    }
                }
            }
            EditorGUILayout.EndHorizontal();



            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(40);
            GameObject behaviourObj = GameObject.Find(executor.ob.objName);
            EditorGUILayout.LabelField("动作物体:", GUILayout.Width(60));
            UnityEngine.Object tmpBO = EditorGUILayout.ObjectField(behaviourObj, typeof(GameObject), true, GUILayout.Width(150));
            if (tmpBO)
            {
                executor.ob.objName = FullPath((tmpBO as GameObject).transform);
            }
            GUILayout.Space(20);
            EditorGUILayout.LabelField("动作类型：", GUILayout.Width(60));
            executor.ob.behaviourMode = (BehaviourMode)EditorGUILayout.Popup((int)executor.ob.behaviourMode, Enum.GetNames(typeof(BehaviourMode)), GUILayout.Width(60));

            switch (executor.ob.behaviourMode)
            {
                case BehaviourMode.Stillness:
                    break;
                case BehaviourMode.Follow:
                    GameObject targetF = GameObject.Find(executor.ob.target);
                    EditorGUILayout.LabelField("目标物体:", GUILayout.Width(60));
                    UnityEngine.Object tmpFO = EditorGUILayout.ObjectField(targetF, typeof(GameObject), true, GUILayout.Width(150));
                    if (tmpFO)
                    {
                        executor.ob.target = FullPath((tmpFO as GameObject).transform);
                    }

                    EditorGUILayout.BeginHorizontal();
                    executor.ob.offset = new CourVector3(EditorGUILayout.Vector3Field("偏移:", executor.ob.offset.GetVector3(), GUILayout.Width(350)));
                    EditorGUILayout.EndHorizontal();

                    break;
                case BehaviourMode.LookAt:
                    GameObject targetLj = GameObject.Find(executor.ob.target);
                    EditorGUILayout.LabelField("目标物体:", GUILayout.Width(60));
                    UnityEngine.Object tmpLO = EditorGUILayout.ObjectField(targetLj, typeof(GameObject), true, GUILayout.Width(150));
                    if (tmpLO)
                    {
                        executor.ob.target = FullPath((tmpLO as GameObject).transform);
                    }
                    break;
            }



            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(40);
            //Path
            EditorGUILayout.LabelField("路径点", GUILayout.Width(60));
            GUILayout.Space(10);
            if (GUILayout.Button("添加路径点", GUILayout.Width(100)))
            {
                executor.path.Add(new Point());
            }
            GUILayout.Space(10);
            EditorGUILayout.LabelField("路径点有" + executor.path.Count + "个", GUILayout.Width(100));

            GUILayout.Space(10);
            string isFold = executor.isFold ? "→" : "↓";
            if (GUILayout.Button(isFold, GUILayout.Width(50)))
            {
                executor.isFold = !executor.isFold;
            }
            EditorGUILayout.EndHorizontal();

            ShowPathEditor(executor);

            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// 显示路径点
    /// </summary>
    /// <param name="executor"></param>
    private void ShowPathEditor(Executor executor)
    {
        if (!executor.isFold)
        {

            for (int i = 0; i < executor.path.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(60);
                Point point = executor.path[i];

                if (GUILayout.Button("提取", GUILayout.Width(80), GUILayout.Height(20)))
                {
                    point.tran.pos = new CourVector3(Selection.activeGameObject.transform.position);
                    point.tran.rot = new CourVector3(Selection.activeGameObject.transform.rotation.eulerAngles);
                }

                GUILayout.Space(10);
                EditorGUILayout.LabelField("用时（秒）:", GUILayout.Width(60));
                point.localTime = EditorGUILayout.FloatField(point.localTime, GUILayout.Width(100));
                GUILayout.Space(10);

                GUI.contentColor = Color.red;
                if (GUILayout.Button("删除", GUILayout.Width(50)))
                {
                    if (EditorUtility.DisplayDialog("删除路径点", "删除路径点:-----------------------若删除错误，不要保存，重新导入xml", "确认", "取消"))
                    {
                        executor.path.RemoveAt(i);
                    }
                }
                GUI.contentColor = Color.white;
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(60);
                point.tran.pos = new CourVector3(EditorGUILayout.Vector3Field("Pos", point.tran.pos.GetVector3(), GUILayout.Width(350)));
                GUILayout.Space(20);
                point.tran.rot = new CourVector3(EditorGUILayout.Vector3Field("Rot", point.tran.rot.GetVector3(), GUILayout.Width(350)));
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(10);
            }
        }
    }


    private string FullPath(Transform t)
    {
        string path = t.name;
        while (t.parent)
        {
            t = t.parent;
            path = t.name + "/" + path;
        }
        return path;
    }



}
