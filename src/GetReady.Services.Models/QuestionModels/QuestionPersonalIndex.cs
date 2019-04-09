namespace GetReady.Services.Models.QuestionModels
{
    using GetReady.Data.Models.QuestionModels;
    using GetReady.Services.Mapping.Contracts;

    public class QuestionPersonalIndex : IMapFrom<PersonalQuestionPackage>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public float AnswerRate { get; set; }

        public int TimesBeingAnswered { get; set; }

        public int Order { get; set; }    
    }
}
