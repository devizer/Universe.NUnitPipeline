using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Universe.NUnitPipeline.ConsoleTreeTable;

namespace Universe.NUnitPipeline
{
	// Internal?
	public class DetailsReport
	{
		public TreeKey Key { get; set; }
		public string OutcomeStatus { get; set; }
		public double Duration { get; set; }
		public double? UserTime { get; set; }
		public double? KernelTime { get; set; }
		public string ErrorMessage { get; set; }

	}

	public class DetailsReportStorage
	{
		public static readonly DetailsReportStorage Instance = new DetailsReportStorage();
		private List<DetailsReport> PlainList = new List<DetailsReport>();
		private readonly object Sync = new object();

		public void EnlistRow(DetailsReport row)
		{
			lock(Sync) PlainList.Add(row);
		}

		public List<DetailsReport> GetRawRows()
		{
			lock (Sync) return new List<DetailsReport>(PlainList);
		}
	}
}
