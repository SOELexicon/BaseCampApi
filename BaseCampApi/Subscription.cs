using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BaseCampApi {

	public interface ISubscribable {
		string SubscriptionUrl { get; }
	}
	public class Subscription : ApiEntryBase {
		public bool subscribed;
		public long count;
		public string url;
		public Person[] subscribers;

		static  public Subscription GetSubscription(Api api, long projectId, long recordingId) {
			return  api.get<Subscription>(Api.Combine("buckets", projectId, "recordings", recordingId, "subscription"));
		}

		static  public Subscription GetSubscription(Api api, ISubscribable item) {
			return  api.get<Subscription>(Api.UriToApi(item.SubscriptionUrl));
		}

		static  public Subscription SubscribeMe(Api api, long projectId, long recordingId) {
			return  api.Post<Subscription>(Api.Combine("buckets", projectId, "recordings", recordingId, "subscription"));
		}

		static public Subscription SubscribeMe(Api api, ISubscribable item) {
			return  api.Post<Subscription>(Api.UriToApi(item.SubscriptionUrl));
		}

		static  public void  UnsubscribeMe(Api api, long projectId, long recordingId) {
			 api.Delete<Subscription>(Api.Combine("buckets", projectId, "recordings", recordingId, "subscription"));
		}

		static  public void UnsubscribeMe(Api api, ISubscribable item) {
			 api.Delete<Subscription>(Api.UriToApi(item.SubscriptionUrl));
		}

		static  public Subscription Update(Api api, long projectId, long recordingId, UpdateSubscriptionsList updates) {
			return  api.Put<Subscription>(Api.Combine("buckets", projectId, "recordings", recordingId, "subscription"), null, updates);
		}

		static  public Subscription Update(Api api, ISubscribable item, UpdateSubscriptionsList updates) {
			return  api.Put<Subscription>(Api.UriToApi(item.SubscriptionUrl), null, updates);
		}

	}

	/// <summary>
	/// Used when adding and removing subscriptions
	/// </summary>
	public class UpdateSubscriptionsList {
		public long[] subscriptions;
		public long[] unsubscriptions;
	}


}
