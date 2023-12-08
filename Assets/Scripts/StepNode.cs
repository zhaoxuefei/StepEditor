using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using ZXFFrame;

[NodeTint("#000000")]
[NodeWidth(300)]
public class StepNode : Node
{
    //������룬���
    [Input] public long id;
    [Input] public Step value;


    [Output] public Step result;

    /// <summary>
    /// �����ڵ��ȡ�ýڵ�����ֵ
    /// </summary>
    public override object GetValue(NodePort port)
    {
        if (port.fieldName == "result")
        {
            //�������Portû�����ӣ������ֶ�Ĭ��ֵ�� ���򷵻�����Port��ֵ
            return GetInputValue<Step>("value", this.value);
        }
        else
            return null;
    }

    /// <summary>
    /// �ڽڵ���Ҽ��˵�����ӷ���
    /// </summary>
    [ContextMenu("UpdateStep")]
    void UpdateStep()
    {
        //��ȡ���Port���ӵ�Port��������ȡ���ڵ�Node��Connection�Ǵ���������Port�л�ȡ��һ���ǿ�Port
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
