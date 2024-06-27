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
    /// Event arguments class for the <see cref="InteractiveBrokersClient.Tape"/> event
    /// 
    /// see https://ibkrcampus.com/ibkr-api-page/twsapi-doc/#receive-tick-data
    /// </summary>
    public sealed class TapeEventArgs : EventArgs
    {
        /// <summary>
        /// The request's unique identifier.
        /// </summary>
        public int TickerId { get; }

        /// <summary>
        /// 0 = “Last”, 1 = “AllLast”
        /// </summary>
        public int TickType { get; }
        
        /// <summary>
        /// real-time tick timestamp
        /// </summary>
        public long Time { get; }

        /// <summary>
        /// real-time tick last price
        /// </summary>
        public double Price { get; }
        
        /// <summary>
        /// real-time tick last size
        /// </summary>
        public decimal Size { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TapeEventArgs"/> class
        /// </summary>
        public TapeEventArgs(int reqId, int tickType, long time, double price, decimal size)
        {
            TickerId = reqId;
            TickType = tickType;
            Time = time;
            Price = price;
            Size = size;
        }
    }
}
