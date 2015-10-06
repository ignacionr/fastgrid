using System;
using System.Windows.Forms;

namespace FastGrid.Sample
{
	public class MainForm: Form
	{
		FastGrid.Control.Grid _grid;

		public MainForm ()
		{
			_grid = new FastGrid.Control.Grid ();
			_grid.Dock = DockStyle.Fill;

			var provider = new TickerProvider ();
			var table = _grid.CreateTable (provider.GetTickers);
			table.AddColumn ("Pair", (Ticker t) => string.Format("{0}-{1}", t.pair.Substring(0,3), t.pair.Substring(3)));
			table.AddColumn ("Ask", (Ticker t) => string.Format ("{0:000.0000} {1}", t.ask, t.currency));
			table.AddColumn ("Bid", (Ticker t) => string.Format ("{0:000.0000} {1}", t.bid, t.currency));
			table.Sampler = (Ticker t) => (t.bid + t.ask) / 2M;
			this.Controls.Add (_grid);
		}
	}
}

