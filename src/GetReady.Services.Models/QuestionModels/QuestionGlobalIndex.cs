using GetReady.Data.Models.QuestionModels;
using GetReady.Services.Mapping.Contracts;

namespace GetReady.Services.Models.QuestionModels
{
    public class QuestionGlobalIndex: IMapFrom<GlobalQuestionPackage>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Order { get; set; }
    }
}
