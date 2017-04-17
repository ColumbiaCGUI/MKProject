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
    [CustomEditor(typeof(ArrowGenerator))]
    public class ArrowGeneratorEditor : UnityEditor.Editor
    {
        [MenuItem("GameObject/CGUI/Create/Arrow/Curved")]
        public static void CreateCurved()
        {
            var gameObject = new GameObject("Curved Arrow");
            var shape = gameObject.AddComponent<ArrowShapeCurved>();
            var properties = gameObject.AddComponent<ArrowProperties>();

            properties.Width = 1;
            properties.WidthHead = 3f;
            properties.Depth = 0.05f;
            properties.SegmentsTotal = 180;
            properties.SegmentsHead = 10;
        
            shape.Properties = properties;

            Create(gameObject, shape);
        }

        [MenuItem("GameObject/CGUI/Create/Arrow/Straight")]
        public static void CreateStraight()
        {
            var gameObject = new GameObject("Straight Arrow");
            var shape = gameObject.AddComponent<ArrowShapeStraight>();
            var properties = gameObject.AddComponent<ArrowProperties>();

            properties.Width = 0.5f;
            properties.WidthHead = 1.5f;
            properties.Depth = 0.2f;
            properties.SegmentsTotal = 2;
            properties.SegmentsHead = 1;
        
            shape.Properties = properties;
            Create(gameObject, shape);
        }

        public static void Create(GameObject gameObject, IArrowShape shape)
        {
            // Components
            var arrow = gameObject.AddComponent<ArrowGenerator>();
            arrow.ArrowShape = shape;        

            arrow.RebuildMesh();
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var dynamicArrow = (ArrowGenerator)target;

            if (GUILayout.Button("Rebuild"))
            {
                dynamicArrow.RebuildMesh();
            }

            if (GUILayout.Button("Save"))
            {
                var mf = dynamicArrow.GetComponentInChildren<MeshFilter>();
			
                var assetName = string.Format("Assets/Resources/Models/{0}Mesh.asset", dynamicArrow.AssetName);
			
                AssetDatabase.CreateAsset(Instantiate(mf.sharedMesh), assetName);
            }
        }
    }
}