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

namespace MercuryXM.Support.Math.Smoothing
{
    /// <summary>
    /// A helper class for smoothing out the incoming matrix values based on double 
    /// exponential smoothing (DES) algorithm (The equations can be found at
    /// http://www.itl.nist.gov/div898/handbook/pmc/section4/pmc433.htm). We use
    /// 'b1 = y2 - y1' equation to set the initial value of b1.
    /// </summary>
    public class DESPosition : MonoBehaviour {

        public Transform Source;
        public float Alpha = 1;
        public float Gamma = 1;

        Vector3 st;
        Vector3 bt;

        Vector3 PrevSt;
        Vector3 PrevBt;

        bool initialized;
	
        // Update is called once per frame
        public void Update() {
            transform.position = ComputeDES(Source.position);
        }

        Vector3 ComputeDES(Vector3 p)
        {
            if (!initialized) {
                PrevSt = p;
                PrevBt = Vector3.zero;
                initialized = true;
            }

            // St = alpha * yt + (1 − alpha)(St_1 + bt_1)
            // bt = gamma (St−St_1) + (1−gamma)bt_1

            st = Vector3.Lerp(PrevSt + PrevBt, p, Alpha);
            bt = Vector3.Lerp(PrevBt, st - PrevSt, Gamma);
		
            PrevSt = st;
            PrevBt = bt;

            return st;
        }

        public void Reset() {
            initialized = false;
            st     = Vector3.zero;
            PrevSt = Vector3.zero;
            bt     = Vector3.zero;
            PrevBt = Vector3.zero;
        }
    }
}
