﻿// <copyright file="SignedRawTransaction.cs" company="SoftChains">
//  Copyright 2016 Dan Gershony
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
//  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
//  EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
//  OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
namespace CCPP.Cryptocurrency.Common.BTC
{
    /// <summary>
    /// The signed raw transaction.
    /// </summary>
    public class SignedRawTransaction
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether complete.
        /// </summary>
        public bool Complete { get; set; }

        /// <summary>
        /// Gets or sets the hex.
        /// </summary>
        public string Hex { get; set; }

        #endregion
    }
}
