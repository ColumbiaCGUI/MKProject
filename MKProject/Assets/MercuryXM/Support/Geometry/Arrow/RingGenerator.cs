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
    public class RingGenerator : MonoBehaviour {

        GameObject geometry;
        public int SegmentsTotal = 180;
        public float Width = 1;
        public float Depth = 0.1f;
        public float Radius = 1;
        public string AssetName = "Ring";

        public float SegmentSize()
        {
            return 360f/ SegmentsTotal;
        }

        public void RebuildMesh()
        {
            var segment = new GeometrySegment();

            for (var i = 0; i <= SegmentsTotal; i++)
            {
                // Calculate vertex positions
                Vector3 vTopOut, vBotOut, vTopIn, vBotIn, nIn, nOut;

                // First X-Z plane
                CreateGeometryForSegment(i, out vTopOut, out vBotOut, out vTopIn, out vBotIn, out nIn, out nOut);

                vTopOut.y = vTopIn.y = Width/2;
                vBotOut.y = vBotIn.y = -Width/2;

                // Add Verts to faces
                segment.FaceOut.AddVertPair(new VertexInfo(vTopOut, nOut, new Vector2(0.5f, 1)), new VertexInfo(vBotOut, nOut, new Vector2(0.5f, 0))); // Outside Face
                segment.FaceIn.AddVertPair(new VertexInfo(vTopIn, nIn, new Vector2(0.5f, 1)), new VertexInfo(vBotIn, nIn, new Vector2(0.5f, 0))); // Inside Face
            
                segment.FaceTop.AddVertPair(new VertexInfo(vTopIn, new Vector2(0.5f, 0)), new VertexInfo(vTopOut, new Vector2(0.5f, 1))); // Top Face
                segment.FaceBot.AddVertPair(new VertexInfo(vBotIn, new Vector2(0.5f, 0)), new VertexInfo(vBotOut, new Vector2(0.5f, 1))); // Bottom Face

                // Create back face on 1st iteration
                if (i != 0)
                {
                    // Add quads for top, bottom, inside, and outside faces
                    segment.AddQuads();
                }
            }

            UpdateUV2s(segment.FaceIn, new Vector2(0, 1));
            UpdateUV2s(segment.FaceOut, new Vector2(1, 0));
            UpdateUV2s(segment.FaceTop, new Vector2(1, 0));
            UpdateUV2s(segment.FaceBot, new Vector2(1, 0));
            UpdateUV2s(segment.FaceOther, new Vector2(1, 0));

            // Create GameObject
            if (geometry != null) DestroyImmediate(geometry);
            geometry = new GameObject("Geometry");

            // Add as child
            geometry.transform.SetParent(transform, false);

            var meshFilter = geometry.AddComponent<MeshFilter>();
            var mesh = new Mesh();
            meshFilter.sharedMesh = mesh;

            // Combine top, bottom, and other with out
            // (so that they get the same material/color)

            segment.FaceOut.Combine(segment.FaceIn);
            segment.FaceOut.Combine(segment.FaceTop);
            segment.FaceOut.Combine(segment.FaceBot);
            segment.FaceOut.Combine(segment.FaceOther);

            mesh.SetVertices(segment.FaceOut.vertices);
            mesh.SetNormals(segment.FaceOut.normals);
            mesh.SetTriangles(segment.FaceOut.indices, 0);
            mesh.SetUVs(0, segment.FaceOut.uv);
            mesh.SetUVs(1, segment.FaceOut.uv2);

            mesh.RecalculateBounds();

            var meshRenderer = geometry.AddComponent<MeshRenderer>();
            meshRenderer.material = new Material(Shader.Find("Standard"));
        }

        void UpdateUV2s(Face f, Vector2 value)
        {
            for (var i = 0; i < f.uv2.Count; i++)
            {
                f.uv2[i] = value;
            }
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
        public void CreateGeometryForSegment(int currentSegment,
            out Vector3 vTopOut,
            out Vector3 vBotOut,
            out Vector3 vTopIn,
            out Vector3 vBotIn,
            out Vector3 nIn,
            out Vector3 nOut)
        {
            // Calculate x/z positions with sin/cos
		
            var angle = currentSegment * SegmentSize();
		
            var xOut = Mathf.Sin(angle * Mathf.Deg2Rad) * Radius;
            var xIn = Mathf.Sin(angle * Mathf.Deg2Rad) * (Radius - Depth);
		
            var zOut = Mathf.Cos(angle * Mathf.Deg2Rad) * Radius;
            var zIn = Mathf.Cos(angle * Mathf.Deg2Rad) * (Radius - Depth);
		
            // Outside Face
		
            nOut = new Vector3(xOut, 0, zOut);
            nOut.Normalize();
		
            vTopOut = new Vector3(xOut, 0, zOut);
            vBotOut = new Vector3(xOut, 0, zOut);
		
            // Inside Face
		
            vTopIn = new Vector3(xIn, 0, zIn);
            vBotIn = new Vector3(xIn, 0, zIn);
		
            nIn = new Vector3(xIn, 0, zIn)*-1;
            nIn.Normalize();
        }
    }
}
