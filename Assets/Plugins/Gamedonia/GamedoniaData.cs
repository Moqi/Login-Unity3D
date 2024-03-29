using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using LitJson;
using MiniJSON;


public class GamedoniaData  {
	
	public GamedoniaData () {
		
	}
	
	public static void Create(string collection, Dictionary<string,object> entity, Action<bool, IDictionary> callback = null) {
		
		string json = JsonMapper.ToJson(entity);
				
		Gamedonia.RunCoroutine(
			GamedoniaRequest.post("/data/" + collection + "/create",json, null, GamedoniaUsers.GetSessionToken(), null,
				delegate (bool success, object data) {
					if (callback!=null) {
						if (success) {
							callback(success,Json.Deserialize((string)data) as IDictionary);
						}else {
							callback(success,null);
						}
						
					}					
				}
		 	 )
		);
	}
	
	public static void Delete(string collection, string entityId, Action<bool> callback = null) {
		Gamedonia.RunCoroutine(
			GamedoniaRequest.delete("/data/" + collection + "/delete/" + entityId,
				delegate (bool success, object data) {					
					callback(success);
				}
		 	 )
		);
	}
	
	public static void Update (string collection, Dictionary<string,object> entity, Action<bool, IDictionary> callback = null, bool overwrite = false) {
		
		string json = JsonMapper.ToJson(entity);
		
		if (!overwrite) {
			Gamedonia.RunCoroutine(
					GamedoniaRequest.post("/data/" + collection + "/update", json , null, GamedoniaUsers.GetSessionToken(), null,
						delegate (bool success, object data) {												
							if (callback!=null) {
								if (success) {
									callback(success,Json.Deserialize((string)data) as IDictionary);
								}else {
									callback(success,null);
								}							
							}	
						}
				 	 )
			);
		}else {
			Gamedonia.RunCoroutine(
					GamedoniaRequest.put("/data/" + collection + "/update", json , null, GamedoniaUsers.GetSessionToken(), null,
						delegate (bool success, object data) {					
							if (callback!=null) {
								if (success) {
									callback(success,Json.Deserialize((string)data) as IDictionary);
								}else {
									callback(success,null);
								}							
							}				
						}
				 	 )
			);	
		}
	}	
	
	public static void Get (string collection, string entityId, Action<bool, IDictionary> callback = null) {
		
		Gamedonia.RunCoroutine(
				GamedoniaRequest.get("/data/" + collection + "/get/" + entityId, GamedoniaUsers.GetSessionToken(),
					delegate (bool success, object data) {					
						if (callback!=null) {
							if (success) {
								callback(success,Json.Deserialize((string)data) as IDictionary);
							}else {
								callback(success,null);
							}							
						}
					}
			 	 )
			);		
	}
	
	public static void Search(string collection, string query, Action<bool, IList> callback = null) {
		 Search(collection,query,0,null,0,callback);
	}
	
	public static void Search(string collection, string query, int limit, Action<bool, IList> callback = null) {
		Search(collection,query,limit,null,0,callback);
	}
	
	public static void Search(string collection, string query, int limit, string sort, Action<bool, IList> callback = null) {
		Search(collection,query,limit,sort,0,callback);
	}
		
	public static void Search(string collection, string query, int limit=0, string sort=null, int skip=0, Action<bool, IList> callback = null) {
		
		query= Uri.EscapeDataString(query);
		string url = "/data/"+ collection + "/search?query=" + query;
		if (limit>0) url += "&limit="+limit;
		if (sort!=null) url += "&sort=" + sort;
		if (skip>0) url += "&skip="+skip;
			
		Gamedonia.RunCoroutine(
			GamedoniaRequest.get(url, GamedoniaUsers.GetSessionToken(), 
				delegate (bool success, object data) {								
					if (callback!=null) {
						if (success) {
							if ((data==null) ||(data.Equals("[]")) )
								callback(success, null);
							else {
								callback(success,Json.Deserialize((string)data) as IList);
							}	
						}else {
							callback(success,null);
						}							
					}
				}
		 	 )
		);
	}
	
}
