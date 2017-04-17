﻿// Copyright (c) 2017, Columbia University 
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

using UnityEngine.Networking;

namespace MercuryXM.Framework
{
    /// <summary>
    /// Mxmessage with MxmTransform payload
    /// </summary>
	public class MxmessageTransform : Mxmessage
	{
        /// <summary>
        /// MxmTransform payload
        /// </summary>
		public MxmTransform MxmTransform;

        /// <summary>
        /// Creates a basic MxmessageTransform
        /// </summary>
		public MxmessageTransform()
		{}

        /// <summary>
        /// Creates a basic MxmessageTransform, with a control block
        /// </summary>
        /// <param name="controlBlock">Object defining the routing of messages</param>
		public MxmessageTransform(MxmControlBlock controlBlock = null)
			: base (controlBlock)
		{
			MxmTransform = new MxmTransform();
		}

        /// <summary>
        /// Create a Mxmessage, with control block, MxmMethod, and a MxmTransform
        /// </summary>
        /// <param name="transform">MxmTransform payload</param>
        /// <param name="mxmMethod">Identifier of target MxmMethod</param>
        /// <param name="controlBlock">Object defining the routing of messages</param>
		public MxmessageTransform(MxmTransform transform,
			MxmMethod mxmMethod = default(MxmMethod),
			MxmControlBlock controlBlock = null)
			: base(mxmMethod, controlBlock)
		{
			MxmTransform = transform;
		}

        /// <summary>
        /// Duplicate a Mxmessage
        /// </summary>
        /// <param name="message">Item to duplicate</param>
        public MxmessageTransform(MxmessageTransform message) : base(message)
		{}

        /// <summary>
        /// Message copy method
        /// </summary>
        /// <returns>Duplicate of MxmMessage</returns>
        public override Mxmessage Copy()
        {
			MxmessageTransform newMessage = new MxmessageTransform (this);
            newMessage.MxmTransform = MxmTransform;

            return newMessage;
        }

        /// <summary>
        /// Deserialize the message
        /// </summary>
        /// <param name="reader">UNET based deserializer object</param>
        public override void Deserialize(NetworkReader reader)
		{
			base.Deserialize (reader);
			MxmTransform.Deserialize (reader);
		}

        /// <summary>
        /// Serialize the Mxmessage
        /// </summary>
        /// <param name="writer">UNET based serializer</param>
        public override void Serialize(NetworkWriter writer)
		{
			base.Serialize (writer);
			MxmTransform.Serialize (writer);
		}
	}
}