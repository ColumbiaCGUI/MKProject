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
    /// Mxmessage with float payload
    /// </summary>
    public class MxmessageFloat : Mxmessage
    {
        /// <summary>
        /// Float payload
        /// </summary>
		public float value;

        /// <summary>
        /// Creates a basic MxmessageByteArray
        /// </summary>
		public MxmessageFloat()
		{}

        /// <summary>
        /// Creates a basic Mxmessage, with a control block
        /// </summary>
        /// <param name="controlBlock">Object defining the routing of messages</param>
		public MxmessageFloat(MxmControlBlock controlBlock = null)
			: base (controlBlock)
		{
		}

        /// <summary>
        /// Create a Mxmessage, with control block, MxmMethod, and float
        /// </summary>
        /// <param name="iVal">Float payload</param>
        /// <param name="mxmMethod">Identifier of target MxmMethod</param>
        /// <param name="controlBlock">Object defining the routing of messages</param>
		public MxmessageFloat(float iVal, 
			MxmMethod mxmMethod = default(MxmMethod), 
            MxmControlBlock controlBlock = null)
            : base(mxmMethod, controlBlock)
        {
			value = iVal;
		}

        /// <summary>
        /// Duplicate a MxmessageFloat
        /// </summary>
        /// <param name="message">Item to duplicate</param>
		public MxmessageFloat(MxmessageFloat message) : base(message)
		{}

        /// <summary>
        /// Message copy method
        /// </summary>
        /// <returns>Duplicate of MxmMessage</returns>
        public override Mxmessage Copy()
        {
			MxmessageFloat newMessage = new MxmessageFloat (this);
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
			value = reader.ReadSingle();
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