using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Queerie
{
	public struct NonQueryResult
	{
		public readonly int RowsAffected;
		public readonly long LastInsertedId;

		internal NonQueryResult(int rowsAffected, long lastInsertedId)
		{
			RowsAffected = rowsAffected;
			LastInsertedId = lastInsertedId;
		}
	}
}
