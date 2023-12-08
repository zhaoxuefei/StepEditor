using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using ZXFFrame;

[NodeTint("#000000")]
[NodeWidth(300)]
public class StepNode : Node
{
    //标记输入，输出
    [Input] public long id;
    [Input] public Step value;


    [Output] public Step result;

    /// <summary>
    /// 其他节点获取该节点的输出值
    /// </summary>
    public override object GetValue(NodePort port)
    {
        if (port.fieldName == "result")
        {
            //如果输入Port没有连接，返回字段默认值， 否则返回连接Port的值
            return GetInputValue<Step>("value", this.value);
        }
        else
            return null;
    }

    /// <summary>
    /// 在节点的右键菜单中添加方法
    /// </summary>
    [ContextMenu("UpdateStep")]
    void UpdateStep()
    {
        //获取输出Port连接的Port，进而获取相邻的Node，Connection是从所有连接Port中获取第一个非空Port
        //NodePort otherPort = GetOutputPort("result").Connection;
        //if (otherPort != null)
        //{
        //    StepNode nextNode = otherPort.node as StepNode;
        //    Debug.Log(nextNode.value.name);
        //}
        this.id = value.id;
        this.name = value.name;


    }
}
