using System;
using System.Runtime.InteropServices;
using Xamarin.Forms;

[assembly: Dependency(typeof(SRPViewer.iOS.OTPLibrary))]

namespace SRPViewer.iOS
{
	public class OTPLibrary : IOTPLibrary
	{
		[DllImport("__Internal", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr getRequestQuery(string alg, long counter, int session, string imageKey);

		public string getOTPQuery(OTPParameter parameter, string imageMatrix)
		{
			IntPtr pStr = getRequestQuery(parameter.Algorithm,
										  parameter.Counter,
										  parameter.Session,
										  imageMatrix);
			var str = Marshal.PtrToStringAuto(pStr);
			Marshal.FreeHGlobal(pStr);
			return str;
		}
	}
}
