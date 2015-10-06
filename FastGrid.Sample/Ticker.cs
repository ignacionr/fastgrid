using System;

namespace FastGrid.Sample
{
	public class Ticker
	{
		public string pair {get;set;}
		public string currency { get; set; }
		public decimal ask {get;set;}
		public decimal bid {get;set;}

		public override int GetHashCode ()
		{
			return (pair == null ? 0 : pair.GetHashCode ()) ^
				(currency == null ? 0 : currency.GetHashCode ()) ^
				ask.GetHashCode () ^
				bid.GetHashCode ();
		}
	}
}

