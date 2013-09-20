using UnityEngine;
using System.Collections.Generic;
using System;


public class GamedoniaStoreInAppPurchases : MonoBehaviour {
	
	public string androidPublickey = "";
	
	public GameObject productTarget;
	public string productPurchasedFunction;
	public string productsRequestedFunction;
	public bool debug = true;
	
	public void ProductPurchased(string response) {
		if (debug) Debug.Log("[GamedoniaStoreInAppPurchases] ProductPurchased: " + response);
		if (response != null) GamedoniaStore.ProductPurchased(response);
		
		if (GamedoniaStore.purchaseResponse != null && GamedoniaStore.purchaseResponse.success) {
			#if UNITY_EDITOR
				if (productTarget != null && productPurchasedFunction != null && !productPurchasedFunction.Equals("")) productTarget.SendMessage(productPurchasedFunction);
			#elif UNITY_IPHONE
				Dictionary<string, object> parameters = new Dictionary<string, object>();
				parameters["deviceId"] = GamedoniaDevices.device.deviceId;
				parameters["receipt"] = GamedoniaStore.purchaseResponse.receipt;			
				GamedoniaStore.VerifyPurchase(parameters, 
					delegate (bool success) {
						if (!success) { 
							GamedoniaStore.purchaseResponse.success = false;
							GamedoniaStore.purchaseResponse.status = "error";
							GamedoniaStore.purchaseResponse.message = "purchase verification failed";
						}
						if (productTarget != null && productPurchasedFunction != null && !productPurchasedFunction.Equals("")) productTarget.SendMessage(productPurchasedFunction);
					}
				);	
			#elif UNITY_ANDROID
				Dictionary<string, object> parameters = new Dictionary<string, object>();
				parameters["deviceId"] = GamedoniaDevices.device.deviceId;
				parameters["receipt"] = GamedoniaStore.purchaseResponse.receipt;			
				GamedoniaStore.VerifyPurchase(parameters, 
					delegate (bool success) {
						if (!success) { 
							GamedoniaStore.purchaseResponse.success = false;
							GamedoniaStore.purchaseResponse.status = "error";
							GamedoniaStore.purchaseResponse.message = "purchase verification failed";
						}
						if (productTarget != null && productPurchasedFunction != null && !productPurchasedFunction.Equals("")) productTarget.SendMessage(productPurchasedFunction);
					}
				);
			#endif
		} else {
			if (productTarget != null && productPurchasedFunction != null && !productPurchasedFunction.Equals("")) productTarget.SendMessage(productPurchasedFunction);
		}
		
	}
	
	public void ProductsRequested(string response) {
		if (debug) Debug.Log("[GamedoniaStoreInAppPurchases] ProductsRequested: " + response);
		if (response != null) GamedoniaStore.ProductsRequested(response);
		if (productTarget != null && productsRequestedFunction != null && !productsRequestedFunction.Equals("")) productTarget.SendMessage(productsRequestedFunction);
	}
		
	#if UNITY_EDITOR
	#elif UNITY_ANDROID
	void Awake() {
		if (debug) Debug.Log("[GamedoniaStoreInAppPurchases] StartInAppBilling");
		GamedoniaStore.StartInAppBilling(androidPublickey);
	}
	void OnDestroy(){
		if (debug) Debug.Log("[GamedoniaStoreInAppPurchases] StopInAppBilling");
		GamedoniaStore.StopInAppBilling();
	}
	#endif
}
