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
    public class ConnectedCube : MonoBehaviour {

        public bool Front  = true;
        public bool Back   = true;
        public bool Top    = true;
        public bool Bottom = true;
        public bool Left   = true;
        public bool Right  = true;

        public bool InsideFaces  = false;

        public string AssetName = "Connected Cube";

        public void RebuildMesh()
        {
            var faceFront  = new Face();
            var faceBack   = new Face();
            var faceTop    = new Face();
            var faceBottom = new Face();
            var faceLeft   = new Face();
            var faceRight  = new Face();

            faceTop.AddQuad(
                new VertexInfo(new Vector3(-0.5f, 0.5f,  0.5f), new Vector2(0, 1)),
                new VertexInfo(new Vector3( 0.5f, 0.5f,  0.5f), new Vector2(1, 1)),
                new VertexInfo(new Vector3( 0.5f, 0.5f, -0.5f), new Vector2(1, 0)),
                new VertexInfo(new Vector3(-0.5f, 0.5f, -0.5f), new Vector2(0, 0)));

            faceBottom.AddQuad(
                new VertexInfo(new Vector3(-0.5f,-0.5f, -0.5f), new Vector2(0, 1)),
                new VertexInfo(new Vector3( 0.5f,-0.5f, -0.5f), new Vector2(1, 1)),
                new VertexInfo(new Vector3( 0.5f,-0.5f,  0.5f), new Vector2(1, 0)),
                new VertexInfo(new Vector3(-0.5f,-0.5f,  0.5f), new Vector2(0, 0)));

            faceLeft.AddQuad(
                new VertexInfo(new Vector3(-0.5f, 0.5f,  0.5f), new Vector2(0, 1)),
                new VertexInfo(new Vector3(-0.5f, 0.5f, -0.5f), new Vector2(1, 1)),
                new VertexInfo(new Vector3(-0.5f,-0.5f, -0.5f), new Vector2(1, 0)),
                new VertexInfo(new Vector3(-0.5f,-0.5f,  0.5f), new Vector2(0, 0)));

            faceRight.AddQuad(
                new VertexInfo(new Vector3( 0.5f, 0.5f, -0.5f), new Vector2(0, 1)),
                new VertexInfo(new Vector3( 0.5f, 0.5f,  0.5f), new Vector2(1, 1)),
                new VertexInfo(new Vector3( 0.5f,-0.5f,  0.5f), new Vector2(1, 0)),
                new VertexInfo(new Vector3( 0.5f,-0.5f, -0.5f), new Vector2(0, 0)));

            faceFront.AddQuad(
                new VertexInfo(new Vector3(-0.5f, 0.5f, -0.5f), new Vector2(0, 1)),
                new VertexInfo(new Vector3( 0.5f, 0.5f, -0.5f), new Vector2(1, 1)),
                new VertexInfo(new Vector3( 0.5f,-0.5f, -0.5f), new Vector2(1, 0)),
                new VertexInfo(new Vector3(-0.5f,-0.5f, -0.5f), new Vector2(0, 0)));

            faceBack.AddQuad(
                new VertexInfo(new Vector3( 0.5f, 0.5f,  0.5f), new Vector2(0, 1)),
                new VertexInfo(new Vector3(-0.5f, 0.5f,  0.5f), new Vector2(1, 1)),
                new VertexInfo(new Vector3(-0.5f,-0.5f,  0.5f), new Vector2(1, 0)),
                new VertexInfo(new Vector3( 0.5f,-0.5f,  0.5f), new Vector2(0, 0)));

            var outsideFace = new Face();
		
            if (Front ) outsideFace.Combine(faceFront );
            if (Back  ) outsideFace.Combine(faceBack  );
            if (Top   ) outsideFace.Combine(faceTop   );
            if (Bottom) outsideFace.Combine(faceBottom);
            if (Left  ) outsideFace.Combine(faceLeft  );
            if (Right ) outsideFace.Combine(faceRight );

            var insideFace = new Face();

            if (InsideFaces) {
                var faceFrontIn  = new Face();
                var faceBackIn   = new Face();
                var faceTopIn    = new Face();
                var faceBottomIn = new Face();
                var faceLeftIn   = new Face();
                var faceRightIn  = new Face();
			
                faceTopIn.AddQuad(
                    new VertexInfo(new Vector3(-0.5f, 0.5f, -0.5f), new Vector2(0, 1)),
                    new VertexInfo(new Vector3( 0.5f, 0.5f, -0.5f), new Vector2(1, 1)),
                    new VertexInfo(new Vector3( 0.5f, 0.5f,  0.5f), new Vector2(1, 0)),
                    new VertexInfo(new Vector3(-0.5f, 0.5f,  0.5f), new Vector2(0, 0)));
		
                faceBottomIn.AddQuad(
                    new VertexInfo(new Vector3(-0.5f,-0.5f,  0.5f), new Vector2(0, 1)),
                    new VertexInfo(new Vector3( 0.5f,-0.5f,  0.5f), new Vector2(1, 1)),
                    new VertexInfo(new Vector3( 0.5f,-0.5f, -0.5f), new Vector2(1, 0)),
                    new VertexInfo(new Vector3(-0.5f,-0.5f, -0.5f), new Vector2(0, 0)));

                faceLeftIn.AddQuad(
                    new VertexInfo(new Vector3(-0.5f, 0.5f, -0.5f), new Vector2(0, 1)),
                    new VertexInfo(new Vector3(-0.5f, 0.5f,  0.5f), new Vector2(1, 1)),
                    new VertexInfo(new Vector3(-0.5f,-0.5f,  0.5f), new Vector2(1, 0)),
                    new VertexInfo(new Vector3(-0.5f,-0.5f, -0.5f), new Vector2(0, 0)));
		
                faceRightIn.AddQuad(
                    new VertexInfo(new Vector3( 0.5f, 0.5f,  0.5f), new Vector2(0, 1)),
                    new VertexInfo(new Vector3( 0.5f, 0.5f, -0.5f), new Vector2(1, 1)),
                    new VertexInfo(new Vector3( 0.5f,-0.5f, -0.5f), new Vector2(1, 0)),
                    new VertexInfo(new Vector3( 0.5f,-0.5f,  0.5f), new Vector2(0, 0)));
		
                faceFrontIn.AddQuad(
                    new VertexInfo(new Vector3( 0.5f, 0.5f, -0.5f), new Vector2(0, 1)),
                    new VertexInfo(new Vector3(-0.5f, 0.5f, -0.5f), new Vector2(1, 1)),
                    new VertexInfo(new Vector3(-0.5f,-0.5f, -0.5f), new Vector2(1, 0)),
                    new VertexInfo(new Vector3( 0.5f,-0.5f, -0.5f), new Vector2(0, 0)));
		
                faceBackIn.AddQuad(
                    new VertexInfo(new Vector3(-0.5f, 0.5f,  0.5f), new Vector2(0, 1)),
                    new VertexInfo(new Vector3( 0.5f, 0.5f,  0.5f), new Vector2(1, 1)),
                    new VertexInfo(new Vector3( 0.5f,-0.5f,  0.5f), new Vector2(1, 0)),
                    new VertexInfo(new Vector3(-0.5f,-0.5f,  0.5f), new Vector2(0, 0)));
			
                if (Front ) insideFace.Combine(faceFrontIn );
                if (Back  ) insideFace.Combine(faceBackIn  );
                if (Top   ) insideFace.Combine(faceTopIn   );
                if (Bottom) insideFace.Combine(faceBottomIn);
                if (Left  ) insideFace.Combine(faceLeftIn  );
                if (Right ) insideFace.Combine(faceRightIn );

                outsideFace.Combine(insideFace,false);
            }

            var meshFilter = GetComponent<MeshFilter>();

            if (meshFilter == null)
                meshFilter = gameObject.AddComponent<MeshFilter>();

            var mesh = new Mesh();
            meshFilter.sharedMesh = mesh;

            mesh.SetVertices(outsideFace.vertices);
            mesh.SetNormals(outsideFace.normals);
            mesh.SetUVs(0, outsideFace.uv);

            if (InsideFaces) {
                mesh.subMeshCount = 2;
                mesh.SetTriangles(outsideFace.indices, 0);
                mesh.SetTriangles(insideFace.indices, 1);
            } else {
                mesh.SetTriangles(outsideFace.indices, 0);
            }
		
            mesh.RecalculateBounds();

            if (GetComponent<MeshRenderer>() == null) {
                var meshRenderer = gameObject.AddComponent<MeshRenderer>();
                meshRenderer.material = new Material(Shader.Find("Standard"));
            }
        }
    }
}
