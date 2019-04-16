using GetReady.Data.Models.QuestionModels;
using GetReady.Services.Mapping.Contracts;

namespace GetReady.Services.Models.QuestionSsheetModels
{
    public class QuestionSheetGet: IMapFrom<QuestionSheet>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        ///Should be between 1 and 10
        public int? Difficulty { get; set; }

        ///Should be between 1 and 10
        public int Importance { get; set; }
    }
}
