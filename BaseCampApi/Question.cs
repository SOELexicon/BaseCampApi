using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BaseCampApi {

	/// <summary>
	/// Also known as Automatic Check-in
	/// </summary>
	public class Questionnaire : CreatedItem {
		public long position;
		public string name;
		public long questions_count;
		public string questions_url;

		 static public Questionnaire GetQuestionnaire(Api api, long projectId, long questionnaireId) {
			return  api.get<Questionnaire>(Api.Combine("buckets", projectId, "questionnaires", questionnaireId));
		}

		 public ApiList<Question> GetAllQuestions(Api api, Status status = Status.active) {
			return  api.get<ApiList<Question>>(Api.UriToApi(questions_url), status == Status.active ? null : new { status });
		}

		 public Question GetQuestion(Api api, long questionId) {
			return  Question.GetQuestion(api, bucket.id, questionId);
		}

#if false
		async public Task<Question> CreateQuestion(Api api, string name) {
			return await Question.Create(api, bucket.id, id, name);
		}

		async public Task<questionnaires> Update(Api api, string name, string description) {
			return await api.PutAsync<questionnaires>(Api.Combine("buckets", bucket.id, "questionnaires"), null, new {
				name,
				description
			});
		}
#endif
	}

	public class QuestionSchedule : ApiEntryBase {
		public string frequency;
		public string[] days;
		public long hour;
		public long minute;
		public string start_date;
	}

	public class Question : Recording {
		public Parent parent;
		public bool paused;
		public QuestionSchedule schedule;
		public long answers_count;
		public string answers_url;

		 static public ApiList<Question> GetAllQuestions(Api api, long projectId, long questionnaireId, Status status = Status.active) {
			return  api.get<ApiList<Question>>(Api.Combine("buckets", projectId, "questionnaires", questionnaireId, "questions"),
				status == Status.active ? null : new { status });
		}

		 static public Question GetQuestion(Api api, long projectId, long questionId) {
			return  api.get<Question>(Api.Combine("buckets", projectId, "questions", questionId));
		}

#if false
		/// <summary>
		/// Not documented in API
		/// </summary>
		async static public Task<Question> Create(Api api, long projectId, long questionnaireId, string name) {
			return await api.PostAsync<Question>(Api.Combine("buckets", projectId, "questionnaires", questionnaireId, "Questions"), null, new {
				name
			});
		}

		/// <summary>
		/// Not documented in API
		/// </summary>
		async public Task<Question> Update(Api api, string name, string description) {
			return await api.PutAsync<Question>(Api.Combine("buckets", bucket.id, "questions", id), null, new {
				name,
				description
			});
		}
#endif

		 public ApiList<QuestionAnswer> GetAllQuestionAnswers(Api api, Status status = Status.active) {
			return  api.get<ApiList<QuestionAnswer>>(Api.UriToApi(answers_url), status == Status.active ? null : new { status });
		}

		public QuestionAnswer GetQuestionAnswer(Api api, long QuestionAnswerId) {
			return  QuestionAnswer.GetQuestionAnswer(api, bucket.id, QuestionAnswerId);
		}

#if false
		async public Task<QuestionAnswer> CreateAnswer(Api api, string name) {
			return await QuestionAnswer.Create(api, bucket.id, id, name);
		}
#endif

	}

	public class QuestionAnswer : RecordingWithComments {
		public Parent parent;
		public string content;
		public string group_on;

		 static public ApiList<QuestionAnswer> GetAllQuestionAnswers(Api api, long projectId, long questionId, Status status = Status.active) {
			return  api.get<ApiList<QuestionAnswer>>(Api.Combine("buckets", projectId, "questions", questionId, "answers"),
				status == Status.active ? null : new { status });
		}

		 static public QuestionAnswer GetQuestionAnswer(Api api, long projectId, long QuestionAnswerId) {
			return  api.get<QuestionAnswer>(Api.Combine("buckets", projectId, "question_answers", QuestionAnswerId));
		}

#if false
		/// <summary>
		/// Not documented in API
		/// </summary>
		async static public Task<QuestionAnswer> Create(Api api, long projectId, long questionId, string content) {
			return await api.PostAsync<QuestionAnswer>(Api.Combine("buckets", projectId, "question_answers", questionId, "question"), null, new {
				content
			});
		}
#endif

	}

}
