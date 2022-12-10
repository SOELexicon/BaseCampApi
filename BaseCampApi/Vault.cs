using System;
using System.Collections.Generic;
using System.IO;

namespace BaseCampApi {
	/// <summary>
	/// Basecamp Folder
	/// </summary>
	public class Vault : Recording {
		public Parent parent;
		public long position;
		public long documents_count;
		public string documents_url;
		public long uploads_count;
		public string uploads_url;
		public long vaults_count;
		public string vaults_url;

		static  public ApiList<Vault> GetVaults(Api api, long projectId, long vaultId) {
			return  api.get<ApiList<Vault>>(Api.Combine("buckets", projectId, "vaults", vaultId, "vaults"));
		}

		static  public Vault GetVault(Api api, long projectId, long vaultId) {
			return  api.get<Vault>(Api.Combine("buckets", projectId, "vaults", vaultId));
		}

		 public ApiList<Vault> GetVaults(Api api) {
			return  GetVaults(api, bucket.id, id);
		}

		 public Vault CreateVault(Api api, string title) {
			return  api.Post<Vault>(Api.Combine("buckets", bucket.id, "vaults", id, "vaults"), null, new {
				title
			});
		}

		 public Vault Update(Api api, string title) {
			return  api.Put<Vault>(Api.Combine("buckets", bucket.id, "vaults", id), null, new {
				title
			});
		}

		 public ApiList<Document> GetDocuments(Api api) {
			return  Document.GetDocuments(api, bucket.id, id);
		}

		 public Document GetDocument(Api api, long documentId) {
			return  Document.GetDocument(api, bucket.id, documentId);
		}

		 public Document CreateDocument(Api api, string title, string content, Status status = Status.active) {
			return  Document.CreateDocument(api, bucket.id, id, title, content, status);
		}

		 public ApiList<Upload> GetUploads(Api api) {
			return  Upload.GetUploads(api, bucket.id, id);
		}

		 public Upload GetUpload(Api api, long uploadId) {
			return  Upload.GetUpload(api, bucket.id, uploadId);
		}

		 public Upload CreateUpload(Api api, string file, string description, string basename = null) {
			return  Upload.CreateUpload(api, bucket.id, id, file, description, basename);
		}

	}

	public class Document : RecordingWithComments {
		public Parent parent;
		public long position;
		public string content;

		 public static ApiList<Document> GetDocuments(Api api, long projectId, long vaultId) {
			return  api.get<ApiList<Document>>(Api.Combine("buckets", projectId, "vaults", vaultId, "documents"));
		}

		 public static Document GetDocument(Api api, long projectId, long documentId) {
			return  api.get<Document>(Api.Combine("buckets", projectId, "documents", documentId));
		}

		 public static Document CreateDocument(Api api, long projectId, long vaultId, 
				string title, string content, Status status = Status.active) {
			return  api.Post<Document>(Api.Combine("buckets", projectId, "vaults", vaultId, "documents"), null, new {
				title,
				content,
				status
			});
		}

		 public Document Update(Api api, string title, string content) {
			return  api.Put<Document>(Api.Combine("buckets", bucket.id, "documents", id), null, new {
				title,
				content
			});
		}

	}

	/// <summary>
	/// Items like uploaded files, and embedded graphics are held in Attachments
	/// </summary>
	public class Attachment : ApiEntry {
		public string attachable_sgid;

		 public static Attachment CreateAttachment(Api api, string file) {
			string name = Path.GetFileName(file);
			using (FileStream f = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
				return  api.Post<Attachment>("attachments", new { name }, f);
		}
	}

	public class Upload : RecordingWithComments {
		public long position;
		public Parent parent;
		public string description;
		public string filename;
		public string content_type;
		public long byte_size;
		public long width;
		public long height;
		public string download_url;
		public string app_download_url;

		 public static ApiList<Upload> GetUploads(Api api, long projectId, long vaultId) {
			return  api.get<ApiList<Upload>>(Api.Combine("buckets", projectId, "vaults", vaultId, "uploads"));
		}

		 public static Upload GetUpload(Api api, long projectId, long uploadId) {
			return  api.get<Upload>(Api.Combine("buckets", projectId, "uploads", uploadId));
		}

		 public static Upload CreateUpload(Api api, long projectId, long vaultId, string file, string description, string base_name = null) {
			Attachment a =  Attachment.CreateAttachment(api, file);
			if (base_name == null)
				base_name = file;
			base_name = Path.GetFileNameWithoutExtension(base_name);
			return  api.Post<Upload>(Api.Combine("buckets", projectId, "vaults", vaultId, "uploads"), null,
				new {
					a.attachable_sgid,
					base_name,
					description
				});
		}

		 public Upload Update(Api api, string description, string base_name = null) {
			if (base_name == null)
				base_name = Path.GetFileNameWithoutExtension(filename);
			return  api.Put<Upload>(Api.Combine("buckets", bucket.id, "uploads", id), null, new {
				base_name,
				description
			});
		}
	}
}
