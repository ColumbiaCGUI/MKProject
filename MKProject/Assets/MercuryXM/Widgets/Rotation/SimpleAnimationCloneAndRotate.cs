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
using MercuryXM.Support.Input;
using UnityEngine;

namespace MercuryXM.Widgets
{
    public class SimpleAnimationCloneAndRotate : MonoBehaviour
    {
        public KeyCode TriggerKey = KeyCode.Space;
        public Transform Target;
        public int SecondsPauseAtEnd = 1;
        public float DegreesPerSecond = 60f;

        // Use this for initialization
        public void Start () {

            if (name.EndsWith("(Clone)")) return;

            KeyboardHandler.AddEntry(TriggerKey, "Create a clone and animate once to target rotation",
                delegate
                {
                    var clone = Instantiate(gameObject, transform.position, transform.rotation) as GameObject;
                    if (clone == null) return;

                    var anim = clone.GetComponent<SimpleAnimationCloneAndRotate>();
                    anim.Animate(gameObject);
                    gameObject.SetActive(false);
                });
        }

        public void Animate(GameObject original)
        {
            StartCoroutine(AnimationCoroutine(original));
        }

        IEnumerator AnimationCoroutine(GameObject original)
        {
            if (Target == null) yield break;

            var start = transform.rotation;
            var end = Target.transform.rotation;
            var relativeRotation = Quaternion.Inverse(start) * end;

            float angle;
            Vector3 axis;

            relativeRotation.ToAngleAxis(out angle, out axis);

            if (angle > 180.0f)
            {
                angle = 360.0f - angle;
                axis *= -1;
            }

            Debug.Log(string.Format("angle: {0}, axis: {1}",angle,axis));

            var remainingAngle = angle;

            while (remainingAngle > 0)
            {
                var deltaAngle = DegreesPerSecond * Time.deltaTime;

                transform.Rotate(axis, deltaAngle);
            
                remainingAngle -= deltaAngle;

                yield return null;
            }

            yield return new WaitForSeconds(SecondsPauseAtEnd);

            original.SetActive(true);

            Destroy(gameObject);
        }
    }
}
