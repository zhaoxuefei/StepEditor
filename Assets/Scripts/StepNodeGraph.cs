using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XNode;
using ZXFFrame;

[CreateAssetMenu]
public class StepNodeGraph : NodeGraph
{
    public static Courseware curCour;
    public Dictionary<long, Step> dicSteps;
    public Dictionary<long, StepNode> dicNodes;
    public void Awake()
    {
        //this.name = "又一个编辑器界面";
    }


    [ContextMenu("ImportCour")]
    void ImportCour()
    {
        string configPath = "Assets/Resources/path.txt";
        string xmlPath = File.ReadAllText(configPath);

        curCour = SerializeHelper.DeserializeWithXmlFile<Courseware>(xmlPath);
        Debug.Log("加载课件成功");

        dicSteps = new Dictionary<long, Step>();
        for (int i = 0; i < curCour.steps.Count; i++)
        {
            dicSteps.Add(curCour.steps[i].id, curCour.steps[i]);
        }
        dicNodes = new Dictionary<long, StepNode>();
        nodes = new List<Node>();
        ShowNode();

        UpdateConnect();
    }

    void ShowNode()
    {
        for (int i = 0; i < curCour.steps.Count; i++)
        {
            Step step = curCour.steps[i];
            StepNode sn = new StepNode();
            sn.value = step;
            sn.id = step.id;
            sn.name = step.name;

            sn.position = new Vector2(300 * step.levelX, 100 * step.levelY);
            nodes.Add(sn);
            dicNodes.Add(sn.id, sn);
            sn.graph = this;
        }
    }

    void UpdateConnect()
    {
        for (int i = 0; i < curCour.connects.Count; i++)
        {
            Connect connect = curCour.connects[i];
            StepNode fromNode = dicNodes[connect.form];
            NodePort npFrom = fromNode.GetOutputPort("result");
            StepNode toNode = dicNodes[connect.to];
            NodePort npTo = toNode.GetInputPort("value");
            npFrom.Connect(npTo);
        }
    }


    [ContextMenu("SaveCour")]
    void SaveCout()
    {
        if(curCour==null)
        {
            curCour = new Courseware();
        }


        curCour.steps = new List<Step>();
        curCour.connects = new List<Connect>();
        for (int i = 0; i < nodes.Count; i++)
        {
            Step step = (nodes[i] as StepNode).value;
            curCour.steps.Add(step);

            List<NodePort> outPorts = (nodes[i] as StepNode).GetOutputPort("result").GetConnections();
            for (int j = 0; j < outPorts.Count; j++)
            {
                StepNode nextNode = outPorts[j].node as StepNode;
                nextNode.value.levelX = step.levelX + 1;
                nextNode.value.levelY = j;
                Connect connect = new Connect();
                connect.form = step.id;
                connect.to = nextNode.id;
                curCour.connects.Add(connect);
            }
        }

        curCour.firstStep = curCour.steps[0];

        string configPath = "Assets/Resources/path.txt";
        string xmlPath = File.ReadAllText(configPath);
        SerializeHelper.SerializeToXmlFile(curCour, xmlPath);
        Debug.Log("保存成功：" + xmlPath);
    }
}