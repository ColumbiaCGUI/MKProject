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
namespace MercuryXM.Support.Geometry
{
    public class GeometrySegment
    {
        public Face FaceOut   = new Face();
        public Face FaceIn    = new Face();
        public Face FaceTop   = new Face();
        public Face FaceBot   = new Face();
        public Face FaceOther = new Face();    

        /// <summary>
        /// Add quads for top, bottom, inside, and outside faces
        /// </summary>
        public void AddQuads()
        {
            // Outside

            // 3 - 1
            // 2 - 0

            FaceOut.AddQuad(0, -1, -3, -2);

            // Inside

            // 1 - 3
            // 0 - 2

            FaceIn.AddQuad(0, -2, -3, -1);

            // Top

            // 1 - 0
            // 3 - 2

            var n = FaceTop.CalcNormal(-3, -2, 0);
            FaceTop.AddQuad(-3, -2, 0, -1, n);

            // Bottom

            // 3 - 2
            // 1 - 0

            n = FaceBot.CalcNormal(0, -2, -3);
            FaceBot.AddQuad(-1, 0, -2, -3, n);
        
        }
    }
}