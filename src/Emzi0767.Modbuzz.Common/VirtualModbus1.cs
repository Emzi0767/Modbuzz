// This file is part of Modbuzz project
//
// Copyright © 2024 Emzi0767
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Diagnostics;

namespace Emzi0767.Modbuzz.Common;

/// <summary>
/// Represents a virtual Modbus device for testing.
/// </summary>
public sealed class VirtualModbus1
{
    public byte SlaveId => 192;

    private readonly Random _random = new();
    private uint _bigRandom = 0;
    private ushort _last, _current, _counter;
    private bool _lastIncrement, _counterOverflow;
    private long _debounceTimerRandom = 0, _debounceTimerCounter = 0;

    public bool GetRegister(ushort address, out ushort value)
    {
        value = 0;
        switch (address)
        {
            case 30000:
                value = this.RandomValue;
                return true;

            case 30001:
                value = this.Counter;
                return true;

            case 30002:
                value = (ushort)(this.BigRandom & 0xFFFF);
                return true;

            case 30003:
                value = (ushort)(this.BigRandom >> 16);
                return true;

            case 20000:
                value = this.RandomMin;
                return true;

            case 20001:
                value = this.RandomMax;
                return true;

            case 20002:
                value = this.RandomRange;
                return true;

            case 20003:
                value = (ushort)(this.TextLongStorage & 0xFFFF);
                return true;

            case 20004:
                value = (ushort)(this.TextLongStorage >> 16);
                return true;

            case 20005:
                value = this.DoubleStorage;
                return true;
        }

        return false;
    }

    public bool SetRegister(ushort address, ushort value)
    {
        switch (address)
        {
            case 20000:
                this.RandomMin = value;
                return true;

            case 20001:
                this.RandomMax = value;
                return true;

            case 20002:
                this.RandomRange = value;
                return true;

            case 20003:
                this.TextLongStorage = (this.TextLongStorage & 0xFFFF0000u) | value;
                return true;

            case 20004:
                this.TextLongStorage = (uint)((this.TextLongStorage & 0xFFFFu) | ((uint)value << 16));
                return true;

            case 20005:
                this.DoubleStorage = value;
                return true;
        }

        return false;
    }

    public bool GetCoil(ushort address, out bool value)
    {
        value = false;
        switch (address)
        {
            case 40000:
                value = this.LastRandomIncrement;
                return true;

            case 40001:
                value = this.CounterOverflowed;
                return true;

            case 10000:
                value = this.EnableRandomness;
                return true;

            case 10001:
                value = this.ModbusLogging;
                return true;
        }

        return false;
    }

    public bool SetCoil(ushort address, bool value)
    {
        switch (address)
        {
            case 10000:
                this.EnableRandomness = value;
                return true;

            case 10001:
                this.ModbusLogging = value;
                return true;
        }

        return false;
    }

    /// <summary>
    /// Gets the random value generated.
    /// </summary>
    // 30000
    public ushort RandomValue
    {
        get
        {
            var now = Stopwatch.GetTimestamp();
            if (now - this._debounceTimerRandom < Stopwatch.Frequency)
                return this._current;

            this._debounceTimerRandom = now;
            this._last = this._current;
            this._bigRandom = (uint)this._random.Next(0, 1_000_000_000);
            if (!this.EnableRandomness)
            {
                return this._current;
            }

            var rngMin = this.RandomMin;
            var rngMax = this.RandomMax;
            if (this.RandomRange > 0)
            {
                rngMin = this.RandomRange > this._last ? ushort.MinValue : (ushort)Math.Max(this._last - this.RandomRange, this.RandomMin);
                rngMax = ushort.MaxValue - this._last < this.RandomRange ? ushort.MaxValue : (ushort)Math.Min(this._last + this.RandomRange, rngMax);
            }

            this._current = (ushort)this._random.Next(rngMin, rngMax);
            this._lastIncrement = this._last < this._current;
            return this._current;
        }
    }

    /// <summary>
    /// Gets the counter value.
    /// </summary>
    // 30001
    public ushort Counter
    {
        get
        {
            var now = Stopwatch.GetTimestamp();
            if (now - this._debounceTimerCounter < Stopwatch.Frequency)
                return this._counter;

            this._debounceTimerCounter = now;
            var last = this._counter++;
            if (this._counter < last)
                this._counterOverflow = true;

            return this._counter;
        }
    }

    /// <summary>
    /// Gets whether the last random number was smaller than the current one.
    /// </summary>
    // 40000
    public bool LastRandomIncrement
        => this._lastIncrement;

    /// <summary>
    /// Gets whether the counter overflowed.
    /// </summary>
    // 40001
    public bool CounterOverflowed
        => this._counterOverflow;

    /// <summary>
    /// Gets whether to enable or disable random number generation.
    /// </summary>
    // 10000
    public bool EnableRandomness { get; set; } = true;

    /// <summary>
    /// Gets or sets the lower bound of random number generator.
    /// </summary>
    // 20000
    public ushort RandomMin { get; set; } = 0;

    /// <summary>
    /// Gets or sets the upper bound of random number generator.
    /// </summary>
    // 20001
    public ushort RandomMax { get; set; } = 100;

    /// <summary>
    /// Gets or sets the random number generation range.
    /// </summary>
    // 20002
    public ushort RandomRange { get; set; } = 0;

    /// <summary>
    /// Gets a big random number.
    /// </summary>
    // 30002
    public uint BigRandom
        => this._bigRandom;

    /// <summary>
    /// Gets or sets the long mapped value.
    /// </summary>
    // 20003, 20004
    public uint TextLongStorage { get; set; } = 1;

    /// <summary>
    /// Gets or sets the stored double value.
    /// </summary>
    // 20005
    public ushort DoubleStorage { get; set; } = 100;

    /// <summary>
    /// Gets or sets whether to enable Modbus logging.
    /// </summary>
    // 10001
    public bool ModbusLogging { get; set; } = false;
}

