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

using System.Linq;
using MercuryXM.Support.FiniteStateMachine;

namespace MercuryXM.Framework
{
    /// <summary>
    /// A MxmNode with an FSM to allow for limited MxmBehavior selection
    /// in the MxmNode's MxmBehaviorList.
    /// </summary>
	public class MxmSwitchNode : MxmNode
    {
        /// <summary>
        /// The Finite state machine of the MxmNode's SubMxmBehaviors.
        /// </summary>
		public FiniteStateMachine<MxmBehaviorListItem> SubBehaviorsFSM;
        
        /// <summary>
        /// FSM Current State
        /// </summary>
        public MxmNode Current
        {
            get { return SubBehaviorsFSM.Current.Behavior.GetNode(); }
        }

        /// <summary>
        /// MxmBehaviorList name of FSM current state.
        /// </summary>
        public string CurrentName
        {
            get { return SubBehaviorsFSM.Current.Name; }
        }

        /// <summary>
        /// Converts the standard MxmBehaviorList into a FSM.
        /// Calls MxmOnAwakeComplete through MxmBehavior Awake.
        /// </summary>
        public override void Awake()
        {
            MxmLogger.LogFramework(gameObject.name + " MxmSwitchNode Awake");

            try
            {
                SubBehaviorsFSM =
                    new FiniteStateMachine<MxmBehaviorListItem>("SubBehaviorsFSM",
                        SubMxmBehaviors.Where(x => x.Behavior is MxmNode && x.Level == MxmLevelFilter.Child).ToList());
            }
            catch
            {
                MxmLogger.LogError(gameObject.name + ": Failed bulding FSM. Missing Node?");
            }

            base.Awake ();
        }

        /// <summary>
        /// Calls MxmOnStartComplete through MxmBehavior Start.
        /// </summary>
		public override void Start()
		{
            MxmLogger.LogFramework(gameObject.name + " MxmSwitchNode Start");

			base.Start ();
		}

        /// <summary>
        /// Overrides MxmNode's Selected check to handle the FSM's current state.
        /// </summary>
        /// <param name="selectedFilter"><see cref="SelectedCheck"/></param>
        /// <param name="behavior">Observed MxmBehavior.</param>
        /// <returns></returns>
	    protected override bool SelectedCheck(MxmSelectedFilter selectedFilter, IMxmBehavior behavior)
	    {
			return selectedFilter == MxmSelectedFilter.All
			       || (SubBehaviorsFSM.Current != null
			           && behavior.MxmGameObject == SubBehaviorsFSM.Current.Behavior.MxmGameObject);
	    }

        /// <summary>
        /// FSM control method: Jump to State, using MxmBehaviorListItem name.
        /// </summary>
        /// <param name="newState">Name of target state.</param>
        public virtual void JumpTo(string newState)
		{
			SubBehaviorsFSM.JumpTo(SubMxmBehaviors[newState]);
		}

        //TODO: Test this again
        /// <summary>
        /// FSM control method: Jump to State, using MxmBehaviorListItem Behavior reference.
        /// </summary>
        /// <param name="newState">Name of target state.</param>
		public virtual void JumpTo(MxmNode newState)
		{
			SubBehaviorsFSM.JumpTo(SubMxmBehaviors[newState]);
		}

        /// <summary>
        /// Accessor for particular state by name.
        /// </summary>
        /// <param name="state">Name of target state.</param>
        /// <returns>FSM State if present.</returns>
	    public StateEvents this[string state]
	    {
			get { return SubBehaviorsFSM[SubMxmBehaviors[state]]; }
			set { SubBehaviorsFSM[SubMxmBehaviors[state]] = value; }
	    }
    }
}