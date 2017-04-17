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

using UnityEngine;

namespace MercuryXM.Support.Geometry.Arrow
{
    public class ArrowShapeStraight : MonoBehaviour, IArrowShape
    {
        /// <summary>
        /// Maximum length of the arrow (dafault = 1)
        /// </summary>
        public float Length = 1;

        public ArrowProperties Properties
        {
            get { return properties; }
            set { properties = value; }            
        }

        [SerializeField] private ArrowProperties properties;

        /// <summary>
        /// Returns the length of 1 segment
        /// </summary>
        /// <returns>Length of 1 segment</returns>
        public float SegmentSize()
        {
            return Length/Properties.SegmentsTotal;
        }

        /// <summary>
        /// Creates vertices and normals for a given arrow segment
        /// </summary>
        /// <param name="currentSegment">A segment index along the arrow (0 to SegmentsTotal)</param>
        /// <param name="vTopOut">Top Outside Vertex</param>
        /// <param name="vBotOut">Bottom Outside Vertex</param>
        /// <param name="vTopIn">Top Inside Vertex</param>
        /// <param name="vBotIn">Bottom Inside Vertex</param>
        /// <param name="nIn">Inside pointing normal</param>
        /// <param name="nOut">Outside pointing normal</param>
        /// <param name="depthMultiplier">We this to have arrow heads also have narrowing depth</param>
        public void CreateXZGeometry(int currentSegment,
            out Vector2 vTopOut,
            out Vector2 vBotOut,
            out Vector2 vTopIn,
            out Vector2 vBotIn,
            out Vector2 nIn, out Vector2 nOut,
            float depthMultiplier = 1
            )
        {
            // Otherwise the arrow goes along the x-axis and its width is along the z-axis

            var len = Length - currentSegment * SegmentSize();
            var dep = Properties.Depth/2 * depthMultiplier;

            var xOut = dep;
            var xIn = -dep;

            var zOut = -len;
            var zIn = -len;

            // Outside Face

            nOut = new Vector2(1, 0);

            vTopOut = new Vector2(xOut, zOut);
            vBotOut = new Vector2(xOut, zOut);

            // Inside Face

            vTopIn = new Vector2(xIn, zIn);
            vBotIn = new Vector2(xIn, zIn);

            nIn = new Vector2(-1, 0);
        }

        public Vector3 AssignWidth(Vector2 inOutCoor, float width)
        {
            return new Vector3(width, inOutCoor[0], inOutCoor[1]);
        }

        public string Shader()
        {
            return "Standard";
        }
    }
}