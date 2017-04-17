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
    public class TriangleArrowGenerator : MonoBehaviour
    {
        public IArrowShape ArrowShape;
        public string AssetName = "TriangleArrow";

        public void Awake()
        {
            ArrowShape = GetComponent<IArrowShape>();
        }

        GameObject geometry;

        /// <summary>
        /// Creates the geometry of the arrow.  This is called once during initialization.
        /// Dynamic resizing happens by showing/hiding existing segments.
        /// </summary>
        public void RebuildMesh()
        {
            Awake();

            var segment = new GeometrySegment();

            for (var i = 0; i <= ArrowShape.Properties.SegmentsTotal; i++)
            {
                // Calculate vertex positions
                Vector2 v2TopOut, v2BotOut, v2TopIn, v2BotIn, v2NormIn, v2NormOut;

                // First X-Z plane
                ArrowShape.CreateXZGeometry(i, out v2TopOut, out v2BotOut, out v2TopIn, out v2BotIn, out v2NormIn,
                    out v2NormOut);

                var isFirstSegmentInTriangle = ((i % ArrowShape.Properties.SegmentsHead) == 0);

                var vTopOut = ArrowShape.AssignWidth(v2TopOut, 0);
                var vTopIn  = ArrowShape.AssignWidth(v2TopIn , 0);
                var vBotOut = ArrowShape.AssignWidth(v2BotOut, 0);
                var vBotIn  = ArrowShape.AssignWidth(v2BotIn , 0);

                var nOut = ArrowShape.AssignWidth(v2NormOut, 0);
                var nIn  = ArrowShape.AssignWidth(v2NormIn , 0);

                if (i > 0 && isFirstSegmentInTriangle)
                {               
                    segment.FaceOut.AddVertPair(new VertexInfo(vTopOut, nOut, new Vector2(0.5f, 1)), new VertexInfo(vBotOut, nOut, new Vector2(0.5f, 0))); // Outside Face
                    segment.FaceIn.AddVertPair(new VertexInfo(vTopIn, nIn, new Vector2(0.5f, 1)), new VertexInfo(vBotIn, nIn, new Vector2(0.5f, 0))); // Inside Face

                    segment.FaceTop.AddVertPair(new VertexInfo(vTopIn, new Vector2(0.5f, 0)), new VertexInfo(vTopOut, new Vector2(0.5f, 1))); // Top Face
                    segment.FaceBot.AddVertPair(new VertexInfo(vBotIn, new Vector2(0.5f, 0)), new VertexInfo(vBotOut, new Vector2(0.5f, 1))); // Bottom Face

                    // Add quads for top, bottom, inside, and outside faces
                    segment.AddQuads();                
                }

                var y = CalculateY(i);

                vTopOut = ArrowShape.AssignWidth(v2TopOut,  y);
                vTopIn  = ArrowShape.AssignWidth(v2TopIn ,  y);
                vBotOut = ArrowShape.AssignWidth(v2BotOut, -y);
                vBotIn  = ArrowShape.AssignWidth(v2BotIn , -y);

                // Add Verts to faces
                segment.FaceOut.AddVertPair(new VertexInfo(vTopOut, nOut, new Vector2(0.5f, 1)), new VertexInfo(vBotOut, nOut, new Vector2(0.5f, 0))); // Outside Face
                segment.FaceIn.AddVertPair(new VertexInfo(vTopIn, nIn, new Vector2(0.5f, 1)), new VertexInfo(vBotIn, nIn, new Vector2(0.5f, 0))); // Inside Face

                segment.FaceTop.AddVertPair(new VertexInfo(vTopIn, new Vector2(0.5f, 0)), new VertexInfo(vTopOut, new Vector2(0.5f, 1))); // Top Face
                segment.FaceBot.AddVertPair(new VertexInfo(vBotIn, new Vector2(0.5f, 0)), new VertexInfo(vBotOut, new Vector2(0.5f, 1))); // Bottom Face

                // Create back face on 1st iteration
                if (isFirstSegmentInTriangle)
                {
                    if (i < ArrowShape.Properties.SegmentsTotal)
                    {
                        segment.FaceOther.AddQuad(
                            new VertexInfo(vTopOut, new Vector2(0, 1)),
                            new VertexInfo(vTopIn, new Vector2(1, 1)),
                            new VertexInfo(vBotIn, new Vector2(1, 0)),
                            new VertexInfo(vBotOut, new Vector2(0, 0))); // Back Face Top                
                    }
                }
                else
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
            meshRenderer.material = new Material(Shader.Find(ArrowShape.Shader()));
        }

        void UpdateUV2s(Face f, Vector2 value)
        {
            for (var i = 0; i < f.uv2.Count; i++)
            {
                f.uv2[i] = value;
            }
        }

        /// <summary>
        /// Calculates y coordinate for vertices (=width) paying attention to when the arrowhead starts
        /// </summary>
        /// <param name="currentSegment">A segment index along the arrow (0 to SegmentsTotal)</param>
        /// <returns>Y Coordinate of current segment's vertices</returns>
        public float CalculateY(int currentSegment)
        {
            float y;

            var i = currentSegment % ArrowShape.Properties.SegmentsHead;

            var segmentCountRemain = ArrowShape.Properties.SegmentsHead - i;

            // Arrow tip is a triangle
            var currSlope = (float)segmentCountRemain / ArrowShape.Properties.SegmentsHead;

            y = (ArrowShape.Properties.WidthHead / 2f) * currSlope;
        
            return y;
        }
    }
}
