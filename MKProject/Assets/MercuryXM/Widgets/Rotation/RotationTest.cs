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

using System.Collections;
using MercuryXM.Support.Audio;
using MercuryXM.Support.Input;

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MercuryXM.Widgets
{

    public class RotationTest : BasicTransformationTaskBehavior
    {

        public KeyCode AnimationTrigger = KeyCode.Space;
        public KeyCode EulerTrigger = KeyCode.E;
        public KeyCode RandomControlTrigger = KeyCode.C;
        public KeyCode RandomTargetTrigger = KeyCode.T;
        public KeyCode ResetTrigger = KeyCode.S;

        public float SecondsPauseAtEnd = 1f;
        public float DegreesPerSecond = 180f;
        public float RandomNoiseAngle = 0;

        public Transform ReferenceFrame;

        bool isAnimating;

        Vector3 originalPosition;
        Quaternion originalRotation;

        public override void Start()
        {
            SetupKeyboard();

			base.Start ();
        }

        private void SetupKeyboard()
        {
            KeyboardHandler.AddEntry(AnimationTrigger, "Animate control object to target rotation (once)", Animate);

            KeyboardHandler.AddEntry(EulerTrigger, "Animate Euler sequence", delegate { StartCoroutine(AnimateEuler()); });

            KeyboardHandler.AddEntry(RandomControlTrigger, "Generate random 'From'", GenerateRandomFrom);

            KeyboardHandler.AddEntry(RandomTargetTrigger, "Generate random 'To'", GenerateRandomTo);

            KeyboardHandler.AddEntry(ResetTrigger, "Reset control and target", Reset);

            KeyboardHandler.AddEntry(KeyCode.L, "Toggle Sound On/Off", delegate { SoundEffectManager.IsOff = !SoundEffectManager.IsOff; });
        }

        public void GenerateRandomFrom()
        {
            if (From != null)
                From.rotation = Random.rotationUniform;        
        }

        public void GenerateRandomTo()
        {
            if (To != null)
                To.rotation = Random.rotationUniform;
        }

        public void Animate()
        {
            if (!Application.isPlaying || isAnimating) return;

            StartCoroutine(AnimationCoroutine(axis, angle, SecondsPauseAtEnd));
        }

        IEnumerator AnimateEuler()
        {
            if (!Application.isPlaying || isAnimating) yield break;

            originalPosition = From.position;
            originalRotation = From.rotation;

            //var fromOriginalX = From.right * Mathf.Sign(euler.x);
            //var fromOriginalY = From.up * Mathf.Sign(euler.y);
            //var fromOriginalZ = From.forward * Mathf.Sign(euler.z);

            //Debug.Log("Rotating around Z");
            //yield return StartCoroutine(AnimationCoroutine(fromOriginalZ, euler.z, false, Space.World));
            //Debug.Log("Rotating around X");
            //yield return StartCoroutine(AnimationCoroutine(fromOriginalX, euler.x, false, Space.World));
            //Debug.Log("Rotating around Y");
            //yield return StartCoroutine(AnimationCoroutine(fromOriginalY, euler.y, false, Space.World));

            /************************************************************************/

            //Debug.Log("Rotating around Y");

            var euler = (Quaternion.Inverse(From.rotation) * To.rotation).eulerAngles;
            euler = EulerAngles.WrapAngles(euler);

            //Debug.Log(euler);

            yield return StartCoroutine(AnimationCoroutine(Vector3.up, euler.y, 0.5f, false));

            /************************************************************************/

            //Debug.Log("Rotating around X");

            euler = (Quaternion.Inverse(From.rotation) * To.rotation).eulerAngles;
            euler = EulerAngles.WrapAngles(euler);

            //Debug.Log(euler);

            yield return StartCoroutine(AnimationCoroutine(Vector3.right, euler.x, 0.5f, false));

            /************************************************************************/

            //Debug.Log("Rotating around Z");

            euler = (Quaternion.Inverse(From.rotation) * To.rotation).eulerAngles;
            euler = EulerAngles.WrapAngles(euler);

            //Debug.Log(euler);

            yield return StartCoroutine(AnimationCoroutine(Vector3.forward, euler.z, SecondsPauseAtEnd, false));

            /************************************************************************/

            From.position = originalPosition;
            From.rotation = originalRotation;
        }

        IEnumerator AnimationCoroutine(Vector3 animAxis, float animAngle, float secondsPauseAtEnd, bool restoreToOriginal = true, Space space = Space.Self)
        {
            if (From == null || To == null) yield break;

            if (restoreToOriginal)
            {
                originalPosition = From.position;
                originalRotation = From.rotation;
            }

            isAnimating = true;

            var remainingAngle = animAngle;
            var dir = Mathf.Sign(animAngle);

            while (Mathf.Abs(remainingAngle) > 0)
            {
                var deltaAngle = Mathf.Clamp(DegreesPerSecond * Time.deltaTime, 0, Mathf.Abs(remainingAngle));

                //var axis2 = animAxis;

                //if (RandomNoiseAngle > 0 && (remainingAngle - deltaAngle) > 0)
                //{
                //    axis2 = Quaternion.AngleAxis(RandomNoiseAngle, Random.onUnitSphere) * axis2;
                //}

                From.Rotate(animAxis, deltaAngle * dir, space);
            
                remainingAngle -= deltaAngle * dir;

                yield return null;
            }

            yield return new WaitForSeconds(secondsPauseAtEnd);

            if (restoreToOriginal)
            {
                From.position = originalPosition;
                From.rotation = originalRotation;
            }		
        
            isAnimating = false;        
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(RotationTest))]
    public class VizTestAnimationEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var vizTestAnimation = (RotationTest)target;

            if (GUILayout.Button("Animate"))
            {
                vizTestAnimation.Animate();
            }

            if (GUILayout.Button("Random Control"))
            {
                vizTestAnimation.GenerateRandomFrom();
            }

            if (GUILayout.Button("Random Target"))
            {
                vizTestAnimation.GenerateRandomTo();
            }

            if (GUILayout.Button("Reset"))
            {
                vizTestAnimation.Reset();
            }
        }
    }
#endif
}