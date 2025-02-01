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

using QuantConnect.Data;

namespace QuantConnect.Brokerages.InteractiveBrokers.Orderbook
{
    /// <summary>
    /// BookSnapshot class for second and minute resolution data:
    /// An OHLC implementation of the QuantConnect BaseData class with parameters for candles.
    /// </summary>
    public class BookSnapshot : BaseData
    {
        /// <summary>
        /// Average bid size
        /// </summary>
        public decimal BidSize { get; set; }

        /// <summary>
        /// Average ask size
        /// </summary>
        public decimal AskSize { get; set; }

        public double BidPrice  { get; set; }
        public double AskPrice  { get; set; }

        /// <summary>
        /// The best bid&ask price
        /// </summary>
        public (double bid, double ask) GetTopOfBook
        {
            get
            {
                // lock (_locker)
                // {
                    return (BidPrice, AskPrice);
                // }
            }
        }
        
        /// <summary>
        /// Convert this <see cref="BookSnapshot"/> to string form.
        /// </summary>
        /// <returns>String representation of the <see cref="BookSnapshot"/></returns>
        public override string ToString()
        {
            return $"{Symbol}: BidPrice: {BidPrice} BidSize: {BidSize} ;; AskPrice: {AskPrice} AskSize: {AskSize}";
        }
    }
}
