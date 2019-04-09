using GetReady.Data.Models.QuestionModels;
using GetReady.Services.Mapping.Contracts;

namespace GetReady.Services.Models.QuestionSsheetModels
{
    public class QuestionSheetChildIndex: IMapFrom<QuestionSheet>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int Order { get; set; }
    }
}
