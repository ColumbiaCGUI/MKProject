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

using System.Collections.Generic;
using UnityEngine;

namespace MercuryXM.Support.Geometry
{
    public class Face {

        internal List<Vector3> vertices;
        internal List<Vector3> normals;
        internal List<Vector2> uv;
        internal List<Vector2> uv2;
        internal List<int> indices;
        internal int offset;

        internal Vector3 LastVert() {
            return vertices[vertices.Count - 1];
        }

        internal Face () {
            vertices = new List<Vector3>();
            normals = new List<Vector3>();
            uv = new List<Vector2>();
            uv2 = new List<Vector2>();
            indices = new List<int>();
        }

        internal void Reset() {
            vertices.Clear();
            normals.Clear();
            uv.Clear();
            uv2.Clear();
            indices.Clear();

            offset = 0;
        }

        internal void AddVert(VertexInfo v)
        {
            vertices.Add(v.Position);
            normals.Add(v.Normal);
            uv.Add(v.Uv);
            uv2.Add(v.Uv2);
        }

        internal void AddVertPair(VertexInfo v1, VertexInfo v2)
        {
            AddVert(v1);
            AddVert(v2);        
        }

        #region Triangle

        internal void AddTriangle(VertexInfo v1, VertexInfo v2, VertexInfo v3) {

            AddVert(v1);
            AddVert(v2);
            AddVert(v3);

            AddTriangleCalcNormal(-2, -1, 0);
        }

        internal void AddTriangleCalcNormal(int offsetFromLast1, int offsetFromLast2, int offsetFromLast3) {

            AddTriangle(offsetFromLast1, offsetFromLast2, offsetFromLast3);

            var lastIndex = vertices.Count - 1;

            var n = CalcNormal(offsetFromLast1, offsetFromLast2, offsetFromLast3);

            normals[lastIndex + offsetFromLast1] = n;
            normals[lastIndex + offsetFromLast2] = n;
            normals[lastIndex + offsetFromLast3] = n;
        }

        internal void AddTriangle(int offsetFromLast1, int offsetFromLast2, int offsetFromLast3) {
            var lastIndex = vertices.Count - 1;

            indices.Add(lastIndex + offsetFromLast1);
            indices.Add(lastIndex + offsetFromLast2);
            indices.Add(lastIndex + offsetFromLast3);
        }

        #endregion

        #region Quads

        internal void AddQuad(VertexInfo v1, VertexInfo v2, VertexInfo v3, VertexInfo v4) {

            AddVert(v1);
            AddVert(v2);
            AddVert(v3);
            AddVert(v4);

            AddQuadCalcNormal(-3, -2, -1, 0);
        }

        internal void AddQuad(int offsetFromLast1, int offsetFromLast2, int offsetFromLast3, int offsetFromLast4) {
            var lastIndex = vertices.Count - 1;

            indices.Add(lastIndex + offsetFromLast1);
            indices.Add(lastIndex + offsetFromLast2);
            indices.Add(lastIndex + offsetFromLast3);

            indices.Add(lastIndex + offsetFromLast1);
            indices.Add(lastIndex + offsetFromLast3);
            indices.Add(lastIndex + offsetFromLast4);
        }

        internal void AddQuadCalcNormal(int offsetFromLast1, int offsetFromLast2, int offsetFromLast3, int offsetFromLast4) {

            AddQuad(offsetFromLast1, offsetFromLast2, offsetFromLast3, offsetFromLast4);

            var lastIndex = vertices.Count - 1;

            var n = CalcNormal(offsetFromLast1, offsetFromLast2, offsetFromLast3);

            normals[lastIndex + offsetFromLast1] = n;
            normals[lastIndex + offsetFromLast2] = n;
            normals[lastIndex + offsetFromLast3] = n;
            normals[lastIndex + offsetFromLast4] = n;
        }

        #endregion

        internal Vector3 CalcNormal(int offsetFromLast1, int offsetFromLast2, int offsetFromLast3)
        {
            var lastIndex = vertices.Count - 1;

            var v1 = vertices[lastIndex + offsetFromLast1];
            var v2 = vertices[lastIndex + offsetFromLast2];
            var v3 = vertices[lastIndex + offsetFromLast3];

            return CalcNormal(v1, v2, v3);        
        }

        internal void AddQuad(int offsetFromLast1, int offsetFromLast2, int offsetFromLast3, int offsetFromLast4, Vector3 normal)
        {
            AddQuad(offsetFromLast1, offsetFromLast2, offsetFromLast3, offsetFromLast4);

            var lastIndex = vertices.Count - 1;

            normals[lastIndex + offsetFromLast1] = normal;
            normals[lastIndex + offsetFromLast2] = normal;
            normals[lastIndex + offsetFromLast3] = normal;
            normals[lastIndex + offsetFromLast4] = normal;
        }

        internal void Combine(Face face) {
            Combine(face, true);
        }

        internal void Combine(Face face, bool addIndices) {

            var n = vertices.Count;

            vertices.AddRange(face.vertices);
            normals.AddRange(face.normals);
            uv.AddRange(face.uv);
            uv2.AddRange(face.uv2);

            face.Offset(n);

            if (addIndices) indices.AddRange(face.indices);
        }

        internal void Offset(int n) {
            offset = n;

            for (int i = 0; i < indices.Count; i++) {
                indices[i] = indices[i] + n;
            }

        }

        static Vector3 CalcNormal(Vector3 v1, Vector3 v2, Vector3 v3) {
            var e1 = v2 - v1;
            var e2 = v3 - v1;

            e1.Normalize();
            e2.Normalize();

            var n = Vector3.Cross(e1,e2);
            n.Normalize();

            return n;
        }
    }
}