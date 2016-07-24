using System;
namespace SRPViewer
{
	public class OTPParameter
	{
		public string Algorithm { get; set; }
		public long Counter { get; set; }
		public int Session { get; set; }
	}

	public interface IOTPLibrary
	{
		string getOTPQuery(OTPParameter pamameter, string imageMatrix);
	}
}

