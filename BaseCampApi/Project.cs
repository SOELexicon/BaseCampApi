using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BaseCampApi {
	public class Parent : ApiEntryBase {
		public long id;
		public string title;
		public string type;
		public string url;
		public string app_url;
	}

	public class Dock : ApiEntryBase {
		public long id;
		public string title;
		public enum Type { chat, message_board, todoset, schedule, questionnaire, vault, inbox , kanban_board}
		public Type name;
		public bool enabled;
		public long? position;
		public string url;
		public string app_url;
	}

	public class Project : ApiEntry {
		public long id;
		public Status status;
		public DateTime created_at;
		public DateTime updated_at;
		public string name;
		public string description;
		public enum Purpose { company_hq, team, topic }
		public Purpose purpose;
		public bool clients_enabled;
		public string bookmark_url;
		public string url;
		public string app_url;
		public List<Dock> dock;
		public bool bookmarked;

		static  public ApiList<Project> GetAllProjects(Api api, Status status = Status.active) {

			return  api.get<ApiList<Project>>("projects", status == Status.active ? null : new { status });
		}

		static  public Project GetProject(Api api, long projectId) {
			return  api.get<Project>(Api.Combine("projects", projectId));
		}

		static  public Project GetProject(Api api, string name) {
			ApiList<Project> projects =  GetAllProjects(api);
			return projects.All(api).FirstOrDefault(p => p.name == name);
		}

		static  public Project CreateProject(Api api, string name, string description = null) {
			JObject j = new JObject();
			j["name"] = name;
			if (description != null)
				j["description"] = description;
			return  api.Post<Project>("projects", null, j);
		}

		 public Project Update(Api api, string name, string description) {
			return  api.Put<Project>(Api.Combine("projects", id), null, new {
				name,
				description
			});
		}

		 public  void Trash(Api api) {
			 api.Delete(Api.Combine("projects", id));
		}

		 public ApiList<Person> GetPeople(Api api) {
			return  Person.GetPeopleOnProject(api, id);
		}

		public bool HasCampfire {
			get { return getIdOf(Dock.Type.chat) > 0; }
		}

		 public Campfire GetCampfire(Api api) {
			long campfireId = getIdOf(Dock.Type.chat);
			return campfireId == 0 ? null :  Campfire.GetCampfire(api, id, campfireId);
		}

		public bool HasMessageBoard {
			get { return getIdOf(Dock.Type.message_board) > 0; }
		}

		 public MessageBoard GetMessageBoard(Api api) {
			long messageBoardId = getIdOf(Dock.Type.message_board);
			return messageBoardId == 0 ? null :  MessageBoard.GetMessageBoard(api, id, messageBoardId);
		}

		public bool HasVault {
			get { return getIdOf(Dock.Type.vault) > 0; }
		}

		 public Vault GetVault(Api api) {
			long vaultId = getIdOf(Dock.Type.vault);
			return vaultId == 0 ? null :  Vault.GetVault(api, id, vaultId);
		}

		public bool HasSchedule {
			get { return getIdOf(Dock.Type.vault) > 0; }
		}

		 public Schedule GetSchedule(Api api) {
			long scheduleId = getIdOf(Dock.Type.schedule);
			return scheduleId == 0 ? null :  Schedule.GetSchedule(api, id, scheduleId);
		}

		public bool HasToDoSet {
			get { return getIdOf(Dock.Type.todoset) > 0; }
		}

		 public ToDoSet GetToDoSet(Api api) {
			long toDoSetId = getIdOf(Dock.Type.todoset);
			return toDoSetId == 0 ? null :  ToDoSet.GetToDoSet(api, id, toDoSetId);
		}

		public bool HasQuestionnaire {
			get { return getIdOf(Dock.Type.questionnaire) > 0; }
		}

		 public Questionnaire GetQuestionnaire(Api api) {
			long questionnaireId = getIdOf(Dock.Type.questionnaire);
			return questionnaireId == 0 ? null : Questionnaire.GetQuestionnaire(api, id, questionnaireId);
		}

		long getIdOf(Dock.Type type) {
			Dock d = dock.FirstOrDefault(i => i.name == type);
			return d == null || !d.enabled ? 0 : d.id;
		}

	}
}
