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
using UnityEngine.Networking;

namespace MercuryXM.Framework
{
	public class MxmNetworkManager : NetworkBehaviour {

        public static MxmNetworkManager Instance { get; private set; }

        public NetworkClient NetworkClient;
		
		public Dictionary<uint, MxmNode> WFNodes = new Dictionary<uint, MxmNode> ();

		public event IMxmCallback WFAwake;
		public event IMxmCallback WFStart;


		public virtual void Awake()
		{
			MxmLogger.LogBehavior (gameObject.name + ": MxmNetworkManager Awake");

            Instance = this;
            
            if (WFAwake != null)
				WFAwake();
		}

		public virtual void Start()
		{
			MxmLogger.LogBehavior(gameObject.name + ": MxmNetworkManager Started");

			if (NetworkClient == null && NetworkManager.singleton != null)
			{
				NetworkClient = NetworkManager.singleton.client;
			}

            if (NetworkClient != null)
			{
				foreach (var value in Enum.GetValues(typeof(MxmMessageType)).Cast<short>())
				{
					NetworkClient.RegisterHandler(value, ReceivedMessage);
				}
			}

		    if (isServer)
		    {
		        foreach (var value in Enum.GetValues(typeof(MxmMessageType)).Cast<short>())
		        {
		            NetworkServer.RegisterHandler(value, ReceivedMessage);
		        }
		    }

		    if (WFStart != null)
				WFStart();
		}

		public void RegisterWFNetworkBehavior(MxmNetworkBehavior behavior)
		{
			WFNodes.Add (behavior.netId.Value, behavior.MxmNode);
		}

		public virtual void ReceivedMessage(NetworkMessage netMsg)
		{
			MxmMessageType mxmMessageType = (MxmMessageType)netMsg.msgType;

		    try
		    {
		        switch (mxmMessageType)
		        {
		            case MxmMessageType.MxmVoid:
		                Mxmessage msg = netMsg.ReadMessage<Mxmessage>();
		                WFNodes[msg.NetId].MxmInvoke(mxmMessageType, msg);
		                break;
		            case MxmMessageType.MxmInt:
		                MxmessageInt msgInt = netMsg.ReadMessage<MxmessageInt>();
		                WFNodes[msgInt.NetId].MxmInvoke(mxmMessageType, msgInt);
		                break;
		            case MxmMessageType.MxmBool:
		                MxmessageBool msgBool = netMsg.ReadMessage<MxmessageBool>();
		                WFNodes[msgBool.NetId].MxmInvoke(mxmMessageType, msgBool);
		                break;
		            case MxmMessageType.MxmFloat:
		                MxmessageFloat msgFloat = netMsg.ReadMessage<MxmessageFloat>();
		                WFNodes[msgFloat.NetId].MxmInvoke(mxmMessageType, msgFloat);
		                break;
		            case MxmMessageType.MxmVector3:
		                MxmessageVector3 msgVector3 = netMsg.ReadMessage<MxmessageVector3>();
                        WFNodes[msgVector3.NetId].MxmInvoke(mxmMessageType, msgVector3);
		                break;
		            case MxmMessageType.MxmVector4:
		                MxmessageVector4 msgVector4 = netMsg.ReadMessage<MxmessageVector4>();
		                WFNodes[msgVector4.NetId].MxmInvoke(mxmMessageType, msgVector4);
		                break;
		            case MxmMessageType.MxmString:
		                MxmessageString msgString = netMsg.ReadMessage<MxmessageString>();
		                WFNodes[msgString.NetId].MxmInvoke(mxmMessageType, msgString);
		                break;
		            case MxmMessageType.MxmByteArray:
		                MxmessageByteArray msgByteArray = netMsg.ReadMessage<MxmessageByteArray>();
		                WFNodes[msgByteArray.NetId].MxmInvoke(mxmMessageType, msgByteArray);
		                break;
		            case MxmMessageType.MxmTransform:
		                MxmessageTransform msgTransform = netMsg.ReadMessage<MxmessageTransform>();
		                WFNodes[msgTransform.NetId].MxmInvoke(mxmMessageType, msgTransform);
		                break;
		            case MxmMessageType.MxmTransformList:
		                MxmessageTransformList msgTransformList = netMsg.ReadMessage<MxmessageTransformList>();
		                WFNodes[msgTransformList.NetId].MxmInvoke(mxmMessageType, msgTransformList);
		                break;
                    case MxmMessageType.MxmSerializable:
		                MxmessageSerializable msgSerializable = netMsg.ReadMessage<MxmessageSerializable>();
		                WFNodes[msgSerializable.NetId].MxmInvoke(mxmMessageType, msgSerializable);
		                break;
		            default:
		                throw new ArgumentOutOfRangeException();
		        }
		    }
		    catch
		    {
                MxmLogger.LogError("Message NetID not found in WFNodes Dictionary.");
            }
		}
	}
}