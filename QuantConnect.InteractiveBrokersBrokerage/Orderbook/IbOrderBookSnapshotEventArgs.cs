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
using System.Collections.Generic;

namespace QuantConnect.Brokerages.InteractiveBrokers.Orderbook
{
    /// <summary>
    /// Event arguments class for the <see cref="IbOrderBook.OrderBookSnapshot"/> event
    /// </summary>
    public sealed class IbOrderBookSnapshotEventArgs : EventArgs
    {
        public List<Double> BidPrices { get; }
        public List<Decimal> BidSizes { get; }
        public List<Double> AskPrices { get; }
        public List<Decimal> AskSizes { get; }
        
        /// <summary>
        /// Gets the new best bid price
        /// </summary>
        public Symbol Symbol { get; }

        public IbOrderBookSnapshotEventArgs(Symbol symbol, List<double> bidPrices,
            List<decimal> bidSizes, List<double> askPrices, List<decimal> askSizes)
        {
            Symbol = symbol;
            // note we copy the underlying datastructures; implies caller holds a lock!
            BidPrices = new List<double>(bidPrices);
            BidSizes = new List<decimal>(bidSizes);
            AskPrices = new List<double>(askPrices);
            AskSizes = new List<decimal>(askSizes);
        }

        // public static IbOrderBookSnapshotEventArgs fromOrderBook(IbOrderBook book)
        // {
        //     return new IbOrderBookSnapshotEventArgs(
        //         book.Symbol,
        //         book.BidPrices,
        //         book.BidSizes,
        //         book.AskPrices,
        //         book.AskSizes
        //     );
        // }
    }
}
