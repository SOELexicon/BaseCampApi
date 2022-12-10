using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BaseCampApi {
	public static class Extensions {
		static Extensions() {
			// Force Enums to be converted as strings
			_converter = new Newtonsoft.Json.Converters.StringEnumConverter();
			_serializer = new JsonSerializer();
			_serializer.Converters.Add(_converter);
		}
		static JsonSerializer _serializer;
		static Newtonsoft.Json.Converters.StringEnumConverter _converter;

		/// <summary>
		/// Convert object to Json string. 
		/// Note Enums are converted as strings.
		/// </summary>
		public static string ToJson(this object o) {
			return Newtonsoft.Json.JsonConvert.SerializeObject(o, Newtonsoft.Json.Formatting.Indented, _converter);
		}

		/// <summary>
		/// Convert JObject to Object.
		/// Note Enums are converted as strings.
		/// </summary>
		public static JObject ToJObject(this object o) {
			return o is JObject ? o as JObject : JObject.FromObject(o, _serializer);
		}

		/// <summary>
		/// Is a JToken null or empty
		/// </summary>
		public static bool IsNullOrEmpty(this JToken token) {
			return (token == null) ||
				   (token.Type == JTokenType.Array && !token.HasValues) ||
				   (token.Type == JTokenType.Object && !token.HasValues) ||
				   (token.Type == JTokenType.String && token.ToString() == String.Empty) ||
				   (token.Type == JTokenType.Null);
		}

	}

	/// <summary>
	/// Just an object whose ToString shows the whole object as Json, for debugging.
	/// </summary>
	public class ApiEntryBase {
		override public string ToString() {
			return this.ToJson();
		}
	}

	/// <summary>
	/// Information sent to and returned from an Api call
	/// </summary>
	public class MetaData {
		/// <summary>
		/// The request status returned by the Api
		/// </summary>
		public long status;
		/// <summary>
		/// An error message returned by the Api
		/// </summary>
		public string error;
		/// <summary>
		/// The Uri called
		/// </summary>
		public string Uri;
		/// <summary>
		/// Uri to get the next items in a List.
		/// </summary>
		public string Link;
		/// <summary>
		/// TotalCount for Lists <see cref="ApiList" />
		/// </summary>
		public long TotalCount;
		/// <summary>
		/// ETag object for caching.
		/// </summary>
		public string ETag;
		/// <summary>
		/// Last modified date for caching.
		/// </summary>
		public DateTime Modified;
	}

	/// <summary>
	/// Standard Api call return value.
	/// </summary>
	public class ApiEntry : ApiEntryBase {
		/// <summary>
		/// MetaData about the call and return values
		/// </summary>
		public MetaData MetaData;
		/// <summary>
		/// Any unexpected json items returned will be in here
		/// </summary>
		[JsonExtensionData]
		public IDictionary<string, JToken> AdditionalData;
		/// <summary>
		/// Whether the Api call returned an error object.
		/// </summary>
		[JsonIgnore]
		public bool Error {
			get { 
			return !App.Com.Library.HasValue(MetaData); }
		}
#if DEBUG
		/// <summary>
		/// For debugging, to flag up when there is AdditionalData
		/// </summary>
		/// <returns></returns>
		public override string ToString() {
			if (AdditionalData != null && AdditionalData.Count > 0)
				System.Diagnostics.Debug.WriteLine("***ADDITIONALDATA***");
			return base.ToString();
		}
#endif
	}

	/// <summary>
	/// Standard Api call List return
	/// </summary>
	/// <typeparam name="T">The type of item in the List</typeparam>
	public class ApiList<T> : ApiEntry where T : new() {
		public static ApiList<T> EmptyList(string uri) {
			ApiList<T> list = new ApiList<T>();
			list.MetaData = new MetaData() { Uri = uri };
			return list;
		}
		/// <summary>
		/// List of items returned in this chunk.
		/// </summary>
		public List<T> List = new List<T>();

		/// <summary>
		/// Number of items retrieved in this chunk.
		/// </summary>
		public long Count {
			get { return List.Count; }
		}

		/// <summary>
		/// Total number of items available
		/// </summary>
		public long TotalCount {
			get { return MetaData == null || MetaData.TotalCount == 0 ? Count : MetaData.TotalCount; }
		}

		/// <summary>
		/// There is data on the server we haven't fetched yet
		/// </summary>
		public bool HasMoreData {
			get { return MetaData != null && !string.IsNullOrEmpty(MetaData.Link); }
		}

		/// <summary>
		/// Get the next chunk of data from the server
		/// </summary>
		public ApiList<T> GetNext(Api api) {
			if (!HasMoreData)
				return null;
			Match m = Regex.Match(MetaData.Link, @"<(.*)>");
			if (!m.Success) {
				MetaData.Link = null;
				return null;
			}
			return  api.get<ApiList<T>>(m.Groups[1].Value);
		}

		/// <summary>
		/// Return an Enumerable of all the items in the list, getting more from the server when required
		/// </summary>
		/// <param name="api"></param>
		/// <returns></returns>
		public IEnumerable<T> All(Api api) {
			ApiList<T> chunk = this;
			while(chunk != null && chunk.Count > 0) {
				foreach(T t in chunk.List)
					yield return t;
				chunk = chunk.GetNext(api);
			}
		}

	}

	/// <summary>
	/// Short info about the Project owning a CreatedItem
	/// </summary>
	public class Bucket : ApiEntryBase {
		public long id;
		public string name;
		public string type;
	}

	/// <summary>
	/// Many Api items have all these fields, so they are in this superclass.
	/// </summary>
	public class CreatedItem : ApiEntry {
		public long id;
		public string status;
		public DateTime created_at;
		public DateTime updated_at;
		public string title;
		public bool inherits_status;
		public string type;
		public string url;
		public string app_url;
		public string bookmark_url;
		public Bucket bucket;
		public Person creator;
	}
}