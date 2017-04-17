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
    /// Base class for messages passed through MxmInvoke
    /// Built on Unity's Message Base, allowing usage of serialize/deserialize
    /// </summary>
    public class Mxmessage : MessageBase
    {
        /// <summary>
        /// The MxmMethod invoked by the calling object.
        /// </summary>
        public MxmMethod MxmMethod;

        /// <summary>
        /// Control parameters designating how a message should traverse a Mxm Graph. 
        /// </summary>
        public MxmControlBlock ControlBlock;

        /// <summary>
        /// Network identifier of sender/recipient objects.
        /// </summary>
        public uint NetId;

        /// <summary>
        /// Utilized by Mxm serialization/deserialization systems.
        /// </summary>
		public bool IsDeserialized { get; private set; }

        /// <summary>
        /// Deprecated - remove in next version
        /// </summary>
        public bool root = true;

        /// <summary>
        /// Message timestamp, assists in collision avoidance
        /// </summary>
        public string TimeStamp;

        /// <summary>
        /// Creates a basic Mxmessage with a default control block
        /// </summary>
        public Mxmessage()
        {
            ControlBlock = new MxmControlBlock();
        }

        /// <summary>
        /// Creates a basic Mxmessage with the passed control block.
        /// </summary>
        /// <param name="controlBlock">Object defining the routing of messages.</param>
		public Mxmessage(MxmControlBlock controlBlock)
		{
			ControlBlock = new MxmControlBlock(controlBlock);
		}

        /// <summary>
        /// Create a Mxmessage, with defined control block and MxmMethod
        /// </summary>
        /// <param name="mxmMethod">Identifier of target MxmMethod</param>
        /// <param name="controlBlock">Object defining the routing of messages through Mxm Graphs.</param>
		public Mxmessage(MxmMethod mxmMethod,
			MxmControlBlock controlBlock = null)
		{
			MxmMethod = mxmMethod;

			if(controlBlock != null)
				ControlBlock = new MxmControlBlock(controlBlock);
			else
				ControlBlock = new MxmControlBlock();
		}

        /// <summary>
        /// Create a Mxmessage, with filters defined directly
        /// </summary>
        /// <param name="mxmMethod">Identifier of target MxmMethod</param>
        /// <param name="levelFilter">Determines direction of messages</param>
        /// <param name="activeFilter">Determines whether message sent to active and/or inactive objects</param>
        /// <param name="selectedFilter">Determines whether message sent to objects "selected" as defined by MxmNode implementation</param>
        /// <param name="networkFilter">Determines whether message will remain local or can be sent over the network</param>
        public Mxmessage(MxmMethod mxmMethod, 
			MxmLevelFilter levelFilter,
			MxmActiveFilter activeFilter,
            MxmSelectedFilter selectedFilter,
            MxmNetworkFilter networkFilter)
        {
            MxmMethod = mxmMethod;

            ControlBlock = new MxmControlBlock();
            ControlBlock.LevelFilter = levelFilter;
            ControlBlock.ActiveFilter = activeFilter;
            ControlBlock.SelectedFilter = selectedFilter;
            ControlBlock.NetworkFilter = networkFilter;
        }

        /// <summary>
        /// Duplicate a Mxmessage
        /// </summary>
        /// <param name="message">Item to duplicate</param>
        public Mxmessage(Mxmessage message) : this(message.MxmMethod, message.ControlBlock)
        {
            NetId = message.NetId;
            IsDeserialized = message.IsDeserialized;
            root = message.root;
            TimeStamp = message.TimeStamp;
        }

        /// <summary>
        /// Message copy method
        /// </summary>
        /// <returns>Deep copy of message</returns>
        public virtual Mxmessage Copy()
        {
            return new Mxmessage(this);
        }

        /// <summary>
        /// Deserialize the Mxmessage
        /// </summary>
        /// <param name="reader">UNET based deserializer object</param>
        public override void Deserialize(NetworkReader reader)
		{
			MxmMethod = (MxmMethod)reader.ReadInt16();
            ControlBlock.Deserialize(reader);
            TimeStamp = reader.ReadString();
            NetId = reader.ReadUInt32();
            IsDeserialized = true;

            //On Deserialize, Network contexts should switch to local
            ControlBlock.NetworkFilter = MxmNetworkFilter.Local;
		}

        /// <summary>
        /// Serialize the Mxmessage
        /// </summary>
        /// <param name="writer">UNET based serializer</param>
		public override void Serialize(NetworkWriter writer)
		{
			writer.Write((short)MxmMethod);
            ControlBlock.Serialize(writer);
            writer.Write(TimeStamp);
            writer.Write(NetId);
        }
    }
}