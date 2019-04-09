namespace GetReady.Services.Models.QuestionSsheetModels
{
    using GetReady.Data.Models.QuestionModels;
    using GetReady.Services.Mapping.Contracts;
    using GetReady.Services.Models.QuestionModels;
    using System.Collections.Generic;

    public class QuestionSheetGlobalIndex: IMapFrom<QuestionSheet>
    {
        public QuestionSheetGlobalIndex()
        {
            this.Children = new List<QuestionSheetChildIndex>();
            this.GlobalQuestions = new List<QuestionGlobalIndex>();
        }

        public int Id { get; set; }
        
        public string Name { get; set; }

        public string Description { get; set; }

        ///Should be between 1 and 10
        public int? Difficulty { get; set; }

        ///Should be between 1 and 10
        public int Importance { get; set; }

        public int Order { get; set; }

        public int? QuestionSheetId { get; set; }

        public List<QuestionSheetChildIndex> Children { get; set; }
        public List<QuestionGlobalIndex> GlobalQuestions { get; set; }
    }
}
