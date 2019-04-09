namespace GetReady.Services.Models.QuestionSsheetModels
{
    using System.ComponentModel.DataAnnotations;

    public class QuestionSheetCoreData
    {
        [MinLength(3, ErrorMessage = "Name must be at least 3 characters long!"), Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Range(1, 10, ErrorMessage = "Diffuculty must be between 1 and 10!"), Required]
        public int? Difficulty { get; set; }

        [Range(1, 10, ErrorMessage = "Importance must be between 1 and 10!"), Required]
        public int? Importance { get; set; }
    }
}
