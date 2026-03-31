using System;
using UnityEngine;

namespace MonoBehaviuors
{
    public class MouseWorldPosition:SingletonMono<MouseWorldPosition>
    {

        public Vector3 GetPosition()
        {
            if(Camera.main==null)
                return Vector3.zero;
            var mouseCameraRay= Camera.main.ScreenPointToRay(Input.mousePosition);
            //使用物理系统进行检测，适用于地面高度不均匀的情况
            // if (Physics.Raycast(mouseCameraRay, out RaycastHit hit))
            // {
            //     return hit.point;
            // }
            var plane=new Plane(Vector3.up,Vector3.zero);
            if (plane.Raycast(mouseCameraRay, out float distance))
            {
                return mouseCameraRay.GetPoint(distance);
            }
            else
            {
                return Vector3.zero;
            }
        }
    }
}