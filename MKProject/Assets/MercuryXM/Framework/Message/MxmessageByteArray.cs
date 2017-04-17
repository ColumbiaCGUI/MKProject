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
    /// Mxmessage with byte array payload
    /// </summary>
	public class MxmessageByteArray : Mxmessage
    {
        /// <summary>
        /// Byte array payload
        /// </summary>
		public byte[] byteArr;

        /// <summary>
        /// Length of array
        /// </summary>
		public int length;

        /// <summary>
        /// Creates a basic MxmessageByteArray
        /// </summary>
		public MxmessageByteArray ()
		{}

        /// <summary>
        /// Creates a basic MxmessageByteArray, with control block
        /// </summary>
        /// <param name="controlBlock">Object defining the routing of messages</param>
		public MxmessageByteArray(MxmControlBlock controlBlock = null)
			: base (controlBlock)
        {}

        /// <summary>
        /// Create a Mxmessage, with control block, MxmMethod, and byte array
        /// </summary>
        /// <param name="iVal">Byte array payload</param>
        /// <param name="mxmMethod">Identifier of target MxmMethod</param>
        /// <param name="controlBlock">Object defining the routing of messages</param>
		public MxmessageByteArray(byte[] iVal, 
			MxmMethod mxmMethod = default(MxmMethod), 
            MxmControlBlock controlBlock = null)
            : base(mxmMethod, controlBlock)
        {
			byteArr = iVal;
		}

        /// <summary>
        /// Duplicate a Mxmessage
        /// </summary>
        /// <param name="message">Item to duplicate</param>
		public MxmessageByteArray(MxmessageByteArray message) : base(message)
		{}

        /// <summary>
        /// Message copy method
        /// </summary>
        /// <returns>Duplicate of MxmMessage</returns>
        public override Mxmessage Copy()
        {
			MxmessageByteArray newMessage = new MxmessageByteArray (this);
            newMessage.length = length;
            byteArr.CopyTo(newMessage.byteArr, 0);

            return newMessage;
        }

        /// <summary>
        /// Deserialize the message
        /// </summary>
        /// <param name="reader">UNET based deserializer object</param>
        public override void Deserialize(NetworkReader reader)
		{
			base.Deserialize (reader);
			length = reader.ReadInt32 ();
			byteArr = reader.ReadBytes(length);
		}

        /// <summary>
        /// Serialize the Mxmessage
        /// </summary>
        /// <param name="writer">UNET based serializer</param>
        public override void Serialize(NetworkWriter writer)
		{
			base.Serialize (writer);
			writer.Write (length);
			writer.Write (byteArr,length);
		}
    }
}