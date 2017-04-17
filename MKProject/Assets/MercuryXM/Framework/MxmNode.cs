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

using System;
using System.Collections.Generic;
using System.Linq;
using MercuryXM.Support.Extensions;
using UnityEngine;

namespace MercuryXM.Framework
{
    /// <summary>
    /// Base routing node of Mercury XM.
    /// Contains a list of MxmBehaviors and sends Mxmessages
    /// to Behaviors in list, following the specifications of 
    /// control blocks to MxmInvoke method.
    /// </summary>
	public class MxmNode : MxmBehavior, IMxmNode
    {
        #region Member Variables

        /// <summary>
        ///  Queue of MxmBehaviors to add once list is no longer in use
        /// by a MxmInvoke
        /// </summary>
		protected Queue<MxmBehaviorListItem> MxmBehaviorsToAdd =
			new Queue<MxmBehaviorListItem>();

        /// <summary>
        /// Queue of Memessages to route if SerialExecution is enabled 
        /// and messages are received while another message is being executed.
        /// </summary>
        protected Queue<KeyValuePair<MxmMessageType, Mxmessage>> SerialExecutionQueue =
            new Queue<KeyValuePair<MxmMessageType, Mxmessage>>();

		/// <summary>
		/// The WF parent list.
		/// </summary>
		private List<MxmNode> MxmParentList =
			new List<MxmNode>(); 

        /// <summary>
        /// Flag to protect priority list from being modified while it's being iterated over
        /// </summary>
		private bool doNotModifyBehaviorPriorityList;

        /// <summary>
        /// List of MxmBehaviorsListItems.
        /// Each MxmBehavior list item contains:
        ///     MxmBehavior,
        ///     Name,
        ///     Level (Self, Child, Parent),
        ///     Cloneable (Indicates whether the Behavior
        ///         should be cloned when MxmNode is awoken),
        ///     MxmTag (Multi-tag filter supported by Mercury XM).
        /// </summary>
        [Header("MXM Behavior List")]
        [ReorderableList]
		public MxmBehaviorList SubMxmBehaviors;

        /// <summary>
        /// Indicates whether cloned MxmBehaviors should be added as children
        /// to the MxmNode's GameObject.
        /// </summary>
        public bool ReparentClonedSubWFNodesToSelf = true;

        /// <summary>
        /// Associated MxmNetworkBehavior.
        /// If a MxmNetworkBehavior is attached o the same GameObject,
        ///     it will automatically attach to this MxmNode.
        ///     This turns all MxmMethod invocations into networked
        ///         MxmMethod invocations, with no additional effort.
        /// </summary>
        public IMxmNetworkBehavior MxmNetworkBehavior { get; private set; }

        /// <summary>
        /// Experimental: Timestamp of last message.
        /// </summary>
        [SerializeField]
        private string _prevMessageTime;

        /// <summary>
        /// Indicates whether MxmInvoke is currently executing a call.
        /// </summary>
        private bool _executing;

        /// <summary>
        /// Indicates whether the MxmNode is ready for use
        /// This gets set either in Awake or on the first 
        /// MxmInvoke.
        /// </summary>
        public bool Initialized;

        /// <summary>
        /// Indicates whether the message was adjusted.
        /// </summary>
        public bool Dirty;

        /// <summary>
        /// Experimental - Allows forced order on 
        /// MxmInvocations received simultaneously
        /// </summary>
        private bool SerialExecution = false;

        /// <summary>
        /// There may be an issue where a message is received before 
        /// self behaviors have been added to the list.
        /// In order to resolve that issue, we allow the node
        /// to automatically grab all behaviors.
        /// The consequence here is that you cannot have behaviors
        /// on a node that do not automatically get added to the list.
        /// </summary>
        public bool AutoGrabAttachedBehaviors = true;

        #endregion

        #region Properties

        /// <summary>
        /// MxmNode name: returns GameObject name.
        /// </summary>
        public string Name
        {
            get { return gameObject.name; }
        }

        #endregion

        #region Initialization Methods

        /// <summary>
        /// Creates an empty MxmBehavior list on construction.
        /// </summary>
		public MxmNode()
		{
            //This was needed at a certain point to deal with usage
            //  of the lists before Awake occured. However,
            //  this causes an issue with the list
            //  present in the editor.
			SubMxmBehaviors = new MxmBehaviorList ();
		}

        /// <summary>
        /// Grab attached MxmNetworkBehavior, if present.
        /// Detect and refresh parents.
        /// Instantiate any cloneable MxmBehaviors.
        /// </summary>
		public override void Awake()
        {
            MxmNetworkBehavior = GetComponent<IMxmNetworkBehavior>();

            InitializeNode();

            InstantiateSubNodes();

			base.Awake ();

            MxmLogger.LogFramework(gameObject.name + " MxmNode Awake called.");
        }

        /// <summary>
        /// Calls MxmOnStartComplete through MxmBehavior Start.
        /// </summary>
        public override void Start()
        {
			base.Start ();

            MxmLogger.LogFramework(gameObject.name + " MxmNode Start called.");

            //Show all items currently in the SubMxmBehaviors list.
            //Debug.Log(gameObject.name + " MxmNode start called. With " +
            //    SubMxmBehaviors.Count +
            //    " items in the MxmBehavior List: " +
            //    String.Join("\n", SubMxmBehaviors.GetMxmNames(MxmBehaviorList.ListFilter.All,
            //    MxmLevelFilterHelper.SelfAndBidirectional).ToArray()));
        }

        protected void InitializeNode()
        {
            //Todo: Needs to happen via the editor or through a manual invocation in code.
            if (!Initialized)
            {
                RefreshParents();
                Initialized = true;

                if (AutoGrabAttachedBehaviors)
                    MxmRefreshBehaviors();
            }
        }

        /// <summary>
        /// Iterate over SubMxmBehaviors and instantiate any GameObjects that were
        /// marked as needing instantiating by the MxmBehaviorList's creator.
        /// </summary>
        private void InstantiateSubNodes()
        {
            for (int i = 0; i < SubMxmBehaviors.Count; i++)
            {
                if (SubMxmBehaviors[i].Clone)
                {
                    GameObject clonedWFNodeGO = SubMxmBehaviors[i].CloneGameObject();

                    if (ReparentClonedSubWFNodesToSelf)
                    {
                        clonedWFNodeGO.transform.parent = this.transform;
                    }
                }
            }
        }

        #endregion

        #region IMxmBehavior List Management Methods

        /// <summary>
        /// Add a MxmBehavior to the MxmBehaviorList, with level designation.
        /// </summary>
        /// <param name="mxmBehavior">MxmBehavior to be added.</param>
        /// <param name="level">Level designation of behavior.</param>
        /// <returns>Reference to new MxmBehaviorList item.</returns>
        public virtual MxmBehaviorListItem MxmAddToBehaviorList(MxmBehavior mxmBehavior, MxmLevelFilter level)
        {
			var behaviorListItem = new MxmBehaviorListItem (mxmBehavior.name, mxmBehavior) {
				Level = level
			};

            if (SubMxmBehaviors.Contains(mxmBehavior))
                return null; // Already in list

            //If there is a MxmInvoke executing, add it to the
            //  MxmBehaviorsToAdd queue.
            if (doNotModifyBehaviorPriorityList)
            {
                MxmBehaviorsToAdd.Enqueue(behaviorListItem);
            }
            else
            {
				SubMxmBehaviors.Add(behaviorListItem);
            }

            return behaviorListItem;
        }

        /// <summary>
        /// Add a MxmBehavior to the MxmBehaviorList, with level designation.
        /// </summary>
        /// <param name="mxmBehavior">MxmBehavior to be added.</param>
        /// <param name="newName">Name of MxmBehavior as it will appear in list.</param>
        /// <returns>Reference to new MxmBehaviorList item.</returns>
        public virtual MxmBehaviorListItem MxmAddToBehaviorList(MxmBehavior mxmBehavior, string newName)
        {
            var level = (mxmBehavior.gameObject == gameObject)
                ? MxmLevelFilter.Self
                : MxmLevelFilter.Child;

            var behaviorListItem = MxmAddToBehaviorList(mxmBehavior, level);

            if (mxmBehavior is MxmNode)
                (mxmBehavior as MxmNode).AddParent(this);

            return behaviorListItem;
        }

        /// <summary>
        /// Grab all MxmBehaviors attached to the same GameObject.
        /// Does not grab any other MxmNodes attached to the same GameObject.
        /// </summary>
        public void MxmRefreshBehaviors()
        {
            List<MxmBehavior> behaviors = GetComponents<MxmBehavior>().Where(
                x => (!(x is MxmNode))).ToList();

            // Add own implementations of IMxmBehavior to priority list
            foreach (var tempBehavior in behaviors)
            {
				if (!SubMxmBehaviors.Contains(tempBehavior))
                	MxmAddToBehaviorList(tempBehavior, MxmLevelFilter.Self);
            }

            for (int i = SubMxmBehaviors.Count - 1; i >= 0; i--)
            {
                if (SubMxmBehaviors[i].Behavior == null)
                    SubMxmBehaviors.RemoveAt(i);
            }
        }

        /// <summary>
        /// Iterates through SubMxmBehaviors list and assigns 
        /// this MxmNode as a parent to child MxmBehaviors.
        /// </summary>
        public void RefreshParents()
        {
			MxmLogger.LogFramework("Refreshing parents on MxmNode: " + gameObject.name);

            foreach (var child in SubMxmBehaviors.Where(x => x.Level == MxmLevelFilter.Child))
            {
                var childNode = child.Behavior.GetNode();
                childNode.AddParent(this);
                childNode.RefreshParents();
            }

            //Optimize later
            foreach (var parent in MxmParentList)
            {
				//bool foundItem = SubMxmBehaviors.Select ((x) => x.Behavior).Any (behavior => behavior == parent);
				MxmBehaviorListItem foundListItem = SubMxmBehaviors.First (x => x.Behavior == parent);
				if (foundListItem == null) {
					MxmAddToBehaviorList (parent, MxmLevelFilter.Parent);
				}
				else
				{
					foundListItem.Level = MxmLevelFilter.Parent;
				}
            }
		}

        /// <summary>
        /// Given a GameObject, extract a MxmNode if present
        /// and add it to this MxmNode's Behavior list.
        /// </summary>
        /// <param name="go">GameObject that should have a MxmNode attached.</param>
        public virtual void MxmAddNodeToList(GameObject go)
        {
			var mxmNode = go.GetComponent<MxmNode>();

            if (mxmNode != null)
                MxmAddToBehaviorList(mxmNode, go.name);
            else
                MxmLogger.LogError("No MxmNode present on " + go.name);
        }

        /// <summary>
        /// Given a MxmNode, add it as parent to this 
        /// instance's MxmBehaviorList.
        /// </summary>
        /// <param name="parent">MxmNode to add as a parent.</param>
		public virtual void AddParent(MxmNode parent)
		{
			if (!MxmParentList.Contains (parent)) {
				MxmParentList.Add (parent);
				MxmAddToBehaviorList (parent, MxmLevelFilter.Parent);
			}
		}

        #endregion

        #region MxmInvoke methods

        /// <summary>
        /// Invoke a MxmMethod. 
        /// </summary>
        /// <param name="msgType">Type of message. This specifies
        /// the type of the payload. This is important in 
        /// networked scenarios, when proper deseriaization into 
        /// the correct type requires knowing what was 
        /// used to serialize the object originally.
        /// </param>
        /// <param name="message">The message to send.
        /// This class builds on UNET's MessageBase so it is
        /// Auto [de]serialized by UNET.</param>
        public override void MxmInvoke(MxmMessageType msgType, Mxmessage message)
        {
            //If the MxmNode has not been initialized, initialize it here,
            //  and refresh the parents - to ensure proper routing can occur.
            InitializeNode();

            //TODO: Switch to using mutex for threaded applications
            doNotModifyBehaviorPriorityList = true;

            //Experimental: Allow forced serial execution (ordered) of messages.
            if (SerialExecution)
            {
                if (!_executing)
                {
                    _executing = true;
                }
                else
                {
                    MxmLogger.LogFramework("<<<<<>>>>>Queueing<<<<<>>>>>");
                    KeyValuePair<MxmMessageType, Mxmessage> newMessage =
                        new KeyValuePair<MxmMessageType, Mxmessage>(msgType, message);
                    SerialExecutionQueue.Enqueue(newMessage);
                    return;
                }
            }


            //MxmLogger.LogFramework (gameObject.name + ": MxmNode received WFMethod call: " + param.MxmMethod.ToString ());

            MxmNetworkFilter networkFilter = NetworkFilterAdjust(ref message);

            //	If a MxmNetworkBehavior is attached to this object, and the Mxmessage has not already been deserialized
            //	then call the MxmNetworkBehavior's network message invocation function.
            if (MxmNetworkBehavior != null &&
                networkFilter != MxmNetworkFilter.Local &&
                !message.IsDeserialized)
            {
                if (!Dirty)
                {
                    Dirty = true;
                    message.TimeStamp = DateTime.UtcNow.ToShortTimeString();
                    _prevMessageTime = message.TimeStamp;
                }

                MxmNetworkBehavior.MxmInvoke (msgType, message);
			}

            //Todo: it's possible to get this to happen only once per graph. Switch Invoke code to support.
            var upwardMessage = message.Copy();
			upwardMessage.ControlBlock.LevelFilter = MxmLevelFilterHelper.SelfAndParents;
			var downwardMessage = message.Copy();
			downwardMessage.ControlBlock.LevelFilter = MxmLevelFilterHelper.SelfAndChildren;

            MxmLevelFilter levelFilter = message.ControlBlock.LevelFilter;
            MxmActiveFilter activeFilter = ActiveFilterAdjust(ref message);
            MxmSelectedFilter selectedFilter = SelectedFilterAdjust(ref message);

            if (MxmNetworkBehavior == null ||
                (message.ControlBlock.NetworkFilter == MxmNetworkFilter.Local || 
                (message.ControlBlock.NetworkFilter == MxmNetworkFilter.All && message.IsDeserialized)))
            {
				foreach (var mxmBehaviorListItem in SubMxmBehaviors) {
					var behavior = mxmBehaviorListItem.Behavior;

					//bool isLocalBehavior = behavior.MxmGameObject == this.gameObject;
					MxmLevelFilter behaviorLevel = mxmBehaviorListItem.Level;
                    
					//Check individual behavior level and then call the right param.
					Mxmessage behaviorSpecificMessage;
					if((behaviorLevel & MxmLevelFilter.Parent) > 0)
					{
						behaviorSpecificMessage = upwardMessage;
					}
					else if((behaviorLevel & MxmLevelFilter.Child) > 0)
					{
						behaviorSpecificMessage = downwardMessage;
					}
					else
					{
						behaviorSpecificMessage = message;
					}

					//MxmLogger.LogFramework (gameObject.name + "observing " + behavior.MxmGameObject.name);

                    if (BehaviorCheck (levelFilter, activeFilter, selectedFilter, 
                        mxmBehaviorListItem, behaviorSpecificMessage)) {
						behavior.MxmInvoke (msgType, behaviorSpecificMessage);
					}
				}

			    if (Dirty && _prevMessageTime == message.TimeStamp)
			    {
			        Dirty = false;
			    }
			}

            doNotModifyBehaviorPriorityList = false;

            while (MxmBehaviorsToAdd.Any())
            {
                var mxmBehaviorListItem = MxmBehaviorsToAdd.Dequeue();

                MxmAddToBehaviorList(mxmBehaviorListItem.Behavior, mxmBehaviorListItem.Level);

                if (BehaviorCheck(levelFilter, activeFilter, selectedFilter,
                    mxmBehaviorListItem, message))
                {
                    mxmBehaviorListItem.Behavior.MxmInvoke(msgType, message);
                }
            }

            if (SerialExecution)
            {
                if (SerialExecutionQueue.Count != 0)
                {
                    MxmLogger.LogFramework("%%%%%%%%%%%Dequeueing%%%%%%%%%");
                    KeyValuePair<MxmMessageType, Mxmessage> DequeuedMessage = SerialExecutionQueue.Dequeue();
                    MxmInvoke(DequeuedMessage.Key, DequeuedMessage.Value);
                }

                _executing = false;
            }
        }

        #region MxmInvoke utility methods

        /// <summary>
        /// Invoke a MxmMethod with no parameter. 
        /// </summary>
        /// <param name="mxmMethod">MxmMethod Identifier - <see cref="MxmMethod"/></param>
        /// <param name="controlBlock">Object defining the routing of 
        /// Mxmessages through Mxm Graphs. <see cref="MxmControlBlock"/></param>
        public virtual void MxmInvoke(MxmMethod mxmMethod,
			MxmControlBlock controlBlock = null)
        {
			Mxmessage msg = new Mxmessage (mxmMethod, controlBlock);
            MxmInvoke(MxmMessageType.MxmVoid, msg);
        }

        /// <summary>
        /// Invoke a MxmMethod with parameter: bool. 
        /// </summary>
        /// <param name="mxmMethod">MxmMethod Identifier - <see cref="MxmMethod"/></param>
        /// <param name="param">MxmMethod parameter: bool.</param>
        /// <param name="controlBlock">Object defining the routing of 
        /// Mxmessages through Mxm Graphs. <see cref="MxmControlBlock"/></param>
        public virtual void MxmInvoke(MxmMethod mxmMethod, 
            bool param,
            MxmControlBlock controlBlock = null)
        {
			Mxmessage msg = new MxmessageBool (param, mxmMethod, controlBlock);
            MxmInvoke(MxmMessageType.MxmBool, msg);
        }

        /// <summary>
        /// Invoke a MxmMethod with parameter: int. 
        /// </summary>
        /// <param name="mxmMethod">MxmMethod Identifier - <see cref="MxmMethod"/></param>
        /// <param name="param">MxmMethod parameter: int.</param>
        /// <param name="controlBlock">Object defining the routing of 
        /// Mxmessages through Mxm Graphs. <see cref="MxmControlBlock"/></param>
        public virtual void MxmInvoke(MxmMethod mxmMethod, 
            int param,
            MxmControlBlock controlBlock = null)
        {
			Mxmessage msg = new MxmessageInt(param, mxmMethod, controlBlock);
            MxmInvoke(MxmMessageType.MxmInt, msg);
        }

        /// <summary>
        /// Invoke a MxmMethod with parameter: float. 
        /// </summary>
        /// <param name="mxmMethod">MxmMethod Identifier - <see cref="MxmMethod"/></param>
        /// <param name="param">MxmMethod parameter: float.</param>
        /// <param name="controlBlock">Object defining the routing of 
        /// Mxmessages through Mxm Graphs. <see cref="MxmControlBlock"/></param>
        public virtual void MxmInvoke(MxmMethod mxmMethod, 
            float param,
            MxmControlBlock controlBlock = null)
        {
			Mxmessage msg = new MxmessageFloat(param, mxmMethod, controlBlock);
            MxmInvoke(MxmMessageType.MxmFloat, msg);
        }

        /// <summary>
        /// Invoke a MxmMethod with parameter: Vector3.  
        /// </summary>
        /// <param name="mxmMethod">MxmMethod Identifier - <see cref="MxmMethod"/></param>
        /// <param name="param">MxmMethod parameter: Vector3.</param>
        /// <param name="controlBlock">Object defining the routing of 
        /// Mxmessages through Mxm Graphs. <see cref="MxmControlBlock"/></param>
        public virtual void MxmInvoke(MxmMethod mxmMethod, 
			Vector3 param,
            MxmControlBlock controlBlock = null)
        {
			Mxmessage msg = new MxmessageVector3(param, mxmMethod, controlBlock);
            MxmInvoke(MxmMessageType.MxmVector3, msg);
		}

        /// <summary>
        /// Invoke a MxmMethod with parameter: Vector4.  
        /// </summary>
        /// <param name="mxmMethod">MxmMethod Identifier - <see cref="MxmMethod"/></param>
        /// <param name="param">MxmMethod parameter: Vector4.</param>
        /// <param name="controlBlock">Object defining the routing of 
        /// Mxmessages through Mxm Graphs. <see cref="MxmControlBlock"/></param>
        public virtual void MxmInvoke(MxmMethod mxmMethod, 
			Vector4 param,
            MxmControlBlock controlBlock = null)
        {
			Mxmessage msg = new MxmessageVector4(param, mxmMethod, controlBlock);
            MxmInvoke(MxmMessageType.MxmVector4, msg);
		}

        /// <summary>
        /// Invoke a MxmMethod with parameter: string. 
        /// </summary>
        /// <param name="mxmMethod">MxmMethod Identifier - <see cref="MxmMethod"/></param>
        /// <param name="param">MxmMethod parameter: string.</param>
        /// <param name="controlBlock">Object defining the routing of 
        /// Mxmessages through Mxm Graphs. <see cref="MxmControlBlock"/></param>
        public virtual void MxmInvoke(MxmMethod mxmMethod, 
			string param,
            MxmControlBlock controlBlock = null)
        {
			Mxmessage msg = new MxmessageString(param, mxmMethod, controlBlock);
            MxmInvoke(MxmMessageType.MxmString, msg);
		}

        /// <summary>
        /// Invoke a MxmMethod with parameter: byte array.  
        /// </summary>
        /// <param name="mxmMethod">MxmMethod Identifier - <see cref="MxmMethod"/></param>
        /// <param name="param">MxmMethod parameter: byte array.</param>
        /// <param name="controlBlock">Object defining the routing of 
        /// Mxmessages through Mxm Graphs. <see cref="MxmControlBlock"/></param>
        public virtual void MxmInvoke(MxmMethod mxmMethod, 
			byte[] param,
            MxmControlBlock controlBlock = null)
        {
			Mxmessage msg = new MxmessageByteArray(param, mxmMethod, controlBlock);
            MxmInvoke(MxmMessageType.MxmByteArray, msg);
		}

        /// <summary>
        /// Invoke a MxmMethod with parameter: MxmTransform. 
        /// </summary>
        /// <param name="mxmMethod">MxmMethod Identifier - <see cref="MxmMethod"/></param>
        /// <param name="param">MxmMethod parameter: MxmTransform. <see cref="MxmTransform"/></param>
        /// <param name="controlBlock">Object defining the routing of 
        /// Mxmessages through Mxm Graphs. <see cref="MxmControlBlock"/></param>
        public virtual void MxmInvoke(MxmMethod mxmMethod, 
			MxmTransform param,
			MxmControlBlock controlBlock = null)
		{
			Mxmessage msg = new MxmessageTransform(param, mxmMethod, controlBlock);
			MxmInvoke(MxmMessageType.MxmTransform, msg);
		}

        /// <summary>
        /// Invoke a MxmMethod with parameter: List<MxmTransform>. 
        /// </summary>
        /// <param name="mxmMethod">MxmMethod Identifier - <see cref="MxmMethod"/></param>
        /// <param name="param">MxmMethod parameter: List<MxmTransform>.</param>
        /// <param name="controlBlock">Object defining the routing of 
        /// Mxmessages through Mxm Graphs. <see cref="MxmControlBlock"/></param>
        public virtual void MxmInvoke(MxmMethod mxmMethod, 
			List<MxmTransform> param,
			MxmControlBlock controlBlock = null)
		{
			Mxmessage msg = new MxmessageTransformList(param, mxmMethod, controlBlock);
			MxmInvoke(MxmMessageType.MxmTransformList, msg);
		}

        /// <summary>
        /// Invoke a MxmMethod with parameter: IMxmSerializable. 
        /// </summary>
        /// <param name="mxmMethod">MxmMethod Identifier - <see cref="MxmMethod"/></param>
        /// <param name="param">MxmMethod parameter: IMxmSerializable. <see cref="IMxmSerializable"/> </param>
        /// <param name="controlBlock">Object defining the routing of 
        /// Mxmessages through Mxm Graphs. <see cref="MxmControlBlock"/></param>
        public virtual void MxmInvoke(MxmMethod mxmMethod,
            IMxmSerializable param,
            MxmControlBlock controlBlock = null)
        {
            Mxmessage msg = new MxmessageSerializable(param, mxmMethod, controlBlock);
            MxmInvoke(MxmMessageType.MxmSerializable, msg);
        }

        #endregion

        #endregion

        #region Support Methods 

        /// <summary>
        /// If the level filter is designated 'Child', then it is recorded locally, 
        /// but converted to a 'Child+Self' for use by the SubMxmBehaviors 
        /// (which need to pass the message on to all children, but still need to be able
        /// to execute the message on their own Behaviors, otherwise, it just goes
        /// to the terminal points of the graph without ever executing).
        /// </summary>
        /// <param name="message">Mxmessage to be adjusted.</param>
        /// <param name="direction">Intended direction of message</param>
        /// <returns>Base implementation returns messages's level filter.</returns>
        protected virtual MxmLevelFilter LevelFilterAdjust(ref Mxmessage message, 
            MxmLevelFilter direction)
		{
            return message.ControlBlock.LevelFilter;
        }

        /// <summary>
        /// Allows modification of active filter in message
        /// as it gets passed between MxmNodes.
        /// </summary>
        /// <param name="message">Mxmessage to be adjusted.</param>
        /// <returns>Base implementation returns messages's active filter.</returns>
		protected virtual MxmActiveFilter ActiveFilterAdjust(ref Mxmessage message)
		{
			return message.ControlBlock.ActiveFilter;
		}

        /// <summary>
        /// Allows modification of selected filter in message
        /// as it gets passed between MxmNodes.
        /// </summary>
        /// <param name="message">Mxmessage to be adjusted.</param>
        /// <returns>Base implementation returns messages's selected filter.</returns>
		protected virtual MxmSelectedFilter SelectedFilterAdjust(ref Mxmessage message)
		{
			return message.ControlBlock.SelectedFilter;
		}

        /// <summary>
        /// Allows modification of network filter in message
        /// as it gets passed between MxmNodes.
        /// </summary>
        /// <param name="message">Mxmessage to be adjusted.</param>
        /// <returns>Base implementation returns messages's network filter.</returns>
        protected virtual MxmNetworkFilter NetworkFilterAdjust(ref Mxmessage message)
        {
            return message.ControlBlock.NetworkFilter;
        }

        /// <summary>
        /// This method determines if a particular MxmBehavior should 
        /// receive a message via MxmInvoke.
        /// This performs 4 checks: Tag Check, Level Check, Active Check, & Selected Check.
        /// </summary>
        /// <param name="levelFilter">Extracted message level filter - before adjust.</param>
        /// <param name="activeFilter">Extracted message active filter - before adjust.</param>
        /// <param name="selectedFilter">Extracted message selected filter - before adjust.</param>
        /// <param name="mxmBehaviorListItem">BehaviorListItem of currently observed MxmBehavior</param>
        /// <param name="message">Mxmessage to be checked.</param>
        /// <param name="behaviorLevel">Level of currently observed MxmBehavior</param>
        /// <returns>Returns whether behavior has passed all checks.</returns>
		protected virtual bool BehaviorCheck(MxmLevelFilter levelFilter, MxmActiveFilter activeFilter,
			MxmSelectedFilter selectedFilter, MxmBehaviorListItem mxmBehaviorListItem, Mxmessage message)
		{
		    if (!TagCheck(mxmBehaviorListItem, message)) return false; // Failed TagCheck

		    return LevelCheck (levelFilter, mxmBehaviorListItem.Behavior, mxmBehaviorListItem.Level)
				&& ActiveCheck (activeFilter, mxmBehaviorListItem.Behavior)
				&& SelectedCheck(selectedFilter, mxmBehaviorListItem.Behavior);
		}

        /// <summary>
        /// Determine if MxmBehavior passes MxmNode tag filter check using value embedded in Mxmessage.
        /// </summary>
        /// <param name="mxmBehaviorListItem">BehaviorListItem of currently observed MxmBehavior</param>
        /// <param name="message">Mxmessage to be checked.</param>
        /// <returns>Returns whether observed MxmBehavior has passed tag check.</returns>
        protected virtual bool TagCheck(MxmBehaviorListItem mxmBehaviorListItem, Mxmessage message)
        {
            //var text = string.Format("Tag Check (GO: {0}, ListItem: {1}, msgType:{2}, msgTag={3}, behaviorTag={4}",
            //    gameObject.name,
            //    mxmBehaviorListItem.Name,
            //    param.MxmMethod,
            //    MxmTagHelper.ToString(param.ControlBlock.Tag),
            //    MxmTagHelper.ToString(mxmBehaviorListItem.Tags));

            // Behavior's TagCheck toggle is not enabled, this passes
            if (!mxmBehaviorListItem.Behavior.TagCheckEnabled)
            {
                //Debug.Log(text + ") Passed -- TagCheckEnabled: FALSE");
                return true;
            }

            var msgTag = message.ControlBlock.Tag;   // This is "Everything" by default, will by-pass Tag-Check
            var behaviorTag = mxmBehaviorListItem.Tags; // This is "Nothing" by default, if a message *has* a specific tag,
                                                   // i.e. something other than "Everything", it won't pass
                                                   // unless it has that tag's flag set to 1

            // This message applies to everyone, this passes
            if (msgTag == MxmTagHelper.Everything)
            {
                //Debug.Log(text + ") Passed -- msgTag = Everything");
                return true;
            }

            // This message has a tag, other than "Everything", but it matches behavior's tag, so it passes
            if ((msgTag & behaviorTag) > 0)
            {
                //Debug.Log(text + ") Passed -- tag match");
                return true;
            }

            // This message has a tag, other than "Everything", and it doesn't match behavior's tag, so it fails
            //Debug.Log(text + ") FAILED");
            return false;
        }

        /// <summary>
        /// Determine if MxmBehavior passes MxmNode level filter check using value embedded in Mxmessage.
        /// </summary>
        /// <param name="levelFilter">Level filter value extracted from Mxmessage.</param>
        /// <param name="behavior">Observed MxmBehavior.</param>
        /// <param name="behaviorLevel">Observed MxmBehavior Level.</param>
        /// <returns>Returns whether observed MxmBehavior has passed level filter check.</returns>
        protected virtual bool LevelCheck(MxmLevelFilter levelFilter, 
            IMxmBehavior behavior, MxmLevelFilter behaviorLevel)
		{
			return (levelFilter & behaviorLevel) > 0;
		}

        /// <summary>
        /// Determine if MxmBehavior passes MxmNode active filter check using value embedded in Mxmessage.
        /// </summary>
        /// <param name="activeFilter">Active filter value extracted from Mxmessage.</param>
        /// <param name="behavior">Observed MxmBehavior</param>
        /// <returns>Returns whether observed MxmBehavior has passed active filter check.</returns>
		protected virtual bool ActiveCheck(MxmActiveFilter activeFilter, 
            IMxmBehavior behavior)
		{
			return ((activeFilter == MxmActiveFilter.All)
				|| (activeFilter == MxmActiveFilter.Active && behavior.MxmGameObject.activeInHierarchy));
		}

        /// <summary>
        /// Determine if MxmBehavior passes MxmNode selected filter check using value embedded in Mxmessage.
        /// </summary>
        /// <param name="selectedFilter">Selected filter value extracted from Mxmessage.</param>
        /// <param name="behavior">Observed MxmBehavior</param>
        /// <returns>Returns whether observed MxmBehavior has passed selected filter check.
        /// In base MxmNode, this always returns true.</returns>
		protected virtual bool SelectedCheck(MxmSelectedFilter selectedFilter, 
            IMxmBehavior behavior)
		{
			return true;
		}

        /// <summary>
        /// Implementation of IMxmBehavior's GetNode, which returns this.
        /// </summary>
        /// <returns>Returns this MxmNode.</returns>
		public override MxmNode GetNode()
		{
			return this;
		}

		#endregion 

        #region Static Methods

        /// <summary>
        /// Given a MxmBehavior, extract a MxmNode from it's GameObject, if one is present.
        /// </summary>
        /// <param name="iMxmBehavior">Observed MxmBehavior</param>
        /// <returns>A MxmNode if one is present on the same GameObject.</returns>
        public static MxmNode MxmGetNode(IMxmBehavior iMxmBehavior)
        {
            var wfNode = ((MonoBehaviour)iMxmBehavior).GetComponent<MxmNode>();
            if (wfNode != null)
            {
                return wfNode;
            }
            else
            {
                MxmLogger.LogError("Could not get MxmNode");
                return null;
            }
        }

        /// <summary>
        /// Given a GameObject, extract the first MxmNode, if any are present.
        /// </summary>
        /// <param name="go">Observed GameObject.</param>
        /// <returns>First MxmNode on object, if any present.</returns>
        public static MxmNode MxmGetNode(GameObject go)
        {
            var wfNode = go.GetComponent<MxmNode>();
            if (wfNode != null)
            {
                return wfNode;
            }
            return null;
        }
        #endregion
    }
}
