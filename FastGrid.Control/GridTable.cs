using System;
using System.Linq;
using System.Collections.Generic;

namespace FastGrid.Control
{
	public interface IGridColumn {
		string Title { get; }
		string GetValue (object target);
	}

	public interface IGridTable {
		TableSnapshot GetSnapShot(TableSnapshot from = null);
		IList<IGridColumn> Columns {get;}
		decimal Sample (object o);
	}

	public class GridColumn<T> :IGridColumn
	{
		public string Title{get;set;}
		public Func<T,string> Extractor {get;set;}

		public string GetValue(object target) {
			return Extractor ((T)target);
		}
	}

	public class GridTable<T>: IGridTable
	{
		List<IGridColumn> _columns = new List<IGridColumn>();

		public Func<IEnumerable<T>> RowProvider { get; set; }
		public Func<T,decimal> Sampler {get;set;}

		public GridColumn<T> AddColumn(string title, Func<T,string> extractor) {
			var col = new GridColumn<T>{
				Title = title,
				Extractor = extractor
			};
			_columns.Add (col);
			return col;
		}

		public TableSnapshot GetSnapShot(TableSnapshot from = null) {
			var result = new TableSnapshot {
				_rowValues = RowProvider ().Cast<object> ().ToList ()
			};
			var range = Enumerable.Range (0, result._rowValues.Count);
			if (from == null) {
				result._modifiedRows = new HashSet<int> (range);
			} else {
				result._modifiedRows = new HashSet<int> (range.Where (idx => idx >= from._rowValues.Count || from._rowValues [idx].GetHashCode () != result._rowValues [idx].GetHashCode ()));
			}
			return result;
		}

		public IList<IGridColumn> Columns {
			get { return _columns; }
		}
		public decimal Sample(object target) {
			return Sampler == null ? 0M : Sampler ((T)target);
		}
	}
}

