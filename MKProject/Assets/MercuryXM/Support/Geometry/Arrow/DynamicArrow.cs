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
    public enum ArrowAlignType
    {
        Tip,
        Middle
    };

    public class DynamicArrow : MonoBehaviour
    {
        public ArrowAlignType AlignToCamera;
        public bool FlipY;
        public float Angle;
        public Material Material;

        MeshRenderer meshRenderer;

        public void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }

        /// <summary>
        /// Updates visibility of segments
        /// </summary>
        public void Update()
        {
            transform.localRotation = Quaternion.identity;

            if (FlipY)
            {
                transform.localRotation = Quaternion.AngleAxis(180, Vector3.forward);
            }

            if (AlignToCamera == ArrowAlignType.Middle)
            {
                transform.localRotation *= Quaternion.AngleAxis(Angle / 2f, Vector3.up);
            }
        }
	    
        public void SetSize(float amount)
        {
            if (float.IsNaN(amount)) return;

            if (Mathf.Abs(amount) > 359.5) amount = 0;

            // For negative angles, we flip the axis upside down, but set the size to the absolute value of angle
            FlipY = (amount < 0);

            Angle = Mathf.Abs(amount);

            meshRenderer.material.SetFloat("_Angle", Angle);
        }	
    }
}