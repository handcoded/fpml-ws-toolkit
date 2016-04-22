//==============================================================================
//  _____      __  __ _                                      
// |  ___| __ |  \/  | |                                     
// | |_ | '_ \| |\/| | |                                     
// |  _|| |_) | |  | | |___                                  
// |_|  | .__/|_| _|_|_____|                 _               
// \ \  |_| / /__| |__/ ___|  ___ _ ____   _(_) ___ ___  ___ 
//  \ \ /\ / / _ \ '_ \___ \ / _ \ '__\ \ / / |/ __/ _ \/ __|
//   \ V  V /  __/ |_) |__) |  __/ |   \ V /| | (_|  __/\__ \
//    \_/\_/ \___|_.__/____/ \___|_|    \_/ |_|\___\___||___/
//                                                           
//
//==============================================================================
// Copyright (C)2014-2016 ISDA and HandCoded Software Ltd.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions
// are met:
//
// 1. Redistributions of source code must retain the above copyright
//    notice, this list of conditions and the following disclaimer.
// 2. Redistributions in binary form must reproduce the above copyright
//    notice, this list of conditions and the following disclaimer in the
//    documentation and/or other materials provided with the distribution.
//
// THIS SOFTWARE IS PROVIDED BY THE AUTHOR AND CONTRIBUTORS ''AS IS'' AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
// ARE DISCLAIMED.  IN NO EVENT SHALL THE AUTHOR OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS
// OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
// LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY
// OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF
// SUCH DAMAGE.
//
// This work is licensed under a Creative Commons Attribution 4.0 International
// License. See http://creativecommons.org/licenses/by/4.0/ for details.
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace FpML.Engine
{
    /// <summary>
    /// A simple thread safe in-memory message queue.
    /// </summary>
    public sealed class MessageQueue
    {
        /// <summary>
        /// Adds a message the queue waking any thread waiting for a message.
        /// </summary>
        /// <param name="message"></param>
        public void Enqueue (Message message)
        {
            lock (queue) {
                queue.Add (message);
                Monitor.PulseAll (queue);
            }
        }

        /// <summary>
        /// Attempts to read a message from the queue which has a matching
        /// message Id.
        /// </summary>
        /// <param name="responseId">The target message Id</param>
        /// <param name="wait">Indicates whether to wait.</param>
        /// <returns>The target <see cref="Message"/> or <b>null</b>.</returns>
        public Message Dequeue (string responseId, bool wait)
        {
            lock (queue) {
                do {
                    foreach (Message message in queue) {
                        if (message.MessageId == responseId) {
                            queue.Remove (message);
                            return (message);
                        }
                    }

                    if (wait) Monitor.Wait (queue);
                } while (wait);
            }
            return (null);
        }

        /// <summary>
        /// Remove a message from the queue optionally waiting if the queue is
        /// empty.
        /// </summary>
        /// <param name="wait">Indicates whether to wait.</param>
        /// <returns>The next <see cref="Message"/> or <b>null</b>.</returns>
        public Message Dequeue (bool wait)
        {
            Message result = null;

            lock (queue) {
                while (wait && (queue.Count == 0))
                    Monitor.Wait (queue);

                if (queue.Count > 0) {
                    result = queue [0];
                    queue.Remove (result);
                }
            }
            return (result);
        }

        /// <summary>
        /// The actual queue of message instances.
        /// </summary>
        private List<Message> queue
            = new List<Message> ();
    }
}