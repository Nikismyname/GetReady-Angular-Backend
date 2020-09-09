namespace GetReady.Services.Contracts
{
    using GetReady.Services.Models.QuestionSsheetModels;

    public interface IQuestionSheetService
    {
        QuestionSheetGet GetOnePersonal(int id, int userId);

        QuestionSheetGet GetOnePublic(int id);


        SheetIndexWithScope CreatePersonal(QuestionSheetCreate data, int userId);

        SheetIndexWithScope CreateGlobal(QuestionSheetCreate data);

        void CreateRoot(int userId); 


        QuestionSheetGlobalIndex GetGlobalSheetIndex(int sheetId);

        QuestionSheetPersonalIndex GetPersonalSheetIndex(int sheetId, int userId);


        SheetIndexWithScope EditPersonal(QuestionSheetEdit data, int userId);

        SheetIndexWithScope EditGlobal(QuestionSheetEdit data);


        int DeleteGlobal(int id);

        int DeletePersonal(int id, int userId);


        PersonalSheetForAllFolders[] GetAllFoldersPersonal(int userId);

        GlobalSheetForAllItems[] GetAllItemsGlobal();

        GlobalSheetForAllFolders[] GetAllFoldersGlobal();
        

        void ReorderGlobal(ReorderSheet data);

        void ReorderPesonal(ReorderSheet data, int userId);


        int[] GetQuestionIds(int sheetId, int userId);
    }
}
