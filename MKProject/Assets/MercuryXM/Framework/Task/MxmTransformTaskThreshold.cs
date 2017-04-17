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
using UnityEngine.Networking;

namespace MercuryXM.Framework
{
    /// <summary>
    /// This class contains transformation threshold information for 
    /// MxmTransformationTaskInfos
    /// </summary>
    [Serializable]
    public class MxmTransformTaskThreshold : IMxmSerializable {

        /// <summary>
        /// MxmNode angular (in degrees) completion threshold.
        /// </summary>
        public float AngleThreshold = 5;

        /// <summary>
        /// MxmNode positional completion threshold.
        /// </summary>
        public float DistanceThreshold = float.MaxValue;

        /// <summary>
        /// MxmNode scalar completion threshold.
        /// </summary>
        public float ScaleThreshold = float.MaxValue;

        /// <summary>
        /// Create a default MxmTransformTaskThreshold.
        /// </summary>
        public MxmTransformTaskThreshold()
        {}

        /// <summary>
        /// Construct a MxmTransformTaskThreshold by setting angle, distance and scale directly.
        /// </summary>
        /// <param name="angleThreshold">Angular threshold.</param>
        /// <param name="distanceThreshold">Distance threshold.</param>
        /// <param name="scaleThreshold">Scale threshold.</param>
        public MxmTransformTaskThreshold(float angleThreshold, float distanceThreshold, float scaleThreshold)
        {
            AngleThreshold = angleThreshold;
            DistanceThreshold = distanceThreshold;
            ScaleThreshold = scaleThreshold;
        }

        /// <summary>
        /// Construct MxmTransformationTaskThreshold by duplicating another threshold.
        /// </summary>
        /// <param name="orig">Original threshold</param>
        public MxmTransformTaskThreshold(MxmTransformTaskThreshold orig)
        {
            AngleThreshold = orig.AngleThreshold;
            DistanceThreshold = orig.DistanceThreshold;
            ScaleThreshold = orig.ScaleThreshold;
        }

        /// <summary>
        /// Duplicate the task threshold.
        /// </summary>
        /// <returns>Copy of MxmTransformTaskThreshold</returns>
        public IMxmSerializable Copy()
        {
            return new MxmTransformTaskThreshold(this);
        }

        /// <summary>
        /// Deserialize the task threshold from serialized form.
        /// </summary>
        /// <param name="reader">UNET deserializer.</param>
        public void Deserialize(NetworkReader reader)
        {
            AngleThreshold = reader.ReadSingle();
            DistanceThreshold = reader.ReadSingle();
            ScaleThreshold = reader.ReadSingle();
        }

        /// <summary>
        /// Serialize the task threshold into serialized form.
        /// </summary>
        /// <param name="writer">UNET serializer.</param>
        public void Serialize(NetworkWriter writer)
        {
            writer.Write(AngleThreshold);
            writer.Write(DistanceThreshold);
            writer.Write(ScaleThreshold);
        }
    }
}
