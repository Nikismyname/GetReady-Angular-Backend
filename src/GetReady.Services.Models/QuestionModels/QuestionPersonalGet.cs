namespace GetReady.Services.Models.QuestionModels
{
    using GetReady.Data.Models.QuestionModels;
    using GetReady.Services.Mapping.Contracts;

    public class QuestionPersonalGet: IMapFrom<PersonalQuestionPackage>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Question { get; set; }

        public string Answer { get; set; }

        public string YourBestAnswer { get; set; }

        public string Comment { get; set; }

        public int Difficulty { get; set; }

        public float AnswerRate { get; set; }

        public int TimesBeingAnswered { get; set; }

        public int Order { get; set; }

        public string LastTenScores { get; set; }
    }
}
