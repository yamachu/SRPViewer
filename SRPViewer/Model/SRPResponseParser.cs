using System;
using System.IO;
using System.Xml.Linq;
using System.Linq;
using CenterCLR.Sgml;
using System.Text.RegularExpressions;
using System.Text;

namespace SRPViewer
{
	public class SRPResponseParser
	{
		public static XDocument StreamToXDocument(Stream stream, Encoding enc)
		{
			using (var sr = new StreamReader(stream, enc))
			{
				using (var sgmlReader = new SgmlReader { DocType = "HTML", CaseFolding = CaseFolding.ToLower })
				{
					sgmlReader.InputStream = sr;
					return XDocument.Load(sgmlReader);
				}
			}
		}

		public static bool IsCorrectPassword(XDocument xml)
		{
			return !xml.Descendants("a").Select(e => e.Value).Contains("再認証");
		}

		public static bool IsLoginPage(XDocument xml)
		{
			try
			{
				return xml.Descendants("title").First().Value == "Secure Reverse Proxy";
			}
			catch
			{
				return false;
			}
		}

		public static bool IsIdPassPage(XDocument xml)
		{
			var existForm = xml.Descendants("form").Where(e => e.Attribute("name").Value == "UIDFORM").Count();
			return existForm == 1;
		}

		public static OTPParameter GetOTPParameter(XDocument xml)
		{
			var scriptContainer = xml.Descendants("script").Where(e => e.FirstAttribute.Name == "type").First();
			var script = scriptContainer.FirstNode.ToString();

			return new OTPParameter
			{
				Algorithm = GetParameterInScript(script, "OTP_algo").Replace("\"",""),
				Counter = long.Parse(GetParameterInScript(script, "OTP_counter")),
				Session = int.Parse(GetParameterInScript(script, "OTP_session")),
			};
		}

		private static string GetParameterInScript(string body, string target)
		{
			var reg = new Regex("var\\s+"+target+"\\s*=\\s*(\\S+);");
			return reg.Match(body).Groups[1].ToString();
		}

	}
}

