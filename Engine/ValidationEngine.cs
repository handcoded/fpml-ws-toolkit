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
using System.Xml;

using HandCoded.FpML;
using HandCoded.FpML.Meta;
using HandCoded.FpML.Validation;
using HandCoded.Validation;
using HandCoded.Xml.Writer;
using HandCoded.Xml;

namespace FpML.Engine
{
    public sealed class ValidationEngine : Engine
    {
        /// <summary>
        /// Constructs a <b>ValidationEngine.</b>
        /// </summary>
        public ValidationEngine ()
        { }

        /// <summary>
        /// Informs the engine that a user has started a session.
        /// </summary>
        /// <param name="context">Identifies the user.</param>
        public override void StartSession (UserContext context)
        {
            context.Notifications.Enqueue (new Message (CreateNotificationMessage (context)));
        }

        /// <summary>
        /// Initiates the processing for a message and returns a resulting
        /// message or <b>null</b> if none.
        /// </summary>
        /// <param name="context">Identifies the user.</param>
        /// <param name="message">The message to be processed.</param>
        /// <returns>The resulting message or <b>null</b>.</returns>
        public override Message Process (UserContext context, Message message)
        {
            ValidationErrorSet errorSet = new ValidationErrorSet ();
            ValidationErrorSetAdapter adapter = new ValidationErrorSetAdapter (errorSet);
            SchemaRelease release = Releases.R5_9_CONFIRMATION;

            XmlDocument document = FpMLUtility.Parse (message.Payload, adapter.SyntaxError);

            if (errorSet.Count == 0) {
                release = (SchemaRelease) Releases.FPML.GetReleaseForDocument (document);
                if (release == null) release = Releases.R5_9_CONFIRMATION;

                if (FpMLUtility.Validate (document, adapter.SemanticError))
                    return (message.Reply (CreateAcceptedMessage (release, context, document)));
            }
            return (message.Reply (CreateRejectedMessage (release, context, document, errorSet)));
        }

        /// <summary>
        /// Creates a sample notification message using the FpML 'serviceNotification'
        /// element.
        /// </summary>
        /// <param name="context">Identifies the user.</param>
        /// <returns>The payload of the notification message.</returns>
        private string CreateNotificationMessage (UserContext context)
        {
            Builder         builder = CreateMessage (Releases.R5_9_CONFIRMATION, "serviceNotification", context, null);

            builder.AppendElementAndText ("serviceName", "FpMLWebServices");
            builder.AppendElementAndText ("status", "Available");

            return (NestedWriter.ToString (builder.Document));
        }

        /// <summary>
        /// Creates a response message for a message that passed full validation.
        /// </summary>
        /// <param name="release">The FpML release to respond with.</param>
        /// <param name="context">Identifies the user.</param>
        /// <param name="document">The parsed form of the input messsage.</param>
        /// <returns></returns>
        private string CreateAcceptedMessage (SchemaRelease release, UserContext context, XmlDocument document)
        {
            Builder         builder = CreateMessage (release, "serviceNotification", context, document);

            builder.AppendElementAndText ("serviceName", "FpMLWebServices");
            builder.AppendElement ("advisory");
            builder.AppendElementAndText ("categpry", "Rules");
            builder.AppendElementAndText ("description", "Message was accepted");
            builder.CloseElement ();

            return (NestedWriter.ToString (builder.Document));
        }

        /// <summary>
        /// Creates a response message for a message that failed XML or FpML
        /// validation.
        /// </summary>
        /// <param name="release">The FpML release to respond with.</param>
        /// <param name="context">Identifies the user.</param>
        /// <param name="document">The parsed form of the input messsage or <b>null</b>.</param>
        /// <param name="errorSet">The set of detected errors.</param>
        /// <returns></returns>
        private string CreateRejectedMessage (SchemaRelease release, UserContext context, XmlDocument document,
            ValidationErrorSet errorSet)
        {
            Builder         builder = CreateMessage (release, "messageRejected", context, document);

            for (int index = 0; index < errorSet.Count; ++index) {
                ValidationError error = errorSet [index];

                builder.AppendElement ("reason");

                builder.AppendElementAndText ("reasonCode", error.Code);
                
                builder.AppendElement ("location");
                builder.SetAttribute ("locationType", error.IsLexical ? "lexical" : "xpath");
                builder.AppendText (error.Context);
                builder.CloseElement ();

                if (error.Description != null)
                    builder.AppendElementAndText ("description", error.Description);

                if (error.RuleName != null)
                    builder.AppendElementAndText ("validationRuleId", error.RuleName);

                if (error.AdditionalData != null) {
                    builder.AppendElement ("additionalData");
                    builder.AppendElementAndText ("string", error.AdditionalData);
                    builder.CloseElement ();
                }

                builder.CloseElement ();
            }

            return (NestedWriter.ToString (builder.Document));
        }

        /// <summary>
        /// Constructs an FpML message builds the message header, if possible
        /// copying data from the original input message.
        /// </summary>
        /// <param name="release">he FpML release to respond with.</param>
        /// <param name="rootElement">The name of the root element.</param>
        /// <param name="context">Identifies the user.</param>
        /// <param name="document">The parsed form of the input messsage or <b>null</b>.</param>
        /// <returns></returns>
        private Builder CreateMessage (SchemaRelease release, string rootElement,
            UserContext context, XmlDocument document)
        {
            Builder         builder = new HandCoded.FpML.Xml.Builder (release.NewInstance (rootElement));

            builder.AppendElement ("header");
            builder.AppendElement ("messageId");
            builder.SetAttribute ("messageIdScheme", "urn:handcoded:message-id");
            builder.AppendText (Guid.NewGuid ().ToString ());
            builder.CloseElement ();

            if (document != null) {
                XmlElement messageId = (XmlElement) XPath.Path (document.DocumentElement, "header", "messageId");
                if (messageId != null) {
                    builder.AppendElement ("inReplyTo");
                    builder.SetAttribute ("messageIdScheme", messageId.GetAttribute ("messageIdScheme"));
                    builder.AppendText (Types.ToString (messageId));
                    builder.CloseElement ();
                }
            }

            builder.AppendElement ("sentBy");
            builder.SetAttribute ("messageAddressScheme", "urn:handcoded:message-address");
            builder.AppendText ("FpMLWebServiceDemo");
            builder.CloseElement ();

            builder.AppendElement ("sendTo");
            builder.SetAttribute ("messageAddressScheme", "urn:handcoded:message-address");
            builder.AppendText (context.Username);
            builder.CloseElement ();

            builder.AppendElementAndText ("creationTimestamp",
                HandCoded.Finance.DateTime.Now (true).ToString ());

            builder.CloseElement ();

            return (builder);
        }
    }
}