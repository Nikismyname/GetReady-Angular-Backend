namespace GetReady.Services.Contracts
{
    using GetReady.Services.Models.QuestionModels;

    public interface IQuestionService
    {
        QuestionIndexWithScope CreatePersonal(QuestionCreate data, int userId);

        QuestionIndexWithScope CreateGlobal(QuestionCreate data);


        int DeleteGlobal(int id);

        int DeletePersonal(int id, int userId);

        void DeleteAllPersonalForSheet(int id, int userId);


        QuestionGlobalGet GetGlobal(int id);

        QuestionPersonalGet GetPersonal(int id, int userId);

        PersonalQuestionSheetForUserReview[] GetAnsweredQuestions(int userId);

        int[] GetQuestionIdsForApproval();


        QuestionIndexWithScope EditPersonal(QuestionEdit data, int userId);

        QuestionIndexWithScope EditGlobal(QuestionEdit data);


        void Reorder(ReorderData data, int userId);

        void ReorderGlobal(ReorderData data);


        void ApproveQuestion(QuestionApprovalData data);

        void RejectQuestion(int questionId);


        void CopyQuestions(CopyQuestions data, int userId);

        void AddNewScore(NewScoreData data, int userId);

        void SuggestForPublishing(int personalQuestionId, int userId); 
    }
}
