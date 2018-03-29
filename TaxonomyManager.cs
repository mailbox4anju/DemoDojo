/******************************************************************************** 
* Class Name:  TaxonomyManager 
* Purpose:     Retrive data from CSR2.0 TaxonomyService proxy class and provide it to the presentation layer 
* Comments: 
* Modified:    Megha for Release 8.7 Enhancement.
*			   A new function GetDisclosurelevelID()is added to retun an int array of DisclosurelevelID's
*			   Create a constant for locale and assign value “EN” and set it to WSPMRequest and WSCVRequest. 
*			   
********************************************************************************/
using System;
using System.Data;
using System.Collections;
using System.Web.Caching;
using System.Web.Services.Protocols;
using HP.Csn.Business.KM.Services.TaxonomyService;
using HP.Sasp.Caching;
using HP.Csn.DataAccess.KM;

namespace HP.Csn.Business.KM
{
	/// <summary>
	/// Summary description for TaxonomyManager.
	/// </summary>
	public class TaxonomyManager : ServiceGateway
	{
		/// <summary>
		/// constants, which represent constant values associated with the class
		/// </summary>		
		#region "### Constant  ###"

		private const string DEFAULT_LANGUAGECODE = "EN";

		#endregion
		
		/// <summary>
		/// fields, which are the variables of the class
		/// </summary>		
		#region "### Fields  ###"
		
		private CsnCache chTaxonomy = new CsnCache();

		#endregion

		/// <summary>
		/// types, which represent the types of the class
		/// </summary>			
		#region "### Types ###"

		public enum PRODUCT_LEVEL {TYPE = 1, CATEGORY, FAMILY, BIGSERIES, SERIES};
		public enum SUBCOMPONENT_LEVEL {CATEGORY = 1, TYPE};
		public enum OS_LEVEL {VENDOR = 3, PRODUCT, VERSION};
		
		#endregion

		/// <summary>
		/// instance constructors, which implement the actions required to
		/// initialize instances of the class
		/// </summary>		
		#region "### Instance Constructors ###"

		public TaxonomyManager()
		{
			GetServiceConfig("CSR_CONFIG");
		}

		#endregion

		/// <summary>
		/// methods, which implement the computations and actions that can be
		/// performed by the class  
		/// </summary>
		#region "### Public Methods  ###"

		public DataTable RetrieveProductTaxonomy(int oid, PRODUCT_LEVEL pLevel,string partnerRole)
		{
			int level = 0;

			foreach ( int tempLevel in Enum.GetValues( typeof( PRODUCT_LEVEL ) ) )
				if ( Enum.GetName( typeof( PRODUCT_LEVEL),tempLevel) == pLevel.ToString())
				{
					level = tempLevel;
					break;
				}

			if ( pLevel == PRODUCT_LEVEL.TYPE)
			{
				if (oid != -1)
					throw new TaxonomyException("Invalid OID", "For Level 0(PRODUCT TYPE), OID should be -1");
				oid = base.product_root_oid;//  PRODUCT_ROOT_OID;	//root oid for product categories.
			}
			else
			{
				if (oid < 1)
					throw new TaxonomyException("Invalid OID", "For PRODUCT "+pLevel+", OID cannot be less than 1");
			}
			
			DataTable retTaxonomy = (DataTable)chTaxonomy.Get("CSN_KM_TAXONOMY","Taxonomy_"+oid);
			if (retTaxonomy == null)
				retTaxonomy = BuildTaxonomy(oid,level,partnerRole);

			return copyAndReturnCacheDataTable(retTaxonomy);
		}
		
		public DataTable RetrieveSubComponentTaxonomy(int taxid, SUBCOMPONENT_LEVEL ssLevel, string partnerRole)
		{
			switch (ssLevel)
			{
				case  SUBCOMPONENT_LEVEL.CATEGORY:
					if (taxid != -1)
						throw new TaxonomyException("Invalid OID", "For Level 0(COMPONENT CATEGORY), Taxid should be -1");
					taxid = base.component_root_oid;// COMPONENT_ROOT_OID; //maping of the oid's to Sub Component OID
					break;
				case  SUBCOMPONENT_LEVEL.TYPE:
					if (taxid < 1)
						throw new TaxonomyException("Invalid OID", "For Level 1(COMPONENT TYPE), Taxid cannot be less than 1");
					break;
			}

			DataTable retTaxonomy = (DataTable)chTaxonomy.Get("CSN_KM_TAXONOMY","Taxonomy_CV");
			if (retTaxonomy == null)
				retTaxonomy = BuildCVTaxonomy(partnerRole);

			return GetCVTaxonomy(retTaxonomy, taxid);
		}
		
		public DataTable RetrieveOSTaxonomy(int oid,OS_LEVEL osLevel,string partnerRole)
		{
			int level = 0;

			foreach ( int tempLevel in Enum.GetValues( typeof( OS_LEVEL ) ) )
				if ( Enum.GetName( typeof( OS_LEVEL),tempLevel) == osLevel.ToString())
				{
					level = tempLevel;
					break;
				}

			switch (osLevel)
			{
				case OS_LEVEL.VENDOR:
					if (oid != -1)
						throw new TaxonomyException("Invalid OID", "For Level 0(OS VENDOR), OID should be -1");
					oid = base.os_root_oid; //OS_ROOT_OID;//maping of the oid's to OS OID
					break;
				case OS_LEVEL.PRODUCT:
					if (oid < 1)
						throw new TaxonomyException("Invalid OID", "For Level 1(OS PRODUCT), OID cannot be less than 1");
					break;
				case OS_LEVEL.VERSION:
					if (oid < 1)
						throw new TaxonomyException("Invalid OID", "For Level 2(OS VERSION), OID cannot be less than 1");
					break;
			}

			DataTable retTaxonomy = (DataTable)chTaxonomy.Get("CSN_KM_TAXONOMY","Taxonomy_"+oid);
			if (retTaxonomy == null)
				retTaxonomy = BuildTaxonomy(oid,level,partnerRole);

			return copyAndReturnCacheDataTable(retTaxonomy);
		}

		
		#endregion
		
		/// <summary>
		/// methods, which implement the computations and actions that can be
		/// performed by the class  
		/// </summary>
		#region "### Private Methods  ###"
			
		private DataTable BuildTaxonomy(int oid,int level,string partnerRole)
		{
			TaxonomyService taxonomyService = new TaxonomyService();
			taxonomyService.SetConnectionInfo(base.taxonomyUrl, base.userId, base.userPassword, base.timeOut);
			
			WSUserInfo wsUserInfo = new WSUserInfo();
			wsUserInfo.userId = base.userId;
			wsUserInfo.portalId = base.portalId;

			OracleUtil objOraUtil  = new OracleUtil();
			
			// Release 8.7 Enhancement				
			wsUserInfo.disclosureLevels = objOraUtil.GetDisclosurelevel(partnerRole);
			
			WSPMRequest pmRequest = new WSPMRequest();
			pmRequest.oid = oid;
			pmRequest.locale = DEFAULT_LANGUAGECODE;

			WSPMNode[] nodes = null;
			try
			{
				nodes = taxonomyService.retrievePMSubtree(wsUserInfo,pmRequest);
			}
			catch (SoapException ex)
			{
				throw new TaxonomyException(3, TaxonomyException.ERROR_TYPE.INFORMATION, "Error While Retrieving Taxonomy", ex.Message,ex);
			}
			catch (System.Net.WebException ex)
			{
				throw new TaxonomyException(2, TaxonomyException.ERROR_TYPE.WARNING, "Error Connecting To Webservices", ex.Message,ex);

			}
			catch (Exception ex)
			{
				throw new TaxonomyException(1, TaxonomyException.ERROR_TYPE.WARNING, "Unexpected Error While Retrieving Taxonomy", ex.Message,ex);
			}
			
			if (nodes == null || nodes.Length == 0)
				throw new TaxonomyException(4, TaxonomyException.ERROR_TYPE.INFORMATION, "Taxonomy Not Available For Given OID", "OID Node Is Empty");
			
			DataTable retTaxonomy = CreateTaxonomyTable();
			for (int iCnt = 0; iCnt < nodes.Length; iCnt++)
			{
				if (nodes[iCnt].level == level)
				{
					DataRow row = retTaxonomy.NewRow();
					row["OID"] = nodes[iCnt].oid;
					row["ParentID"] = oid;
					row["Description"] =  nodes[iCnt].displayName;
					retTaxonomy.Rows.Add(row);
				}
			}
			// Sort taxonomy on description column
			DataView dvSortedTaxonomy = new DataView(retTaxonomy,null,"Description",DataViewRowState.CurrentRows);
			DataTable dtSortedTaxonomy = CreateTaxonomyTable();
			for (int iCnt = 0; iCnt < dvSortedTaxonomy.Count; iCnt++)
			{
					DataRow row = dtSortedTaxonomy.NewRow();
					row["OID"] = dvSortedTaxonomy[iCnt]["OID"];
					row["ParentID"] = dvSortedTaxonomy[iCnt]["ParentID"];
					row["Description"] =  dvSortedTaxonomy[iCnt]["Description"];
					dtSortedTaxonomy.Rows.Add(row);
			}
			
			if (oid == 0)
			{
				DateTime d= DateTime.Now;
				DateTime d1 = new DateTime(d.Year,d.Month,d.Day,base.expiration_hrs,base.expiration_min,base.expiration_sec);
				long expiration = (d1.Ticks-d.Ticks);
				if (expiration < 0)
					expiration = TimeSpan.TicksPerDay+expiration;
				expiration = expiration/TimeSpan.TicksPerSecond;
				int absoluteExpiration = Convert.ToInt32(expiration);
				int noSliding = Cache.NoSlidingExpiration.Seconds;
				
				chTaxonomy.Insert("CSN_KM_TAXONOMY","Taxonomy_"+oid,dtSortedTaxonomy,null,absoluteExpiration,noSliding);
			}
			else
			{
				CacheDependency cDep = new CacheDependency(null,new string[]{"CSN_KM_TAXONOMY"+":"+"Taxonomy_0"});
				chTaxonomy.Insert("CSN_KM_TAXONOMY","Taxonomy_"+oid,dtSortedTaxonomy, cDep);
			}
		
			return dtSortedTaxonomy;
		
		}
	
		private DataTable BuildCVTaxonomy(string partnerRole)
		{
			TaxonomyService taxonomyService = new TaxonomyService();
			taxonomyService.SetConnectionInfo(base.taxonomyUrl, base.userId, base.userPassword, base.timeOut);
			
			WSUserInfo wsUserInfo = new WSUserInfo();
			wsUserInfo.userId = base.userId;
			wsUserInfo.portalId = base.portalId;

			OracleUtil objOraUtil  = new OracleUtil();				
			wsUserInfo.disclosureLevels = objOraUtil.GetDisclosurelevel(partnerRole);	
			
			WSCVRequest cvRequest = new WSCVRequest();
			cvRequest.taxoId = base.component_root_oid; //COMPONENT_ROOT_OID;
			cvRequest.locale =  DEFAULT_LANGUAGECODE; // Release 8.7 Enhancement
			WSCVNode[] nodes=null;
			try
			{
				nodes = taxonomyService.retrieveCV(wsUserInfo,cvRequest);
			}
			catch (SoapException ex)
			{
				throw new TaxonomyException(3, TaxonomyException.ERROR_TYPE.INFORMATION, "Error While Retrieving Taxonomy", ex.Message,ex);
			}
			catch (System.Net.WebException ex)
			{
				throw new TaxonomyException(2, TaxonomyException.ERROR_TYPE.WARNING, "Error Connecting To Webservices", ex.Message,ex);

			}
			catch (Exception ex)
			{
				throw new TaxonomyException(1, TaxonomyException.ERROR_TYPE.WARNING, "Unexpected Error While Retrieving Taxonomy", ex.Message,ex);

			}
			
			if (nodes == null || nodes.Length == 0)
				throw new TaxonomyException(4,TaxonomyException.ERROR_TYPE.INFORMATION, "Taxonomy Not Available For Given OID", "OID Node Is Empty");
			
			DataTable retTaxonomy = CreateTaxonomyTable();
			
			for (int iCnt = 0; iCnt<nodes.Length; iCnt++)
			{
				DataRow row = retTaxonomy.NewRow();
				row["OID"] = nodes[iCnt].oid;
				row["ParentID"] = nodes[iCnt].parents[0];
				row["Description"] =  nodes[iCnt].displayName;
				retTaxonomy.Rows.Add(row);
			}
			//sorting & storing...
			DataView dvSortedTaxonomy = new DataView(retTaxonomy);
			dvSortedTaxonomy.Sort = "Description";
			DataTable dtSortedTaxonomy = CreateTaxonomyTable();

			for (int iCnt = 0; iCnt < dvSortedTaxonomy.Count; iCnt++)
			{
				DataRow row = dtSortedTaxonomy.NewRow();
				row["OID"] = dvSortedTaxonomy[iCnt]["OID"];
				row["ParentID"] = dvSortedTaxonomy[iCnt]["ParentID"];
				row["Description"] =  dvSortedTaxonomy[iCnt]["Description"];
				dtSortedTaxonomy.Rows.Add(row);
			}


			DateTime d =  DateTime.Now;
			DateTime d1 = new DateTime(d.Year,d.Month,d.Day,base.expiration_hrs,base.expiration_min,base.expiration_sec);
			long expiration = (d1.Ticks-d.Ticks);
			if (expiration < 0)
				expiration = TimeSpan.TicksPerDay+expiration;
			expiration = expiration/TimeSpan.TicksPerSecond;
			int absoluteExpiration = Convert.ToInt32(expiration);
			int noSliding = Cache.NoSlidingExpiration.Seconds;

			chTaxonomy.Insert("CSN_KM_TAXONOMY","Taxonomy_CV",dtSortedTaxonomy,null,absoluteExpiration,noSliding);
		
			return dtSortedTaxonomy;
		
		}

		private DataTable CreateTaxonomyTable()
		{
			DataTable dt =  new DataTable();
			dt.Columns.Add("OID");
			dt.Columns.Add("ParentID");
			dt.Columns.Add("Description");
			return dt;
		}

		private DataTable GetCVTaxonomy(DataTable dtTaxonomy, int taxid)
		{
			//Sort taxonomy on description column
			DataView dvSortedTaxonomy = new DataView(dtTaxonomy,"ParentID="+taxid,"",DataViewRowState.CurrentRows);
			DataTable dtSortedTaxonomy = CreateTaxonomyTable();

			for (int iCnt = 0; iCnt < dvSortedTaxonomy.Count; iCnt++)
			{
				DataRow row = dtSortedTaxonomy.NewRow();
				row["OID"] = dvSortedTaxonomy[iCnt]["OID"];
				row["ParentID"] = dvSortedTaxonomy[iCnt]["ParentID"];
				row["Description"] =  dvSortedTaxonomy[iCnt]["Description"];
				dtSortedTaxonomy.Rows.Add(row);
			}

			return dtSortedTaxonomy;	
		}
		
		private DataTable copyAndReturnCacheDataTable(DataTable dtTaxonomy)
		{
			DataTable retTaxonomy = CreateTaxonomyTable();

			for (int iCnt = 0; iCnt < dtTaxonomy.Rows.Count; iCnt++)
			{
				DataRow row = retTaxonomy.NewRow();
				row["OID"] = dtTaxonomy.Rows[iCnt]["OID"];
				row["ParentID"] = dtTaxonomy.Rows[iCnt]["ParentID"];
				row["Description"] =  dtTaxonomy.Rows[iCnt]["Description"];
				retTaxonomy.Rows.Add(row);
			}
			return retTaxonomy;
		}
		
		
		#endregion
	
	}
	
	//Exception class thrown by taxonomy manager
	public class TaxonomyException : System.ApplicationException
	{
		public enum ERROR_TYPE {INFORMATION,WARNING};
		public int ID = 0;
		public ERROR_TYPE TYPE = ERROR_TYPE.INFORMATION;
		public string ErrMessage = "TAXONOMY EXCEPTION";
		public string Description = "Error while retrieving Taxonomy Information";

		public TaxonomyException() : base("TAXONOMY EXCEPTION")
		{
		}

		public TaxonomyException(int id, ERROR_TYPE type, string msg, string desc,Exception ex) : base(msg,ex)
		{
			ID = id;
			TYPE = type;
			ErrMessage = msg;
			Description = desc;
		}

		public TaxonomyException(ERROR_TYPE type, string desc,Exception ex) : base("TAXONOMY EXCEPTION",ex)
		{
			TYPE = type;
			Description = desc;
		}

		public TaxonomyException(int id, ERROR_TYPE type, string msg, string desc) : base(msg)
		{
			ID = id;
			TYPE = type;
			ErrMessage = msg;
			Description = desc;
		}

		public TaxonomyException(ERROR_TYPE type, string desc) : base("TAXONOMY EXCEPTION")
		{
			TYPE = type;
			Description = desc;
		}
		public TaxonomyException(string msg) : base(msg)
		{
			ErrMessage = msg;
		}

		public TaxonomyException(string msg, string desc) : base(msg)
		{
			ErrMessage = msg;
			Description = desc;
		}
	}


}
