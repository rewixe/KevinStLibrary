using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Library.Models
{
	public class CopyItemTransacViewModel
	{
		public string Name { get; set; }
		public Nullable<System.DateTime> Borrow_Date { get; set; }
		public Nullable<System.DateTime> Return_Date { get; set; }
		public long Isbn { get; set; }
		public int cuID { get; set; }
	}
}