using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using LitJson;
using MiniJSON;

public class GamedoniaStore 
{	
	#if UNITY_EDITOR	
	public static bool CanMakePayments () { return true; }
	public static void RequestProducts (string [] productIdentifiers, int size) {
		string response = "[{\"identifier\":\"premium\", \"description\":\"Premium product\", \"priceLocale\":\"$ 0.89\"}]";
		GameObject.Find("Gamedonia").SendMessage("ProductsRequested", response);
	}	
	public static void BuyProduct (string productIdentifier) {
		string response = "{\"success\":true, \"status\": \"success\", \"identifier\":\"" + productIdentifier +"\", \"message\":\"\", \"transactionId\":\"\", \"receipt\":\"\"}";
		GameObject.Find("Gamedonia").SendMessage("ProductPurchased", response);
	}	
	
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	public static extern bool CanMakePayments ();
	[DllImport("__Internal")]
	public static extern void RequestProducts(String [] productIdentifiers, int size);	
	[DllImport ("__Internal")] 
	public static extern void BuyProduct (String productIdentifier);
	
	#elif UNITY_ANDROID
	private static bool started = false;
	public static bool CanMakePayments () { return true; }	
	public static void BuyProduct (string productIdentifier) {
	}	
	
	
	private static void _StartInAppBilling(string key) {
		AndroidJNI.AttachCurrentThread(); 
		
		AndroidJavaClass billingClass = new AndroidJavaClass("com.gamedonia.inapppurchase.UnityAndroidInterface");					
		billingClass.CallStatic("StartInAppBilling",new object [] {key});		
	}
	
	public static void StartInAppBilling(string key){
		if(!started){
			started = true;
			if( Application.platform == RuntimePlatform.Android )
				_StartInAppBilling(key);
		}
	}
	
	private static void _StopInAppBilling() {
		try {
			AndroidJNI.AttachCurrentThread(); 
		
			AndroidJavaClass billingClass = new AndroidJavaClass("com.gamedonia.inapppurchase.UnityAndroidInterface");					
			billingClass.CallStatic("StopInAppBilling",new object [] {});			
		} catch (Exception ex) {
				Debug.Log(ex.Message);
		}
	}	
	
	public static void StopInAppBilling(){
		if(started){
			started = false;
			if( Application.platform == RuntimePlatform.Android )
				_StopInAppBilling();
		}
	}	
	
	private static void _RequestProducts(string [] productIdentifiers) {
		
		AndroidJNI.AttachCurrentThread(); 
		
		AndroidJavaClass billingClass = new AndroidJavaClass("com.gamedonia.inapppurchase.UnityAndroidInterface");					
		billingClass.CallStatic("RequestProducts",new object [] {productIdentifiers});			
	}
	
	// Request products
	public static void RequestProducts( string [] productIdentifiers, int size )
	{
		if( Application.platform == RuntimePlatform.Android )
			if(GameObject.Find("Gamedonia")) {
				GamedoniaStoreInAppPurchases inAppPurchases = GameObject.Find("Gamedonia").GetComponent<GamedoniaStoreInAppPurchases>();
				StartInAppBilling(inAppPurchases.androidPublickey);
			}
			_RequestProducts(productIdentifiers);
	}	
	#endif
	
	private static PurchaseResponse _purchaseResponse;
	private static Dictionary<string, ProductResponse> _productsResponse = new Dictionary<string, ProductResponse>();

	public GamedoniaStore () {}
	
	public static void ProductPurchased(string response) {
		_purchaseResponse = JsonMapper.ToObject<PurchaseResponse>((string) response);
	}
	
	public static PurchaseResponse purchaseResponse {
		get { return _purchaseResponse; }
	}
	
	public static void ProductsRequested(string response) {
		
		if (response != null &&  !response.Equals("[]") ) {
			IList products = Json.Deserialize((string) response) as IList;
			
			for(int i = 0; i < products.Count; i++) {
					IDictionary product = (IDictionary) products[i];
					_productsResponse[(string) product["identifier"]] = new ProductResponse((string) product["identifier"], 
																							(string) product["description"], 
																							(string) product["priceLocale"]);
			}			
			
		}
	
	}
	
	public static void VerifyPurchase(Dictionary<string,object> parameters, Action<bool> callback = null) {
		
		string json = JsonMapper.ToJson(parameters);
		
		Gamedonia.RunCoroutine(
			GamedoniaRequest.post("/purchase/verify", json, null, GamedoniaUsers.GetSessionToken(), null,
				delegate (bool success, object data) {
					if (callback!=null) callback(success);
				}
		 	 )
		);
	}	
	
	public static Dictionary<string, ProductResponse> productsResponse {
		get { return _productsResponse; }
	}
}

public class PurchaseResponse {

		public bool success;
		public string status;
		public string message;
		public string identifier;
		public string transactionId;
		public string receipt;
	
}
	

public class ProductResponse {
	
		public string identifier;
		public string description;
		public string priceLocale;
	
		public ProductResponse (string identifier, string description, string priceLocale) {
			this.identifier = identifier;
			this.description = description;
			this.priceLocale = priceLocale;
		}
	
}
