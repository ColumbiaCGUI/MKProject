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
using UnityEngine;

namespace MercuryXM.Framework
{
    /// <summary>
    /// MxmBehavior that implements a switch handling
    /// the framework-provided MxmMethods.
    /// </summary>
	public class MxmBaseBehavior : MxmBehavior
	{
        /// <summary>
        /// Invoke a MxmMethod. 
        /// Implements a switch that handles the different MxmMethods
        /// defined by default in MxmMethod <see cref="MxmMethod"/>
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
            var type = message.MxmMethod;

            switch (type)
            {
                case MxmMethod.NoOp:
                    break;
                case MxmMethod.SetActive:
                    var messageBool = (MxmessageBool) message;
                    SetActive(messageBool.value);
                    break;
                case MxmMethod.Refresh:
                    var messageTransform = (MxmessageTransformList) message;
                    Refresh(messageTransform.transforms);
                    break;
                case MxmMethod.Initialize:
                    Initialize();
                    break;
                case MxmMethod.Switch:
                    var messageString = (MxmessageString) message;
                    Switch(messageString.value);
                    break;
                case MxmMethod.Complete:
                    var messageCompleteBool = (MxmessageBool)message;
                    Complete(messageCompleteBool.value);
                    break;
                case MxmMethod.TaskInfo:
                    var messageSerializable = (MxmessageSerializable)message;
                    ApplyTaskInfo(messageSerializable.value);
                    break;
				case MxmMethod.Message:
					var messageMessage = (MxmessageString)message;
					ReceivedMessage(messageMessage.value);
					break;
                default:
                    Debug.Log(message.MxmMethod.ToString());
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Handle MxmMethod: SetActive
        /// </summary>
        /// <param name="active">Value of active state.</param>
        public virtual void SetActive(bool active)
        {
            gameObject.SetActive(active);

            MxmLogger.LogBehavior("SetActive(" + active + ") called on " + gameObject.name);
        }

        /// <summary>
        /// Handle MxmMethod: Initialize
        /// Initialize allows you to provide additional initialization logic
        /// in-between calls to Monobehavior provided Awake() and Start() calls.
        /// </summary>
        public virtual void Initialize()
        {
            MxmLogger.LogBehavior("Initialize called on " + gameObject.name);
        }

        /// <summary>
        /// Handle MxmMethod: Refresh
        /// </summary>
        /// <param name="transformList">List of transforms needed in refreshing a MxmBehavior.</param>
        public virtual void Refresh(List<MxmTransform> transformList)
        {
            MxmLogger.LogBehavior("Refresh called on " + gameObject.name);
        }

        /// <summary>
        /// Handle MxmMethod: Switch
        /// </summary>
        /// <param name="iName">Name of value in which to active.</param>
        protected virtual void Switch(string iName)
        {
        }

        /// <summary>
        /// Handle MxmMethod: Switch
        /// </summary>
        /// <param name="active">Can be used to indicate active state 
        /// of object that triggered complete message</param>
        protected virtual void Complete(bool active)
        {
        }

        /// <summary>
        /// Handle MxmMethod: TaskInfo
        /// Given a IMxmSerializable, extract TaskInfo.
        /// </summary>
        /// <param name="serializableValue">Serializable class containing MxmTask Info</param>
	    protected virtual void ApplyTaskInfo(IMxmSerializable serializableValue)
	    {
	    }

        /// <summary>
        /// Handle MxmMethod: Message
        /// </summary>
        /// <param name="message">String message extracted from MxmessageString.</param>
		protected virtual void ReceivedMessage(string message)
		{
		}

        /// <summary>
        /// Implementation of IMxmBehavior's GetNode.
        /// </summary>
        /// <returns>Returns MxmNode if one attached to GameObject, 
        /// Otherwise returns NULL.
        /// </returns>
	    public override MxmNode GetNode()
		{
			return GetComponent<MxmNode>();
		}
    }
}