namespace GetReady.Services.Models.QuestionSsheetModels
{
    using GetReady.Data.Models.QuestionModels;
    using GetReady.Services.Mapping.Contracts;
    using GetReady.Services.Models.QuestionModels;
    using System.Collections.Generic;

    public class QuestionSheetPersonalIndex: IMapFrom<QuestionSheet>
    {
        public QuestionSheetPersonalIndex()
        {
            this.Children = new List<QuestionSheetChildIndex>();
            this.PersonalQuestions = new List<QuestionPersonalIndex>();
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
        public List<QuestionPersonalIndex> PersonalQuestions { get; set; }
    }
}
