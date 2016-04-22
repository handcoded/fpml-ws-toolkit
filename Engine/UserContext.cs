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

namespace FpML.Engine
{
    /// <summary>
    /// A <b>UserContext</b> instance is use to access information specific to
    /// the user making webservice requests.
    /// </summary>
    public sealed class UserContext
    {
        /// <summary>
        /// Contains the owning username.
        /// </summary>
        public string Username
        {
            get
            {
                return (username);
            }
        }

        /// <summary>
        /// The queue of reponse messages waiting to be recovered.
        /// </summary>
        public MessageQueue Responses
        {
            get
            {
                return (responses);
            }
        }

        /// <summary>
        /// The queue of notification messages waiting to be recovered.
        /// </summary>
        public MessageQueue Notifications
        {
            get
            {
                return (notifications);
            }
        }

        /// <summary>
        /// Finds or creates the <b>UserContext</b> associated with the
        /// indicated username.
        /// </summary>
        /// <param name="username">The target username.</param>
        /// <returns>The associate <b>UserContext</b> instance.</returns>
        public static UserContext ForUser (string username)
        {
            lock (extent) {
                if (!extent.ContainsKey (username))
                    extent.Add (username, new UserContext (username));
            }
            return (extent [username]);
        }

        /// <summary>
        /// The set of all active <b>UserContext</b> instances.
        /// </summary>
        private static Dictionary<string, UserContext> extent
            = new Dictionary<string, UserContext> ();

        private readonly string username;

        private MessageQueue    responses
            = new MessageQueue ();

        private MessageQueue    notifications
            = new MessageQueue ();

        private UserContext (string username)
        {
            this.username = username;
        }
    }
}