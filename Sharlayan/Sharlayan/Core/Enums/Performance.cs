// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Performance.cs" company="SyndicatedLife">
//   Copyright(c) 2018 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (http://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Performance.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Core.Enums {
	public class Performance {

		public enum Status : byte {
			Closed = 0,
			Loading = 1,
			Opened = 2,
			SwitchingNote = 3,
			HoldingNote = 4
		}

		public enum Instrument {
			Harp = 1,
			Piano = 2,
			Lute = 3,
			Fiddle = 4,
			Flute = 5,
			Oboe = 6,
			Clarinet = 7,
			Fife = 8,
			Panpipes = 9,
			Timpani = 10,
			Bongo = 11,
			BassDrum = 12,
			SnareDrum = 13,
			Cymbal = 14,
			Trumpet = 15,
			Trombone = 16,
			Tuba = 17,
			Horn = 18,
			Saxophone = 19,
		}
	}
}
