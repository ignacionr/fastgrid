using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Linq;

namespace FastGrid.Sample
{
	public class TickerProvider
	{
		IList<Ticker> _data= null;

		public void QueryBitReserve() {
			using (var wc = new WebClient ()) {
				for (;;) {
					var contents = wc.DownloadString ("https://api.bitreserve.org/v0/ticker");
					this._data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Ticker>> (contents);
					Thread.Sleep (5000);
				}
			}
		}

		public TickerProvider() {
			var th = new Thread (QueryBitReserve);
			th.Start ();
		}

		public IEnumerable<Ticker> GetTickers() {
			return _data ?? Enumerable.Repeat(new Ticker(), 0);
		}
	}
}

