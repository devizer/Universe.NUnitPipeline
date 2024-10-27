using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universe.NUnitPipeline.Shared
{
	internal class FileEx
	{
		public static void WriteAll(string fileName, string content)
		{
			TryAndForget.Execute(() => Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(fileName))));
			using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
			using (StreamWriter wr = new StreamWriter(fs, new UTF8Encoding(false)))
			{
				wr.Write(content);
			}

		}
	}
}
