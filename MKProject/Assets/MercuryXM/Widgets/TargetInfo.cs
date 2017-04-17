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

namespace MercuryXM.Widgets
{
    [ExecuteInEditMode]
    public class TargetInfo : MonoBehaviour
    {
        //This is probably better named "AnchorInfo", "AnchorData", or simply "Anchor"
        //TODO: We might want to separate the threshold and completion checks to their own component, because something like Rubberband might not necessarily have a concept of completion (we just tie two things together)

        //TODO: We can include an offset here, maybe not as a matrix, but as separate rotation and position (so that they can be edited in the editor)
        // We can have a gettransform accessor that pre-multiplies the offset and force have everybody use that accessor
        // We can have a flag which indicates that the visual representation is not a child of the Target, and implement an Update function here, that automatically moves the visual representation to colocate with the target (plus offset).  This way, nobody else has to loop through anchors and update their visual representations externally

        public string Name;

        public Transform Target;

        public GameObject VisualRepresentation;

        /// <summary>
        /// MxmNode angular (in degrees) competion threshold.
        /// </summary>
        public float AngleThreshold = 5;

        /// <summary>
        /// MxmNode positional competion threshold.
        /// </summary>
        public float DistanceThreshold = float.MaxValue;

        /// <summary>
        /// Determines whether to use global 
        /// placement or local placement for position values.
        /// </summary>
        public bool UseGlobalTransform;
	    
    }
}
