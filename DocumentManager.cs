/******************************************************************************** 
* Class Name:  DocumentManager 
* Purpose:     Retrive data from CSR2.0 SearchService proxy class and provide it to the presentation layer 
* Comments: 
* Modified:    Megha for Release 8.7 Enhancement.
*			   A new function GetDisclosurelevelID()is added to retun an int array of DisclosurelevelID's
*         	   A new function GetOdsDocumentTypesID()is added to retun an int array of DocumentTypeID's.
*			   Obtain language code from presentation layer and assign it to wsSearchRequest object.
*			   Map Document Type ID to corresponding Document Type description.
*			   Map Disclosure Level ID to corresponding Disclosure Level description.
*
*              Senthil Sathiya for Self Maintainer              
*              A new function GetDocument is added for the SM project , This method formats the input parameter
*              service advisory number and then calls the method RetrieveDocument 
************************************************************************************/
using System;
using System.Data;
using System.Net;
using System.Collections;
using System.Web.Services.Protocols;
using HP.Csn.Business.KM.Services.SearchService;
using HP.Csn.Business.KM.Services.PresentationService;
//ODP.Net 10.7 changes
//using System.Data.OleDb;
using System.Data.SqlClient;
using HP.Sasp.Data;
using HP.Sasp.Logging;
using HP.Csn.DataAccess.KM;
using System.Xml;

namespace HP.Csn.Business.KM
{
	/// <summary>
	/// Summary description for DocumentManager.
	/// </summary>
	public class DocumentManager : ServiceGateway
	{
		
		/// <summary>
		/// constants, which represent constant values associated with the class
		/// </summary>		
		#region "### Constants  ###"
		protected string DEFAULT_LANGUAGE = "en";
		
		#endregion

		/// <summary>
		/// instance constructors, which implement the actions required to
		/// initialize instances of the class
		/// </summary>		
		#region "### Instance Constructors ###"
		
		public DocumentManager()
		{
			GetServiceConfig("CSR_CONFIG");
		}

		#endregion
		
		/// <summary>
		/// methods, which implement the computations and actions that can be
		/// performed by the class  
		/// </summary>
		#region "### Public Methods  ###"

		public DataTable SearchDocuments(string partnerRole, string openTextValue, int productOid, int osOid, int componentOid, int[] docTypes, DateTime releaseDate, ref string searchID,string searchLanguage)
		{
    
			if ((openTextValue == null || openTextValue.Trim() == "" ) && ( productOid == -1 || productOid == 0))
				throw new SearchException(16, SearchException.ERROR_TYPE.INFORMATION, "Please Specify Alteast One Search Criteria : Key Value (or) Taxonomy Id");

			if (docTypes == null || docTypes.Length == 0)
				throw new SearchException(15, SearchException.ERROR_TYPE.INFORMATION, "Please Specify Atleast One Document Type To Search");
			
			try
			{
                OracleUtil objOraUtil  = new OracleUtil();
				// Release 8.7 Enhancement
				int[] disclosureLevels = objOraUtil.GetDisclosurelevel(partnerRole);
				int [] odsDocTypeIDs = null;
				
				HP.Csn.Business.KM.Services.SearchService.WSUserInfo wsUserInfo = new HP.Csn.Business.KM.Services.SearchService.WSUserInfo();
				wsUserInfo.userId = base.userId;
				wsUserInfo.portalId = base.portalId;
				wsUserInfo.disclosureLevels = disclosureLevels;

				// Temp fix : Send disclosure levels to entitlements as expected by CSR
				if (disclosureLevels != null)
				{
					string [] tempDisclosureLevels = new string[disclosureLevels.Length];
					for (int i = 0; i < disclosureLevels.Length; i++)
					{
						tempDisclosureLevels[i] = disclosureLevels[i].ToString();
					}

					// Temp fix
					wsUserInfo.entitlements = tempDisclosureLevels;
				}

				WSSearchRequest wsSearchRequest = new WSSearchRequest();
				wsSearchRequest.query = openTextValue.Trim();
				wsSearchRequest.searchCriteria = 0;
				wsSearchRequest.maxResults = base.max_results;
				wsSearchRequest.resultsPerPage = base.results_per_page; 
				
				if (searchLanguage == null && searchLanguage == "")
						searchLanguage = DEFAULT_LANGUAGE;

				if(searchLanguage != null && searchLanguage != "")
					wsSearchRequest.searchLanguages = new string[]{searchLanguage};
				
				if ( docTypes.Length > 0)
				{
					odsDocTypeIDs = new int[docTypes.Length];
				
				
					OracleUtil obj1 = new OracleUtil();
					odsDocTypeIDs = obj1.GetOdsDocumentTypes(docTypes); 

				}
				wsSearchRequest.docTypes = odsDocTypeIDs;

				

				if (searchID != null && searchID != "")
					wsSearchRequest.searchId = searchID;
			
				WSMetaDataQuery[] tempMdqs = new WSMetaDataQuery[3]; // Maximun Metadata Query supported is 3
				int countMdq = 0;
                //start
                //By Ajit: to filter Original systems from the query while pulling the data from different systems.
                //e.g. -> string[] s = { "389", "391", "392", "2429", "2288", "2291", "2454" };
                string[] OriginalSystems = base.GetKmOriginalSystem();
                if (OriginalSystems != null)
                {
                    if (OriginalSystems.Length > 0)
                    {
                        WSMetaDataQuery SystemMdq = new WSMetaDataQuery();
                        SystemMdq.name = "original_system";//Parameter to filter system
                        SystemMdq.type = 1;	// 0 = text, 1 = taxo, 2 = date
                        SystemMdq.values = OriginalSystems;//array of systems to filter
                        tempMdqs[countMdq++] = SystemMdq;//Making array of Parameter to webservice
                    }
                }
                //end

				// 1. Component metadata search
				if (componentOid != -1 && componentOid != 0)
				{
					WSMetaDataQuery componentMdq = new WSMetaDataQuery();
					componentMdq.name = "main_component";
					componentMdq.values = new string[]{"" + componentOid };
					tempMdqs[countMdq++] = componentMdq;
				}
			
				// 2. Content Update Date metadata search
				if (releaseDate.CompareTo(new DateTime(0))!=0)
				{
					WSMetaDataQuery dateMdq = new WSMetaDataQuery();
					dateMdq.name = "content_update_date";
					dateMdq.values = new string[]{releaseDate.ToString(), DateTime.Now.ToString()};
					tempMdqs[countMdq++] = dateMdq;
				}

				// 3. OID metadata search for Product & OS search
				string[] pathOids = new string[2];
				int countOid = 0;

				if (productOid != -1 && productOid != 0)
					pathOids[countOid++] = "" + productOid;

				if (osOid !=-1 && osOid != 0)
					pathOids[countOid++] = "" + osOid;

				if (countOid != 0)
				{
					WSMetaDataQuery oidMdq = new WSMetaDataQuery();
					oidMdq.name = "path_oids";
					if (countOid == 2)
						oidMdq.values = pathOids;
					else
						oidMdq.values = new string[]{pathOids[0]};
					tempMdqs[countMdq++] = oidMdq;
				}
			
				// copying tempMdqs to metaDataQuerys upto countMdq
                //Adding For Filter By Document_sataus-13.11-Start
                string[] WorkFlowStatuses = objOraUtil.workflowstate();
                if (WorkFlowStatuses != null)
                {
                    WSMetaDataQuery metaDataQuerysPublished = new WSMetaDataQuery();
                    metaDataQuerysPublished.name = "workflow_state";
                    metaDataQuerysPublished.values = WorkFlowStatuses;
                    tempMdqs[countMdq++] = metaDataQuerysPublished;

                }
                //Adding For Filter By Document_sataus-13.11-End   

				WSMetaDataQuery[] metaDataQuerys = null;
				if (countMdq != 0 )
				{
					metaDataQuerys = new WSMetaDataQuery[countMdq];
					for (int iCnt = 0; iCnt < countMdq; iCnt++)
						metaDataQuerys[iCnt] = tempMdqs[iCnt];
				}
   
				WSSearchResults wsSearchResults = null;

				SearchService searchService = new SearchService();
				searchService.SetConnectionInfo(base.searchUrl, base.userId, base.userPassword, base.timeOut);

				wsSearchResults = searchService.search(wsUserInfo, wsSearchRequest, metaDataQuerys);

				if (wsSearchResults == null)
					throw new SearchException(13, SearchException.ERROR_TYPE.INFORMATION, "No Documents Found For Given Search Criteria");
			
				WSSearchResult[] wsResult = wsSearchResults.results;
				if (wsResult == null || wsResult.Length == 0)
					throw new SearchException(14, SearchException.ERROR_TYPE.INFORMATION, "No Documents Found For Given Search Criteria");
				
				searchID = wsSearchRequest.searchId;

				DataTable dtSearchResults = new DataTable();
				dtSearchResults.Columns.Add("DocID");
				dtSearchResults.Columns.Add("Title");
				dtSearchResults.Columns.Add("DocType");
				dtSearchResults.Columns.Add("Size");
				dtSearchResults.Columns.Add("Disclosurelvl");
				dtSearchResults.Columns.Add("LastUpdate", typeof(DateTime));

				for (int iCnt = 0; iCnt < wsResult.Length; iCnt++)
				{
					DataRow row = dtSearchResults.NewRow();
					row["DocID"] = wsResult[iCnt].docId;
					string size = wsResult[iCnt].docSize.ToString();
					Int32 DocSize = Convert.ToInt32(size) / 1000;
					size = "  (" + DocSize + "K)"; 
					string title = System.Text.Encoding.UTF8.GetString(wsResult[iCnt].title);
					row["Title"] = title + size;
					row["DocType"] = wsResult[iCnt].docType.ToString();					
					row["Size"] = wsResult[iCnt].docSize;
					row["Disclosurelvl"] = wsResult[iCnt].disclosureLevel.ToString(); 
					row["LastUpdate"] = wsResult[iCnt].contentUpdateDate;
				
					dtSearchResults.Rows.Add(row);
				}
				return dtSearchResults;
			}


			catch (SoapException ex)
			{
				throw new SearchException(12, SearchException.ERROR_TYPE.INFORMATION, "Error While Searching for Documents. ERROR :" + ex.Message,ex);
			}
			catch (WebException ex)
			{
				throw new SearchException(11, SearchException.ERROR_TYPE.WARNING, "Error Connecting to Webservices. ERROR :" + ex.Message,ex);
			}
			catch (SearchException ex)
			{
				//Catching and throwing zero results found exception 
				throw ex;
			}
			catch (Exception ex)
			{
				throw new SearchException(10, SearchException.ERROR_TYPE.WARNING, "Unexpected Error While Accessing Webservices. ERROR :" + ex.Message,ex);
			}

		}
       
        
        /// <summary>
        ///A Function created for Self Maintainer application to validate the input Service Advisory Number,the document number is retrieved from 
        ///the input service advisory number prefixed with the string “emr_na-“.
        ///Make a cll to RetrieveDocument method in the DocumentManager.cs class. If the method throws an exception then return false
        ///If the method returns a byte array and the length of the array is greater than 0 then the service advisory
        ///number is a valid one, return true
        /// @author Senthil
        ///@Date 07/13/2011
        /// </summary>
        /// <param name="serviceAdvisoryNumber"></param>       
        /// <param name="prefixString"></param>
        /// <param name="language"></param>
        /// <returns>bool</returns>
        public bool GetDocument(string serviceAdvisoryNumber, string prefixString)
        {
            bool isValidDocument= false;           

            if (serviceAdvisoryNumber != null)
            {                                
                char delimiterChar = '-';           // Split on hyphens of the input service advisory number
                string docNumber = "";
                string[] substrings = serviceAdvisoryNumber.Split(delimiterChar);               
               
               for(int index = 0; index < substrings.Length; index++)
                {
                   if (index == 1) {
                        
                        docNumber = (string)substrings.GetValue(index);
                    }
                  
                }                                
                // This method will retrieve the document based on the provided service advisory number 
                string mimeType ="";
               
                    string DocumentID = prefixString + docNumber;
                    byte[] retrieveDocument = null;

                    try
                    {
                        retrieveDocument = RetrieveDocument(DocumentID, ref mimeType, DEFAULT_LANGUAGE);

                     if ((retrieveDocument != null) && (retrieveDocument.Length > 0))
                        {
                            isValidDocument = true;
                        }
                        
                    }
                    catch (Exception exception)
                    {
                                            
                            LogMessage logMessage = new LogMessage(Components.CAS, MessageType.Debug);
                            logMessage.Source = "GetDocument";
                            logMessage.Message = exception.Message;
                            logMessage.MessageKey = serviceAdvisoryNumber;
                            logMessage.Log();
                            throw exception;                           
                        }
                      
                    }
              

            return isValidDocument;
            
        }

		
		public byte[] RetrieveDocument(string DocumentID, ref string mimeType, string Language)
		{

			if (DocumentID == null || DocumentID == "")
				throw new DocumentSearchException(21, DocumentSearchException.ERROR_TYPE.INFORMATION, "Please Specify The Document Id To Search.");
			
			try
			{
				HP.Csn.Business.KM.Services.PresentationService.WSUserInfo wsUserInfo = new HP.Csn.Business.KM.Services.PresentationService.WSUserInfo();
				wsUserInfo.userId = base.userId;
				wsUserInfo.portalId = base.portalId;

				// Disclosure levels supported by CSN
				// Get from database in future
				int[] disclosureLevel = new int[4]{1,3,5,7};
				wsUserInfo.disclosureLevels = disclosureLevel;
				
				// Temp fix : Send disclosure levels to entitlements as expected by CSR
				string[] tempDisclosureLevels = new string[4]{"1","3", "5", "7"};
				wsUserInfo.entitlements = tempDisclosureLevels;

				WSCPRequest wsCPRequest = new WSCPRequest();
				wsCPRequest.docId = DocumentID;
				// Release 8.7 Enhancement
				if (Language != null && Language != "")
					wsCPRequest.locale = Language;
				else
					wsCPRequest.locale = DEFAULT_LANGUAGE;
			
				WSDocument wsDocument = null;

				PresentationService presentationService = new PresentationService();
				presentationService.SetConnectionInfo(base.presentationUrl, base.userId, base.userPassword, base.timeOut);

				wsDocument = presentationService.retrieveDocument(wsUserInfo, wsCPRequest);

                //Sachin Added for 13.11 Release-Start
                string strMetaData = wsDocument.metaData;
                string strDocumentStatus = string.Empty;
                XmlDocument objXmlMetaData = new XmlDocument();
                XmlNode objXmlNode;
                XmlNodeList objXmlNodeList;
                objXmlMetaData.LoadXml(strMetaData);
                objXmlNodeList = objXmlMetaData.GetElementsByTagName("document_status");

                if (objXmlNodeList.Count > 0)
                {
                    objXmlNode = objXmlNodeList.Item(0);
                    strDocumentStatus = objXmlNode.InnerText;
                }
                OracleUtil objOraUtil = new OracleUtil();
                string[] WorkFlowStatuses = objOraUtil.workflowstate();
                if (WorkFlowStatuses != null)
                {

                    bool bFindStatus = false;
                    for (int i = 0; i <= WorkFlowStatuses.Length - 1; i++)
                    {
                        if (strDocumentStatus == WorkFlowStatuses[i].ToString())
                        {
                            bFindStatus = true;
                        }
                    }
                    if (bFindStatus == false)
                    {
                        throw new DocumentSearchException(23, DocumentSearchException.ERROR_TYPE.INFORMATION, "No records found for this document number." + DocumentID);
                    }
                }
                //Sachin Added for 13.11 Release-End


				if (wsDocument == null)
                    throw new DocumentSearchException(23, DocumentSearchException.ERROR_TYPE.INFORMATION, "Unable To Retrieve Document " + DocumentID);

				if (wsDocument.payload.Length == 0)
					throw new DocumentSearchException(24, DocumentSearchException.ERROR_TYPE.INFORMATION, "Error While Retrieving Document. Payload Is Empty");
				
				mimeType = wsDocument.mimetype;

				return wsDocument.payload;
			}
			catch (SoapException ex)
			{
				throw new DocumentSearchException(25, DocumentSearchException.ERROR_TYPE.INFORMATION, "Error While Retrieving Document. ERROR :" + ex.Message,ex);
			}
			catch (WebException ex)
			{
				throw new DocumentSearchException(26, DocumentSearchException.ERROR_TYPE.WARNING, "Error Connecting To Web Service. ERROR :" + ex.Message,ex);
			}
			catch (DocumentSearchException ex)
			{
				//Catching and throwing document search exception when payload size is 0
				throw ex;
			}
			catch (Exception ex)
			{
				throw new DocumentSearchException(27, DocumentSearchException.ERROR_TYPE.WARNING, "Unexpected Error While Accessing Web Service. ERROR :" + ex.Message,ex);
			}

			
		}
		
		#endregion
	}

	public class SearchException : System.ApplicationException
	{
		public enum ERROR_TYPE {INFORMATION,WARNING};
		public int ID = 1;
		public ERROR_TYPE TYPE = ERROR_TYPE.INFORMATION;
		public string ErrMessage = "SEARCH EXCEPTION";
		public string Description = "Error While Searching Documents";

		public SearchException() : base("SEARCH EXCEPTION")
		{
		}

		public SearchException(int id,ERROR_TYPE type,string msg,string desc,Exception ex) : base(msg,ex)
		{
			ID = id;
			TYPE = type;
			ErrMessage = msg;
			Description = desc;
		}

		public SearchException(int id,ERROR_TYPE type,string desc,Exception ex) : base("SEARCH EXCEPTION",ex)
		{
			ID = id;
			TYPE = type;
			Description = desc;
		}

		public SearchException(ERROR_TYPE type,string desc,Exception ex) : base("SEARCH EXCEPTION",ex)
		{
			TYPE = type;
			Description = desc;
		}

		public SearchException(int id,ERROR_TYPE type,string msg,string desc) : base(msg)
		{
			ID = id;
			TYPE = type;
			ErrMessage = msg;
			Description = desc;
		}

		public SearchException(int id,ERROR_TYPE type,string desc) : base("SEARCH EXCEPTION")
		{
			ID = id;
			TYPE = type;
			Description = desc;
		}

		public SearchException(ERROR_TYPE type,string desc) : base("SEARCH EXCEPTION")
		{
			TYPE = type;
			Description = desc;
		}
		public SearchException(string msg) : base(msg)
		{
			ErrMessage = msg;
		}

		public SearchException(string msg,string desc) : base(msg)
		{
			ErrMessage = msg;
			Description = desc;
		}
	}


	public class DocumentSearchException : System.ApplicationException
	{
		public enum ERROR_TYPE {INFORMATION,WARNING};
		public int ID = 2;
		public ERROR_TYPE TYPE = ERROR_TYPE.INFORMATION;
		public string ErrMessage = "DOCUMENT SEARCH EXCEPTION";
		public string Description = "Error While Retrieving Document";

		public DocumentSearchException() : base("DOCUMENT SEARCH EXCEPTION")
		{
		}

		public DocumentSearchException(int id,ERROR_TYPE type,string msg,string desc) : base(msg)
		{
			ID = id;
			TYPE = type;
			ErrMessage = msg;
			Description = desc;
		}

		public DocumentSearchException(int id,ERROR_TYPE type,string desc) : base("DOCUMENT SEARCH EXCEPTION")
		{
			ID = id;
			TYPE = type;
			Description = desc;
		}

		public DocumentSearchException(ERROR_TYPE type,string desc) : base("DOCUMENT SEARCH EXCEPTION")
		{
			TYPE = type;
			Description = desc;
		}
		public DocumentSearchException(int id,ERROR_TYPE type,string msg,string desc,Exception ex) : base(msg,ex)
		{
			ID = id;
			TYPE = type;
			ErrMessage = msg;
			Description = desc;
		}

		public DocumentSearchException(int id,ERROR_TYPE type,string desc,Exception ex) : base("DOCUMENT SEARCH EXCEPTION",ex)
		{
			ID = id;
			TYPE = type;
			Description = desc;
		}

		public DocumentSearchException(ERROR_TYPE type,string desc,Exception ex) : base("DOCUMENT SEARCH EXCEPTION",ex)
		{
			TYPE = type;
			Description = desc;
		}

		public DocumentSearchException(string msg) : base(msg)
		{
			ErrMessage = msg;
		}

		public DocumentSearchException(string msg,string desc) : base(msg)
		{
			ErrMessage = msg;
			Description = desc;
		}
	}

}
