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

using MercuryXM.Framework;
using MercuryXM.Support.Extensions;
using MercuryXM.Support.GUI;
using UnityEngine;

namespace MercuryXM.Widgets
{
    /// <summary>
    /// Simple static visualization. Tracked Object on the right,
    /// target object on the left.
    /// </summary>
    public class StaticVisualization : BasicTransformationTaskBehavior, IMxmBehavior
    {
        /// <summary>
        /// GameObject to activate on SetActive
        /// </summary>
        public GameObject TargetObject;
        public GameObject CamControlObj, PlacementControlObj;

        private MxmNode camNode, placementNode;

		//Todo: Add ability to use references (CamControlObj, PlacementControlObj) 
		//  instead of instantiating based on GameObjects.

        public CameraControllerVisualization CameraController;
        public PlaceObject PlacementController;

        public Vector3 offset;
        public float CameraOffset = -5;

        public GameObject Canvas;
        public GameObject UI2DElements;

        public override void Awake()
        {
            base.Awake();

            if (Canvas == null)
            {
                if (MxmGUIHandler.Instance != null && MxmGUIHandler.Instance.Canvas != null)
                {
                    Canvas = MxmGUIHandler.Instance.Canvas;
                }
            }

            if (Canvas != null)
            {
                UI2DElements.transform.SetParent(Canvas.transform, false);
            }
        }
        
        public override void Initialize()
        {
            base.Initialize();
            
            if (CamControlObj == null)
            {
                CamControlObj = new GameObject("CamControlObj");
                CameraController =
                    CamControlObj.AddComponent<CameraControllerVisualization>();
                camNode = CamControlObj.AddComponent<MxmNode>();
				camNode.MxmAddToBehaviorList(CameraController, "CameraController");                
            }
            else
            {
                CamControlObj =
                    Instantiate(CamControlObj) as GameObject;
                CameraController =
                    CamControlObj.GetComponent<CameraControllerVisualization>();
                camNode = CamControlObj.GetComponent<MxmNode>();
            }
            CamControlObj.transform.parent = transform;
            CameraController.DistanceAlongForward = CameraOffset;

            if (PlacementControlObj == null)
            {
                PlacementControlObj = new GameObject("PlacementControlObj");
                PlacementController =
                    PlacementControlObj.AddComponent<PlaceObject>();
                placementNode = PlacementControlObj.AddComponent<MxmNode>();
				placementNode.MxmAddToBehaviorList(PlacementController, "PlacementController");                
            }
            else
            {
                PlacementControlObj =
                    Instantiate(PlacementControlObj) as GameObject;
                PlacementController =
                    PlacementControlObj.GetComponent<PlaceObject>();
                placementNode = PlacementControlObj.GetComponent<MxmNode>();
            }
            PlacementController.ReturnObjectToOrig = false;
            PlacementController.AutoSetInActivate = false;
            PlacementController.setRot = false;
            PlacementControlObj.transform.parent = transform;

            var staticVizNode = GetComponent<MxmNode>();

            staticVizNode.MxmAddToBehaviorList(camNode, "CameraController");
            staticVizNode.MxmAddToBehaviorList(placementNode, "PlacementController");

            //staticVizNode.RefreshIWFBehaviors();

            TargetObject = To.GetChild(0).gameObject;

            CameraController.Anchors.Add("From", From.gameObject);
            CameraController.Anchors.Add("To", To.gameObject);

            PlacementController.Object = To;

            PlacementController.origPosition = To.position;
            PlacementController.origRotation = To.rotation;
        }

        public override void Update()
        {
            base.Update();

            if (From == null)
                return;

            if (PlacementController != null && PlacementController.Object != null)
                PlacementController.Object.SetPosition(From.position + offset, true);
        }

        public override void SetActive(bool active)
        {
            base.SetActive(active);

			if (PlacementController != null && PlacementController.Object != null)
				PlacementController.Object.SetPosition(From.position + offset, true);

            if (TargetObject != null)
                TargetObject.SetActive(active);
        }
    }
}
