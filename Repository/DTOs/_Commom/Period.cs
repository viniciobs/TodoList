using Domains.Exceptions;
using System;

namespace Repository.DTOs._Commom
{
	public class Period
	{		
		public DateTime? Start { get; private set; }
		public DateTime? End { get; private set; }
		public bool HasValue => Start.HasValue || End.HasValue;

		public Period(DateTime? start, DateTime? end)
		{
			if (start > end) throw new RuleException("Start date can't be higher than end date");

			Start = start;
			End = end;
		}

		public bool IsBetween(DateTime date)
		{
			if (Start == null)
				return End > date;
			else if (End == null)
				return date > Start;
			else
				return date > Start && date < End;
		}
	}
}
