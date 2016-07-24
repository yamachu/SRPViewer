using System;
using System.Net;
using System.Text;

namespace SRPViewer
{
	public sealed class HttpClientHelper
	{
		private static HttpClientHelper _instance_ = new HttpClientHelper();
		private CookieContainer mCookieContaier;
		private readonly Uri sBaseAddress;
		private readonly Encoding sEncodingEuc;
		private readonly Encoding sEncodingSjis;

		public static HttpClientHelper Instance
		{
			get
			{
				return _instance_;
			}
		}

		public static CookieContainer SessionCookie
		{
			get 
			{
				return _instance_.mCookieContaier;
			}
		}

		public static Uri BaseAddress
		{
			get
			{
				return _instance_.sBaseAddress;
			}
		}

		public static Encoding EncodingEuc
		{
			get
			{
				return _instance_.sEncodingEuc;
			}
		}

		public static Encoding EncodingSjis
		{
			get
			{
				return _instance_.sEncodingSjis;
			}
		}

		private HttpClientHelper()
		{
			mCookieContaier = new CookieContainer();
			sBaseAddress = new Uri("https://www.srp.tohoku.ac.jp");
			sEncodingEuc = Portable.Text.Encoding.GetEncoding("euc_jp");
			sEncodingSjis = Portable.Text.Encoding.GetEncoding("shift_jis");
		}

	}
}

