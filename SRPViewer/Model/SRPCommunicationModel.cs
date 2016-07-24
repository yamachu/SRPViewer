using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using System.IO;
using System.Xml.Linq;
using Xamarin.Forms;

namespace SRPViewer
{
	public class SRPCommunicationModel
	{
		public enum LoginStatus
		{
			IncorrectIdPass,
			IncorrectMatrix,
			ReachPortal,
			GoIdPass,
			GoMatrix,
		};

		/// <summary>
		/// Do login.
		/// </summary>
		/// <returns>LoginStatus: ReachPortal, IncorrectIdPass, IncorrectMatrix</returns>
		/// <param name="id">Identifier.</param>
		/// <param name="pass">Pass.</param>
		/// <param name="matrix">Matrix.</param>
		public Task<LoginStatus> DoLogin(string id, string pass, string matrix)
		{
			Tuple<LoginStatus, OTPParameter> result;

			result = CheckCurrentSessionPage().Result;

			if (result.Item1 == LoginStatus.ReachPortal)
				return Task.FromResult(result.Item1);

			if (result.Item1 == LoginStatus.GoIdPass)
			{
				result = LoginIdAndPasswordPhase(id, pass).Result;
				if (result.Item1 == LoginStatus.IncorrectIdPass)
					return Task.FromResult(LoginStatus.IncorrectIdPass);

				if (result.Item1 == LoginStatus.ReachPortal)
					return Task.FromResult(LoginStatus.ReachPortal);
			}

			var fin = LoginMatrixPhase(result.Item2, matrix).Result;

			return Task.FromResult(fin);
		}

		private Task<Tuple<LoginStatus, OTPParameter>> CheckCurrentSessionPage()
		{
			using (var handler = new HttpClientHandler()
			{
				CookieContainer = HttpClientHelper.SessionCookie,
				UseCookies = true,
			})
			using (var client = new HttpClient(handler)
			{
				BaseAddress = HttpClientHelper.BaseAddress,
			})
			{
				client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
				var result = client.GetAsync("").Result;
				var body = result.Content.ReadAsStreamAsync().Result;
				var xml = SRPResponseParser.StreamToXDocument(body, HttpClientHelper.EncodingEuc);

				if (!SRPResponseParser.IsLoginPage(xml))
					return Task.FromResult(new Tuple<LoginStatus, OTPParameter>(LoginStatus.ReachPortal, null));
				
				if (SRPResponseParser.IsIdPassPage(xml))
					return Task.FromResult(new Tuple<LoginStatus, OTPParameter>(LoginStatus.GoIdPass, null));
				
				var param = SRPResponseParser.GetOTPParameter(xml);

				return Task.FromResult(new Tuple<LoginStatus, OTPParameter>(LoginStatus.GoMatrix, param));
			}
		}

		private Task<Tuple<LoginStatus, OTPParameter>> LoginIdAndPasswordPhase(string id, string pass)
		{
			using (var handler = new HttpClientHandler()
			{
				CookieContainer = HttpClientHelper.SessionCookie,
				UseCookies = true,
			})
			using (var client = new HttpClient(handler)
			{
				BaseAddress = HttpClientHelper.BaseAddress,
			})
			{
				var content = new FormUrlEncodedContent(
					new Dictionary<string, string>
				{
					{ "twuser", id},
					{ "twpassword", pass},
				});
				client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
				var result = client.PostAsync("", content).Result;
				var body = result.Content.ReadAsStreamAsync().Result;
				var xml = SRPResponseParser.StreamToXDocument(body, HttpClientHelper.EncodingEuc);
				if (!SRPResponseParser.IsCorrectPassword(xml))
					return Task.FromResult(new Tuple<LoginStatus, OTPParameter>(LoginStatus.IncorrectIdPass, null));

				if (!SRPResponseParser.IsLoginPage(xml))
					return Task.FromResult(new Tuple<LoginStatus, OTPParameter>(LoginStatus.ReachPortal, null));

				var param = SRPResponseParser.GetOTPParameter(xml);

				return Task.FromResult(new Tuple<LoginStatus, OTPParameter>(LoginStatus.GoMatrix, param));
			}
		}

		private Task<LoginStatus> LoginMatrixPhase(OTPParameter otpParam, string matrix)
		{
			using (var handler = new HttpClientHandler()
			{
				CookieContainer = HttpClientHelper.SessionCookie,
				UseCookies = true,
				AllowAutoRedirect = true,
			})
			using (var client = new HttpClient(handler)
			{
				BaseAddress = HttpClientHelper.BaseAddress,
			})
			{
				var otpCode = DependencyService.Get<IOTPLibrary>().getOTPQuery(otpParam, matrix);
				otpCode = otpCode.Replace(' ', '+');
				client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
				var path = "/?candr=" + otpCode;
				var result = client.GetAsync(path).Result;
				var body = result.Content.ReadAsStreamAsync().Result;
				var xml = SRPResponseParser.StreamToXDocument(body, HttpClientHelper.EncodingEuc);
				var headers = result.Headers.GetValues("X-TW-AUTH-RESULT");
				foreach (var val in headers)
				{
					if (val == "OK")
						return Task.FromResult(LoginStatus.ReachPortal);	
				}

				return Task.FromResult(LoginStatus.IncorrectMatrix);


			}

		}

	}
}

