using System;
using System.Data;
using System.Collections;
//ODP.Net 10.7 changes
//using System.Data.OracleClient;
using Oracle.DataAccess.Client;
using HP.Sasp.Data;
using HP.Sasp.Caching;
using HP.Sasp.Utility;


namespace HP.Csn.Business.KM
{
	/// <summary>
	/// Summary description for ServiceGateway.
	/// </summary>
	public class ServiceGateway
	{
		
		#region "### Fields  ###"
		/// <summary>
		/// Holds the URL of Taxonomy Service
		/// </summary>
		protected string taxonomyUrl;
		/// <summary>
		/// Holds the URL of Search Service
		/// </summary>
		protected string searchUrl;
		/// <summary>
		/// Holds the URL of Presentation Service
		/// </summary>
		protected string presentationUrl;
		/// <summary>
		/// Holds the Password information
		/// </summary>
		protected string userPassword;
		/// <summary>
		/// Holds the userId information
		/// </summary>
		protected string userId;
		/// <summary>
		/// To set the name of the portalId
		/// </summary>
		protected string portalId;
		/// <summary>
		/// Holds the maximum of time a web service can take to return value
		/// </summary>
		protected int timeOut;
		/// <summary>
		/// Holds the maximum number that should be returned from teh web service for each call
		/// </summary>
		protected int max_results;
		/// <summary>
		/// Holds the maximum number that should be displayed per page
		/// </summary>
		protected int results_per_page;
		/// <summary>
		/// Holds the Taxonomy Root ID for product 
		/// </summary>
		protected int product_root_oid;//PRODUCT_ROOT_OID = 0;
		/// <summary>
		/// Holds the ODS TAXONOMY ID for Component System
		/// </summary>
		protected int component_root_oid;//COMPONENT_ROOT_OID = 1290;  
		/// <summary>
		/// Holds the ODS TAXONOMY ID for Operating System
		/// </summary>
		protected int os_root_oid;//OS_ROOT_OID = 391525;	
		/// <summary>
		/// Holds the expiration hrs to clear taxonomy cache
		/// </summary>
		protected int expiration_hrs;//EXPIRATION_HRS = 4;
		/// <summary>
		/// Holds the expiration mins to clear taxonomy cache
		/// </summary>
		protected int expiration_min;//EXPIRATION_MIN = 30;
		/// <summary>
		/// Holds the expiration sec to clear taxonomy cache
		/// </summary>
		protected int expiration_sec;//EXPIRATION_SEC = 0;
		/// <summary>
		/// Holds the cached hashtable for all the conifuration data.
		/// </summary>

        private CsnCache chConfig = new CsnCache();

		#endregion

		#region "### Properties  ###"

		/// <summary>
		/// Gets the URL Used for Taxonomy Service
		/// </summary>
		public string TaxonomyUrl
		{
			get
			{
				return taxonomyUrl;
			}
		}
		/// <summary>
		/// Gets the URL Used for Search Service
		/// </summary>
		public string SearchUrl
		{
			get
			{
				return searchUrl;
			}
		}
		
		/// <summary>
		/// Gets the URL Used for Presentation Service
		/// </summary>
		public string PresentationUrl
		{
			get
			{
				return presentationUrl;
			}
		}
		
		#endregion


		#region "### Methods  ###"
        /// <summary>
        /// Code by Ajit: to filter system
		/// This gives the information of the External Server. Here the code to fetch the connection should be written
		/// This can be called from class inheriting this Base class 
		/// </summary>
        protected string[] GetKmOriginalSystem()
        {
            string sErrorMsg;//to handle error msg.
            string[] Original_Systems;//array to hold original systems
            // Holds the Cached Array of original systems from which data to be fetched. //By AJIT :To exclude Systems
            Original_Systems = (string[])chConfig.Get("CSN_KM_CONFIG", "SystemConfig");
            if (Original_Systems == null || Original_Systems.Length == 0)
            {
                try
                {//if Cache is empty then, cache it from database as below.
                    string sSql = "SELECT SERVER_ID FROM CSN.KM_SYSTEMCONFIG WHERE SELECTED = 1";
                    OracleDataAccess objConn = (OracleDataAccess)OracleDataAccess.GetObj("CSN");
                    DataSet dsLanguage = objConn.ExecuteDataSet(CommandType.Text, sSql);
                    if (dsLanguage!=null)
                    {
                        DataTable disClosLevel = dsLanguage.Tables[0];
                        if (disClosLevel.Rows.Count > 0)
                        {//dynamically assigning array lenth
                            Original_Systems = new string[disClosLevel.Rows.Count];
                            //adding values to array using for loop
                            for (int cnt = 0; cnt < disClosLevel.Rows.Count; cnt++)
                            {
                                Original_Systems[cnt] = disClosLevel.Rows[cnt]["SERVER_ID"].ToString();
                            }
                            chConfig.Insert("CSN_KM_CONFIG", "SystemConfig", Original_Systems);//caching the db values
                        }
                        disClosLevel = null;
                    }
                    dsLanguage = null;
                }
                catch (OracleException ex)
                {
                    sErrorMsg = "Unable to fetch server configuration from Database:" + ex.Message;
                    return Original_Systems;//will return null array
                }
            }
            return Original_Systems;//return array of system values
        }
		/// <summary>
		/// This gives the information of the External Service. Here the code to fetch the connection should be written
		/// This is can be set when in the constructor of the class inheriting the abstract class 
		/// </summary>
		protected bool GetServiceConfig(string serviceName)
		{
			string sConfigName, sErrorMsg, sExpirationTime;
			string [] aExpirationTime = null;
           
			Hashtable htConfigData = ( Hashtable ) chConfig.Get( "CSN_KM_CONFIG", "ServiceGatewayConfigData" );
			if (htConfigData == null)
			{
				try
				{
					htConfigData = new Hashtable();
					string sSql = "SELECT CONFIGNAME, CONFIGVALUE FROM CSN.SERVICECONFIG WHERE SERVICENAME = '" + serviceName + "'";
                    //ODP.Net 10.7 Changes
					OracleDataAccess objConn = (OracleDataAccess)OracleDataAccess.GetObj("CSN");
                    //ODPDataAccess objConn = (ODPDataAccess)ODPDataAccess.GetObj("CSN");
					IDataReader dataReader = objConn.ExecuteReader(CommandType.Text, sSql);
				
					while (dataReader.Read())
					{
						sConfigName = dataReader["CONFIGNAME"].ToString();
						switch (sConfigName)
						{
							case "TAXONOMYURL":
								taxonomyUrl = dataReader["CONFIGVALUE"].ToString();
								htConfigData.Add("TAXONOMYURL",taxonomyUrl);
								break;
							case "SEARCHURL":
								searchUrl = dataReader["CONFIGVALUE"].ToString();
								htConfigData.Add("SEARCHURL",searchUrl);
								break;
							case "PRESENTATIONURL":
								presentationUrl = dataReader["CONFIGVALUE"].ToString();
								htConfigData.Add("PRESENTATIONURL",presentationUrl);
								break;
							case "USERID":
								userId = dataReader["CONFIGVALUE"].ToString();
								htConfigData.Add("USERID",userId);
								break;
							case "PASSWORD":
								userPassword = HP.Sasp.Utility.Cryptography.Decrypt(dataReader["CONFIGVALUE"].ToString());
								htConfigData.Add("PASSWORD",userPassword);
								break;
							case "PORTALID":
								portalId = dataReader["CONFIGVALUE"].ToString();
								htConfigData.Add("PORTALID",portalId);
								break;
							case "TIMEOUT":
								timeOut = Convert.ToInt32(dataReader["CONFIGVALUE"]);
								htConfigData.Add("TIMEOUT",timeOut);
								break;
							case "MAX_RESULTS":
								max_results = Convert.ToInt32(dataReader["CONFIGVALUE"]);
								htConfigData.Add("MAX_RESULTS",max_results);
								break;
							case "RESULTS_PER_PAGE":
								results_per_page = Convert.ToInt32(dataReader["CONFIGVALUE"]);
								htConfigData.Add("RESULTS_PER_PAGE",results_per_page);
								break;
							case "PRODUCT_ROOT_OID":
								product_root_oid = Convert.ToInt32(dataReader["CONFIGVALUE"]);
								htConfigData.Add("PRODUCT_ROOT_OID",product_root_oid);
								break;
							case "COMPONENT_ROOT_OID":
								component_root_oid = Convert.ToInt32(dataReader["CONFIGVALUE"]);
								htConfigData.Add("COMPONENT_ROOT_OID",component_root_oid);
								break;
							case "OS_ROOT_OID":
								os_root_oid = Convert.ToInt32(dataReader["CONFIGVALUE"]);
								htConfigData.Add("OS_ROOT_OID",os_root_oid);
								break;
							case "TAXCACHE_RESET_TIME":
								sExpirationTime = dataReader["CONFIGVALUE"].ToString();
								aExpirationTime = sExpirationTime.Split(":".ToCharArray());
								expiration_hrs = Convert.ToInt32(aExpirationTime[0]);
								expiration_min = Convert.ToInt32(aExpirationTime[1]);
								expiration_sec = Convert.ToInt32(aExpirationTime[2]);
								htConfigData.Add("TAXCACHE_RESET_TIME",sExpirationTime);
								break;
                          
						}

					}
				}
				catch (OracleException ex)
				{
					sErrorMsg = "Unable to fetch service configuration from Database:" + ex.Message;
					return false;
				}
				chConfig.Insert("CSN_KM_CONFIG", "ServiceGatewayConfigData", htConfigData);
			}
			else
			{
				System.Collections.IDictionaryEnumerator configDataEnumerator = htConfigData.GetEnumerator();

				while (configDataEnumerator.MoveNext())
				{
					sConfigName = configDataEnumerator.Key.ToString();
					switch (sConfigName)
					{
						case "TAXONOMYURL":
							taxonomyUrl = configDataEnumerator.Value.ToString();
							break;
						case "SEARCHURL":
							searchUrl = configDataEnumerator.Value.ToString();
							break;
						case "PRESENTATIONURL":
							presentationUrl = configDataEnumerator.Value.ToString();
							break;
						case "USERID":
							userId = configDataEnumerator.Value.ToString();
							break;
						case "PASSWORD":
							userPassword = configDataEnumerator.Value.ToString();
							break;
						case "PORTALID":
							portalId = configDataEnumerator.Value.ToString();
							break;
						case "TIMEOUT":
							timeOut = (int)configDataEnumerator.Value;
							break;
						case "MAX_RESULTS":
							max_results = (int)configDataEnumerator.Value;
							break;
						case "RESULTS_PER_PAGE":
							results_per_page = (int)configDataEnumerator.Value;
							break;
						case "PRODUCT_ROOT_OID":
							product_root_oid = (int)configDataEnumerator.Value;
							break;
						case "COMPONENT_ROOT_OID":
							component_root_oid = (int)configDataEnumerator.Value;
							break;
						case "OS_ROOT_OID":
							os_root_oid = (int)configDataEnumerator.Value;
							break;
						case "TAXCACHE_RESET_TIME":
							sExpirationTime = configDataEnumerator.Value.ToString();
							aExpirationTime = sExpirationTime.Split(":".ToCharArray());
							expiration_hrs = Convert.ToInt32(aExpirationTime[0]);
							expiration_min = Convert.ToInt32(aExpirationTime[1]);
							expiration_sec = Convert.ToInt32(aExpirationTime[2]);							
							break;
                     
					}
				}
			}
			return true;
				
		}

		#endregion

	}
}
