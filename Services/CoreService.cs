using System;
using System.Net;
using System.Web.Services.Protocols;

namespace HP.Csn.Business.KM.Services
{
	/// <summary>
	/// Summary description for CoreService.
	/// </summary>
	public class CoreService : SoapHttpClientProtocol
	{
		public CoreService()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public void SetConnectionInfo(string serviceUrl, string userId, string userPassword, int nTimeout)
		{
			//
			// TODO: Add constructor logic here
			//
			ICredentials csnCredentials = new NetworkCredential(userId, userPassword);
			base.Credentials = csnCredentials;
			base.Url = serviceUrl;
			base.Timeout = nTimeout;
		}
		

	}
}
