﻿// Copyright (c) 2017, Columbia University 
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

using UnityEditor;
using UnityEngine;

namespace MercuryXM.Support.Geometry.Arrow
{
    [CustomEditor(typeof(RingGenerator))]
    public class RingGeneratorEditor : UnityEditor.Editor
    {
        [MenuItem("GameObject/CGUI/Create/Ring")]
        public static void CreateRing()
        {
            var gameObject = new GameObject("Ring");
        
            var ring = gameObject.AddComponent<RingGenerator>();
        
            ring.RebuildMesh();
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var ring = (RingGenerator)target;

            if (GUILayout.Button("Rebuild"))
            {
                ring.RebuildMesh();
            }

            if (GUILayout.Button("Save"))
            {
                var mf = ring.GetComponentInChildren<MeshFilter>();

                var assetName = string.Format("Assets/Resources/Models/{0}Mesh.asset", ring.AssetName);

                AssetDatabase.CreateAsset(Instantiate(mf.sharedMesh), assetName);
            }
        }
    }
}