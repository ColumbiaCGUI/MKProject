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
    public class ArrowProperties : MonoBehaviour {

        /// <summary>
        /// Width of the tail (default=1, along the up(Y)-axis)
        /// </summary>
        public float Width = 1;

        /// <summary>
        /// Width of the widest part of the head (default=1.5, along the up(Y)-axis)
        /// </summary>
        public float WidthHead = 3f;

        /// <summary>
        /// Depth of the arrow (default=0.1, i.e flat, along the forward(Z)-axis)
        /// </summary>
        public float Depth = 0.1f;

        /// <summary>
        /// Number of total segments (default = 30)
        /// </summary>
        public int SegmentsTotal = 30;

        /// <summary>
        /// Number of segments dedicated to the arrow head (default = 6)
        /// </summary>
        public int SegmentsHead = 6;

        public float HeadDepthMultiplier = 1.5f;

        public bool PointyTip = false;

        public int SubArrowCount = 1;
        public int SubArrowGap = 0;

        public int SegmentsTail { get { return SegmentsTotal - SegmentsHead; } }    
    }
}
