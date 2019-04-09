using GetReady.Data.Models.UserModels;
using System.Collections.Generic;

namespace GetReady.Data.Models.QuestionModels
{
    public class QuestionSheet
    {
        public QuestionSheet()
        { 
            this.PersonalQuestions = new HashSet<PersonalQuestionPackage>();
            this.GlobalQuestions = new HashSet<GlobalQuestionPackage>();
            this.Children = new HashSet<QuestionSheet>();
        }

        public int Id { get; set; } 

        public string  Name { get; set; }

        public string  Description { get; set; }

        ///Should be between 1 and 10
        public int?  Difficulty { get; set; }

        ///Should be between 1 and 10
        public int  Importance { get; set; }

        public int  Order { get; set; }

        public int? QuestionSheetId  { get; set; }
        public QuestionSheet QestionSheet  { get; set; }

        public int? UserId { get; set; }
        public User User { get; set; }

        public bool IsGlobal { get; set; }

        public ICollection<PersonalQuestionPackage> PersonalQuestions { get; set; }
        public ICollection<GlobalQuestionPackage> GlobalQuestions { get; set; }
        public ICollection<QuestionSheet> Children { get; set; }
    }
}
