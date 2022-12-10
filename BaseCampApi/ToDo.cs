using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BaseCampApi {
	public class ToDoSet : CreatedItem {
		public long position;
		public bool completed;
		public string completed_ratio;
		public string name;
		public long todolists_count;
		public string todolists_url;
		public string app_todoslists_url;

		 static public ToDoSet GetToDoSet(Api api, long projectId, long toDoSetId) {
			return  api.get<ToDoSet>(Api.Combine("buckets", projectId, "todosets", toDoSetId));
		}

		 public ApiList<ToDoList> GetAllToDoLists(Api api, Status status = Status.active) {
			return  api.get<ApiList<ToDoList>>(Api.UriToApi(todolists_url), status == Status.active ? null : new { status });
		}

		public ToDoList GetToDoList(Api api, long toDoListId) {
			return  ToDoList.GetToDoList(api, bucket.id, toDoListId);
		}

		 public ToDoList CreateToDoList(Api api, string name, string description) {
			return  ToDoList.Create(api, bucket.id, id, name, description);
		}

#if false
		async public Task<ToDoSet> Update(Api api, string name, string description) {
			return await api.PutAsync<ToDoSet>(Api.Combine("buckets", bucket.id, "todosets", id), null, new {
				name,
				description
			});
		}
#endif
	}

	public class ToDoList : RecordingWithComments {
		public long position;
		public Parent parent;
		public string description;
		public bool completed;
		public string completed_ratio;
		public string name;
		public string todos_url;
		public string groups_url;
		public string app_todos_url;

		 static public ApiList<ToDoList> GetAllToDoLists(Api api, long projectId, long toDoSetId, Status status = Status.active) {
			return  api.get<ApiList<ToDoList>>(Api.Combine("buckets", projectId, "todosets", toDoSetId, "todolists"),
				status == Status.active ? null : new { status });
		}

		static public ToDoList GetToDoList(Api api, long projectId, long toDoListId) {
			return  api.Post<ToDoList>(Api.Combine("buckets", projectId, "todolists", toDoListId));
		}

		 static public ToDoList Create(Api api, long projectId, long toDoSetId, string name, string description) {
			return  api.Post<ToDoList>(Api.Combine("buckets", projectId, "todosets", toDoSetId, "todolists"), null, new {
				name,
				description
			});
		}

		 public ToDoList Update(Api api, string name, string description) {
			return  api.Put<ToDoList>(Api.Combine("buckets", bucket.id, "todolists", id), null, new {
				name,
				description
			});
		}

		 public ApiList<ToDoListGroup> GetAllToDoListGroups(Api api, Status status = Status.active) {
			return  api.get<ApiList<ToDoListGroup>>(Api.UriToApi(groups_url), status == Status.active ? null : new { status });
		}

		 public ToDoListGroup GetToDoListGroup(Api api, long toDoListGroupId) {
			return ToDoListGroup.GetToDoListGroup(api, bucket.id, toDoListGroupId);
		}

		 public ToDoListGroup CreateGroup(Api api, string name) {
			return  ToDoListGroup.Create(api, bucket.id, id, name);
		}

		 public ApiList<ToDo> GetAllToDos(Api api, Status status = Status.active) {
			return  api.get<ApiList<ToDo>>(Api.UriToApi(todos_url), status == Status.active ? null : new { status });
		}
	}

	public class ToDoListGroup : RecordingWithComments {
		public long position;
		public Parent parent;
		public string description;
		public bool completed;
		public string completed_ratio;
		public string name;
		public string todos_url;
		public string group_position_url;
		public string app_todos_url;

		 static public ApiList<ToDoListGroup> GetAllToDoListGroups(Api api, long projectId, long toDoListId, Status status = Status.active) {
			return  api.get<ApiList<ToDoListGroup>>(Api.Combine("buckets", projectId, "todolists", toDoListId, "groups"),
				status == Status.active ? null : new { status });
		}

		 static public ToDoListGroup GetToDoListGroup(Api api, long projectId, long toDoListGroupId) {
			return  api.get<ToDoListGroup>(Api.Combine("buckets", projectId, "todolists", toDoListGroupId));
		}

		 static public ToDoListGroup Create(Api api, long projectId, long toDoListId, string name) {
			return  api.Post<ToDoListGroup>(Api.Combine("buckets", projectId, "todolists", toDoListId, "groups"), null, new {
				name
			});
		}

		 public void Reposition(Api api, long newPosition) {
			 api.Put(Api.UriToApi(group_position_url), null, new {
				position
			});
		}

		/// <summary>
		/// Not documented in api
		/// </summary>
		 public ToDoListGroup Update(Api api, string name, string description) {
			return  api.Put<ToDoListGroup>(Api.Combine("buckets", bucket.id, "todolists", id), null, new {
				name,
				description
			});
		}

		 public ApiList<ToDo> GetAllToDos(Api api, Status status = Status.active) {
			return  api.get<ApiList<ToDo>>(Api.UriToApi(todos_url), status == Status.active ? null : new { status });
		}
	}

	public class ToDoData : ApiEntryBase {
		public string content;
		public long[] assignee_ids;
		public long[] completion_subscriber_ids;
		public bool? notify;
		public DateTime due_on;
		public DateTime starts_on;

		public JObject ToJObject() {
			JObject j = new JObject();
			if (!string.IsNullOrEmpty(content))
				j["content"] = content;
			if (assignee_ids != null)
				j["assignee_ids"] = new JArray(assignee_ids);
			if (completion_subscriber_ids != null)
				j["completion_subscriber_ids"] = new JArray(completion_subscriber_ids);
			if (notify != null)
				j["notify"] = notify;
			if (due_on != DateTime.MinValue)
				j["due_on"] = due_on;
			if (starts_on != DateTime.MinValue)
				j["starts_on"] = starts_on;
			return j;
		}
	}

	public class ToDo : RecordingWithComments {
		public long position;
		public Parent parent;
		public string description;
		public bool completed;
		public string content;
		public string starts_on;
		public string due_on;
		public List<Person> assignees;
		public List<Person> completion_subscribers;
		public string completion_url;

		 static public ApiList<ToDo> GetAllToDos(Api api, long projectId, long toDoListId, Status status = Status.active) {
			return  api.get<ApiList<ToDo>>(Api.Combine("buckets", projectId, "todolists", toDoListId),
				status == Status.active ? null : new { status });
		}

		 static public ApiList<ToDo> GetCompletedToDos(Api api, long projectId, long toDoListId, Status status = Status.active) {
			return  api.get<ApiList<ToDo>>(Api.Combine("buckets", projectId, "todolists", toDoListId),
				status == Status.active ? (object)new { completed = true } : (object)new { status, completed = true });
		}

		 static public ToDo GetToDo(Api api, long projectId, long toDoId) {
			return  api.get<ToDo>(Api.Combine("buckets", projectId, "todos", toDoId));
		}

		 static public ToDo CreateToDo(Api api, long projectId, long toDoListId, ToDoData data) {
			if (string.IsNullOrEmpty(data.content))
				throw new ApplicationException("Content not supplied");
			return  api.Post<ToDo>(Api.Combine("buckets", projectId, "todolists", toDoListId, "todos"), null, data.ToJObject());
		}

		 public ToDo Update(Api api, ToDoData data) {
			return  api.Post<ToDo>(Api.Combine("buckets", bucket.id, "todos", id, "todos"), null, data.ToJObject());
		}

		 public void Complete(Api api) {
			 api.Post(Api.UriToApi(completion_url));
		}

		 public void Uncomplete(Api api) {
			 api.Delete(Api.UriToApi(completion_url));
		}

		 public void Reposition(Api api, long newPosition) {
			 api.Put(Api.Combine("buckets", bucket.id, "todos", id, "position"), null, new {
				position
			});
		}

	}
}
