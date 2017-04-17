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

using UnityEngine;
using UnityEngine.Networking;

namespace MercuryXM.Framework
{
	public class MxmNetworkBehavior : NetworkBehaviour, IMxmNetworkBehavior
	{
        /// <summary>
        /// Get a handle to a MxmNode that shares the same GameObject.
        /// </summary>
	    public MxmNode MxmNode { get; private set; }

        #region Implementation of IMxmNetworkBehavior

        /// <summary>
        /// Event triggered in the MxmNetworkBehavior when 
        /// this object is awoken.
        /// </summary>
        event IMxmCallback MxmAwakeCompleteCallback;

        /// <summary>
        /// Event triggered in the MxmNetworkBehavior when 
        /// this object is started.
        /// </summary>
        event IMxmCallback MxmStartCompleteCallback;

        /// <summary>
        /// Event triggered in the MxmNetworkBehavior when 
        /// this object is awoken.
        /// Todo: Needs to be swapped for MxmAwakeCompleteCallback
        /// </summary>
        public event IMxmCallback MxmAwake;

        /// <summary>
        /// Event triggered in the MxmNetworkBehavior when 
        /// this object is started.
        /// Todo: Needs to be swapped for MxmStartCompleteCallback
        /// </summary>
        public event IMxmCallback MxmStart;

        /// <summary>
        /// Post Awake callback used between objects implementing this interface.
        /// This allows for initialization steps that must occur before one
        /// instance's Awake and its Start.
        /// </summary>
        public virtual void MxmOnAwakeComplete()
        {
            if (MxmAwakeCompleteCallback != null)
                MxmAwakeCompleteCallback();
        }

        /// <summary>
        /// Post Start callback used between objects implementing this interface
        /// This allows for initialization steps that must occur before one
        /// instance's Start and its first Update.
        /// </summary>
        public virtual void MxmOnStartComplete()
        {
            if (MxmStartCompleteCallback != null)
                MxmStartCompleteCallback();
        }

        /// <summary>
        /// It is possible that certain handles are not going to be in-place
        /// when registration of the OnAwakeComplete callback is invoked.
        /// The is especially true in scenarios where MxmNodes are networked.
        /// This allows for a deferred registration, eliminating most 
        /// instances where the Awake callback invocations fail.
        /// </summary>
        /// <param name="callback">Callback to be registered.</param>
        public void MxmRegisterAwakeCompleteCallback(IMxmCallback callback)
        {
            MxmAwakeCompleteCallback += callback;
        }

        /// <summary>
        /// It is possible that certain handles are not going to be in-place
        /// when registration of the OnStartComplete callback is invoked.
        /// The is especially true in scenarios where MxmNodes are networked.
        /// This allows for a deferred registration, eliminating most 
        /// instances where the Start callback invocations fail.
        /// </summary>
        /// <param name="callback">Callback to be registered.</param>
        public void MxmRegisterStartCompleteCallback(IMxmCallback callback)
        {
            MxmStartCompleteCallback += callback;
        }

        /// <summary>
        /// Allows us to stop messages on client from sending.
        /// </summary>
        [SerializeField]
	    private bool allowClientToSend;

        /// <summary>
        /// Allows us to stop messages on client from sending.
        /// </summary>
        public bool AllowClientToSend { get {return allowClientToSend;} }

        /// <summary>
        /// Set when Network Obj is active & enabled.
        /// This is important since Objects in UNET scenarios
        /// UNET may not be awake/active at the same times.
        /// </summary>
        public bool IsActiveAndEnabled { get { return isActiveAndEnabled; } }

        /// <summary>
        /// This is the network equivalent of IMxmBehavior's MxmInvoke.
        /// The difference is this class allows specification of connectionIDs
        /// which can be used to ensure messages are routed to the correct
        /// objects on network clients.
        /// </summary>
        /// <param name="msgType">Type of message. This specifies
        /// the type of the payload. This is important in 
        /// networked scenarios, when proper deseriaization into 
        /// the correct type requires knowing what was 
        /// used to serialize the object originally.
        /// </param>
        /// <param name="msg">The message to send.
        /// This class builds on UNET's MessageBase so it is
        /// Auto [de]serialized by UNET.</param>
        /// <param name="connectionId">Connection ID - use to identify clients.</param>
        public virtual void MxmInvoke(MxmMessageType msgType, Mxmessage message, int connectionId = -1)
        {
            message.NetId = netId.Value;

            //If the connection ID is defined, only send it there,
            //  otherwise, it follows the standard execution flow for the chosen 
            //  network solution.
            if (connectionId != -1)
            {
                MxmSendMessageToClient(connectionId, (short) msgType, message);
                return;
            }

            //Need to call the right method based on whether this object 
            //  is a client or a server.
            if (isServer)
                MxmSendMessageToClient((short)msgType, message);
            else if (allowClientToSend)
                MxmSendMessageToServer((short)msgType, message);
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Awake gets the MxmNode, if one is present.
        /// Also calls the post-awake callback.
        /// </summary>
        public virtual void Awake()
		{
            MxmLogger.LogFramework(gameObject.name + ": Network Behavior Awake");

            MxmNode = GetComponent<MxmNode>();

            if (MxmAwake != null)
                MxmAwake();
        }

        /// <summary>
        /// Attempts to register the this behavior with the 
        /// MxmNetworkManager.
        /// Also calls the post-start callback.
        /// </summary>
        public virtual void Start()
        {
            MxmLogger.LogFramework(gameObject.name + ": Network Behavior Started");

			MxmNetworkManager.Instance.RegisterWFNetworkBehavior (this);
			
            if (MxmStart != null)
                MxmStart();
        }

        #endregion

        /// <summary>
        /// Method serializes message and sends it to server.
        /// </summary>
        /// <param name="msgType">Type of message. This specifies
        /// the type of the payload. This is important in 
        /// networked scenarios, when proper deseriaization into 
        /// the correct type requires knowing what was 
        /// used to serialize the object originally.
        /// </param>
        /// <param name="msg">The message to send.
        /// This utilises UNET's MessageBase so it is
        /// Auto [de]serialized by UNET.
        /// This also allows us to send messages that are not
        /// part of Mercury XM</param>
        public virtual void MxmSendMessageToServer(short msgType, MessageBase msg)
        {
            if (MxmNetworkManager.Instance.NetworkClient == null)
            {
                MxmLogger.LogFramework("No client present on host");
                return;
            }

            NetworkWriter writer = new NetworkWriter();
            writer.StartMessage(msgType);
            msg.Serialize(writer);
            writer.FinishMessage();

            MxmNetworkManager.Instance.NetworkClient.SendWriter(writer, Channels.DefaultReliable);
        }

        /// <summary>
        /// Send a message to a specific client over chosen UNET.
        /// </summary>
        /// <param name="channelId">Client connection ID</param>
        /// <param name="msgType">Type of message. This specifies
        /// the type of the payload. This is important in 
        /// networked scenarios, when proper deseriaization into 
        /// the correct type requires knowing what was 
        /// used to serialize the object originally.
        /// </param>
        /// <param name="msg">The message to send.
        /// This utilises UNET's MessageBase so it is
        /// Auto [de]serialized by UNET.
        /// This also allows us to send messages that are not
        /// part of Mercury XM</param>
        public virtual void MxmSendMessageToClient(int channelId, short msgType, Mxmessage msg)
        {
            NetworkServer.SendToClient(channelId, msgType, msg);
        }

        /// <summary>
        /// Send a message to all clients using UNET
        /// </summary>
        /// <param name="msgType">Type of message. This specifies
        /// the type of the payload. This is important in 
        /// networked scenarios, when proper deseriaization into 
        /// the correct type requires knowing what was 
        /// used to serialize the object originally.
        /// </param>
        /// <param name="msg">The message to send.
        /// This utilises UNET's MessageBase so it is
        /// Auto [de]serialized by UNET.
        /// This also allows us to send messages that are not
        /// part of Mercury XM</param>
        public virtual void MxmSendMessageToClient(short msgType, Mxmessage msg)
        {
            foreach (var connection in NetworkServer.connections)
                if (connection != null)
                    NetworkServer.SendToClient(connection.connectionId, msgType, msg);
        }
    }
}
