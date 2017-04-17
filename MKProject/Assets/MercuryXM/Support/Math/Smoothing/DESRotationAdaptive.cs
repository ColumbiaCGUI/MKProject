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
    /// Adaptive DES: https://msdn.microsoft.com/en-us/library/jj131429.aspx#ID4E5GAE
    /// When the object is not moving quickly more aggressive filtering is applied by
    /// using smaller alpha and gamma parameters. This adaptation results in smoothed
    /// output when an object is not moving quickly. Alternately, larger alpha and gamma
    /// parameters are used when the object is moving quickly, which results in better
    /// responsiveness to input changes and, hence, a lower latency.
    /// </summary>
    public class DESRotationAdaptive : MonoBehaviour {

        public Transform Source;

        /// <summary>
        /// Alpha Low - Aggressive alpha filter (closer to 0) for when velocity is lower than VelocityLow
        /// </summary>
        public float AlphaLow = 0.2f;

        /// <summary>
        /// Alpha High - Responsive alpha filter (closer to 1) for when velocity is higher than VelocityHigh
        /// </summary>
        public float AlphaHigh = 0.8f;

        /// <summary>
        /// Gamma Low - Aggressive gamma filter (closer to 0) for when velocity is lower than VelocityLow
        /// </summary>
        public float GammaLow = 0.2f;

        /// <summary>
        /// Gamma High - Responsive gamma filter (closer to 1) for when velocity is higher than VelocityHigh
        /// </summary>
        public float GammaHigh = 0.8f;

        /// <summary>
        /// Velocity Low (degrees per second) - Velocity threshold for deciding when object is moving slow.
        /// When velocity is lower than this, we will filter aggresively (i.e. have smoother output)
        /// by using Alpha Low and Gamma Low
        /// </summary>
        public float VelocityLow = 5f;

        /// <summary>
        /// Velocity High (degrees per second) - Velocity threshold for deciding when object is moving fast.
        /// When velocity is higher than this, we will filter less to be more responsive
        /// by using Alpha High and Gamma High
        /// </summary>
        public float VelocityHigh = 10f;

        Quaternion st;
        Quaternion bt;

        Quaternion PrevSt;
        Quaternion PrevBt;
        Quaternion PrevQ;

        bool initialized;
	
        // Update is called once per frame
        public void Update() {
            transform.rotation = ComputeDES(Source.rotation);
        }

        Quaternion ComputeDES(Quaternion q)
        {
            if (!initialized) {
                PrevQ = q;
                PrevSt = q;
                PrevBt = Quaternion.identity;
                initialized = true;
            }

            Vector3 axis;
            var velocity = Math3d.CalcOptimalRotation(PrevQ,q,out axis)/Time.deltaTime;

            var t = (velocity-VelocityLow)/(VelocityHigh-VelocityLow);

            var alpha = Mathf.Lerp(AlphaLow,AlphaHigh,t);
            var gamma = Mathf.Lerp(GammaLow,GammaHigh,t);

            Debug.LogFormat("velocity: {0},t: {1}, alpha:{2}, gamma:{3}",velocity, t, alpha, gamma);

            // St = alpha * yt + (1 − alpha)(St_1 + bt_1)
            // bt = gamma (St−St_1) + (1−gamma)bt_1

            st = Quaternion.Slerp(PrevSt * PrevBt, q, alpha);
            bt = Quaternion.Slerp(PrevBt, Quaternion.Inverse(PrevSt) * st, gamma);
		
            PrevSt = st;
            PrevBt = bt;
            PrevQ = q;

            return st;
        }

        public void Reset() {
            initialized = false;
            st     = Quaternion.identity;
            PrevSt = Quaternion.identity;
            bt     = Quaternion.identity;
            PrevBt = Quaternion.identity;
        }
    }
}
