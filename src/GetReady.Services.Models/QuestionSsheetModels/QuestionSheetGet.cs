namespace GetReady.Services.Models.QuestionSsheetModels
{
    public class QuestionSheetGet
    {
        public string Name { get; set; }

        public string Description { get; set; }

        ///Should be between 1 and 10
        public int? Difficulty { get; set; }

        ///Should be between 1 and 10
        public int Importance { get; set; }
    }
}
