using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BaseCampApi {
	public class Campfire : CreatedItem, ISubscribable{
		public bool visible_to_clients;
		public string subscription_url;
		public long position;
		public string topic;
		public string lines_url;

		public string SubscriptionUrl => subscription_url;

		static public ApiList<Campfire> GetAllCampfires(Api api) {
			return  api.get<ApiList<Campfire>>("chats");
		}

		static public Campfire GetCampfire(Api api, long projectId, long campfireId) {
			return  api.get<Campfire>(Api.Combine("buckets", projectId, "chats", campfireId));
		}

		public ApiList<CampfireLine> GetLines(Api api, Status status = Status.active) {
			return  api.get<ApiList<CampfireLine>>(Api.UriToApi(lines_url), status == Status.active ? null : new { status });
		}

		public CampfireLine GetLine(Api api, long lineId) {
			return  CampfireLine.GetLine(api, bucket.id, id, lineId);
		}

		public CampfireLine CreateLine(Api api, string content) {
			return  api.Post<CampfireLine>(Api.Combine("buckets", bucket.id, "chats", id, "lines"), null, new {
				content
			});
		}

	}

	public class CampfireLine : Campfire {
		public class Attachment : ApiEntryBase {
			public string title;
			public string url;
		}

		public Parent parent;
		public string content;
		public List<Attachment> attachments;

		static public CampfireLine GetLine(Api api, long projectId, long campfireId, long lineId) {
			return  api.get<CampfireLine>(Api.Combine("buckets", projectId, "chats", campfireId, "lines", lineId));
		}

		async public Task Delete(Api api) {
			 api.Delete(Api.Combine("buckets", bucket.id, "chats", parent.id, "lines", id));
		}

	}
}
