﻿// <copyright file="JsonRpcError.cs" company="SoftChains">
//  Copyright 2016 Dan Gershony
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
//  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
//  EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
//  OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
namespace CCPP.Cryptocurrency.Common
{
    #region Using Directives

    using Newtonsoft.Json;

    #endregion

    /// <summary>
    /// The json rpc error.
    /// </summary>
    public class JsonRpcError
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        [JsonProperty(PropertyName = "code", Order = 0)]
        public int Code { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        [JsonProperty(PropertyName = "message", Order = 1)]
        public string Message { get; set; }

        #endregion
    }
}
