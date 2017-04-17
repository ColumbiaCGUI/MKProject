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

namespace MercuryXM.Framework
{
    /// <summary>
    /// Utility to allow for easy testing of a MxmSwitchNode & MxmSwitchBehavior.
    /// </summary>
    [RequireComponent(typeof(MxmSwitchBehavior))]
    public class MxmTestManager : MonoBehaviour
    {
        /// <summary>
        /// The index of the currently selected MxmNode.
        /// </summary>
		private int currentStateIndex;

        /// <summary>
        /// Handle to control MxmSwitchNode.
        /// </summary>
        public MxmSwitchNode RootSwitchNode;

        /// <summary>
        /// Handle to RootSwitchNode's associated MxmSwitchBehavior.
        /// </summary>
        private MxmSwitchBehavior rootSwitchBehavior;

        /// <summary>
        /// List of MxmNode names in FSM.
        /// </summary>
        protected List<string> nodeNames; 

        /// <summary>
        /// Registers callbacks for OnStart & gets handle to 
        /// MxmSwitchBehavior attached to the same GameObject.
        /// </summary>
        public virtual void Awake () {
			MxmLogger.LogFramework ("MxmTestManager Awake");

            rootSwitchBehavior = GetComponent<MxmSwitchBehavior>();
            rootSwitchBehavior.MxmRegisterStartCompleteCallback(TestSetup);
        }

        /// <summary>
        /// Empty Start - derived classes need to override.
        /// </summary>
        public virtual void Start ()
        {
        }

        /// <summary>
        /// Once components are registered and in place, get the names of MxmNodes in the FSM.
        /// Also, get index of the current state in the FSM.
        /// </summary>
        public virtual void TestSetup()
        {
            nodeNames = RootSwitchNode.SubMxmBehaviors.GetMxmNames(MxmBehaviorList.ListFilter.NodeOnly, 
                MxmLevelFilter.Child);

            currentStateIndex = nodeNames.IndexOf(rootSwitchBehavior.InitialState);
        }

        /// <summary>
        /// Steps through the MxmNodes in FSM based on step size.
        /// </summary>
        /// <param name="step">Step size.</param>
        public void MxmStep(int step)
        {
            currentStateIndex += step;
            currentStateIndex = 
				((currentStateIndex %= nodeNames.Count) >= 0) 
                    ? currentStateIndex : 
				currentStateIndex + nodeNames.Count;

            //Note: May cause issue if the name of the item is not in the 
            //dictionary
            string name = nodeNames[currentStateIndex];
            RootSwitchNode.MxmInvoke (MxmMethod.Switch, name, 
				new MxmControlBlock(MxmLevelFilter.Self, MxmActiveFilter.All));
        }
    }
}
