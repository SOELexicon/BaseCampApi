using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BaseCampApi {
	public class MessageBoard : CreatedItem {
		public bool visible_to_clients;
		public long position;
		public long messages_count;
		public string messages_url;
		public string app_messages_url;

		static  public MessageBoard GetMessageBoard(Api api, long projectId, long messageBoardId) {
			return  api.get<MessageBoard>(Api.Combine("buckets", projectId, "message_boards", messageBoardId));
		}

		 public ApiList<Message> GetMessages(Api api, Status status = Status.active) {
			if (messages_count == 0)
				return ApiList<Message>.EmptyList(Api.UriToApi(messages_url));
			return  api.get<ApiList<Message>>(Api.UriToApi(messages_url), status == Status.active ? null : new { status });
		}

		 public Message GetMessage(Api api, long messageId) {
			return  Message.GetMessage(api, bucket.id, messageId);
		}

		 public Message CreateMessage(Api api, string subject, string content, long category_id = 0) {
			return  api.Post<Message>(Api.Combine("buckets", bucket.id, "messages"), null, new {
				subject,
				content,
				category_id
			});
		}

	}

	public class MessageType : ApiEntry {
		public long id;
		public string name;
		public string icon;
		public DateTime created_at;
		public DateTime updated_at;

		 public static ApiList<MessageType> GetMessageTypes(Api api, long projectId) {
			return  api.get<ApiList<MessageType>>(Api.Combine("buckets", projectId, "categories"));
		}

		 public static MessageType GetMessageType(Api api, long projectId, long messageTypeId) {
			return  api.get<MessageType>(Api.Combine("buckets", projectId, "categories", messageTypeId));
		}

		 public static MessageType Create(Api api, long projectId, string name, string icon) {
			return  api.Post<MessageType>(Api.Combine("buckets", projectId, "categories"), null, new {
				name,
				icon
			});
		}

		 public MessageType Update(Api api, long projectId, string name, string icon) {
			return  api.Put<MessageType>(Api.Combine("buckets", projectId, "categories", id), null, new {
				name,
				icon
			});
		}

		 public void Destroy(Api api, long projectId) {
			 api.Delete(Api.Combine("buckets", projectId, "categories", id));
		}
	}

	public class Message : RecordingWithComments {
		public Parent parent;
		public MessageType category;
		public string content;
		public string subject;

		static  public ApiList<Message> GetAllMessages(Api api, long projectId, Status status = Status.active, DateSort sort = DateSort.created_at, SortDirection direction = SortDirection.desc) {
			return  GetAllRecordings<Message>(api, RecordingType.Message, projectId, status, sort, direction);
		}

		static  public ApiList<Message> GetAllMessages(Api api, long[] projectIds = null, Status status = Status.active, DateSort sort = DateSort.created_at, SortDirection direction = SortDirection.desc) {
			return  GetAllRecordings<Message>(api, RecordingType.Message, projectIds, status, sort, direction);
		}

		static  public Message GetMessage(Api api, long projectId, long messageId) {
			return  api.get<Message>(Api.Combine("buckets", projectId, "messages", messageId));
		}

		 public Message Update(Api api) {
			return  api.Put<Message>(Api.Combine("buckets", bucket.id, "messages", id), null, new {
				subject,
				content,
				category_id = category.id
			});
		}

		 public Comment GetComment(Api api, long commentId) {
			return  Comment.GetComment(api, bucket.id, commentId);
		}

		 public Comment CreateComment(Api api, string content) {
			return  api.Post<Comment>(Api.Combine("buckets", bucket.id, "recordings", id, "comments"), null, new {
				content
			});
		}

	}

	public class Comment : Recording {
		public Parent parent;
		public string content;

		static  public ApiList<Comment> GetAllComments(Api api, long projectId, Status status = Status.active, DateSort sort = DateSort.created_at, SortDirection direction = SortDirection.desc) {
			return  GetAllRecordings<Comment>(api, RecordingType.Comment, projectId, status, sort, direction);
		}

		static  public ApiList<Comment> GetAllComments(Api api, long[] projectIds = null, Status status = Status.active, DateSort sort = DateSort.created_at, SortDirection direction = SortDirection.desc) {
			return  GetAllRecordings<Comment>(api, RecordingType.Comment, projectIds, status, sort, direction);
		}

		static  public Comment GetComment(Api api, long projectId, long commentId) {
			return  api.get<Comment>(Api.Combine("buckets", projectId, "comments", commentId));
		}

		 public Comment Update(Api api, string content) {
			return  api.Put<Comment>(Api.Combine("buckets", bucket.id, "comments", id), null, new {
				content
			});
		}

	}

}
