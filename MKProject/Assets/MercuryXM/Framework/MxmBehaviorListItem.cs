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

namespace MercuryXM.Framework
{
    /// <summary>
    /// Wrapper objects for MxmBehaviors for use in MxmBehaviorLists.
    /// </summary>
    [System.Serializable]
    public class MxmBehaviorListItem
    {
        /// <summary>
        /// Name of the list item.
        /// These are not intended to be global.
        /// In their use in the editor, they will take the 
        /// name of the GameObject upon which the MxmBehavior was 
        /// placed.
        /// </summary>
        public string Name;

        /// <summary>
        /// The MxmBehavior.
        /// In the base system, these can be MxmNodes,
        /// MxmSwitchNodes, and MxmBaseBehavior and its
        /// derivations.
        /// </summary>
		public MxmBehavior Behavior;

        /// <summary>
        /// Indicates whether the MxmBehavior should be cloned
        /// on start or if the MxmBehaviorListItem should reference
        /// the original.
        /// </summary>
        public bool Clone;

        /// <summary>
        /// MxmTags allow you to specify filters for execution in 
        /// Mercury XM graphs. <see cref="MxmTag"/>
        /// </summary>
		public MxmTag Tags;

        /// <summary>
        /// The level of the MxmBehavior relative to the container of the
        /// MxmBehaviorList and MxmBehaviorListItem.
        /// </summary>
		public MxmLevelFilter Level;

        /// <summary>
        /// Default constructor
        /// </summary>
        public MxmBehaviorListItem()
        {}

        /// <summary>
        /// Create a MxmBehaviorListItem.
        /// </summary>
        /// <param name="name">Name of the MxmBehaviorListItem</param>
        /// <param name="behavior">Reference to the MxmBehavior to be stored.</param>
		public MxmBehaviorListItem(string name, MxmBehavior behavior)
        {
            Name = name;
			Behavior = behavior;
        }

        /// <summary>
        /// Create a MxmBehaviorListItem.
        /// </summary>
        /// <param name="name">Name of the MxmBehaviorListItem</param>
        /// <param name="behavior">Reference to the MxmBehavior to be stored.</param>
        /// <param name="clone">Whether to clone the MxmBehavior & GameObject
        /// or to use the original.</param>
		public MxmBehaviorListItem(string name, MxmBehavior behavior, bool clone)
        {
            Name = name;
			Behavior = behavior;
            Clone = clone;
        }

        /// <summary>
        /// Create a MxmBehaviorListItem.
        /// </summary>
        /// <param name="name">Name of the MxmBehaviorListItem</param>
        /// <param name="behavior">Reference to the MxmBehavior to be stored.</param>
        /// <param name="clone">Whether to clone the MxmBehavior & GameObject
        /// or to use the original.</param>
        /// <param name="tags">Tags to apply to the MxmBehaviorListItem.</param>
		public MxmBehaviorListItem(string name, MxmBehavior behavior, bool clone, MxmTag tags)
        {
            Name = name;
			Behavior = behavior;
            Clone = clone;

            Tags = tags;
        }

        /// <summary>
        /// Method to clone a GameObject of MxmBehaviorListItem.
        /// </summary>
        /// <returns>Handle to the new GameObject.</returns>
        public GameObject CloneGameObject()
        {
			GameObject clonedWFNodeGO = UnityEngine.GameObject.Instantiate(Behavior.gameObject);
			Behavior = clonedWFNodeGO.GetComponent<MxmNode>();

            return clonedWFNodeGO;
        }
    }
}