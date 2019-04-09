namespace GetReady.Services.Models.QuestionModels
{
    using System.ComponentModel.DataAnnotations;

    public class QuestionCoreData
    {
        [MinLength(10, ErrorMessage = "Question must be at least 10 characters long!"), Required]
        public string Question { get; set; }

        [MinLength(10, ErrorMessage = "Answer must be at least 10 characters long!"), Required]
        public string Answer { get; set; }

        public string Comment { get; set; }

        [MinLength(2, ErrorMessage = "Name must be at least two character long!"), Required]
        public string Name { get; set; }

        [Range(1, 10, ErrorMessage = "Difficulty must be between 1 and 10!"), Required]
        public int? Difficulty { get; set; }
    }
}
