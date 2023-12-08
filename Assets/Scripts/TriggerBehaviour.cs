using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace ZXFFrame
{
    public class TriggerBehaviour : MonoBehaviour
    {

        public Action<Step> triggerAction;
        private TriggerMode triggerMode;
        private Step step;
        private GameObject otherTriggerObj;
        private bool isCollider = false;
        public  long hoverTime;
        void Start()
        {

        }

        void Update()
        {

        }

        public void Init(Step step)
        {
            this.step = step;
            triggerMode = step.triggerMode;

            if (triggerMode == TriggerMode.Collide)
            {
                var outline = GetComponentInChildren<Outline>();
                if (outline)
                {
                    outline.enabled = false;
                }

                otherTriggerObj = GameObject.Find(step.triggerObjNames[1]);
                if (!otherTriggerObj)
                {
                    for (int i = 0; i < AppRun.instence.hideObjs.Count; i++)
                    {
                        if (AppRun.instence.hideObjs[i].name == step.triggerObjNames[1])
                        {
                            otherTriggerObj = AppRun.instence.hideObjs[i];
                        }
                    }
                }


                Rigidbody rig = gameObject.GetOrCreatComponent<Rigidbody>();
                rig.useGravity = false;
            }

        }

        private void OnMouseDown()
        {
            if (triggerMode == TriggerMode.Click)
            {
                triggerAction?.Invoke(step);
            }
        }

        private void OnMouseUp()
        {
            if (triggerMode == TriggerMode.Collide && isCollider == true)
            {
                step.isTriggered = true;
                triggerAction?.Invoke(step);
            }
            else
            {
                step.isTriggered = false;
            }

            if (triggerMode == TriggerMode.Collide && isCollider == false)
            {
                if (step.isOldPos)
                {
                    transform.position = step.oldPos.pos.GetVector3();
                    transform.rotation = Quaternion.Euler(step.oldPos.rot.GetVector3());
                }
            }

            if (triggerMode == TriggerMode.Collide)
            {
                for (int i = 0; i < otherTriggerObj.gameObject.GetComponentsInChildren<Transform>(true).Length; i++)
                {
                    GameObject tmp = otherTriggerObj.gameObject.GetComponentsInChildren<Transform>(true)[i].gameObject;
                    SkinnedMeshRenderer skinnedMeshRenderer = tmp.GetComponent<SkinnedMeshRenderer>();
                    if (skinnedMeshRenderer)
                    {
                        Material[] mas = skinnedMeshRenderer.sharedMaterials;
                        for (int j = 0; j < mas.Length; j++)
                        {
                            if (mas[j].name == "SuLiaoTong_OP 1")
                            {
                                mas[j].DisableKeyword("_EMISSION");
                            }
                        }
                    }
                }
            }
        }



        private void OnMouseDrag()
        {

            if (triggerMode == TriggerMode.Collide)
            {
                Vector3 targetScreenPos = Camera.main.WorldToScreenPoint(gameObject.transform.position);
                Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, targetScreenPos.z);
                gameObject.transform.position = Camera.main.ScreenToWorldPoint(mousePos);

                otherTriggerObj.SetActive(true);

            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == otherTriggerObj)
            {
                isCollider = true;

                for (int i = 0; i < other.gameObject.GetComponentsInChildren<Transform>(true).Length; i++)
                {
                    GameObject tmp = other.gameObject.GetComponentsInChildren<Transform>(true)[i].gameObject;
                    SkinnedMeshRenderer skinnedMeshRenderer = tmp.GetComponent<SkinnedMeshRenderer>();
                    if (skinnedMeshRenderer)
                    {
                        Material[] mas = skinnedMeshRenderer.sharedMaterials;
                        for (int j = 0; j < mas.Length; j++)
                        {
                            if (mas[j].name == "SuLiaoTong_OP 1")
                            {
                                mas[j].EnableKeyword("_EMISSION");
                            }
                        }
                    }
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject == otherTriggerObj)
            {
                isCollider = false;
                for (int i = 0; i < other.gameObject.GetComponentsInChildren<Transform>(true).Length; i++)
                {
                    GameObject tmp = other.gameObject.GetComponentsInChildren<Transform>(true)[i].gameObject;
                    SkinnedMeshRenderer skinnedMeshRenderer = tmp.GetComponent<SkinnedMeshRenderer>();
                    if (skinnedMeshRenderer)
                    {
                        Material[] mas = skinnedMeshRenderer.sharedMaterials;
                        for (int j = 0; j < mas.Length; j++)
                        {
                            if (mas[j].name == "SuLiaoTong_OP 1")
                            {
                                mas[j].DisableKeyword("_EMISSION");
                            }
                        }
                    }
                }
            }
        }




        private void OnMouseEnter()
        {
            if (triggerMode == TriggerMode.Hover)
            {
                triggerAction?.Invoke(step);
            }

            if (triggerMode == TriggerMode.DragHover)
            {
                hoverTime = Extension.GetTimeStamp();

            }


            var outline = gameObject.GetComponentInChildren<Outline>();
            if (outline)
            {
                outline.enabled = true;
            }
        }

        private void OnMouseExit()
        {
            var outline = gameObject.GetComponentInChildren<Outline>();
            if (outline)
            {
                outline.enabled = false;
            }

        }

        private void OnDestroy()
        {
            var outline = gameObject.GetComponentInChildren<Outline>();
            if (outline)
            {
                outline.enabled = false;
            }
            Rigidbody rig = gameObject.GetComponent<Rigidbody>();
            if (rig)
            {
                Destroy(rig);
            }
        }
    }
}


