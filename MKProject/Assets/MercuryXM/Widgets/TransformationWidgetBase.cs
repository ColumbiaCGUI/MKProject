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
using MercuryXM.Framework;
using MercuryXM.Support.Math;
using UnityEngine;

namespace MercuryXM.Widgets
{
	public class TransformationWidgetBase<U> : MxmTransformationTaskBehavior<U>, IWFTransformationWidget 
        where U : MxmTransformationTaskInfo, new()
    {
		public Transform From;
		public Transform To;

		//Todo: Are these used?
		public virtual Transform GetFrom() { return From; }
		public virtual Transform GetTo() { return To; }

        public float Angle() { return angle; }
        public Vector3 Axis() { return axis; }
        public Vector3 AxisGlobal() { return From.TransformDirection(axis); }


        public override void Awake()
		{
			//Debug.Log ("Awake called on: " + gameObject.name);

			if(mxmTaskInfo.Threshold == null)
			    mxmTaskInfo.Threshold = new MxmTransformTaskThreshold();

            base.Awake ();
		}

		public override void Update()
		{
			Recalculate();

		    base.Update();
		}

        protected virtual void Recalculate()
        {
            if (From == null || To == null) return;

            //TODO: Incorporate UseGlobalTransform
            angle = Math3d.CalcOptimalRotation(From.rotation, To.rotation, out axis);
            rotationFromTo = Quaternion.AngleAxis(angle, axis);

            distance = (To.position - From.position).magnitude;

            scaleOffset = (To.localScale - From.localScale).magnitude;
        }

        public override void Refresh(List<MxmTransform> transforms)
		{
//			float angle;
//			Vector3 axis;
//			transforms[1].rotation.ToAngleAxis (out angle, out axis);
//			Debug.Log("GameObject: " + gameObject.name + ", Base values: (Angle) " + 
//				angle + 
//				"; (Axis) " + axis);

			//TODO: Fix this (i.e. make it generic)
			//From.rotation = transforms [0].rotation;
			//To.rotation = transforms [1].rotation;

			base.Refresh(transforms);
		}
    }
}
