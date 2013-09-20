using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using MiniJSON;

public class GamedoniaPushNotifications : MonoBehaviour {
	
	public string androidSenderId = "";
	
	#if UNITY_EDITOR	
	public static void AddNotification(string json) {
		if (Instance.debug) Debug.Log("AddNotification: " + json);
		Dictionary<string,object> notification = new Dictionary<string,object>();
		
		IDictionary data = Json.Deserialize(json) as IDictionary;
		if (data.Contains("message")) notification["message"] = data["message"];
		if (data.Contains("payload")) {
			
			Hashtable payload = new Hashtable();
			IDictionary detail = (IDictionary) data["payload"];
			
			foreach(string key in detail.Keys) {
				payload.Add(key, detail[key]);
			}
			
			notification["payload"] = payload;
		}
		notifsEditor.Add(notification);
	}
	private static void RegisterForRemoteNotifications() {}
	private static string GetDeviceTokenForRemoteNotifications() {
		return "10000000";	
	}	
	private static List<Dictionary<string,object>> GetRemoteNotifications() {
		List<Dictionary<string,object>> result = new List<Dictionary<string,object>>();
		
		if (notifsEditor.Count > 0) {
			for (int r = 0; r < notifsEditor.Count; r++) {
				Dictionary<string,object> notification = notifsEditor[r];
				result.Add(new Dictionary<string,object>() {{"message", notification["message"]},
															{"payload", notification["payload"]}});
			}
			notifsEditor.Clear();
		}		
		
		return result;
	}
	
	#elif UNITY_IPHONE	
	private static void RegisterForRemoteNotifications () {
		NotificationServices.RegisterForRemoteNotificationTypes(GamedoniaPushNotifications.notificationType);
	}
	private static string GetDeviceTokenForRemoteNotifications () {
		return NotificationServices.deviceToken != null ? System.BitConverter.ToString(NotificationServices.deviceToken).ToLower().Replace("-", "") : null;
	}		
	private static List<Dictionary<string,object>> GetRemoteNotifications() {
		List<Dictionary<string,object>> result = new List<Dictionary<string,object>>();		
			
		if (NotificationServices.remoteNotificationCount > 0) {
			for (int r = 0; r < NotificationServices.remoteNotificationCount; r++) {
				RemoteNotification remoteNotification = NotificationServices.GetRemoteNotification(r);
				result.Add(new Dictionary<string,object>() {{"message", remoteNotification.alertBody},
															{"payload", remoteNotification.userInfo}});
			}
			NotificationServices.ClearRemoteNotifications();
			if (GamedoniaPushNotifications.notificationType == RemoteNotificationType.Badge) ClearBadge();
		}
		
		return result;
	}
	[DllImport("__Internal")]
	private static extern void ClearBadge ();
	
	#elif UNITY_ANDROID
	public void AddNotification(string json) {
		if (debug) Debug.Log("AddNotification: " + json);
		Dictionary<string,object> notification = new Dictionary<string,object>();
		
		IDictionary data = Json.Deserialize(json) as IDictionary;
		if (data.Contains("message")) notification["message"] = data["message"];
		if (data.Contains("payload")) {
			
			Hashtable payload = new Hashtable();
			IDictionary detail = (IDictionary) data["payload"];
			
			foreach(string key in detail.Keys) {
				payload.Add(key, detail[key]);
			}
			
			notification["payload"] = payload;
		}
		notifsAndroid.Add(notification);		
	}	
	
	public void SetDeviceTokenForRemoteNotifications(string regId) {
		registrationId = regId;
	}	
	
	private static void _RegisterForRemoteNotifications(string senderId) {
		
		AndroidJNI.AttachCurrentThread(); 
		
		AndroidJavaClass pushClass = new AndroidJavaClass("com.gamedonia.pushnotifications.PushNotifications");						
		pushClass.CallStatic("registerForRemoteNotifications",new object [] {senderId});	
	}
	
	// Starts up the SDK
	private static void RegisterForRemoteNotifications()
	{
		if( Application.platform == RuntimePlatform.Android )
			_RegisterForRemoteNotifications(GamedoniaPushNotifications.Instance.androidSenderId);
	}	
	
	private static void _Pause() {
		
		AndroidJNI.AttachCurrentThread(); 
		
		AndroidJavaClass pushClass = new AndroidJavaClass("com.gamedonia.pushnotifications.PushNotifications");						
		pushClass.CallStatic("pause",new object [] {});	
	}
	
	// On Pause
	private static void Pause()
	{
		if( Application.platform == RuntimePlatform.Android )
			_Pause();
	}	
	
	private static void _Resume() {
		
		AndroidJNI.AttachCurrentThread(); 
		
		AndroidJavaClass pushClass = new AndroidJavaClass("com.gamedonia.pushnotifications.PushNotifications");						
		pushClass.CallStatic("resume",new object [] {});	
	}
	
	// On Resume
	private static void Resume()
	{
		if( Application.platform == RuntimePlatform.Android )
			_Resume();
	}	
	
	private static void _ClearNotifications() {
		
		AndroidJNI.AttachCurrentThread(); 
		
		AndroidJavaClass pushClass = new AndroidJavaClass("com.gamedonia.pushnotifications.PushNotifications");						
		pushClass.CallStatic("clearNotifications",new object [] {});	
	}
	
	// Clear Notifications
	private static void ClearNotifications()
	{
		if( Application.platform == RuntimePlatform.Android )
			_ClearNotifications();
	}	

	private static string GetDeviceTokenForRemoteNotifications() {
		return registrationId;
	}	
	private static List<Dictionary<string,object>> GetRemoteNotifications() {
		List<Dictionary<string,object>> result = new List<Dictionary<string,object>>();
		
		if (notifsAndroid.Count > 0) {
			for (int r = 0; r < notifsAndroid.Count; r++) {
				Dictionary<string,object> notification = notifsAndroid[r];
				result.Add(new Dictionary<string,object>() {{"message", notification["message"]},
															{"payload", notification["payload"]}});
			}
			notifsAndroid.Clear();
		}		
		
		return result;
	}	
	#endif	
	
	#if UNITY_EDITOR
		private static List<Dictionary<string,object>> notifsEditor = new List<Dictionary<string,object>>();
	#elif UNITY_IPHONE
		public static RemoteNotificationType notificationType = RemoteNotificationType.Badge;
	#elif UNITY_ANDROID
		private static string registrationId;
		private static List<Dictionary<string,object>> notifsAndroid = new List<Dictionary<string,object>>();
	#endif
	
	public GameObject notificationTarget;
	public string notificationFunction;
	public bool clearBadgeOnActivate = true;
	public bool debug = true;
	
	public static string deviceToken;
	
	private static GamedoniaPushNotifications _instance;
	private static List<Dictionary<string,object>> notifications;
	private static int count;
	
	public static GamedoniaPushNotifications Instance
	{
		get {
			return _instance;
		}
	}
	
	
	private static void Profile (GDDeviceProfile device, Action<bool> callback) {
		Gamedonia.RunCoroutine(
			ProfilePushNotification(device,
				delegate(bool success) {
					callback(success);
				}
			)
		);	
	}
	
	
	private static IEnumerator ProfilePushNotification (GDDeviceProfile device, Action<bool> callback) {
		
		if (Instance.debug) Debug.Log("[Register Notification] RegisterForRemoteNotifications");
		RegisterForRemoteNotifications();
		
		while (GetDeviceTokenForRemoteNotifications() == null) { yield return null; }
		deviceToken = GetDeviceTokenForRemoteNotifications();
		
		if (Instance.debug) Debug.Log("[Register Notification] token:" + deviceToken);
		device.deviceToken = deviceToken;
		callback(true);
		
	}	
		
	void Awake () {
		
		//make sure we only have one object with this Gamedonia script at any time
		if (_instance != null)
		{
			Destroy(gameObject);
			return;
		}
		
		_instance = this;
		notifications = new List<Dictionary<string,object>>();
		DontDestroyOnLoad(this);
		
		#if UNITY_EDITOR 
		#elif UNITY_IPHONE
			if (notificationType == RemoteNotificationType.Badge && clearBadgeOnActivate) ClearBadge();
		#endif
		
		
		GDService service = new GDService();
		service.ProfileEvent += new ProfilerEventHandler(Profile);
		GamedoniaDevices.services.Add(service);
		
	}	
	
	void FixedUpdate () {
		
		count++;
		if ((count % Application.targetFrameRate) == 0) {
			count = 0;
			
			notifications = GetRemoteNotifications();
			if (notifications.Count > 0) {
				if (debug) Debug.Log("[Remote Notification] count: " + notifications.Count);
				foreach(Dictionary<string,object> notification in notifications){
					if (debug) { 
						Debug.Log("[Remote Notification] message: " + notification["message"]);
					}
					if (debug) {
						foreach (DictionaryEntry entry in (Hashtable) notification["payload"]) {
							Debug.Log("[Remote Notification] payload: " + entry.Key + " => " + entry.Value);
						}
					}

					if (notificationTarget != null && notificationFunction != null && !notificationFunction.Equals("")) {
						if (debug) Debug.Log("[Remote Notification] SendMessage: " + notifications);
						notificationTarget.SendMessage(notificationFunction, notification);	
					}
								
				}
				notifications.Clear();				
			}
		}

	}
	
	void OnApplicationPause (bool pause) {
		if (debug) Debug.Log("[Remote Notification] OnApplicationPause : " + pause);
		#if UNITY_EDITOR
		#elif UNITY_IPHONE
			if (!pause) 	
				if (notificationType == RemoteNotificationType.Badge && clearBadgeOnActivate) ClearBadge();
		#elif UNITY_ANDROID
			if (!pause) {
				Resume();
				if (clearBadgeOnActivate) ClearNotifications();
			}else {
				Pause();
			}
		#endif		
	}
	
	void OnEnable () {
		if (debug) Debug.Log("[Remote Notification] OnEnable");
	    #if UNITY_EDITOR		
		#elif UNITY_ANDROID
				Resume();
				if (clearBadgeOnActivate) ClearNotifications();
		#endif
	}
	
	void OnApplicationQuit () {
		if (debug) Debug.Log("[Remote Notification] OnApplicationQuit");
	    #if UNITY_EDITOR		
		#elif UNITY_ANDROID
				Pause();
		#endif
	}	
	
}
