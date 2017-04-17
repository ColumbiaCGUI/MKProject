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

namespace MercuryXM.Support.Geometry
{
    public class Pyramid : MonoBehaviour {

        public string AssetName = "Pyramid";

        public void RebuildMesh()
        {
            var faceFront  = new Face();
            var faceBack   = new Face();
            var faceBottom = new Face();
            var faceLeft   = new Face();
            var faceRight  = new Face();

            faceBottom.AddQuad(
                new VertexInfo(new Vector3(-0.5f, 0f,  0.5f), new Vector2(0, 0)),
                new VertexInfo(new Vector3(-0.5f, 0f, -0.5f), new Vector2(0, 1)),
                new VertexInfo(new Vector3( 0.5f, 0f, -0.5f), new Vector2(1, 1)),
                new VertexInfo(new Vector3( 0.5f, 0f,  0.5f), new Vector2(1, 0)));


            faceLeft.AddTriangle(
                new VertexInfo(new Vector3(-0.5f, 0f,  0.5f), new Vector2(0, 0)),
                new VertexInfo(new Vector3(   0f, 1f,    0f), new Vector2(0.5f, 1)),
                new VertexInfo(new Vector3(-0.5f, 0f, -0.5f), new Vector2(1, 0)));

            faceRight.AddTriangle(
                new VertexInfo(new Vector3( 0.5f, 0f, -0.5f), new Vector2(0, 0)),
                new VertexInfo(new Vector3(   0f, 1f,    0f), new Vector2(0.5f, 1)),
                new VertexInfo(new Vector3( 0.5f, 0f,  0.5f), new Vector2(1, 0)));

            faceFront.AddTriangle(
                new VertexInfo(new Vector3(-0.5f, 0f, -0.5f), new Vector2(0, 0)),
                new VertexInfo(new Vector3(   0f, 1f,    0f), new Vector2(0.5f, 1)),
                new VertexInfo(new Vector3( 0.5f, 0f, -0.5f), new Vector2(1, 0)));

            faceBack.AddTriangle(
                new VertexInfo(new Vector3( 0.5f, 0f,  0.5f), new Vector2(0, 0)),
                new VertexInfo(new Vector3(   0f, 1f,    0f), new Vector2(0.5f, 1)),
                new VertexInfo(new Vector3(-0.5f, 0f,  0.5f), new Vector2(1, 0)));

            var outsideFace = new Face();

            outsideFace.Combine(faceFront );
            outsideFace.Combine(faceBack  );
            outsideFace.Combine(faceBottom);
            outsideFace.Combine(faceLeft  );
            outsideFace.Combine(faceRight );

            var meshFilter = GetComponent<MeshFilter>();

            if (meshFilter == null)
                meshFilter = gameObject.AddComponent<MeshFilter>();

            var mesh = new Mesh();
            meshFilter.sharedMesh = mesh;

            mesh.SetVertices(outsideFace.vertices);
            mesh.SetNormals(outsideFace.normals);
            mesh.SetUVs(0, outsideFace.uv);
            mesh.SetTriangles(outsideFace.indices, 0);

            mesh.RecalculateBounds();

            if (GetComponent<MeshRenderer>() == null) {
                var meshRenderer = gameObject.AddComponent<MeshRenderer>();
                meshRenderer.material = new Material(Shader.Find("Standard"));
            }
        }
    }
}
