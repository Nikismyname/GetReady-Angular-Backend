namespace GetReady.Services.Implementations
{
    using GetReady.Data;
    using GetReady.Data.Models.QuestionModels;
    using GetReady.Services.Contracts;
    using GetReady.Services.Models.UserModels;
    using GetReady.Services.Utilities;
    using System.Linq;

    public class SeederService : ISeederService
    {
        private readonly GetReadyDbContext context;
        private readonly IUserService userService;

        public SeederService(GetReadyDbContext context, IUserService userService)
        {
            this.context = context;
            this.userService = userService;
        }

        public void Seed123()
        {
            var usersCount = context.Users.Count();

            if (usersCount == 0)
            {
                this.userService.Register(new UserRegisterDto
                {
                    Username = "123",
                    FirstName = "123",
                    LastName = "123",
                    Password = "123",
                    RepeatPassword = "123",
                });
            }
        }

        public void SeedAdmin()
        {
            var adminUsersCount = context.Users.Where(x => x.Role == "Admin").Count();

            if (adminUsersCount == 0)
            {
                this.userService.Register(new UserRegisterDto
                {
                    Username = "admin",
                    FirstName = "Admin",
                    LastName = "Adminov",
                    Password = "admin",
                    RepeatPassword = "admin",
                }, true);
            }
        }

        public void SeedGlobalRoot()
        {
            if(context.QuestionSheets.Any(x=>x.Name == Constants.GlobalRootQestionSheetName))
            {
                return;
            }

            var sheet = new QuestionSheet {
                Description = "Global Root Question Sheet",
                Difficulty = 1,
                Importance = 10,
                Name = Constants.GlobalRootQestionSheetName,
                Order = 1,
                QuestionSheetId = null,
                IsGlobal = true,
                UserId = null,
            };

            context.QuestionSheets.Add(sheet);
            context.SaveChanges();
        }
    }
}
