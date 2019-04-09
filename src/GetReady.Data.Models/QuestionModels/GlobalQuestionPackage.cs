namespace GetReady.Data.Models.QuestionModels
{
    using GetReady.Services.Mapping.Contracts;

    public class GlobalQuestionPackage : IMapFrom<PersonalQuestionPackage>
    {
        public GlobalQuestionPackage()
        {
            this.Approved = true;
        }

        public int Id { get; set; } 

        public string Name { get; set; }

        public string Question { get; set; }

        public string Answer { get; set; }

        public string Comment { get; set; }

        public int Difficulty { get; set; }

        public int Order { get; set; }

        public bool Approved { get; set; }

        /// <summary>
        /// The Personal Question this one has been copied from.
        /// </summary>
        public int? DerivedFromId { get; set; }

        public int? QuestionSheetId { get; set; }
        public QuestionSheet QuestionSheet { get; set; }
    }
}
