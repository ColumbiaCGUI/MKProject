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
using MercuryXM.Framework;
using MercuryXM.Support.Extensions;
using UnityEngine;
using UnityEngine.Events;

namespace MercuryXM.Widgets
{
    public class MultiAnchorVisualization : MonoBehaviour, IMxmBehavior
    {
        #region IMxmBehavior Implementation

        event IMxmCallback WFAwakeCompleteCallback;
        event IMxmCallback WFStartCompleteCallback;

        public void MxmOnStartComplete()
        {
            if (WFStartCompleteCallback != null)
                WFStartCompleteCallback();
        }

        public void MxmOnAwakeComplete()
        {
            if (WFAwakeCompleteCallback != null)
                WFAwakeCompleteCallback();
        }

        public void MxmRegisterAwakeCompleteCallback(IMxmCallback callback)
        {
            WFAwakeCompleteCallback += callback;
        }

        public void MxmRegisterStartCompleteCallback(IMxmCallback callback)
        {
            WFStartCompleteCallback += callback;
        }

        #endregion

        //TODO: On awake, getcomponents for TargetInfo and populate dictionary
        //TODO: Vice versa: create components from dictionary 
        //TODO: There should probably be other Event like OnDestroy and OnSetVizActive, so that implementation can add delegates and handle special cases if necessary

        public UnityAction OnInitialize;

        /// <summary>
        /// Gets or sets the anchors. 
        /// Anchors are the external objects that are used by the visualizations.
        /// </summary>
        /// <value>The anchors.</value>
        public Dictionary<string,TargetInfo> Anchors;

        /// <summary>
        /// Gets or sets a value indicating whether this 
        /// <see cref="MultiAnchorVisualization"/> anchor will auto attach to the anchors.
        /// </summary>
        /// <value><c>true</c> if anchor auto attach; otherwise, <c>false</c>.</value>
        public bool AnchorAutoAttach;

        /// <summary>
        /// Root of the visualization. Changing this 
        /// will modify all of this visualization's game objects 
        /// </summary>
        private Transform visualizationRoot;

        /// <summary>
        /// MxmNode layer.
        /// </summary>
        private string visualizationLayer;

        /// <summary>
        /// Gets or sets the visualization layer.
        /// </summary>
        /// <value>The visualization layer.</value>
        public string VisualizationLayer
        {
            get { return visualizationLayer; }
            set
            {
                visualizationLayer = value;
            }
        }

        [SerializeField]
        private MxmTag _mxmTag = MxmTagHelper.Everything;

        [SerializeField]
        private bool tagCheckEnabled = false;

        [SerializeField]
        private int priority = 0;

        public GameObject MxmGameObject
        { get { return gameObject; } }

        public MxmTag Tag
        {
            get { return _mxmTag; }
            set { _mxmTag = value; }
        }

        public bool TagCheckEnabled
        {
            get { return tagCheckEnabled; }
            set { tagCheckEnabled = value; }
        }

        public int Priority
        {
            get { return priority; }
            set { priority = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiAnchorVisualization"/> class.
        /// </summary>
        public MultiAnchorVisualization () : 
            base()
        {
            AnchorAutoAttach = true;

            //Default dictionaries
            Anchors = new Dictionary<string, TargetInfo>();

            VisualizationLayer = "Default";
        }

        public void Awake()
        {
            VisualizationRoot = gameObject.transform;
        }

        // TODO: This doesn't seem to be used anymore.  We are using the AddAnchor below
        // Having an external entity pass a list of TargetInfos probably doesn't make sense
        /// <summary>
        /// Initialize an anchored visualization
        /// </summary>
        /// <param name="anchors"></param>
        /// <param name="offsetsFromAnchors"></param>
        /// <param name="visualizationTargets"></param>
        /// <param name="operationMode"></param>
        /// <param name="defaultEndpointMat"></param>
        /// <param name="anchorAutoAttach"></param>
        /// <param name="anchorParentName"></param>
        public virtual void InitializeAnchoredVisualization(
            Dictionary<string, TargetInfo> anchors,
            bool anchorAutoAttach = true)
        {
            //If anchors is not null then try to assign anchors
            if (anchors != null)
            {
                Anchors = anchors;
            }

            //Set whether associated visualization game objects 
            AnchorAutoAttach = anchorAutoAttach;

            if (OnInitialize != null)
                OnInitialize();
        }

        /// <summary>
        /// Adds a game object anchor.
        /// </summary>
        /// <returns><c>true</c>, if game object was added, <c>false</c> otherwise.</returns>
        /// <param name="anchor">Anchor.</param>
        /// <param name="visualizationGameObject">MxmNode game object.</param>
        /// <param name="target">Target.</param>
        public virtual TargetInfo AddAnchor(string name, GameObject anchor,
            GameObject visualizationGameObject = null)
        {
            //Add the TargetInfo Component to the visualization
            var targetInfo = gameObject.AddComponent<TargetInfo>();
            targetInfo.Name = name;
            targetInfo.Target = anchor.transform;
            Anchors.Add(name, targetInfo);

            if (visualizationGameObject != null)
            {
                //Create enpoint game object
                GameObject endpoint = visualizationGameObject;

                //Assign the layer
                endpoint.layer = LayerMask.NameToLayer(VisualizationLayer);

                //Assign the endpoint's parent
                if (AnchorAutoAttach)
                    endpoint.transform.parent = anchor.transform;
                else
                    endpoint.transform.parent = VisualizationRoot.transform;

                targetInfo.VisualRepresentation = endpoint;
            }
	    
            return targetInfo;
        }

        /// <summary>
        /// Removes the anchor.
        /// </summary>
        /// <param name="key">Key.</param>
        public virtual void RemoveAnchor(string key)
        {
            Anchors.Remove (key);
        }

        // TODO: This can probably happen inside each individual TargetInfo (please see my comment there)
        /// <summary>
        /// Updates the anchors.
        /// </summary>
        protected virtual void UpdateAnchors()
        {
            if (!AnchorAutoAttach) {
                foreach (TargetInfo anchor in Anchors.Values)
                {
                    var mat = anchor.Target.localToWorldMatrix;
				
                    MatrixUtilities.AssignGlobalTransform (
                        anchor.VisualRepresentation,
                        anchor.Target.transform.localToWorldMatrix *
                        mat);
                }
            }
        }

        public void Refresh()
        {
        }

        /// <summary>
        /// Set visualization's activity state.
        /// </summary>
        /// <param name="active"></param>
        public virtual void SetActive(bool active)
        {
            foreach (TargetInfo target in Anchors.Values) {
                if (target.VisualRepresentation != null) {
                    target.VisualRepresentation.SetActive (active);
                }
            }
        }

        /// <summary>
        /// Override this method to return whether a task is finished.
        /// </summary>
        /// <returns>Returns whether the task represented by the 
        /// visualization is finished.
        /// </returns>
        public virtual bool IsComplete()
        {
            return false;
        }

        /// <summary>
        /// Gets or sets the visualization parent.
        /// </summary>
        /// <value>The anchor parent.</value>
        public Transform VisualizationRoot
        {
            get
            {
                return visualizationRoot;
            }
            set
            {
                visualizationRoot = value;
            }
        }

        public void MxmInvoke(MxmMessageType msgType, Mxmessage message)
        {
            var type = message.MxmMethod;

            switch (type)
            {
                case MxmMethod.SetActive:
                    var param = (MxmessageBool)message;
                    SetActive(param.value);
                    break;
                case MxmMethod.Refresh:
                    Refresh();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

		public  MxmNode GetNode ()
		{
			return GetComponent <MxmNode>();
		}
    }
}