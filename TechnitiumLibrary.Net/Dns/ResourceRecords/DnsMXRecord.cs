﻿/*
Technitium Library
Copyright (C) 2021  Shreyas Zare (shreyas@technitium.com)

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.

*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;

namespace TechnitiumLibrary.Net.Dns.ResourceRecords
{
    public class DnsMXRecord : DnsResourceRecordData, IComparable<DnsMXRecord>
    {
        #region variables

        ushort _preference;
        string _exchange;

        #endregion

        #region constructor

        public DnsMXRecord(ushort preference, string exchange)
        {
            DnsClient.IsDomainNameValid(exchange, true);

            _preference = preference;
            _exchange = exchange;
        }

        public DnsMXRecord(Stream s)
            : base(s)
        { }

        public DnsMXRecord(dynamic jsonResourceRecord)
        {
            _rdLength = Convert.ToUInt16(jsonResourceRecord.data.Value.Length);

            string[] parts = (jsonResourceRecord.data.Value as string).Split(' ');

            _preference = ushort.Parse(parts[0]);
            _exchange = parts[1].TrimEnd('.');
        }

        #endregion

        #region protected

        protected override void Parse(Stream s)
        {
            _preference = DnsDatagram.ReadUInt16NetworkOrder(s);
            _exchange = DnsDatagram.DeserializeDomainName(s);
        }

        protected override void WriteRecordData(Stream s, List<DnsDomainOffset> domainEntries)
        {
            DnsDatagram.WriteUInt16NetworkOrder(_preference, s);
            DnsDatagram.SerializeDomainName(_exchange, s, domainEntries);
        }

        #endregion

        #region internal

        internal override void NormalizeName()
        {
            _exchange = _exchange.ToLower();
        }

        #endregion

        #region public

        public int CompareTo(DnsMXRecord other)
        {
            return _preference.CompareTo(other._preference);
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj is DnsMXRecord other)
                return _exchange.Equals(other._exchange, StringComparison.OrdinalIgnoreCase);

            return false;
        }

        public override int GetHashCode()
        {
            return _exchange.GetHashCode();
        }

        public override string ToString()
        {
            return _preference + " " + _exchange.ToLower() + ".";
        }

        #endregion

        #region properties

        public ushort Preference
        { get { return _preference; } }

        public string Exchange
        { get { return _exchange; } }

        [IgnoreDataMember]
        public override ushort UncompressedLength
        { get { return Convert.ToUInt16(2 + _exchange.Length + 2); } }

        #endregion
    }
}
