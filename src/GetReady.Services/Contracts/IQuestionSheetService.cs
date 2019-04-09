namespace GetReady.Services.Contracts
{
    using GetReady.Services.Models.QuestionSsheetModels;

    public interface IQuestionSheetService
    {
        QuestionSheetGet GetOnePersonal(int id, int userId);

        QuestionSheetGet GetOnePublic(int id);


        int CreatePersonal(QuestionSheetCreate data, int userId);

        int CreateGlobal(QuestionSheetCreate data);

        void CreateRoot(int userId);


        QuestionSheetGlobalIndex GetGlobalSheetIndex(int sheetId);

        QuestionSheetPersonalIndex GetPersonalSheetIndex(int sheetId, int userId);


        void EditPersonal(QuestionSheetEdit data, int userId);

        void EditGlobal(QuestionSheetEdit data);


        void DeleteGlobal(int id);

        void DeletePersonal(int id, int userId);


        PersonalSheetForAllFolders[] GetAllFoldersPersonal(int userId);

        GlobalSheetForAllItems[] GetAllItemsGlobal();

        GlobalSheetForAllFolders[] GetAllFoldersGlobal();
        

        void ReorderGlobal(ReorderSheet data);

        void ReorderPesonal(ReorderSheet data, int userId);


        int[] GetQuestionIds(int sheetId, int userId);
    }
}
