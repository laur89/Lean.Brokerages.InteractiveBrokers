/*
 * QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
 * Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*/

using System;

namespace QuantConnect.Brokerages.InteractiveBrokers.Client
{
    /// <summary>
    /// Event arguments class for the <see cref="InteractiveBrokersClient.MktDepth"/> event
    /// 
    /// see https://ibkrcampus.com/ibkr-api-page/twsapi-doc/#receive-market-depth
    /// </summary>
    public sealed class MktDepthEventArgs : EventArgs
    {
        ///
        /// 
        /// <summary>
        /// The request's unique identifier.
        /// </summary>
        public int TickerId { get; }

        /// <summary>
        /// The order book’s row being updated
        /// </summary>
        public int Position { get; }
        
        /// <summary>
        /// 0 = insert, 1 = update, 2 = delete
        /// </summary>
        public int Operation { get; }
        
        /// <summary>
        /// 0 = ask, 1 = bid
        /// </summary>
        public int Side { get; }

        public double Price { get; }
        public decimal Size { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MktDepthEventArgs"/> class
        /// </summary>
        public MktDepthEventArgs(int tickerId, int position, int operation, int side, double price, decimal size)
        {
            TickerId = tickerId;
            Position = position;
            Operation = operation;
            Side = side;
            Price = price;
            Size = size;
        }
    }
}
