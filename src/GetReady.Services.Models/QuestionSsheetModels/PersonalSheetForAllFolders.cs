namespace GetReady.Services.Models.QuestionSsheetModels
{
    using GetReady.Data.Models.QuestionModels;
    using GetReady.Services.Mapping.Contracts;

    public class PersonalSheetForAllFolders : IMapFrom<QuestionSheet>
    {
        public int Id { get; set; }
         
        public string Name { get; set; } 

        public int? QuestionSheetId { get; set; }
    }
}
