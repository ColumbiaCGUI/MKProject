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

using System.Collections.Generic;
using MercuryXM.Framework;
using MercuryXM.Support.Geometry.Arrow;
using MercuryXM.Support.Math;
using UnityEngine;

/**
 * @brief A Single-Axis Rotation arrow viz, using a dynamic-length arrow.
 * The arc length of the arrow corresponds to the remaining angle in the
 * rotation.
 */
namespace MercuryXM.Widgets
{
    public class OptimalAxisStatic : BasicTransformationTaskBehavior
    {
        public ArrowAlignType ArrowAlignType = ArrowAlignType.Tip;
        public Transform DynamicElements;
        public Transform StaticElements;
        public DynamicArrow GuideArrow;

        DynamicArrow[] arrows;
        private Vector3 guideFrom;
        private Vector3 guideTo;
        private Vector3 guideAxis;
        private Quaternion tilt;

        public override void Awake()
        {
            base.Awake();

            arrows = StaticElements.GetComponentsInChildren<DynamicArrow>();
        }

		public override void Refresh(List<MxmTransform> transformList) {
			base.Refresh (transformList);
            Recalculate();
            tilt = new Quaternion();
            tilt.SetFromToRotation(Vector3.up, Axis());
            StaticElements.rotation = From.rotation * tilt;
        }

        public override void Update()
        {
            base.Update();
		
            if (angle < 1) return;
		
            //Quaternion tilt = new Quaternion();
            //tilt.SetFromToRotation(Vector3.up, Axis());
            DynamicElements.rotation = From.rotation * tilt;
		
            if (arrows == null) return;
		
            for (var i = 0; i < arrows.Length; i++)
            {
                arrows[i].SetSize(angle);
                arrows[i].AlignToCamera = ArrowAlignType;
            }

            UpdateGuide();
        }

        void UpdateGuide()
        {
            guideFrom = Vector3.up;

            var guideWorld = StaticElements.TransformDirection(guideFrom);

            if (Vector3.Dot(guideWorld,Camera.main.transform.forward) > 0) {
                guideFrom *= -1;
                guideWorld *= -1;
            }

            guideTo = DynamicElements.InverseTransformDirection(guideWorld);
            guideTo.Normalize();

            guideAxis = Vector3.Cross(guideFrom, guideTo);
            guideAxis.Normalize();

            var guideAngle = Math3d.SignedAngleBetween(guideFrom, guideTo, guideAxis);

            //Debug.Log(guideAngle);

            GuideArrow.gameObject.SetActive(Mathf.Abs(guideAngle) > 0.5);

            if (guideAngle < 0)
            {
                //guideAxis *= -1;
                GuideArrow.FlipY = true;
                guideAngle *= -1;
            }
            else
            {
                GuideArrow.FlipY = false;
            }

            GuideArrow.SetSize(guideAngle);

            GuideArrow.transform.parent.localRotation = Quaternion.LookRotation(guideTo,guideAxis);
        }

        public void OnDrawGizmos()
        {
            UnityEngine.Gizmos.color = Color.cyan;

            const float length = 5f;

            UnityEngine.Gizmos.DrawRay(DynamicElements.position, DynamicElements.TransformDirection(guideFrom) * length);
            UnityEngine.Gizmos.DrawRay(DynamicElements.position, DynamicElements.TransformDirection(guideTo) * length);

            UnityEngine.Gizmos.color = Color.yellow;

            UnityEngine.Gizmos.DrawRay(DynamicElements.position, DynamicElements.TransformDirection(guideAxis) * length);
        }
    }
}
