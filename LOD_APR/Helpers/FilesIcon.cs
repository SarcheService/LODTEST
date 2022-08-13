using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LOD_APR.Helpers
{
	public static class FilesIcon
	{
		public static string fileIcon(string extension) {

			if (extension == ".doc" || extension == ".docx") {
				return "fa-file-word-o";
			}else if (extension == ".xls" || extension == ".xlsx")
			{
				return "fa-file-excel-o";
			}
			else if (extension == ".ppt" || extension == ".pptx")
			{
				return "fa-file-powerpoint-o";
			}
			else if (extension == ".txt")
			{
				return "fa-file-text-o";
			}
			else if (extension == ".bmp" || extension == ".gif" || extension == ".jpeg" || extension == ".jpg" || extension == ".png")
			{
				return "fa-file-image-o";
			}
			else if (extension == ".gz" || extension == ".gzip" || extension == ".iso" || extension == ".rar" || extension == ".tar" || extension == ".tgz" || extension == ".zip")
			{
				return "fa-file-archive-o";
			}
			else if (extension == ".pdf")
			{
				return "fa-file-pdf-o";
			}
			else if (extension == ".sql")
			{
				return "fa-file-code-o";
			}
			else
			{
				return "fa-file-o";
			}
		}
	}
}