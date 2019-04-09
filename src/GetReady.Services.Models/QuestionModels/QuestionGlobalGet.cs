namespace GetReady.Services.Models.QuestionModels
{
    using GetReady.Data.Models.QuestionModels;
    using GetReady.Services.Mapping.Contracts;

    public class QuestionGlobalGet: IMapFrom<GlobalQuestionPackage> 
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Question { get; set; }

        public string Answer { get; set; }

        public string Comment { get; set; }

        public int Difficulty { get; set; }
    }
}
