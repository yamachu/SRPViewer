﻿using System;
using System.Runtime.InteropServices;
using Xamarin.Forms;

[assembly: Dependency(typeof(SRPViewer.Droid.OTPLibrary))]

namespace SRPViewer.Droid
{
	public class OTPLibrary : IOTPLibrary
	{
		[DllImport("libotp.so", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
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