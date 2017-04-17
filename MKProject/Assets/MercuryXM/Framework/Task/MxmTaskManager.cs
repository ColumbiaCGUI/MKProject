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
using MercuryXM.Data;
using UnityEngine;

namespace MercuryXM.Framework
{
    /// <summary>
    /// Class to manage a collection of tasks.
    /// </summary>
    /// <typeparam name="U">Must implement IMxmTaskInfo</typeparam>
    [Serializable]
    [RequireComponent(typeof(MxmTaskUserConfigurator))]
    public class MxmTaskManager<U> : MxmBaseBehavior
        where U : class, IMxmTaskInfo, new()
    {
        /// <summary>
        /// This switch node allows you to iterate through a set of tasks
        /// using the same mechanisms that allow you to iterate through an
        /// FSM of MxmBehaviors.
        /// </summary>
        public MxmSwitchNode TasksNode;

        /// <summary>
        /// The loaded task infos.
        /// </summary>
        public LinkedList<U> TaskInfos;

        /// <summary>
        /// Contains information of where to load task data, 
        /// and which tasks to load based on user ID.
        /// </summary>
        public MxmTaskUserConfigurator MxmTaskUserData;

        /// <summary>
        /// Node of currently selected task.
        /// </summary>
        protected LinkedListNode<U> currentTaskInfo;

        /// <summary>
        /// Current task info.
        /// </summary>
		public U CurrentTaskInfo { get { return currentTaskInfo.Value;  } set {currentTaskInfo.Value = value;} }
        /// <summary>
        /// Next task info.
        /// </summary>
        public U NextTaskInfo { get { return (currentTaskInfo.Next == null) ? null : currentTaskInfo.Next.Value; } }
        /// <summary>
        /// Previous task info.
        /// </summary>
        public U PrevTaskInfo { get { return (currentTaskInfo.Previous == null) ? null : currentTaskInfo.Previous.Value; } }

        /// <summary>
        /// This should be an item placed on the same GameObject as the MxmTaskManager itself
        /// </summary>
	    private ITaskInfoCollectionLoader<U> taskInfoCollectionLoader;
        
        /// <summary>
        /// Total number of task infos that have the same name.
        /// </summary>
        public int TotalTasksWithCurrentName
        {
            get
            {
                return (from t in TaskInfos
                        where t.TaskName == CurrentTaskInfo.TaskName
                        select t).Count();
            }
        }

        /// <summary>
        /// Handle to the task info collection loader that is used to load the 
        /// task collection.
        /// </summary>
	    public ITaskInfoCollectionLoader<U> TaskInfoCollectionLoader
	    {
	        get { return taskInfoCollectionLoader; }
	    }

	    #region MonoBehaviour Methods

        /// <summary>
        /// Get attached ItaskInfoCollectionLoader, which must be attached to the 
        /// same game object.
        /// </summary>
        public override void Awake()
	    {
            MxmLogger.LogApplication("MxmTaskManager Awake");

            taskInfoCollectionLoader = GetComponent<ITaskInfoCollectionLoader<U>>();
            MxmTaskUserData = GetComponent<MxmTaskUserConfigurator>();
        }

        /// <summary>
        /// Prepare the tasks that were loaded.
        /// </summary>
	    public override void Start()
	    {
            MxmLogger.LogApplication("MxmTaskManager Start");

	        PrepareTasks();
	    }

        #endregion
        
        /// <summary>
        /// Get current task behavior.
        /// </summary>
        /// <returns>Current task behavior</returns>
        public MxmTaskBehavior<U> GetCurrentTaskBehavior()
        {
            return TasksNode.Current.GetComponent<MxmTaskBehavior<U>>();
        }

        /// <summary>
        /// Extract a task behavior from a given MxmNode.
        /// </summary>
        /// <param name="mxmNode">MxmNode - should share GameObject with 
        /// a task behavior.</param>
        /// <returns>TaskBehavior, if present on GameObject.</returns>
        public MxmTaskBehavior<U> GetTaskBehavior(MxmNode mxmNode)
        {
            return mxmNode.GetComponent<MxmTaskBehavior<U>> ();
        }

        /// <summary>
        /// Set the current pointer to the first task in the list.
        /// </summary>
        public virtual void ProceedToFirstTask()
		{
			currentTaskInfo = TaskInfos.First;
		}

        /// <summary>
        /// Prepare the tasks that were loaded by the 
        /// TaskInfoCollectionLoader.
        /// </summary>
	    public virtual void PrepareTasks()
	    {
            int taskLoadStatus = taskInfoCollectionLoader.PrepareTasks(
                ref TaskInfos, MxmTaskUserData.UserId);

	        if (taskLoadStatus >= 0)
	        {
	            currentTaskInfo = GetNodeAt(taskLoadStatus);
	        }
	        else
	            MxmLogger.LogError("Task Load Failed");

	        ApplySequenceID();
	    }

        /// <summary>
        /// Move current task pointer to the next task info.
        /// If the instance can, it will attempt to trigger a switch message to 
        /// move the MxmFSM to the next task state.
        /// </summary>
	    public virtual void ProceedToNextTask()
	    {
            TaskInfoCollectionLoader.SaveCurrentTaskSequenceValue(CurrentTaskInfo.UserSequence);

	        currentTaskInfo = (currentTaskInfo == null)
			        ? TaskInfos.First
			        : currentTaskInfo.Next;

	        if (ShouldTriggerSwitch())
	        {
	            TasksNode.MxmInvoke(MxmMethod.Switch, CurrentTaskInfo.TaskName,
	                new MxmControlBlock(MxmLevelFilter.Self, MxmActiveFilter.All));
	        }

	        ApplySequenceID();
	    }

        /// <summary>
        /// Base class implementation of ShouldTriggerSwitch always returns true.
        /// </summary>
        /// <returns>Whether to trigger a switch in the MxmFSM</returns>
        public virtual bool ShouldTriggerSwitch()
        {
            return true;
        }

        /// <summary>
        /// Index accessor for LinkedList of task infos.
        /// </summary>
        /// <param name="index">Value index.</param>
        /// <returns>If in bounds, LinkedListNode at [index] from start.</returns>
        public LinkedListNode<U> GetNodeAt(int index)
        {
            if (index >= TaskInfos.Count) return null;

            var count = 0;
            var task = TaskInfos.First;

            while (task != null)
            {
                if (index == count) return task;
                task = task.Next;
                count++;
            }

            return null;
        }

        /// <summary>
        /// Apply the user-based sequence ID to the associated
        /// MxmTaskUserConfigurator.
        /// </summary>
        public virtual void ApplySequenceID()
        {
            MxmTaskUserData.SequenceId = currentTaskInfo.Value.UserSequence;
        }
    }
}