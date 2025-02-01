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
using System.Linq;
using QuantConnect.Data.Market;
using QuantConnect.Data.OrderBook;

namespace QuantConnect.Brokerages.InteractiveBrokers.Orderbook
{
    /// <summary>
    /// Represents a full order book for a security.
    /// It contains prices and order sizes for each bid and ask level.
    /// The best bid and ask prices are also kept up to date.
    /// </summary>
    public class IbOrderBook //: IOrderBookUpdater<decimal, decimal>
    {
        protected double _bestBidPrice;
        protected decimal _bestBidSize;
        protected double _bestAskPrice;
        protected decimal _bestAskSize;

        // price-to-size mappings for bid&ask that _actually traded_ on tape:
        public readonly Dictionary<double, decimal> TapeAskSizes = new(10);
        public readonly Dictionary<double, decimal> TapeBidSizes = new(10);

        protected readonly object _locker = new();

        /// <summary>
        /// TODO
        /// </summary>
        public readonly List<double> BidPrices = new(10);

        public readonly List<decimal> BidSizes = new(10);
        public readonly List<double> AskPrices = new(10);
        public readonly List<decimal> AskSizes = new(10);

        /// <summary>
        /// Represents a unique security identifier of current Order Book
        /// </summary>
        public Symbol Symbol { get; }

        /// <summary>
        /// Event fired each time <see cref="BestBidPrice"/> or <see cref="BestAskPrice"/> are changed
        /// </summary>
        public event EventHandler<IbBestBidAskUpdatedEventArgs> BestBidAskUpdated;

        /// <summary>
        /// Event fired each time orderbook state is changed
        /// </summary>
        public event EventHandler<IbOrderBookSnapshotEventArgs> OrderBookSnapshot;

        /// <summary>
        /// The best bid price
        /// </summary>
        public double BestBidPrice
        {
            get
            {
                lock (_locker)
                {
                    return _bestBidPrice;
                }
            }
        }

        /// <summary>
        /// The best bid size
        /// </summary>
        public decimal BestBidSize
        {
            get
            {
                lock (_locker)
                {
                    return _bestBidSize;
                }
            }
        }

        /// <summary>
        /// The best ask price
        /// </summary>
        public double BestAskPrice
        {
            get
            {
                lock (_locker)
                {
                    return _bestAskPrice;
                }
            }
        }

        /// <summary>
        /// The best bid&ask price
        /// </summary>
        public (double bid, double ask) GetTopOfBook
        {
            get
            {
                lock (_locker)
                {
                    return (BidPrices[0], AskPrices[0]);
                }
            }
        }

        /// <summary>
        /// The best ask size
        /// </summary>
        public decimal BestAskSize
        {
            get
            {
                lock (_locker)
                {
                    return _bestAskSize;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IbOrderBook"/> class
        /// </summary>
        /// <param name="symbol">The symbol for the order book</param>
        public IbOrderBook(Symbol symbol)
        {
            Symbol = symbol;
        }

        /// <summary>
        /// Clears all bid/ask levels and prices.
        /// </summary>
        public void Clear()
        {
            lock (_locker)
            {
                _bestBidPrice = 0;
                _bestBidSize = 0;
                _bestAskPrice = 0;
                _bestAskSize = 0;

                TapeBidSizes.Clear();
                TapeAskSizes.Clear();

                BidPrices.Clear();
                BidSizes.Clear();
                AskPrices.Clear();
                AskSizes.Clear();
            }
        }

        private void manageUpdateInsertBestBid(int position, double price, decimal size)
        {
            if (_bestBidPrice == 0 || (position == 0 || price >= _bestBidPrice))
            {
                //Console.WriteLine("OB!: setting bestBid to " + price); // TODO deleteme
                _bestBidPrice = price;
                _bestBidSize = size;

                BestBidAskUpdated?.Invoke(this,
                    new IbBestBidAskUpdatedEventArgs(Symbol, _bestBidPrice, _bestBidSize, _bestAskPrice, _bestAskSize));
            }

            OrderBookSnapshot?.Invoke(this,
                new IbOrderBookSnapshotEventArgs(Symbol, BidPrices, BidSizes, AskPrices, AskSizes));
            // Console.WriteLine("Bidsize() " + BidPrices.Count + "; & " + BidSizes.Count); // TODO deleteme
        }

        private void manageUpdateInsertBestAsk(int position, double price, decimal size)
        {
            if (_bestAskPrice == 0 || (position == 0 || price <= _bestAskPrice))
            {
                //Console.WriteLine("OB!: setting bestAsk to " + price); // TODO deleteme
                _bestAskPrice = price;
                _bestAskSize = size;

                BestBidAskUpdated?.Invoke(this,
                    new IbBestBidAskUpdatedEventArgs(Symbol, _bestBidPrice, _bestBidSize, _bestAskPrice, _bestAskSize));
            }

            OrderBookSnapshot?.Invoke(this,
                new IbOrderBookSnapshotEventArgs(Symbol, BidPrices, BidSizes, AskPrices, AskSizes));
            // Console.WriteLine("asksSAize() " + AskPrices.Count + "; & " + AskSizes.Count); // TODO deleteme
        }

        /// <summary>
        /// Insert bid
        /// </summary>
        /// <param name="position"></param>
        public void InsertBid(int position, double price, decimal size)
        {
            lock (_locker)
            {
                // TODO: which checks need to be done?
                // if (Bids.Count > position)
                // {
                BidPrices.Insert(position, price);
                BidSizes.Insert(position, size);
                manageUpdateInsertBestBid(position, price, size);
                // }
            }
        }

        /// <summary>
        /// Insert ask
        /// </summary>
        /// <param name="position"></param>
        public void InsertAsk(int position, double price, decimal size)
        {
            lock (_locker)
            {
                // TODO: which checks need to be done?
                // if (Asks.Count > position)
                // {
                AskPrices.Insert(position, price);
                AskSizes.Insert(position, size);
                manageUpdateInsertBestAsk(position, price, size);
                // }
            }
        }


        /// <summary>
        /// Update bid
        /// </summary>
        /// <param name="position"></param>
        public void UpdateBid(int position, double price, decimal size)
        {
            lock (_locker)
            {
                // TODO: which checks need to be done?
                // if (BidPrices.Count > position)
                // {
                //     Bids.RemoveAt(position);
                // }
                //
                BidPrices[position] = price;
                BidSizes[position] = size;
                manageUpdateInsertBestBid(position, price, size);
            }
        }

        /// <summary>
        /// Update ask
        /// </summary>
        /// <param name="position"></param>
        public void UpdateAsk(int position, double price, decimal size)
        {
            lock (_locker)
            {
                // TODO: which checks need to be done?
                // if (AskPrices.Count > position)
                // {
                //     Asks.RemoveAt(position);
                // }
                
                AskPrices[position] = price;
                AskSizes[position] = size;
                manageUpdateInsertBestAsk(position, price, size);
            }
        }

        /// <summary>
        /// Remove ask at position/row
        /// </summary>
        /// <param name="position"></param>
        public void RemoveAsk(int position)
        {
            lock (_locker)
            {
                if (AskPrices.Count > position)
                {
                    double price = AskPrices[position];
                    AskPrices.RemoveAt(position);
                    AskSizes.RemoveAt(position);
                    TapeAskSizes.Remove(price);  // TODO: correct, do we just remove all the same?

                    if (price == _bestAskPrice)
                    {
                        if (AskPrices.Count == 0)
                        {
                            _bestAskPrice = 0;
                            _bestAskSize = 0;
                        }
                        else
                        {
                            _bestAskPrice = AskPrices[0];
                            _bestAskSize = AskSizes[0];
                        }

                        BestBidAskUpdated?.Invoke(this,
                            new IbBestBidAskUpdatedEventArgs(Symbol, _bestBidPrice, _bestBidSize, _bestAskPrice,
                                _bestAskSize));
                    }

                    OrderBookSnapshot?.Invoke(this,
                        new IbOrderBookSnapshotEventArgs(Symbol, BidPrices, BidSizes, AskPrices, AskSizes));
                }
            }
        }

        /// <summary>
        /// Remove bid at position/row
        /// </summary>
        /// <param name="position"></param>
        public void RemoveBid(int position)
        {
            lock (_locker)
            {
                if (BidPrices.Count > position)
                {
                    double price = BidPrices[position];
                    BidPrices.RemoveAt(position);
                    BidSizes.RemoveAt(position);
                    TapeBidSizes.Remove(price);  // TODO: correct, do we just remove all the same?

                    if (price == _bestBidPrice)
                    {
                        if (BidPrices.Count == 0)
                        {
                            _bestBidPrice = 0;
                            _bestBidSize = 0;
                        }
                        else
                        {
                            _bestBidPrice = BidPrices[0];
                            _bestBidSize = BidSizes[0];
                        }

                        BestBidAskUpdated?.Invoke(this,
                            new IbBestBidAskUpdatedEventArgs(Symbol, _bestBidPrice, _bestBidSize, _bestAskPrice,
                                _bestAskSize));
                    }

                    OrderBookSnapshot?.Invoke(this,
                        new IbOrderBookSnapshotEventArgs(Symbol, BidPrices, BidSizes, AskPrices, AskSizes));
                }
            }
        }

        public OrderBook ToOrderBookSnapshot(DateTime time)
        {
            // first clean up tapeSizes of prices not tracked by the book at the time of taking the snapshot;
            // this is also useful for general house-keeping:
            // TODO: this however leaves out bunch of tape data it consolidated OrderBookBars, no?
            TapeBidSizes.Keys.Except(BidPrices).ToList()
                .ForEach(key => TapeBidSizes.Remove(key));
            TapeAskSizes.Keys.Except(AskPrices).ToList()
                .ForEach(key => TapeAskSizes.Remove(key));

            // TODO: do we need to lock here?:
            return new OrderBook(
                Symbol, time, BidPrices, BidSizes, AskPrices,
                AskSizes, TapeBidSizes, TapeAskSizes);
        }

        private void UpdateTradedSizeForPrice(Tape tape, Dictionary<double, decimal> priceToSizeMap)
        {
            priceToSizeMap.TryGetValue(tape.Price, out var currentSize);
            priceToSizeMap[tape.Price] = currentSize + tape.Size;
        }

        // TODO: is locking needed here or not?
        public void UpdateForTape(Tape tape)
        {
            switch (tape.Side)
            {
                case Side.Green or Side.Ask:
                    UpdateTradedSizeForPrice(tape, TapeAskSizes);
                    break;
                case Side.Red or Side.Bid:
                    UpdateTradedSizeForPrice(tape, TapeBidSizes);
                    break;
                // case Side.Middle:  // TODO: skip middle?
                    // break;
            }
        }
    }
}
