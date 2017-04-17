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

using System.Collections.Generic;
using System.Linq;
using MercuryXM.Support.Extensions;

namespace MercuryXM.Framework
{
    /// <summary>
    /// A form of Reorderable List <see cref="ReorderableList{T}"/>
    ///     specifically for all derivations of MxmBehavior.
    /// </summary>
    [System.Serializable]
    public class MxmBehaviorList : ReorderableList<MxmBehaviorListItem>
    {
        /// <summary>
        /// Useful for extracting certain types of MxmBehaviors from the list.
        /// </summary>
        public enum ListFilter { All = 0, NodeOnly, BehaviorOnly };

        /// <summary>
        /// Accessor for MxmBehaviorListItems by name.
        /// Will throw KeyNotFoundException if not found.
        /// </summary>
        /// <param name="name">Name of MxmBehaviorListItem.</param>
        /// <returns>First item with the name.</returns>
        public MxmBehaviorListItem this[string name]
        {
            get { return _list.Find(item => item.Name == name); }
            set
            {
                MxmBehaviorListItem refVal = this[name];

                if (refVal == null)
                {
                    throw new KeyNotFoundException();
                }
                int itemIndex = _list.IndexOf(refVal);
                _list[itemIndex] = value;
            }
        }

        /// <summary>
        /// Accessor for MxmBehaviorListItems by MxmBehavior reference.
        /// </summary>
        /// <param name="behavior">MxmBehavior for which to search.</param>
        /// <returns>MxmBehaviorListItem with reference or NULL.</returns>
        public MxmBehaviorListItem this[MxmBehavior behavior]
        {
            get { return _list.Find(item => item.Behavior == behavior); }
        }

        /// <summary>
        /// Get a list of the names all MxmBehaviorListItems that 
        /// match the provided filters.
        /// </summary>
        /// <param name="filter">ListFilter <see cref="ListFilter"/></param>
        /// <param name="levelFilter">LevelFilter <see cref="MxmLevelFilter"/></param>
        /// <returns>List of names of MxmBehaviorListItems that pass filter checks.</returns>
        public List<string> GetMxmNames(ListFilter filter = default(ListFilter),
            MxmLevelFilter levelFilter = MxmLevelFilterHelper.Default)
        {
            return GetMxmBehaviorListItems(filter, levelFilter).
                    Select(x => x.Name).ToList();
        }

        /// <summary>
        /// Get a list of all MxmBehaviorListItems that 
        /// match the provided filters.
        /// </summary>
        /// <param name="filter">ListFilter <see cref="ListFilter"/></param>
        /// <param name="levelFilter">LevelFilter <see cref="MxmLevelFilter"/></param>
        /// <returns>List of MxmBehaviorListItems that pass filter checks.</returns>
        public List<MxmBehaviorListItem> GetMxmBehaviorListItems(
            ListFilter filter = default(ListFilter),
            MxmLevelFilter levelFilter = MxmLevelFilterHelper.Default)
        {
            return this.Where(x => CheckFilter(x, filter, levelFilter)).ToList();
        }

        /// Get a list of all MxmBehaviorListItems that reference MxmNodes.
        /// <returns>List of all MxmBehaviorListItems that reference MxmNodes.</returns>
        public List<MxmNode> GetOnlyMxmNodes()
        {
            return this.Where(x => x.Behavior is MxmNode).
                Select(x => (MxmNode)(x.Behavior)).ToList();
        }

        /// <summary>
        /// Checks whether the MxmBehaviorList contains an item with the provided name.
        /// </summary>
        /// <param name="key">Name for which to search.</param>
        /// <returns>Whether the MxmBehaviorList contains an item with 
        /// the provided name.</returns>
        public bool ContainsKey(string key)
        {
            return (this[key] != null);
        }

        /// <summary>
        /// Checks whether the MxmBehaviorList contains an item with the 
        /// provided MxmBehavior reference.
        /// </summary>
        /// <param name="behavior">MxmBehavior for which to search.</param>
        /// <returns>Whether the MxmBehaviorList contains an item with the 
        /// provided MxmBehavior reference.</returns>
        public bool Contains(MxmBehavior behavior)
        {
            return (this[behavior] != null);
        }

        /// <summary>
        /// Checks the provided MxmBehaviorListItem to see
        /// whether it passes the list filter requirements.
        /// </summary>
        /// <param name="item">Observed MxmBehaviorListItem.</param>
        /// <param name="listFilter">ListFilter <see cref="ListFilter"/></param>
        /// <param name="levelFilter">LevelFilter <see cref="MxmLevelFilter"/></param>
        /// <returns>Whether MxmBehaviorListItem passes filter check.</returns>
        public bool CheckFilter(MxmBehaviorListItem item, 
            ListFilter listFilter, MxmLevelFilter levelFilter)
        {
            //Level Check
            if ((levelFilter & item.Level) == 0)
                return false;

            //List Filter check
            if (listFilter == ListFilter.NodeOnly && !(item.Behavior is MxmNode))
                return false;
            if (listFilter == ListFilter.BehaviorOnly && item.Behavior is MxmNode)
                return false;

            //All conditions passed, return true
            return true;
        }
    }
}
