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

using MercuryXM.Support.Geometry.Arrow;
using MercuryXM.Support.GUI;
using UnityEngine;

namespace MercuryXM.Widgets
{
    public class EulerAngles : BasicTransformationTaskBehavior
    {

        public Vector3 Angles;
        public ArrowAlignType AlignToCamera;
        public AutoAlignType AutoAlign;
        public bool AutoHide;
        public bool OneAtATime;
    
        public Transform ReferenceFrame;

		public GameObject Canvas;
		public GameObject UI2DElements;

        readonly string[] names = {"XAxis","YAxis","ZAxis"};
        readonly DynamicArrow[] arrows = new DynamicArrow[3];
        AutoAlign[] autoAlign;
        private Quaternion fromOriginal;
        private Vector3 originalAngles;

        // Use this for initialization
        public override void Awake () {

            base.Awake();

            for (var i = 0; i < arrows.Length; i++) {
                arrows[i] = transform.FindChild(names[i]).GetComponentInChildren<DynamicArrow>();
                if (arrows[i] == null) continue;
                arrows[i].transform.localRotation = Quaternion.identity;
            }

            autoAlign = GetComponentsInChildren<AutoAlign>();

			// If Canvas is not assigned directly, try to grab it from GUIHandler
			if (Canvas == null) {
				if (MxmGUIHandler.Instance != null && MxmGUIHandler.Instance.Canvas != null) {
					Canvas = MxmGUIHandler.Instance.Canvas;
				}
			}

			if (Canvas != null) {
				UI2DElements.transform.SetParent(Canvas.transform, false);
			}
        }

        public bool CheckNull() {
            for (var i = 0; i < arrows.Length; i++) {
                if (arrows[i] == null) return true;
            }
            return (To == null);
        }

        public override void Update()
        {
            base.Update();

			//Refresh(new MxmTransform());

            if (CheckNull()) return;

            if (OneAtATime) {
                var closenessThreshold = 5; // degrees
                var reallyCloseThreshold = 2; // degrees

                var isCloseY = (Mathf.Abs(Angles.y) < closenessThreshold);
                var isCloseX = (Mathf.Abs(Angles.x) < closenessThreshold);

                var isReallyCloseX = (Mathf.Abs(Angles.x) < reallyCloseThreshold);
                var isReallyCloseY = (Mathf.Abs(Angles.y) < reallyCloseThreshold);
                var isReallyCloseZ = (Mathf.Abs(Angles.z) < reallyCloseThreshold);

                arrows[1].gameObject.SetActive(!isReallyCloseY);
                arrows[0].gameObject.SetActive(!isReallyCloseX && isCloseY);			
                arrows[2].gameObject.SetActive(!isReallyCloseZ && isCloseY && isCloseX);
            }

            if (AutoHide)
            {
                var autoHideThreshold = 3; // degrees
            
                var isCloseY = (Mathf.Abs(Angles.y) < autoHideThreshold);
                var isCloseX = (Mathf.Abs(Angles.x) < autoHideThreshold);
                var isCloseZ = (Mathf.Abs(Angles.z) < autoHideThreshold);

                arrows[1].gameObject.SetActive(!isCloseY);
                arrows[0].gameObject.SetActive(!isCloseX);
                arrows[2].gameObject.SetActive(!isCloseZ);
            }

            for (var i = 0; i < arrows.Length; i++) {
                arrows[i].AlignToCamera = AlignToCamera;
                arrows[i].SetSize(Angles[i]);
            }

            if (autoAlign == null) return;
            for (var i = 0; i < autoAlign.Length; i++)
            {
                autoAlign[i].AlignMode = AutoAlign;            
            }
        }
	
        public static Vector3 GetAngles(Quaternion rot) {
            return WrapAngles(rot.eulerAngles);
        }

        public static Vector3 WrapAngles(Vector3 euler)
        {
            for (var i = 0; i < 3; i++)
            {
                if (euler[i] > 180.0f)
                    euler[i] -= 360.0f;
            }

            return euler;
        }

        public void RefreshUnused()
        {
            if (ReferenceFrame == null)
            {
                fromOriginal = From.rotation;

                var originalRot = Quaternion.Inverse(fromOriginal)*To.rotation;

                originalAngles = GetAngles(originalRot);

                // We assign the arrows/rings to "from" at the beginning, but do not
                // continuously update afterwards

                transform.rotation = From.rotation;
            }
            else
            {
                fromOriginal = Quaternion.Inverse(ReferenceFrame.rotation) * From.rotation;

                var toInReferenceFrame = Quaternion.Inverse(ReferenceFrame.rotation) * To.rotation;
                var originalRot = Quaternion.Inverse(fromOriginal) * toInReferenceFrame;

                originalAngles = GetAngles(originalRot);

                // We assign the arrows/rings to "from" at the beginning, but do not
                // continuously update afterwards

                transform.rotation = ReferenceFrame.rotation;
            }
        }

        protected override void Recalculate()
        {
            base.Recalculate();

            if (From == null || To == null) return;

            // Euler angles are relative to where we started from, so we keep a copy

            fromOriginal = From.rotation;

            var originalRot = Quaternion.Inverse(fromOriginal) * To.rotation;

            originalAngles = GetAngles(originalRot);

            // We assign the arrows/rings to "from" at the beginning, but do not
            // continuously update afterwards

            transform.rotation = From.rotation;

            //var axisCopy = axis;

            //if (ReferenceFrame != null)
            //{
            //    var axisWorld = GetFrom().InverseTransformDirection(axisCopy);
            //    axisCopy = ReferenceFrame.TransformDirection(axisWorld);
            //    //transform.localRotation = Quaternion.Inverse(GetFrom().rotation) * ReferenceFrame.rotation;
            //    transform.rotation = ReferenceFrame.rotation;
            //}
            //else
            //{
            //    transform.rotation = From.rotation;
            //}

            // We find the current rotation relative to our starting pose
            var currentRelativeToBeginning = (angle < 0.5)
                ? Quaternion.identity
                : Quaternion.Inverse(fromOriginal) * From.rotation;

            // Euler angles for current 
            var angleProgressSoFar = GetAngles(currentRelativeToBeginning);

            // Calculate remaining degrees (this will make sure arrows are
            // getting smaller as we near our target rotation)
            Angles = originalAngles - angleProgressSoFar;
        }
    }
}
