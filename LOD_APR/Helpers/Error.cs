using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LOD_APR.Helpers
{
	public class Status_Error
	{
		public int ErrorCode { get; set; }
		public string ErrorMnsg { get; set; }
		public string Status { get; set; }
		public bool Error { get; set; }
	}
	public class Post_Status
	{
		public string Mnsg { get; set; }
		public bool Error { get; set; }
	}
}