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

namespace MercuryXM.Support.Audio
{
    /// <summary>
    /// Sample sounds provided as part of the library.
    /// </summary>
    public enum SoundItem {
        Off,
        Correct,
        Error,
        GoodJob,
        GotIt,
        LetsGo,
        Next,
        Pling,
        StartRound,
        StartSign,
        TaDa,
        Twang,
        Victory,
        Wrong,
        TeleportGo
    }

    /// <summary>
    /// Manager for sample sounds provided as an example in the library.
    /// </summary>
    public class SoundEffectManager : MonoBehaviour {

        public static bool IsOff = false;

        /// <summary>
        /// AudioClips of sample sounds.
        /// </summary>
        public AudioClip Correct;
        public AudioClip Error;
        public AudioClip GoodJob;
        public AudioClip GotIt;
        public AudioClip LetsGo;
        public AudioClip Next;
        public AudioClip Pling;
        public AudioClip StartRound;
        public AudioClip StartSign;
        public AudioClip TaDa;
        public AudioClip Twang;
        public AudioClip Victory;
        public AudioClip Wrong;
        public AudioClip TeleportGo;

        static private AudioSource source;

        /// <summary>
        /// Collection of AudioClips keyed by SoundItem. <see cref="SoundItem"/>
        /// </summary>
        static Dictionary<SoundItem, AudioClip> soundLibrary;

        /// <summary>
        /// Grabs attached AudioSource if one present.
        /// </summary>
        public void Awake () {
            source = GetComponent<AudioSource>();

            SetupLibrary();	
        }

        /// <summary>
        /// Setup dictionary of sample sounds using SoundItem. <see cref="SoundItem"/>
        /// </summary>
        void SetupLibrary() {
            soundLibrary = new Dictionary<SoundItem, AudioClip>
            {
                { SoundItem.Correct   , Correct   },
                { SoundItem.Error     , Error     },
                { SoundItem.GoodJob   , GoodJob   },
                { SoundItem.GotIt     , GotIt     },
                { SoundItem.LetsGo    , LetsGo    },
                { SoundItem.Next      , Next      },
                { SoundItem.Pling     , Pling     },
                { SoundItem.StartRound, StartRound},
                { SoundItem.StartSign , StartSign },
                { SoundItem.TaDa      , TaDa      },
                { SoundItem.Twang     , Twang     },
                { SoundItem.Victory   , Victory   },
                { SoundItem.Wrong     , Wrong     },
                { SoundItem.TeleportGo, TeleportGo}
            };

        }
	
        /// <summary>
        /// Invoke to play sound effect.
        /// </summary>
        /// <param name="soundType"><see cref="SoundItem"/></param>
        /// <param name="volume">Audio volume.</param>
        public static void PlayEffect(SoundItem soundType, float volume = 1) {
            if (!IsOff)
                source.PlayOneShot(soundLibrary[soundType], volume);
        }
    }
}