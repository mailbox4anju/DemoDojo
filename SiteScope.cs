using System;
using System.Net;
using System.Web.Services.Protocols;
using HP.Csn.Business.KM.Services.TaxonomyService;
using HP.Csn.Business.KM.Services.SearchService;
using HP.Csn.Business.KM.Services.PresentationService;


namespace HP.Csn.Business.KM
{
	/// <summary>
	/// Summary description for SiteScope.
	/// </summary>
	public class SiteScope : ServiceGateway
	{

		/// <summary>
		/// instance constructors, which implement the actions required to
		/// initialize instances of the class
		/// </summary>		
		#region "### Instance Constructors ###"
		
		public SiteScope()
		{
			GetServiceConfig("CSR_CONFIG");
		}

		#endregion

		#region "### Check Service Methods ###"

		private const string DEFAULT_LANGUAGECODE = "EN";

		public bool CheckKMTaxonomyService(int oid, ref int count,ref string errorMsg)
		{
			count = -1;	
			WSPMNode[] nodes = null;
			try
			{
				TaxonomyService taxonomyService = new TaxonomyService();
				taxonomyService.SetConnectionInfo(base.taxonomyUrl, base.userId, base.userPassword, base.timeOut);
				
				HP.Csn.Business.KM.Services.TaxonomyService.WSUserInfo wsUserInfo = new HP.Csn.Business.KM.Services.TaxonomyService.WSUserInfo();
				wsUserInfo.userId = base.userId;
				wsUserInfo.portalId = base.portalId;
				//wsUserInfo.protalId = base.portalId;
			
				WSPMRequest pmRequest = new WSPMRequest();
				pmRequest.oid = oid;
				pmRequest.locale = DEFAULT_LANGUAGECODE;
			
				nodes = taxonomyService.retrievePMSubtree(wsUserInfo,pmRequest);
			}
			catch (SoapException ex)
			{
				errorMsg = "Error While Retrieving Taxonomy : (" + ex.Message + ")";
				return false;
			}
			catch (System.Net.WebException ex)
			{
				errorMsg = "Error Connecting To Webservices : (" + ex.Message + ")";
				return false;
			}
			catch (Exception ex)
			{
				errorMsg = "Unexpected Error While Accessing Webservices. ERROR : (" + ex.Message + ")";
				return false;
			}
		
			count = 0;
			if (nodes != null && nodes.Length != 0)
				count = nodes.Length;
			else
				errorMsg = "Taxonomy Not Available For Given OID :" + oid;
		
			return true;
			
		}
		
		public bool CheckKMSearchService(int oid, int[] docTypes, ref string searchId,ref int count,ref string errorMsg)
		{
			count = -1;
			searchId = "";
			WSSearchResults wsSearchResults = null;
			int [] disclosureLevels = new int[4] {1,3,5,7};
			
			try
			{
				HP.Csn.Business.KM.Services.SearchService.WSUserInfo wsUserInfo = new HP.Csn.Business.KM.Services.SearchService.WSUserInfo();
				wsUserInfo.userId = base.userId;
				wsUserInfo.portalId = base.portalId;
				wsUserInfo.disclosureLevels = disclosureLevels;							
								
				WSSearchRequest wsSearchRequest = new WSSearchRequest();
				wsSearchRequest.query = "Servers";
				wsSearchRequest.searchCriteria = 0;
				wsSearchRequest.maxResults = 100;
				wsSearchRequest.resultsPerPage = 100;
				wsSearchRequest.docTypes = docTypes;
				wsSearchRequest.searchLanguages = new string[]{DEFAULT_LANGUAGECODE}; 
				
				//WSMetaDataQuery[] metaDataQuerys = new WSMetaDataQuery[1]; // Maximum Metadata Query supported is 3
                WSMetaDataQuery[] metaDataQuerys = new WSMetaDataQuery[2]; // By Ajit: Increased the Array to support Original System filter.
				WSMetaDataQuery oidMdq = new WSMetaDataQuery();
				oidMdq.name = "path_oids";
                oidMdq.values = new string[]{"0"};			
				metaDataQuerys[0] = oidMdq;
                //start
                //By Ajit: to filter Original systems from the query while pulling the data from different systems.
                //string[] systems = { "389", "391", "392", "2429", "2288", "2291", "2454" };
                string[] OriginalSystems = base.GetKmOriginalSystem();
                if (OriginalSystems != null)
                {
                    if (OriginalSystems.Length > 0)
                    {
                        WSMetaDataQuery SystemMdq = new WSMetaDataQuery();
                        SystemMdq.name = "original_system";
                        SystemMdq.type = 1;	// 0 = text, 1 = taxo, 2 = date
                        SystemMdq.values = OriginalSystems;
                        metaDataQuerys[1] = SystemMdq;
                    }
                }
                //end

				SearchService searchService = new SearchService();
				searchService.SetConnectionInfo(base.searchUrl, base.userId, base.userPassword, base.timeOut);

				wsSearchResults = searchService.search(wsUserInfo, wsSearchRequest, metaDataQuerys);

			}
			catch (SoapException ex)
			{
				errorMsg = "Error While Searching for Documents. ERROR : (" + ex.Message + ")";
				return false;
			}
			catch (WebException ex)
			{
				errorMsg = "Error Connecting to Webservices. ERROR : (" + ex.Message + ")";
				return false;
			}
			catch (Exception ex)
			{
				errorMsg = "Unexpected Error While Accessing Webservices. ERROR : (" + ex.Message + ")";
				return false;
			}
			
			count = 0;
			if (wsSearchResults == null)
				errorMsg = "No Documents Found For Given Search Criteria";
			else
				searchId = wsSearchResults.searchId;
			
			WSSearchResult[] wsResult = wsSearchResults.results;
			if (wsResult == null || wsResult.Length == 0)
				errorMsg = "No Documents Found For Given Search Criteria";
			else
				count = wsResult.Length;

			return true;
		}
		
		public bool CheckKMPresentationService(string docId,ref int size, ref string errorMsg)
		{
			size = -1;
			WSDocument wsDocument = null;
			try
			{
				HP.Csn.Business.KM.Services.PresentationService.WSUserInfo wsUserInfo = new HP.Csn.Business.KM.Services.PresentationService.WSUserInfo();
				wsUserInfo.userId = base.userId;
				wsUserInfo.portalId = base.portalId;

				WSCPRequest wsCPRequest = new WSCPRequest();
				wsCPRequest.docId = docId;
				wsCPRequest.locale = DEFAULT_LANGUAGECODE;
			
				PresentationService presentationService = new PresentationService();
				presentationService.SetConnectionInfo(base.presentationUrl, base.userId, base.userPassword, base.timeOut);

				wsDocument = presentationService.retrieveDocument(wsUserInfo, wsCPRequest);
			}
			catch (SoapException ex)
			{
				errorMsg = "Error While Retrieving Document. ERROR : (" + ex.Message + ")";
				return false;
			}
			catch (WebException ex)
			{
				errorMsg = "Error Connecting To Webservices. ERROR : (" + ex.Message + ")";
				return false;
			}
			catch (Exception ex)
			{
				errorMsg = "Unexpected Error While Accessing Webservices. ERROR : (" + ex.Message + ")";
				return false;
			}

			
			if (wsDocument == null)
				errorMsg = "Unable To Retrieve Document " + docId;

			if (wsDocument.payload.Length == 0)
			{
				size = 0;
				errorMsg = "Error While Retrieving Document. Payload Is Empty";
			}
			else
				size = wsDocument.payload.Length;
			
			return true;
		}

		
		#endregion

		#region "### Connection Info Methods ###"
		
		public string GetTaxonomyServiceConnectionInfo()
		{
			return base.taxonomyUrl;
		}
		public string GetSearchServiceConnectionInfo()
		{
			return base.searchUrl;
		}
		public string GetPresentationServiceConnectionInfo()
		{
			return base.presentationUrl;
		}
		
		#endregion
		
	}
}
