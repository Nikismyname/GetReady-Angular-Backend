namespace GetReady.Services.Models.QuestionSsheetModels
{
    using GetReady.Data.Models.QuestionModels;
    using GetReady.Services.Mapping.Contracts;
    using GetReady.Services.Models.QuestionModels;
    using System.Collections.Generic;

    public class GlobalSheetForAllItems: IMapFrom<QuestionSheet>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int? QuestionSheetId { get; set; }

        public List<QuestionGlobalIndex> GlobalQuestions { get; set; }
    }
}
