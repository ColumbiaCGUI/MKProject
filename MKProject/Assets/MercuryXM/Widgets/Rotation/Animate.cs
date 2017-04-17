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
using System.Collections.Generic;
using MercuryXM.Framework;
using MercuryXM.Support.Extensions;
using MercuryXM.Support.Interpolators;
using UnityEngine;

namespace MercuryXM.Widgets
{
    public class Animate : BasicTransformationTaskBehavior
    {
        public GameObject AnimatedObject;
        public float Duration = 2; // Will be overridden if Speed > 0
        public float MinDuration = 0.2f;
        public float MaxDuration = 2f;
        public float Speed = 90; // Degrees per second
        public float PauseBetweenLoops = 0.5f;
        public bool Loop = true;

        public bool initialized = false;

        InterpolatorRotation interpolator;
        Coroutine activeCoroutine;

		public override void Awake()
		{
            base.Awake();

            //Debug.Log("Animate Awake");

			/*
			VizBaseCompleteState[CompletionState.Complete] = new StateEvents
			{
				Enter = delegate
				{
					if (activeCoroutine != null) StopCoroutine(activeCoroutine);
				},
				Exit = delegate
				{
					activeCoroutine = StartCoroutine(PauseAndRestart());
				}
			};
			*/
        }

        public override void Initialize()
        {
            base.Initialize();

            AnimatedObject = (AnimatedObject == null)
                ? From.gameObject.Replicate()
                : Instantiate(AnimatedObject);

            AnimatedObject.transform.parent = transform;

            interpolator = AnimatedObject.AddComponent<InterpolatorRotation>();
        }

        public override void Refresh(List<MxmTransform> transformList)
        {
			base.Refresh(transformList);

            if (activeCoroutine != null) StopCoroutine(activeCoroutine);

            Recalculate();

			if (gameObject.activeInHierarchy && 
                angle < mxmTaskInfo.Threshold.AngleThreshold) {
				activeCoroutine = StartCoroutine(PauseAndRestart(transformList));
				return; // Nothing to do
			}

            if (Speed > 0)
            {
                Duration = Mathf.Clamp(angle/Speed, MinDuration, MaxDuration);
            }

            interpolator.ValueOriginal = From.rotation;
            interpolator.ValueTarget = To.rotation;
            interpolator.Duration = Duration;
			interpolator.Forward();

            if (Loop)
				interpolator.OnFinish = delegate { activeCoroutine = StartCoroutine(PauseAndRestart(transformList)); };
            else
                interpolator.OnFinish = null;

            if (AnimatedObject.activeInHierarchy)
                interpolator.Begin();
        }

		IEnumerator PauseAndRestart(List<MxmTransform> transformList)
        {
            yield return new WaitForSeconds(PauseBetweenLoops);
			Refresh(transformList);
        }

        public override void SetActive(bool active)
        {
            base.SetActive(active);

            if (interpolator != null) {
                if (active && AnimatedObject.activeInHierarchy)
                    interpolator.Begin();
                else
                    interpolator.Stop();
            }
        }
    }
}
