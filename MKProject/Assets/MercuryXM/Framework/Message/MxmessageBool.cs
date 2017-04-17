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

using UnityEngine.Networking;

namespace MercuryXM.Framework
{
    /// <summary>
    /// Mxmessage with bool payload
    /// </summary>
    public class MxmessageBool : Mxmessage
    {
        /// <summary>
        /// Boolean payload
        /// </summary>
        public bool value;

        /// <summary>
        /// Creates a basic MxmessageBool
        /// </summary>
		public MxmessageBool()
		{}

        /// <summary>
        /// Creates a basic Mxmessage, with control block
        /// </summary>
        /// <param name="controlBlock">Object defining the routing of messages</param>
		public MxmessageBool(MxmControlBlock controlBlock = null)
			: base (controlBlock)
		{}

        /// <summary>
        /// Create a Mxmessage, with defined control block, MxmMethod, and boolean value
        /// </summary>
        /// <param name="iVal">Boolean value</param>
        /// <param name="mxmMethod">Identifier of target MxmMethod</param>
        /// <param name="controlBlock">Object defining the routing of messages</param>
		public MxmessageBool(bool iVal, 
			MxmMethod mxmMethod = default(MxmMethod), 
            MxmControlBlock controlBlock = null)
			: base (mxmMethod, controlBlock)
		{
			value = iVal;
		}

        /// <summary>
        /// Duplicate a Mxmessage
        /// </summary>
        /// <param name="message">Item to duplicate</param>
		public MxmessageBool(Mxmessage message) : base (message)
		{}

        /// <summary>
        /// Message copy method
        /// </summary>
        /// <returns>Deep copy of message</returns>
        public override Mxmessage Copy()
        {
			//Todo: Change here.
			MxmessageBool newMessage = new MxmessageBool(this);
            newMessage.value = value;

            return newMessage;
        }

        /// <summary>
        /// Deserialize the message
        /// </summary>
        /// <param name="reader">UNET based deserializer object</param>
        public override void Deserialize(NetworkReader reader)
		{
			base.Deserialize (reader);
			value = reader.ReadBoolean();
		}

        /// <summary>
        /// Serialize the Mxmessage
        /// </summary>
        /// <param name="writer">UNET based serializer</param>
		public override void Serialize(NetworkWriter writer)
		{
			base.Serialize (writer);
			writer.Write (value);
		}
    }
}