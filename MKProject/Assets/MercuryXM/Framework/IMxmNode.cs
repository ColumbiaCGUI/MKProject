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
using UnityEngine;

namespace MercuryXM.Framework
{
    /// <summary>
    /// Defines methods required by Mxm behavior & utility methods 
    /// making invocation MxmMethods with built-in Mxmessages easier.
    /// </summary>
    public interface IMxmNode
    {
        /// <summary>
        /// Invoke a MxmMethod. 
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
        void MxmInvoke(MxmMessageType msgType, Mxmessage message);

        /// <summary>
        /// Invoke a MxmMethod with no parameter. 
        /// </summary>
        /// <param name="mxmMethod">MxmMethod Identifier - <see cref="MxmMethod"/></param>
        /// <param name="controlBlock">Object defining the routing of 
        /// Mxmessages through Mxm Graphs. <see cref="MxmControlBlock"/></param>
        void MxmInvoke(MxmMethod mxmMethod, MxmControlBlock controlBlock);

        /// <summary>
        /// Invoke a MxmMethod with parameter: int. 
        /// </summary>
        /// <param name="mxmMethod">MxmMethod Identifier - <see cref="MxmMethod"/></param>
        /// <param name="param">MxmMethod parameter: int.</param>
        /// <param name="controlBlock">Object defining the routing of 
        /// Mxmessages through Mxm Graphs. <see cref="MxmControlBlock"/></param>
        void MxmInvoke(MxmMethod mxmMethod, int param, MxmControlBlock controlBlock);

        /// <summary>
        /// Invoke a MxmMethod with parameter: Vector3.  
        /// </summary>
        /// <param name="mxmMethod">MxmMethod Identifier - <see cref="MxmMethod"/></param>
        /// <param name="param">MxmMethod parameter: Vector3.</param>
        /// <param name="controlBlock">Object defining the routing of 
        /// Mxmessages through Mxm Graphs. <see cref="MxmControlBlock"/></param>
        void MxmInvoke(MxmMethod mxmMethod, Vector3 param, MxmControlBlock controlBlock);

        /// <summary>
        /// Invoke a MxmMethod with parameter: Vector4.  
        /// </summary>
        /// <param name="mxmMethod">MxmMethod Identifier - <see cref="MxmMethod"/></param>
        /// <param name="param">MxmMethod parameter: Vector4.</param>
        /// <param name="controlBlock">Object defining the routing of 
        /// Mxmessages through Mxm Graphs. <see cref="MxmControlBlock"/></param>
        void MxmInvoke(MxmMethod mxmMethod, Vector4 param, MxmControlBlock controlBlock);

        /// <summary>
        /// Invoke a MxmMethod with parameter: string. 
        /// </summary>
        /// <param name="mxmMethod">MxmMethod Identifier - <see cref="MxmMethod"/></param>
        /// <param name="param">MxmMethod parameter: string.</param>
        /// <param name="controlBlock">Object defining the routing of 
        /// Mxmessages through Mxm Graphs. <see cref="MxmControlBlock"/></param>
        void MxmInvoke(MxmMethod mxmMethod, string param, MxmControlBlock controlBlock);

        /// <summary>
        /// Invoke a MxmMethod with parameter: float. 
        /// </summary>
        /// <param name="mxmMethod">MxmMethod Identifier - <see cref="MxmMethod"/></param>
        /// <param name="param">MxmMethod parameter: float.</param>
        /// <param name="controlBlock">Object defining the routing of 
        /// Mxmessages through Mxm Graphs. <see cref="MxmControlBlock"/></param>
        void MxmInvoke(MxmMethod mxmMethod, float param, MxmControlBlock controlBlock);

        /// <summary>
        /// Invoke a MxmMethod with parameter: byte array.  
        /// </summary>
        /// <param name="mxmMethod">MxmMethod Identifier - <see cref="MxmMethod"/></param>
        /// <param name="param">MxmMethod parameter: byte array.</param>
        /// <param name="controlBlock">Object defining the routing of 
        /// Mxmessages through Mxm Graphs. <see cref="MxmControlBlock"/></param>
        void MxmInvoke(MxmMethod mxmMethod, byte[] param, MxmControlBlock controlBlock);

        /// <summary>
        /// Invoke a MxmMethod with parameter: bool. 
        /// </summary>
        /// <param name="mxmMethod">MxmMethod Identifier - <see cref="MxmMethod"/></param>
        /// <param name="param">MxmMethod parameter: bool.</param>
        /// <param name="controlBlock">Object defining the routing of 
        /// Mxmessages through Mxm Graphs. <see cref="MxmControlBlock"/></param>
        void MxmInvoke(MxmMethod mxmMethod, bool param, MxmControlBlock controlBlock);

        /// <summary>
        /// Invoke a MxmMethod with parameter: IMxmSerializable. 
        /// </summary>
        /// <param name="mxmMethod">MxmMethod Identifier - <see cref="MxmMethod"/></param>
        /// <param name="param">MxmMethod parameter: IMxmSerializable. <see cref="IMxmSerializable"/> </param>
        /// <param name="controlBlock">Object defining the routing of 
        /// Mxmessages through Mxm Graphs. <see cref="MxmControlBlock"/></param>
        void MxmInvoke(MxmMethod mxmMethod, IMxmSerializable param, MxmControlBlock controlBlock);

        /// <summary>
        /// Invoke a MxmMethod with parameter: MxmTransform. 
        /// </summary>
        /// <param name="mxmMethod">MxmMethod Identifier - <see cref="MxmMethod"/></param>
        /// <param name="param">MxmMethod parameter: MxmTransform. <see cref="MxmTransform"/></param>
        /// <param name="controlBlock">Object defining the routing of 
        /// Mxmessages through Mxm Graphs. <see cref="MxmControlBlock"/></param>
        void MxmInvoke(MxmMethod mxmMethod, MxmTransform param, MxmControlBlock controlBlock);

        /// <summary>
        /// Invoke a MxmMethod with parameter: List<MxmTransform>. 
        /// </summary>
        /// <param name="mxmMethod">MxmMethod Identifier - <see cref="MxmMethod"/></param>
        /// <param name="param">MxmMethod parameter: List<MxmTransform>.</param>
        /// <param name="controlBlock">Object defining the routing of 
        /// Mxmessages through Mxm Graphs. <see cref="MxmControlBlock"/></param>
        void MxmInvoke(MxmMethod mxmMethod, List<MxmTransform> param, MxmControlBlock controlBlock);
    }
}