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
using UnityEngine;

namespace MercuryXM.Widgets
{
    /// <summary>
    /// Simple, multi-endpoint rubber band visualization with
    /// GameObject endpoints.
    /// </summary>
    public class MultiAnchorRubberBand : MultiAnchorVisualization
    {
        /// <summary>
        /// Gets or sets the rubber band's line renderer.
        /// </summary>
        /// <value>The rubber band line renderer.</value>
        public LineRenderer RBLineRenderer;

        /// <summary>
        /// Gets or sets the rubber band's material.
        /// </summary>
        /// <value>The rubber band material.</value>
        public Material RBMaterial;

        /// <summary>
        /// Default width used for all endpoints of the rubberband
        /// </summary>
        public float RBDefaultLineWidth;

        /// <summary>
        /// Gets or sets the default endpoint scale.
        /// </summary>
        /// <value>The endpoint scale.</value>
        public Vector3 RBDefaultEndpointScale;

        /// <summary>
        /// Gets or sets the default endpoint material.
        /// </summary>
        /// <value>The default endpoint mat.</value>
        public Material RRDefaultEndpointMat;

        /// <summary>
        /// Initializes a new instance of the <see cref="RubberBand"/> class.
        /// </summary>
        public MultiAnchorRubberBand() : base()
        {
            RBDefaultEndpointScale = Vector3.one;
            RBDefaultLineWidth = 1f;
            RRDefaultEndpointMat = new Material(Shader.Find("Diffuse"));
        }
    
        public virtual void InitializeRBVisualization(
            Dictionary<string, GameObject> anchors,
            Dictionary<string, GameObject> shapes,
            Vector3 defaultEndpointScale,
            Material defaultLineMat = null,
            Material defaultEndpointMat = null,
            float defaultLineWidth = 1f,
            bool anchorAutoAttach = true)
        {
            if(defaultEndpointMat != null)
                RRDefaultEndpointMat = defaultEndpointMat;

            if (defaultLineMat != null)
                RBMaterial = defaultLineMat;

            RBDefaultEndpointScale = defaultEndpointScale;
            RBDefaultLineWidth = defaultLineWidth;

            AddEndpoints(anchors, shapes);

            AnchorAutoAttach = anchorAutoAttach;

            CreateRubberBandLineRenderer();
        }

        /// <summary>
        /// Update this instance.
        /// </summary>
        public void Update()
        {
            UpdateAnchors();
        }

        /// <summary>
        /// Updates the anchors.
        /// </summary>
        protected override void UpdateAnchors()
        {
            base.UpdateAnchors ();

            int i = 0;
            foreach(var endpoint in Anchors.Values)
            {
                RBLineRenderer.SetPosition (i, endpoint.VisualRepresentation.transform.position);
                i++;
            }
        }

        /// <summary>
        /// Add endpoint objects to the visualization base as gameobject dictionary
        /// </summary>
        /// <param name="endpointObjs"></param>
        protected virtual void AddEndpoints(Dictionary<string, GameObject> endpointObjs, 
            Dictionary<string, GameObject> endpointShapes)
        {
            if (endpointShapes == null)
                endpointShapes = new Dictionary<string, GameObject> ();

            foreach (var anchor in endpointObjs)
            {
                GameObject endpointShape;
                endpointShapes.TryGetValue (anchor.Key, out endpointShape);

                if(endpointShape == null)
                    endpointShape =  new GameObject(anchor.Key + "_EndPoint"); 

                //Assign the scale based on default.
                endpointShape.transform.localScale = RBDefaultEndpointScale;

                //Assign the material
                if (RRDefaultEndpointMat != null)
                    endpointShape.GetComponent<Renderer>().material =
                        RRDefaultEndpointMat;
            
                AddAnchor(anchor.Key, anchor.Value, endpointShape);
            }
        }

        /// <summary>
        /// Instantiate a rubber band based on current set parameters
        /// </summary>
        protected virtual void CreateRubberBandLineRenderer()
        {
            RBLineRenderer =
                GetComponent<LineRenderer>() ? GetComponent<LineRenderer>() :
                    gameObject.AddComponent<LineRenderer>();

            if (RBMaterial != null)
            {
                RBLineRenderer.material = RBMaterial;
            }

            RBLineRenderer.SetWidth(RBDefaultLineWidth, RBDefaultLineWidth);

            RBLineRenderer.SetVertexCount(Anchors.Count);
        }
    }
}
