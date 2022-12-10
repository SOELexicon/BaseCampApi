using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BaseCampApi {
	/// <summary>
	/// People can optionally belong to a Company
	/// </summary>
	public class Company : ApiEntry {
		public long id;
		public string name;
	}

	/// <summary>
	/// For adding a new person to Basecamp
	/// </summary>
	public class NewPerson : ApiEntryBase {
		public string name;
		public string email_address;
		public string title;
		public string company_name;
	}

	/// <summary>
	/// Used when adding people to a project
	/// </summary>
	public class UpdateProjectUsersList {
		public long [] grant;
		public long [] revoke;
		public NewPerson [] create;
	}

	/// <summary>
	/// Result returned when adding people to a project
	/// </summary>
	public class UpdateProjectUsersResult : ApiEntry {
		public Person [] granted;
		public Person [] revoked;
	}

	public class Person : ApiEntry {
		public long id;
		public string attachable_sgid;
		public string name;
		public string email_address;
		public string personable_type;
		public string title;
		public string bio;
		public DateTime created_at;
		public DateTime updated_at;
		public bool admin;
		public bool owner;
		public string time_zone;
		public string avatar_url;
		public Company company;
		public bool client;

		static  public ApiList<Person> GetAllPeople(Api api) {
			return  api.get<ApiList<Person>>("people");
		}

		static  public ApiList<Person> GetPeopleOnProject(Api api, long projectId) {
			return  api.get<ApiList<Person>>(Api.Combine("projects", projectId, "people"));
		}

		/// <summary>
		/// Update who can access a project
		/// </summary>
		static  public UpdateProjectUsersResult UpdateProjectUsers(Api api, long projectId, UpdateProjectUsersList changes) {
			return  api.Put<UpdateProjectUsersResult>(Api.Combine("projects", projectId, "people", "users"), null, changes);
		}

		static  public ApiList<Person> GetPingablePeople(Api api) {
			return  api.get<ApiList<Person>>(Api.Combine("circles", "people"));
		}

		static  public Person GetPerson(Api api, long id) {
			return  api.get<Person>(Api.Combine("people", id));
		}

		static  public Person GetMyProfile(Api api) {
			return  api.get<Person>(Api.Combine("my", "profile"));
		}

	}
}
