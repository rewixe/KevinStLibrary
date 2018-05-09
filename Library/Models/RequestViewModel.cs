using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Library.Models
{
	public class RequestViewModel
	{
		public int CustID { get; set; }
		public long Isbn { get; set; }
		public string Name { get; set; }
		public int Year { get; set; }
		public string Subject { get; set; }
		public string Type { get; set; }
		public string AuthName { get; set; }
		public string ReqConfirmation { get; set; }
	}

}