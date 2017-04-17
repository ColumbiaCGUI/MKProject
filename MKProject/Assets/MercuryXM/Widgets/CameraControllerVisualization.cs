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
using System.Linq;
using MercuryXM.Support.Extensions;
using UnityEngine;

namespace MercuryXM.Widgets
{
    public class CameraControllerVisualization : PlaceObject
    {

        public enum CameraControlMode
        {
            BinaryMode = 0
        };

        /// <summary>
        /// The control mode for the camera control visualization
        /// </summary>
        public CameraControlMode ControlMode;

        /// <summary>
        /// Distance 
        /// </summary>
        public float DistanceAlongForward;

        public float FOV = 45;
        private float OriginalFOV;

        public float OrthographicSize = 2.5f;
        private float OriginalOrthographicSize;

        public bool UpdateSizeEachFrame = true;
        public bool SetToOrthographic = false;

        public Camera cam;
        public Dictionary<string, GameObject> Anchors; 


        // Use this for initialization
        public override void Awake ()
        {
            Anchors = new Dictionary<string, GameObject>();

            if (cam == null)
                cam = Camera.main;

            Object = cam.gameObject.transform;
            Object.transform.GetPosRot(
                out origPosition, out origRotation, true);

            OriginalOrthographicSize = cam.orthographicSize;
            OriginalFOV = cam.fieldOfView;
        }

        public override void Start()
        {
            //Todo: Why is this value zero??
            //Object.transform.GetPosRot(
            //    out origPosition, out origRotation, true);
        }
	
        // Update is called once per frame
        public override void Update ()
        {
            PrepareCameraTransformation();

            if (UpdateSizeEachFrame)
            {
                if (SetToOrthographic)
                    cam.orthographicSize = OrthographicSize;
                else
                    cam.fieldOfView = FOV;
            }
        }

        /// <summary>
        /// SetActive here will not call into MxmNode SetActive
        /// This will simply capture the current position/rotation before
        /// activating the visualization.
        /// </summary>
        /// <param name="active"></param>
        public override void SetActive(bool active)
        {
			base.SetActive (active);

			PrepareCameraSettings (active);

            enabled = active;
        }

        /// <summary>
        /// The the transformation of the camera based on the current visualization
        /// </summary>
        public void PrepareCameraTransformation()
        {
            if (ControlMode == CameraControlMode.BinaryMode &&
                Anchors.Count == 2)
            {
                Transform anchor0 = Anchors.ElementAt(0).Value.transform;
                Transform anchor1 = Anchors.ElementAt(1).Value.transform;

				if (!anchor0.hasChanged || !anchor1.hasChanged)
					return;

                Vector3 posOfMidpoint = 
                    (anchor0.position + anchor1.position)/2;

                Vector3 lrDir = anchor1.transform.position -
                                anchor0.transform.position;
                Vector3 inDir = Vector3.Cross(lrDir, Vector3.up).normalized;

                Object.transform.position = posOfMidpoint + 
                                            DistanceAlongForward * inDir;

                Object.transform.LookAt(posOfMidpoint);
            }
        }

		public void PrepareCameraSettings(bool active)
		{
			if (!active)
			{
				if(SetToOrthographic)
					cam.orthographic = false;

				if(SetToOrthographic)
					cam.orthographicSize = OriginalOrthographicSize;
				else
					cam.fieldOfView = OriginalFOV;
			}
			else
			{
				if (SetToOrthographic)
					cam.orthographic = true;

				if(SetToOrthographic)
					OriginalOrthographicSize = cam.orthographicSize;
				else
					OriginalFOV = cam.fieldOfView;
			}
		}
    }
}
