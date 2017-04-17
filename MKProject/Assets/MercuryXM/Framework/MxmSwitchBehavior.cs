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

namespace MercuryXM.Framework
{
    /// <summary>
    /// Controller for MxmSwitchNode.
    /// </summary>
	public class MxmSwitchBehavior : MxmBaseBehavior {

        /// <summary>
        /// Handle to MxmSwtichNode.
        /// </summary>
		public MxmSwitchNode MxmSwitchNode { get; private set; }

        /// <summary>
        /// Initial state for MxmSwitchNode FSM.
        /// </summary>
		public string InitialState;

        /// <summary>
        /// Auto-grabs the MxmSwitchNode. Also assigns/invokes
        /// Awake callbacks.
        /// </summary>
		public override void Awake()
		{
            MxmLogger.LogBehavior("MxmSwitchBehavior Awake");

			MxmSwitchNode = GetComponent <MxmSwitchNode>();

            //This is to avoid the situation where the MxmSwitchNode is started
            //  before this script is.
			if(MxmSwitchNode.Initialized)
			{
				OnMxmSwitchNodeAwakeCompleteCallback ();
			}
			else
			{
				MxmSwitchNode.MxmRegisterAwakeCompleteCallback(OnMxmSwitchNodeAwakeCompleteCallback);
			}

            MxmSwitchNode.MxmRegisterStartCompleteCallback(OnMxmNodeSwitchStartCompleteCallback);

            if (MxmSwitchNode.MxmNetworkBehavior == null ||
                MxmSwitchNode.MxmNetworkBehavior.IsActiveAndEnabled)
            {
                MxmRegisterStartCompleteCallback(MxmSwitchSetup);
            }
            else
            {
                MxmSwitchNode.MxmNetworkBehavior.MxmRegisterStartCompleteCallback(MxmSwitchSetup);
            }

            base.Awake ();
		}

        /// <summary>
        /// Assign Global enter/exit Delegates for the MxmSwitchNode's FSM.
        /// </summary>
	    public override void Start()
	    {
            MxmLogger.LogBehavior("MxmSwitchBehavior Start");

            // Assign default behavior for transitioning between WFNodes
            MxmSwitchNode.SubBehaviorsFSM.GlobalExit += delegate
            {
                MxmLogger.LogApplication("MxmSwitchNode GlobalExit");

                MxmSwitchNode.Current.MxmInvoke(MxmMethod.SetActive, false,
                    new MxmControlBlock(MxmLevelFilterHelper.Default, MxmActiveFilter.All,
                    default(MxmSelectedFilter), MxmNetworkFilter.Local));
            };

            MxmSwitchNode.SubBehaviorsFSM.GlobalEnter += delegate
            {
                MxmLogger.LogApplication("MxmSwitchNode GlobalEnter");
                
                MxmSwitchNode.Current.MxmInvoke(MxmMethod.SetActive, true,
                    new MxmControlBlock(MxmLevelFilterHelper.Default, MxmActiveFilter.All,
                    default(MxmSelectedFilter), MxmNetworkFilter.Local));
            };

            base.Start();
	    }

        /// <summary>
        /// Callback called after associated MxmSwitchNode awoken.
        /// </summary>
		public virtual void OnMxmSwitchNodeAwakeCompleteCallback()
		{
            MxmLogger.LogBehavior("OnMxmSwitchNodeAwake invoked");
		}


        /// <summary>
        /// Callback called after associated MxmSwitchNode started.
        /// </summary>
		public virtual void OnMxmNodeSwitchStartCompleteCallback()
		{
            MxmLogger.LogBehavior("OnMxmSwitchNodeStart invoked");

			if (MxmSwitchNode.SubMxmBehaviors.Count == 0) {
                MxmLogger.LogBehavior("No MxmNodes in SubBehaviorsFSM on: " + gameObject.name);
			}
		}

        /// <summary>
        /// This is called when the MxmNetworkBehavior is started
        /// Important Note: this will only trigger locally. The reason for this is that
        /// the following code needs to execute on every instance of the MxmNode across the network
        /// Thus, in order to avoid triggering the message each time a client connects, we just trigger the message locally.
        /// </summary>
	    public virtual void MxmSwitchSetup()
	    {
			MxmLogger.LogBehavior(gameObject.name + " WFSwitchSetup Invoked.");

            MxmSwitchNode.MxmInvoke(MxmMethod.Initialize,
                new MxmControlBlock(MxmLevelFilter.Child, MxmActiveFilter.All,
                    default(MxmSelectedFilter), MxmNetworkFilter.Local));

            MxmSwitchNode.MxmInvoke(MxmMethod.SetActive, false,
                new MxmControlBlock(MxmLevelFilter.Child, MxmActiveFilter.All,
                    default(MxmSelectedFilter), MxmNetworkFilter.Local));

            if (InitialState != "")
            {
                MxmLogger.LogBehavior(gameObject.name + " attempting to jump to: " + InitialState);
                MxmSwitchNode.MxmInvoke(MxmMethod.Switch,
                    MxmSwitchNode.SubMxmBehaviors[InitialState].Name,
                    new MxmControlBlock(MxmLevelFilter.Self, MxmActiveFilter.All, 
                    default(MxmSelectedFilter), MxmNetworkFilter.Local));
            }
        }

        /// <summary>
        /// This behavior will receive the switch message triggered by an MxmInvoke.
        /// It will then trigger the jump state in the FSM of the associated 
        /// MxmSwitchNode.
        /// </summary>
        /// <param name="iName">Name of state to jump to.</param>
		protected override void Switch(string iName)
		{
		    if (MxmSwitchNode) // This can be null if gameObject has not awoken
		        MxmSwitchNode.JumpTo(iName);
			//Debug.Log ("Jumping to: " + iName);

			base.Switch (iName);
		}
	}
}