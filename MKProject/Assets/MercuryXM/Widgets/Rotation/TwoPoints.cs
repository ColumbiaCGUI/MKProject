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

namespace MercuryXM.Widgets
{
    public enum TwoPointAlignType
    {
        DefaultHandles,
        FrontHemisphereObjectAligned,
        FrontHemisphereAngleOffset,
        OptimalAxis
    }

    public class TwoPoints : BasicTransformationTaskBehavior, IWFTransformationWidget, IMxmBehavior
    {

        public TwoPointAlignType AlignToCamera;

        public float[] Angles = new float[2];
        public Vector3[] Handles = new Vector3[2];
        public DynamicArrow[] Arrows = new DynamicArrow[2];
        public Transform[] HandleLines = new Transform[2];

        public Transform TargetRingParent;

        public TwoPointTarget Targets;

        public Color CompleteColor = Color.green;

        #region Private Fields

		private Color[] originalColors = new Color[2];
		private Material[] handleMaterials = new Material[2];

        readonly Vector3[] defaultHandles =
        {
            new Vector3 (0,0,-1),
            new Vector3 (-1,0,0)
        };

        readonly Vector3[] handleTo = new Vector3[2];
        readonly Vector3[] offsetRotAxis = new Vector3[2];
        readonly float[] radius = new float[2];
        
        GameObject pathParent;
        readonly List<TwoPointTarget> crumbs = new List<TwoPointTarget>();
        float pathStepSize = 10f; // every n degrees

        public bool ShowArrows = false;
        public bool ShowPath = true;
        public bool ShowConnectingBar = true;

        #endregion

        public override void Awake()
        {
            base.Awake();

			for (var i = 0; i < handleMaterials.Length; i++)
			{
				handleMaterials[i] = Targets.Rings[i].GetComponent<Renderer>().material;
				originalColors[i] = handleMaterials[i].color;
			}		
        }

        void RefreshHandles()
        {
            //Debug.Log("Refresh Called in TwoPoints");
            for (var i = 0; i < Arrows.Length; i++)
            {
                if (Arrows[i] == null) continue;

                radius[i] = 2.5f;

                switch (AlignToCamera)
                {
                    case TwoPointAlignType.DefaultHandles:
                        Handles[i] = defaultHandles[i];
                        break;
                    case TwoPointAlignType.FrontHemisphereObjectAligned:
                        Handles[i] = DefaultFrontHemisphere(i);
                        break;
                    case TwoPointAlignType.FrontHemisphereAngleOffset:
                        Handles[i] = FrontHemisphereAngleOffset(i, 30);
                        break;
                    case TwoPointAlignType.OptimalAxis:
                        Handles[i] = OptimalAxis(i);
                        break;
                }

                //Debug.Log("To Pos: " + To.position + "; To Rot: " + To.rotation);
                //Debug.Log("From Pos: " +From.position + "; From Rot: " + From.rotation);
                //Debug.Log("AlignToCamera: " + AlignToCamera);
                //Debug.Log("Handles[" + i + "]" + Handles[i]);
                //Debug.Log("Camera Pos: " + Camera.main.transform.position
                //    + "Camera Rot: " + Camera.main.transform.rotation);
            }
        }

        void RefreshTargetRings() {
		
            TargetRingParent.rotation = To.rotation;

        }

		public override void Refresh(List<MxmTransform> transformList)
        {
			base.Refresh(transformList);
            base.Recalculate(); // We need axis calculated
            RefreshHandles();
            Targets.Prepare(Handles, ShowConnectingBar);
            PreparePath();
        }

        #region Different Options for Initial Handle Placement

        public bool IsFacingAwayFromCamera(Vector3 directionInWorldSpace)
        {
            var dot = Vector3.Dot(directionInWorldSpace, Camera.main.transform.forward);
            return dot >= 0;
        }

        public Vector3 OptimalAxis(int i)
        {
            var facingAway = IsFacingAwayFromCamera(From.TransformDirection(axis));

            if (i == 0) return (facingAway) ? (axis * -1) : axis;

            // We want the arrow to end at the interection of Camera Forward and rotation plane
            // So we calculate where that is in To's space (where arrow will end), and then convert
            // back into From's space
            var axisTo = To.InverseTransformDirection(From.TransformDirection(axis));

            var vizToCamLocal = Camera.main.transform.position - transform.position;
            vizToCamLocal.Normalize();
            vizToCamLocal = To.InverseTransformDirection(vizToCamLocal);

            var vizToCamOnRotationPlane = Math3d.ProjectVectorOnPlane(axisTo, vizToCamLocal);
            vizToCamOnRotationPlane.Normalize();

            facingAway = IsFacingAwayFromCamera(vizToCamOnRotationPlane);

            if (facingAway)
                vizToCamOnRotationPlane *= -1;

            var handle = To.TransformDirection(vizToCamOnRotationPlane);
            handle = From.InverseTransformDirection(handle);

            return handle;
        }

        public Vector3 DefaultFrontHemisphere(int i)
        {
            Handles[i] = defaultHandles[i];

            // Flip vector if it's pointing away from the camera
            var camTrans = Camera.main.transform;
            var handleTransformed = To.TransformDirection(Handles[i]);
            var dot = Vector3.Dot(handleTransformed, camTrans.forward);

            return (dot > 0) ? Handles[i]*-1 : Handles[i];        
        }

        public Vector3 FrontHemisphereAngleOffset(int i, float angleOffset)
        {
            // Find the vector towards camera
            var vizToCamLocal = Camera.main.transform.position - transform.position;
            vizToCamLocal.Normalize();
            vizToCamLocal = To.InverseTransformDirection(vizToCamLocal);

            // Find camera's up vector
            var camUpLocal = Camera.main.transform.up;
            camUpLocal = To.InverseTransformDirection(camUpLocal);

            // Rotate camera direction by angleOffset around camera up
            // (either in the positive or negative direction)
            var adjAngle = (i == 0) ? angleOffset : -angleOffset;

            var handleTo2 = Quaternion.AngleAxis(adjAngle, camUpLocal)*vizToCamLocal;

            // Convert from To's space to From's space (which is what we're anchored to)
            handleTo2 = To.TransformDirection(handleTo2);
            handleTo2 = From.InverseTransformDirection(handleTo2);

            // That is where we want the arrow to end (= the tip), we return where the arrow should start
            var rot = Quaternion.Inverse(From.rotation)*To.rotation;
            var invRot = Quaternion.Inverse(rot);

            return invRot*handleTo2;
        }

        #endregion

        public override void Update()
        {
            base.Update();
        
            if (pathParent != null)
                pathParent.SetActive(ShowPath);

            for (var i = 0; i < Arrows.Length; i++)
            {
                Arrows[i].gameObject.SetActive(ShowArrows);
                if (ShowArrows)
                    Arrows[i].SetSize(Angles[i]);            
            }
        }

        protected override void Recalculate()
        {
            base.Recalculate();

            if (From == null || To == null) return;

            transform.rotation = From.rotation;

            RefreshTargetRings(); // This has to be done every frame, because the whole Euler gameObject mirrors From's orientation,
            // so we need to keep "resetting" targetRing's parent to To's orientation

            RefreshPath();

            for (var i = 0; i < Handles.Length; i++)
            {
                // Calculate where handles should end up
                handleTo[i] = rotationFromTo*Handles[i];

                // Calculate axis for rotating handle from its 'from' position to its 'to' position
                offsetRotAxis[i] = Vector3.Cross(Handles[i], handleTo[i]);

                // Calculate angle for that rotation
                Angles[i] = Math3d.SignedAngleBetween(Handles[i], handleTo[i], offsetRotAxis[i]);

                // Align arrow's 'forward' direction with handle's 'to' postion
                // and arrow's 'up' direction with handle rotation axis
                Arrows[i].transform.parent.localRotation = Quaternion.LookRotation(handleTo[i], offsetRotAxis[i]);

                // Handles are siblings of arrows, i.e. also child of the node that was rotated in the line above
                // As a reminder, arrows span in the X-Z plane and their tip points foward, i.e. positive Z (0,0,1)
                // This is why we rotate by negative amount of the remaining angle along the up axis (i.e. Y-axis)
                HandleLines[i].localRotation = Quaternion.AngleAxis(-Angles[i], Vector3.up);

                // If one of the handles is within a threshold, turn the handle and rings a different color
                handleMaterials[i].color = (Angles[i] < 3) ? CompleteColor : originalColors[i];
            }
        }

        void PreparePath() {

            var crumbTemplate = TargetRingParent.gameObject;

            var steps = Mathf.FloorToInt(180/pathStepSize);

            if (pathParent != null) {
                crumbs.Clear();
                Destroy(pathParent);
            }

            pathParent = new GameObject("Path");
            pathParent.transform.parent = transform;

            for(var i = 0; i < steps; i++) {
                var crumb = Instantiate(crumbTemplate);
                crumb.transform.parent = pathParent.transform;
                var target = crumb.GetComponent<TwoPointTarget>();
                crumbs.Add(target);

                for (var j = 0; j < target.Rings.Length; j++)
                {
                    target.Rings[j].gameObject.SetActive(false);
                    target.PathMarkers[j].gameObject.SetActive(true);                    
                }
            }
        }

        void RefreshPath() {

            if (pathParent == null) return;

            pathParent.transform.rotation = To.rotation;

            var rotToFrom = Quaternion.Inverse(rotationFromTo);

            var shownSteps = Mathf.FloorToInt(angle/pathStepSize);

            for(var i = 0; i < crumbs.Count; i++) {
                var shown = (i < shownSteps);
                crumbs[i].gameObject.SetActive(shown);

                if (!shown) continue;
                var step = ((float)(i + 1))/(shownSteps + 1);
                crumbs[i].transform.localRotation = Quaternion.Slerp(Quaternion.identity, rotToFrom, step);

                for (var j = 0; j < crumbs[i].PathMarkers.Length; j++)
                {
                    // For the first crumb, we turn away from previous marker
                    // For subsequent crumbs, we turn towards next marker
                    var index = (i == 0)
                        ? i + 1
                        : i - 1;

                    var nextMarkerWorldPos = crumbs[index].PathMarkers[j].position;
                    var dir = crumbs[i].PathMarkers[j].parent.InverseTransformPoint(nextMarkerWorldPos);
                    dir.y = 0;
                    dir.Normalize();

                    if (i == 0)  // For the first crumb, we turn away from previous marker
                        dir *= -1;

                    crumbs[i].PathMarkers[j].localRotation = Quaternion.LookRotation(dir, Vector3.up);
                }
            }

            for (var i = 0; i < crumbs[0].PathMarkers.Length; i++)
            {
                var dist = (crumbs[0].PathMarkers[i].position - crumbs[1].PathMarkers[i].position).magnitude;
                
                var freq = Mathf.CeilToInt(0.3f / dist);
                
                for (var j = 0; j < crumbs.Count; j++)
                {
                    crumbs[j].PathMarkers[i].gameObject.SetActive((j % freq) == 0);
                }
            }
        }

        public void OnDrawGizmosSelected()
        {
            const float length = 3;

            UnityEngine.Gizmos.color = Color.cyan;

			if (From == null) return;
            
			UnityEngine.Gizmos.DrawRay(transform.position, From.TransformDirection(axis) * length);
        }

        public void OnDestroy()
        {
            for (var i = 0; i < handleMaterials.Length; i++)
            {
                handleMaterials[i].color = originalColors[i];
            }
        }
    }
}