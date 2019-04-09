namespace GetReady.Data.Models.QuestionModels
{
    public class PersonalQuestionPackage
    {
        public int Id { get; set; }
         
        public string Name { get; set; }

        public string Question { get; set; }

        public string Answer { get; set; }

        public string Comment { get; set; }

        public int Difficulty { get; set; }

        public int Order { get; set; }

        public float AnswerRate { get; set; }

        public int TimesBeingAnswered { get; set; }

        public string YourBestAnswer { get; set; }

        /// <summary>
        /// coma separated scores(1-10)
        /// </summary>
        public string LatestScores { get; set; }

        /// <summary>
        /// The Golbal Question this one has been copied from.
        /// </summary>
        public int? DerivedFromId { get; set; }

        public int QuestionSheetId { get; set; }
        public QuestionSheet QuestionSheet { get; set; }
    }
}
