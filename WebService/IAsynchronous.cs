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

namespace FpML.WebService
{
    /// <summary>
    /// Defines the WebService API for asynchronous message processing.
    /// </summary>
    public interface IAsynchronous
    {
        /// <summary>
        /// Submits an FpML document for processing and returns a reponse
        /// message identifier that can be used for correlation.
        /// </summary>
        /// <param name="document">The FpML message document to process.</param>
        /// <returns>The unique response message correlation identifier.</returns>
        string SubmitMessage (string document);

        /// <summary>
        /// Returns the next response message optionally allowing the
        /// caller to wait if the queue is empty.
        /// </summary>
        /// <param name="wait">Indicates if the call should wait.</param>
        /// <returns>The next response message or <c>null</c> if there
        /// are none in the queue.</returns>
        string RetrieveMessage (bool wait);

        /// <summary>
        /// Returns a specific response message based on the identifier
        /// returned at the time of request submission.
        /// </summary>
        /// <param name="requestId">The unique correlation identifier.</param>
        /// <param name="wait">Indicates if the call should wait.</param>
        /// <returns>The next response message or <c>null</c> if there
        /// are none in the queue.</returns>
        string RetrieveResponseMessage (string requestId, bool wait);
    }
}