// Copyright (c) 2017, Columbia University 
// All rights reserved. 
// 
// Redistribution and use in source and binary forms, with or without 
// modification, are permitted provided that the following conditions are met: 
// 
//  * Redistributions of source code must retain the above copyright notice, 
//    this list of conditions and the following disclaimer. 
//  * Redistributions in binary form must reproduce the above copyright 
//    notice, this list of conditions and the following disclaimer in the 
//    documentation and/or other materials provided with the distribution. 
//  * Neither the name of Columbia University nor the names of its 
//    contributors may be used to endorse or promote products derived from 
//    this software without specific prior written permission. 
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE 
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE 
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF 
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
// POSSIBILITY OF SUCH DAMAGE. 
// 
// =============================================================
// Authors: 
// Carmine Elvezio, Mengu Sukan, Steven Feiner
// =============================================================
// 
// 

using System;
using System.Collections.Generic;
using System.Linq;
using MercuryXM.Support.Math;
using UnityEngine;

namespace MercuryXM.Support.Geometry.Arrow
{
    public enum AutoAlignType
    {
        Off,
        Horizontal,
        Vertical,
        ClosestPoint
    };

    public class IntersectionData
    {
        public AutoAlignType AlignType;
        public Vector3 Intersection;
        public float Projection;

        public IntersectionData(AutoAlignType alignType)
        {
            AlignType = alignType;
        }

        public void Update(Vector3 normal1, Vector3 normal2, Vector3 camForward)
        {
            Intersection = Vector3.Cross(normal1, normal2);
            Intersection.Normalize();

            // Flip vector if it's pointing away from the camera
            Projection = Vector3.Dot(Intersection, camForward);

            if (Projection > 0)
            {
                Intersection *= -1;
            }
            else
            {
                Projection *= -1;
            }
        }

        public void Update(Vector3 circleNormal, float radius, Vector3 point) {
            var projPoint = Math3d.ProjectPointOnPlane(circleNormal,Vector3.zero,point);
            projPoint.Normalize();
            Intersection = projPoint * radius;
            Projection = 1;
        }
    }

    public class AutoAlign : MonoBehaviour
    {
    
        public AutoAlignType AlignMode;
        public bool ChooseBest;

        private IntersectionData chosenIntersection;
        readonly Dictionary<AutoAlignType, IntersectionData> intersections = new Dictionary<AutoAlignType, IntersectionData>();
        private Vector3 camRightLocal;
        private Vector3 camUpLocal;

        public void Awake()
        {
            foreach (var alignType in Enum.GetValues(typeof(AutoAlignType)).Cast<AutoAlignType>())
            {
                intersections.Add(alignType, new IntersectionData(alignType));
            }       
        }

        // Update is called once per frame
        public void LateUpdate()
        {
            if (AlignMode == AutoAlignType.Off) return;

            var camTrans = Camera.main.transform;
            var camToArrow = transform.position - camTrans.position;
            var camRight = Vector3.Cross(camTrans.up, camToArrow);
            camRight.Normalize();

            camRightLocal = transform.parent.InverseTransformDirection(camRight);
            camUpLocal = transform.parent.InverseTransformDirection(camTrans.up);

            var camForwardLocal = transform.parent.InverseTransformDirection(camTrans.forward);

            intersections[AutoAlignType.Vertical].Update(Vector3.up, camRightLocal, camForwardLocal);
            intersections[AutoAlignType.Horizontal].Update(Vector3.up, camUpLocal, camForwardLocal);
            intersections[AutoAlignType.ClosestPoint].Update(Vector3.up, 1, transform.parent.InverseTransformPoint(camTrans.position));

            chosenIntersection = (ChooseBest)
                ? intersections.OrderBy(key => key.Value.Projection).Last().Value
                : intersections[AlignMode];

            if (ChooseBest)
                AlignMode = chosenIntersection.AlignType;

            // Align arrow's 'forward' direction with handle's 'to' postion
            // and arrow's 'up' direction with handle rotation axis
            if (chosenIntersection.Intersection.sqrMagnitude > 0.00001f)
                transform.localRotation = Quaternion.LookRotation(chosenIntersection.Intersection, Vector3.up);
        }

        public void OnDrawGizmosSelected()
        {
            const float length = 3;

            UnityEngine.Gizmos.color = Color.yellow;

            foreach (var intersectionInfo in intersections.Values)
            {
                UnityEngine.Gizmos.DrawRay(transform.position, transform.parent.TransformDirection(intersectionInfo.Intersection) * length);
            }

            UnityEngine.Gizmos.color = Color.cyan;

            UnityEngine.Gizmos.DrawRay(transform.position, transform.parent.TransformDirection(camUpLocal) * length);
        }
    }
}