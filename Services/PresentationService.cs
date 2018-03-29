﻿//------------------------------------------------------------------------------
// <autogenerated>
//     This code was generated by a tool.
//     Runtime Version: 1.1.4322.2300
//
//     Changes to this file may cause incorrect behavior and will be lost if 
//     the code is regenerated.
// </autogenerated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by wsdl, Version=1.1.4322.2300.
// 
using System.Diagnostics;
using System.Xml.Serialization;
using System;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Web.Services;
namespace HP.Csn.Business.KM.Services.PresentationService
{

	/// <remarks/>
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Web.Services.WebServiceBindingAttribute(Name="cp-2.0Port", Namespace="http://www.hp.com/seeker/cp")]
	[System.Xml.Serialization.SoapIncludeAttribute(typeof(WSVersionEntry))]
	public class PresentationService : CoreService 
	{
    
		/// <remarks/>
		public PresentationService() 
		{
			//this.Url = "http://ccedl01.cce.cpqcorp.net:80/cp-2.0/service";
		}
    
		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://www.hp.com/seeker/cp", ResponseNamespace="http://www.hp.com/seeker/cp")]
		[return: System.Xml.Serialization.SoapElementAttribute("result")]
		public WSVersionEntry[] retrieveFullHistory(WSUserInfo wSUserInfo, WSCPRequest wSCPRequest) 
		{
			object[] results = this.Invoke("retrieveFullHistory", new object[] {
																				   wSUserInfo,
																				   wSCPRequest});
			return ((WSVersionEntry[])(results[0]));
		}
    
		/// <remarks/>
		public System.IAsyncResult BeginretrieveFullHistory(WSUserInfo wSUserInfo, WSCPRequest wSCPRequest, System.AsyncCallback callback, object asyncState) 
		{
			return this.BeginInvoke("retrieveFullHistory", new object[] {
																			wSUserInfo,
																			wSCPRequest}, callback, asyncState);
		}
    
		/// <remarks/>
		public WSVersionEntry[] EndretrieveFullHistory(System.IAsyncResult asyncResult) 
		{
			object[] results = this.EndInvoke(asyncResult);
			return ((WSVersionEntry[])(results[0]));
		}
    
		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://www.hp.com/seeker/cp", ResponseNamespace="http://www.hp.com/seeker/cp")]
		[return: System.Xml.Serialization.SoapElementAttribute("result")]
		public WSVersionEntry[] retrieveLocalizations(WSUserInfo wSUserInfo, WSCPRequest wSCPRequest) 
		{
			object[] results = this.Invoke("retrieveLocalizations", new object[] {
																					 wSUserInfo,
																					 wSCPRequest});
			return ((WSVersionEntry[])(results[0]));
		}
    
		/// <remarks/>
		public System.IAsyncResult BeginretrieveLocalizations(WSUserInfo wSUserInfo, WSCPRequest wSCPRequest, System.AsyncCallback callback, object asyncState) 
		{
			return this.BeginInvoke("retrieveLocalizations", new object[] {
																			  wSUserInfo,
																			  wSCPRequest}, callback, asyncState);
		}
    
		/// <remarks/>
		public WSVersionEntry[] EndretrieveLocalizations(System.IAsyncResult asyncResult) 
		{
			object[] results = this.EndInvoke(asyncResult);
			return ((WSVersionEntry[])(results[0]));
		}
    
		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://www.hp.com/seeker/cp", ResponseNamespace="http://www.hp.com/seeker/cp")]
		[return: System.Xml.Serialization.SoapElementAttribute("result")]
		public WSVersionEntry[] retrieveRevisionHistory(WSUserInfo wSUserInfo, WSCPRequest wSCPRequest) 
		{
			object[] results = this.Invoke("retrieveRevisionHistory", new object[] {
																					   wSUserInfo,
																					   wSCPRequest});
			return ((WSVersionEntry[])(results[0]));
		}
    
		/// <remarks/>
		public System.IAsyncResult BeginretrieveRevisionHistory(WSUserInfo wSUserInfo, WSCPRequest wSCPRequest, System.AsyncCallback callback, object asyncState) 
		{
			return this.BeginInvoke("retrieveRevisionHistory", new object[] {
																				wSUserInfo,
																				wSCPRequest}, callback, asyncState);
		}
    
		/// <remarks/>
		public WSVersionEntry[] EndretrieveRevisionHistory(System.IAsyncResult asyncResult) 
		{
			object[] results = this.EndInvoke(asyncResult);
			return ((WSVersionEntry[])(results[0]));
		}
    
		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://www.hp.com/seeker/cp", ResponseNamespace="http://www.hp.com/seeker/cp")]
		[return: System.Xml.Serialization.SoapElementAttribute("result")]
		public string[] updatedDocuments(WSUserInfo wSUserInfo, WSUpdatedDocsRequest wSUpdatedDocsRequest) 
		{
			object[] results = this.Invoke("updatedDocuments", new object[] {
																				wSUserInfo,
																				wSUpdatedDocsRequest});
			return ((string[])(results[0]));
		}
    
		/// <remarks/>
		public System.IAsyncResult BeginupdatedDocuments(WSUserInfo wSUserInfo, WSUpdatedDocsRequest wSUpdatedDocsRequest, System.AsyncCallback callback, object asyncState) 
		{
			return this.BeginInvoke("updatedDocuments", new object[] {
																		 wSUserInfo,
																		 wSUpdatedDocsRequest}, callback, asyncState);
		}
    
		/// <remarks/>
		public string[] EndupdatedDocuments(System.IAsyncResult asyncResult) 
		{
			object[] results = this.EndInvoke(asyncResult);
			return ((string[])(results[0]));
		}
    
		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://www.hp.com/seeker/cp", ResponseNamespace="http://www.hp.com/seeker/cp")]
		[return: System.Xml.Serialization.SoapElementAttribute("result")]
		public WSDocument retrieveDocument(WSUserInfo wSUserInfo, WSCPRequest wSCPRequest) 
		{
			object[] results = this.Invoke("retrieveDocument", new object[] {
																				wSUserInfo,
																				wSCPRequest});
			return ((WSDocument)(results[0]));
		}
    
		/// <remarks/>
		public System.IAsyncResult BeginretrieveDocument(WSUserInfo wSUserInfo, WSCPRequest wSCPRequest, System.AsyncCallback callback, object asyncState) 
		{
			return this.BeginInvoke("retrieveDocument", new object[] {
																		 wSUserInfo,
																		 wSCPRequest}, callback, asyncState);
		}
    
		/// <remarks/>
		public WSDocument EndretrieveDocument(System.IAsyncResult asyncResult) 
		{
			object[] results = this.EndInvoke(asyncResult);
			return ((WSDocument)(results[0]));
		}
    
		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://www.hp.com/seeker/cp", ResponseNamespace="http://www.hp.com/seeker/cp")]
		[return: System.Xml.Serialization.SoapElementAttribute("result")]
		public int referenceRelease(WSUserInfo wSUserInfo, WSReferenceRequest wSReferenceRequest) 
		{
			object[] results = this.Invoke("referenceRelease", new object[] {
																				wSUserInfo,
																				wSReferenceRequest});
			return ((int)(results[0]));
		}
    
		/// <remarks/>
		public System.IAsyncResult BeginreferenceRelease(WSUserInfo wSUserInfo, WSReferenceRequest wSReferenceRequest, System.AsyncCallback callback, object asyncState) 
		{
			return this.BeginInvoke("referenceRelease", new object[] {
																		 wSUserInfo,
																		 wSReferenceRequest}, callback, asyncState);
		}
    
		/// <remarks/>
		public int EndreferenceRelease(System.IAsyncResult asyncResult) 
		{
			object[] results = this.EndInvoke(asyncResult);
			return ((int)(results[0]));
		}
    
		/// <remarks/>
		[System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://www.hp.com/seeker/cp", ResponseNamespace="http://www.hp.com/seeker/cp")]
		[return: System.Xml.Serialization.SoapElementAttribute("result")]
		public int referenceSet(WSUserInfo wSUserInfo, WSReferenceRequest wSReferenceRequest) 
		{
			object[] results = this.Invoke("referenceSet", new object[] {
																			wSUserInfo,
																			wSReferenceRequest});
			return ((int)(results[0]));
		}
    
		/// <remarks/>
		public System.IAsyncResult BeginreferenceSet(WSUserInfo wSUserInfo, WSReferenceRequest wSReferenceRequest, System.AsyncCallback callback, object asyncState) 
		{
			return this.BeginInvoke("referenceSet", new object[] {
																	 wSUserInfo,
																	 wSReferenceRequest}, callback, asyncState);
		}
    
		/// <remarks/>
		public int EndreferenceSet(System.IAsyncResult asyncResult) 
		{
			object[] results = this.EndInvoke(asyncResult);
			return ((int)(results[0]));
		}
	}

	/// <remarks/>
	[System.Xml.Serialization.SoapTypeAttribute("WSUserInfo", "java:com.hp.seeker.request")]
	public class WSUserInfo 
	{
    
		/// <remarks/>
		public int[] disclosureLevels;
    
		/// <remarks/>
		public string[] entitlements;
    
		/// <remarks/>
		public string portalId;
    
		/// <remarks/>
		public string userId;
	}

	/// <remarks/>
	[System.Xml.Serialization.SoapTypeAttribute("WSReferenceRequest", "java:com.hp.seeker.cp.request")]
	public class WSReferenceRequest 
	{
    
		/// <remarks/>
		public string docId;
    
		/// <remarks/>
		public string entity;
    
		/// <remarks/>
		public string systemId;
	}

	/// <remarks/>
	[System.Xml.Serialization.SoapTypeAttribute("WSDocument", "java:com.hp.seeker.cp.result.content")]
	public class WSDocument 
	{
    
		/// <remarks/>
		public string docId;
    
		/// <remarks/>
		public string metaData;
    
		/// <remarks/>
		public string mimetype;
    
		/// <remarks/>
		[System.Xml.Serialization.SoapElementAttribute(DataType="base64Binary")]
		public System.Byte[] payload;
    
		/// <remarks/>
		public int payloadType;
	}

	/// <remarks/>
	[System.Xml.Serialization.SoapTypeAttribute("WSUpdatedDocsRequest", "java:com.hp.seeker.cp.request")]
	public class WSUpdatedDocsRequest 
	{
    
		/// <remarks/>
		public System.DateTime odsUpdateTimestamp;
    
		/// <remarks/>
		public string xpath;
	}

	/// <remarks/>
	[System.Xml.Serialization.SoapTypeAttribute("WSVersionEntry", "java:com.hp.seeker.cp.result.history")]
	public class WSVersionEntry 
	{
    
		/// <remarks/>
		public System.DateTime contentUpdateDate;
    
		/// <remarks/>
		public int contentVersion;
    
		/// <remarks/>
		public string disclosureLevel;
    
		/// <remarks/>
		public string docId;
    
		/// <remarks/>
		public string locale;
    
		/// <remarks/>
		public int odsRevision;
    
		/// <remarks/>
		public string payloadStatus;
    
		/// <remarks/>
		public string title;
	}

	/// <remarks/>
	[System.Xml.Serialization.SoapTypeAttribute("WSCPRequest", "java:com.hp.seeker.cp.request")]
	public class WSCPRequest 
	{
    
		/// <remarks/>
		public string docId;
    
		/// <remarks/>
		public string locale;
    
		/// <remarks/>
		public bool readProdNums;
	}
}