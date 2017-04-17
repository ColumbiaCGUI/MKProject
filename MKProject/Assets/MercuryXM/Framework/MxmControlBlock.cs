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
    /// MxmControlBlock is a collection of settings 
    /// allowing you to specify the precise invocation path
    /// of an Mxmessage invoked on a MxmNode through its
    /// Mxm Graph.
    /// </summary>
    public class MxmControlBlock
    {
        /// <summary>
        /// <see cref="MxmLevelFilter"/>
        /// </summary>
        public MxmLevelFilter LevelFilter;

        /// <summary>
        /// <see cref="MxmActiveFilter"/>
        /// </summary>
        public MxmActiveFilter ActiveFilter;

        /// <summary>
        /// <see cref="MxmSelectedFilter"/>
        /// </summary>
        public MxmSelectedFilter SelectedFilter;
        
        /// <summary>
        /// <see cref="MxmNetworkFilter"/>
        /// </summary>
        public MxmNetworkFilter NetworkFilter;
        
        /// <summary>
        /// <see cref="MxmTag"/>
        /// </summary>
        public MxmTag Tag;

        /// <summary>
        /// Create an MxmControlBlock
        /// </summary>
        /// <param name="levelFilter"><see cref="MxmLevelFilter"/></param>
        /// <param name="activeFilter"><see cref="MxmActiveFilter"/></param>
        /// <param name="selectedFilter"><see cref="MxmSelectedFilter"/></param>
        /// <param name="networkFilter"><see cref="MxmNetworkFilter"/></param>
        public MxmControlBlock(
            MxmLevelFilter levelFilter = MxmLevelFilterHelper.Default,
            MxmActiveFilter activeFilter = MxmActiveFilter.Active,
            MxmSelectedFilter selectedFilter = MxmSelectedFilter.All,
            MxmNetworkFilter networkFilter = MxmNetworkFilter.All)
        {
            LevelFilter = levelFilter;
            ActiveFilter = activeFilter;
            SelectedFilter = selectedFilter;
            NetworkFilter = networkFilter;
            Tag = MxmTagHelper.Everything;
        }

        /// <summary>
        /// Create an MxmControlBlock
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="levelFilter"><see cref="MxmLevelFilter"/></param>
        /// <param name="activeFilter"><see cref="MxmActiveFilter"/></param>
        /// <param name="selectedFilter"><see cref="MxmSelectedFilter"/></param>
        /// <param name="networkFilter"><see cref="MxmNetworkFilter"/></param>
        public MxmControlBlock(MxmTag tag,
            MxmLevelFilter levelFilter = MxmLevelFilterHelper.Default,
            MxmActiveFilter activeFilter = default(MxmActiveFilter),
            MxmSelectedFilter selectedFilter = default(MxmSelectedFilter),
            MxmNetworkFilter networkFilter = default(MxmNetworkFilter))
        {
            LevelFilter = levelFilter;
            ActiveFilter = activeFilter;
            SelectedFilter = selectedFilter;
            NetworkFilter = networkFilter;
            Tag = tag;
        }

        /// <summary>
        /// Copy Constructor for MxmControlBlock
        /// </summary>
        /// <param name="original">MxmControlBlock to be copied.</param>
        public MxmControlBlock (MxmControlBlock original)
		{
			LevelFilter = original.LevelFilter;
			ActiveFilter = original.ActiveFilter;
			SelectedFilter = original.SelectedFilter;
		    NetworkFilter = original.NetworkFilter;
		    Tag = original.Tag;
		}

        /// <summary>
        /// Deserialize the MxmControlBlock
        /// </summary>
        /// <param name="reader">UNET based deserializer object</param>
        public virtual void Deserialize(NetworkReader reader)
        {
            LevelFilter = (MxmLevelFilter) reader.ReadInt16();
            ActiveFilter = (MxmActiveFilter) reader.ReadInt16();
            SelectedFilter = (MxmSelectedFilter) reader.ReadInt16();
            NetworkFilter = (MxmNetworkFilter) reader.ReadInt16();
            Tag = (MxmTag)reader.ReadInt16();
        }

        /// <summary>
        /// Serialize the MxmControlBlock
        /// </summary>
        /// <param name="writer">UNET based serializer</param>
        public virtual void Serialize(NetworkWriter writer)
        {
            writer.Write((short) LevelFilter);
            writer.Write((short) ActiveFilter);
            writer.Write((short) SelectedFilter);
            writer.Write((short) NetworkFilter);
            writer.Write((short) Tag);
        }
    }

    /// <summary>
    /// Helper class to easily create common MxmControlBlocks
    /// </summary>
    public static class MxmControlBlockHelper
    {
        static public MxmControlBlock Default
        {
            get
            {
                return new MxmControlBlock(
                    default(MxmTag),
                    MxmLevelFilterHelper.Default,
                    default(MxmActiveFilter),
                    default(MxmSelectedFilter),
                    default(MxmNetworkFilter)
                );
            }
        }

		static public MxmControlBlock SelfDefaultTagAll
		{
			get
			{
				return new MxmControlBlock(
					MxmTagHelper.Everything,
					MxmLevelFilter.Self,
					default(MxmActiveFilter),
					default(MxmSelectedFilter),
					default(MxmNetworkFilter)
				);
			}
		}
    }
}