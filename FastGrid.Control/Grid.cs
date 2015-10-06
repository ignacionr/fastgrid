using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using System.Linq;

namespace FastGrid.Control
{
	public class Grid: System.Windows.Forms.Control
	{
		const int ROW_HEIGHT = 18;

		IGridTable _table;
		Bitmap _contents = new Bitmap(1,1);
		ManualResetEventSlim _stopRefreshing = new ManualResetEventSlim(false);

		void RefreshSource() {
			TableSnapshot oldSnapshot = null;
			Dictionary<int,List<decimal>> samples = new Dictionary<int, List<decimal>> ();
			while (!_stopRefreshing.Wait(5000)) 
			{
				var snapshot = _table.GetSnapShot (oldSnapshot);
				if (snapshot._rowValues.Count > 0) 
				{
					var bmp = new Bitmap (_table.Columns.Count * 100 + 100, snapshot._rowValues.Count * ROW_HEIGHT);
					using (var dc = Graphics.FromImage (bmp))
					using (var font = new Font (FontFamily.GenericMonospace, 9f)) 
					{
						int y = 0;
						foreach (var row in snapshot._rowValues) {
							int x = 0;
							dc.FillRectangle (
								(snapshot._modifiedRows.Contains (y) ? Brushes.HotPink : Brushes.LightGray),
								0, y * ROW_HEIGHT, 100 * _table.Columns.Count, ROW_HEIGHT);
							foreach (var col in _table.Columns) {
								dc.DrawString (col.GetValue (row), 
									font, Brushes.Black, 
									x * 100, y * ROW_HEIGHT);
								x++;
							}
							// draw samples
							List<decimal> rowSamples;
							if (!samples.TryGetValue (y, out rowSamples)) {
								rowSamples = new List<decimal> ();
								samples [y] = rowSamples;
							}
							rowSamples.Add (_table.Sample (row));
							if (rowSamples.Count > 10)
								rowSamples.RemoveRange (0, rowSamples.Count - 10);
							var max = rowSamples.Max ();
							var min = rowSamples.Min ();
							if (max != min) {
								var from = new Point (x * 100, (int)(y * ROW_HEIGHT + ROW_HEIGHT - (rowSamples [0] - min) * (ROW_HEIGHT / (max - min))));
								for (int x1 = 1; x1 < rowSamples.Count; x1++) {
									var to = new Point (x * 100 + x1 * 10, (int)(y * ROW_HEIGHT + ROW_HEIGHT - (rowSamples [x1] - min) * (ROW_HEIGHT / (max - min))));
									dc.DrawLine (Pens.DarkRed, from, to);
									from = to;
								}
							}
							y++;
						}
					}
					var oldbmp = _contents;
					_contents = bmp;
					oldbmp.Dispose ();
					this.Invoke (new Action (() => this.Invalidate ()));
					oldSnapshot = snapshot;
				}
			}
		}

		public Grid() {
			var th = new Thread (RefreshSource);
			th.Start ();
		}

		protected override void OnPaint (PaintEventArgs e)
		{
			e.Graphics.DrawImageUnscaled (_contents, 0, 0);
		}

		public GridTable<T> CreateTable<T>(Func<IEnumerable<T>> getter) {
			var ret = new GridTable<T> {
				RowProvider = getter
			};
			this._table = ret;
			return ret;
		}

		protected override void DestroyHandle ()
		{
			_stopRefreshing.Set ();
			Thread.Sleep (100);
			base.DestroyHandle ();
		}
	}
}

