using System;
using System.Collections.Generic;
using System.Linq;

namespace BaseCampApi {
	/// <summary>
	/// Token returned from Auth call
	/// </summary>
	public class Token : ApiEntry {
		public string access_token;
		public string refresh_token;
		public long expires_in;
	}

	/// <summary>
	/// Identity of person logged in
	/// </summary>
	public class Identity : ApiEntryBase {
		public long id;
		public string first_name;
		public string last_name;
		public string email_address;
	}

	/// <summary>
	/// Company this person can access
	/// </summary>
	public class Account : ApiEntryBase {
		public string product;
		public long id;
		public string name;
		public string href;
		public string app_href;
	}

	/// <summary>
	/// Return value of the GetAuthorization call.
	/// </summary>
	public class Authorization : ApiEntry {
		public DateTime expires_at;
		public Identity identity;
		public List<Account> accounts;

		/// <summary>
		/// Whether this person is authorised for the given company id.
		/// </summary>
		public bool AuthorisedFor(long companyId) {
			return accounts.Any(a => a.id == companyId);
		}
	}
}
