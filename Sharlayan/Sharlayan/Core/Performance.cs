// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CurrentPlayer.cs" company="SyndicatedLife">
//   Copyright(c) 2018 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (http://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   CurrentPlayer.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Core {
	using System.Collections.Generic;

	using Sharlayan.Core.Enums;
	using Sharlayan.Core.Interfaces;
	using Sharlayan.Extensions;

	public class PerformanceItem : IPerformance {
		
		public float Animation { get; set; }

		public byte Unknown1 { get; set; }

		public byte Id { get; set; }

		public byte Variant { get; set; }

		public byte Type { get; set; }

		public Performance.Status Status { get; set; }

		public Performance.Instrument Instrument { get; set; }

		public short Unknown2 { get; set; }

		public bool IsReady() {
			return (Status >= Performance.Status.Opened);
		}

		public bool IsSimpleInstrument() {
			return ((Type & 0x1) == 1);
		}

		public bool IsWindInstrument() {
			return (
				Instrument == Performance.Instrument.Flute ||
				Instrument == Performance.Instrument.Oboe ||
				Instrument == Performance.Instrument.Clarinet ||
				Instrument == Performance.Instrument.Fife ||
				Instrument == Performance.Instrument.Panpipes
				);
		}
	}
}