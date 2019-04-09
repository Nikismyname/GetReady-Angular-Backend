namespace GetReady.Data.Models.UserModels
{
    using GetReady.Data.Models.QuestionModels;
    using System.Collections.Generic;

    public class User
    {
        public User()
        {
            this.QuestionSheets = new HashSet<QuestionSheet>();
        }

        public int  Id { get; set; }

        public string Username { get; set; }

        public string HashedPassword { get; set; }

        public string  Salt { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string  Role { get; set; }

        public IEnumerable<QuestionSheet> QuestionSheets { get; set; }
    }
}
