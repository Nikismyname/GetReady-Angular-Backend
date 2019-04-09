#region INIT
namespace GetReady.Services.Implementations
{
    using GetReady.Data;
    using GetReady.Data.Models.QuestionModels;
    using GetReady.Services.Contracts;
    using GetReady.Services.Exceptions;
    using GetReady.Services.Mapping;
    using GetReady.Services.Models.QuestionSsheetModels;
    using GetReady.Services.Utilities;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;

    public class QuestionSheetService : IQuestionSheetService
    {
        private readonly GetReadyDbContext context;

        public QuestionSheetService(GetReadyDbContext context)
        {
            this.context = context;
        }
        #endregion

        #region GetOne
        public QuestionSheetGet GetOnePersonal(int id, int userId)
        {
            var user = context.Users.SingleOrDefault(x => x.Id == userId);
            if (user == null)
            {
                throw new ServiceException("User Not Found!");
            }

            var sheet = context.QuestionSheets
                .SingleOrDefault(x => x.Id == id && x.IsGlobal == false);
            if (sheet == null)
            {
                throw new ServiceException("Sheet not found");
            }

            if (user.Id != sheet.UserId)
            {
                throw new ServiceException("Sheet does not belong to you!");
            }

            return new QuestionSheetGet
            {
                Name = sheet.Name,
                Description = sheet.Description,
                Difficulty = sheet.Difficulty,
                Importance = sheet.Importance,
            };
        }

        public QuestionSheetGet GetOnePublic(int id)
        {
            var sheet = context.QuestionSheets
                .SingleOrDefault(x => x.Id == id && x.IsGlobal == true);
            if (sheet == null)
            {
                throw new ServiceException("Sheet not found!");
            }

            return new QuestionSheetGet
            {
                Name = sheet.Name,
                Description = sheet.Description,
                Difficulty = sheet.Difficulty,
                Importance = sheet.Importance,
            };
        }
        #endregion

        #region GetIndex
        public QuestionSheetGlobalIndex GetGlobalSheetIndex(int sheetId)
        {
            QuestionSheetGlobalIndex result = null;

            var globalSheets = context.QuestionSheets
                .Where(x => x.IsGlobal == true)
                .To<QuestionSheetGlobalIndex>();

            if (sheetId > 0)
            {
                result = globalSheets.SingleOrDefault(x => x.Id == sheetId);
            }
            else
            {
                result = globalSheets
                    .SingleOrDefault(x => x.Name == Constants.GlobalRootQestionSheetName);
            }

            if (result == null)
            {
                throw new ServiceException("Global Sheet Could Not Be Found!");
            }

            return result;
        }

        public QuestionSheetPersonalIndex GetPersonalSheetIndex(int sheetId, int userId)
        {
            QuestionSheetPersonalIndex result = null;

            ///TODO: Fix this 
            var personalSheets = context.QuestionSheets
                .Where(x => x.UserId == userId)
                .Where(x => x.IsGlobal == false)
                .To<QuestionSheetPersonalIndex>();

            if (sheetId > 0)
            {
                result = personalSheets.SingleOrDefault(x => x.Id == sheetId);
            }
            else
            {
                result = personalSheets.SingleOrDefault(x => x.QuestionSheetId == null);
            }

            if (result == null)
            {
                throw new ServiceException("Personal Sheet Could Not Be Found!");
            }

            return result;
        }
        #endregion

        #region Create
        public int CreateGlobal(QuestionSheetCreate data)
        {
            var parentSheet = this.context.QuestionSheets
                .SingleOrDefault(x => x.Id == data.ParentSheetId);

            if (parentSheet.IsGlobal == false)
            {
                throw new ServiceException("Bad Request!");
            }

            var order = 0;
            var orders = context.QuestionSheets
                .Where(x => x.QuestionSheetId == data.ParentSheetId)
                .Select(x=>x.Order)
                .ToArray();

            if(orders.Length > 0)
            {
                order = orders.Max() + 1;
            }

            var sheet = new QuestionSheet
            {
                Description = data.Description,
                Difficulty = data.Difficulty,
                Importance = data.Importance.Value,
                Name = data.Name,
                Order = order,
                QuestionSheetId = data.ParentSheetId,
                IsGlobal = true,
                UserId = null,
            };

            context.QuestionSheets.Add(sheet);
            context.SaveChanges();
            return sheet.Id;
        }

        public int CreatePersonal(QuestionSheetCreate data, int userId)
        {
            var parentSheet = this.context.QuestionSheets
                .SingleOrDefault(x => x.Id == data.ParentSheetId);
            if (parentSheet == null)
            {
                throw new ServiceException("Parent Sheet Could Not Be Found!");
            }

            var user = this.context.Users.SingleOrDefault(x => x.Id == userId);
            if (parentSheet.UserId != user.Id)
            {
                throw new ServiceException("Parent Sheet Does Not Belong To You!");
            }

            var order = 0;
            var orders = context.QuestionSheets
                .Where(x => x.QuestionSheetId == data.ParentSheetId)
                .Select(x => x.Order)
                .ToArray();
            if (orders.Length > 0)
            {
                order = orders.Max() + 1;
            }

            var sheet = new QuestionSheet
            {
                Description = data.Description,
                Difficulty = data.Difficulty.Value,
                Importance = data.Importance.Value,
                Name = data.Name,
                Order = order,
                QuestionSheetId = data.ParentSheetId,
                IsGlobal = false,
                UserId = user.Id,
            };

            context.QuestionSheets.Add(sheet);
            context.SaveChanges();
            return sheet.Id;
        }

        public void CreateRoot(int userId)
        {
            var user = context.Users
                .Include(x => x.QuestionSheets)
                .SingleOrDefault(x => x.Id == userId);

            if (user == null)
            {
                throw new ServiceException("User Not Found!");
            }

            if (user.QuestionSheets.Any())
            {
                throw new ServiceException("User Already Has Sheets!");
            }

            var sheet = new QuestionSheet
            {
                Description = "Personal Root Sheet",
                Difficulty = 1,
                Importance = 10,
                Name = $"{user.Username}{Constants.PersonalRootSheetNameSuffix}",
                Order = 1,
                QuestionSheetId = null,
                IsGlobal = false,
                UserId = user.Id,
            };

            context.QuestionSheets.Add(sheet);
            context.SaveChanges();
        }
        #endregion

        #region Delete
        public void DeleteGlobal(int id)
        {
            var globalSheet = this.context.QuestionSheets
                .Include(x => x.GlobalQuestions)
                .Include(x => x.PersonalQuestions)
                .SingleOrDefault(x => x.Id == id && x.IsGlobal == true);

            if (globalSheet == null)
            {
                throw new ServiceException("Sheet Could Not Be Fould!");
            }

            try
            {
                this.CascadeDeleteGlobal(globalSheet);
                context.SaveChanges();
            }
            catch
            {
                throw new ServiceException("There Was a Problem With Cascade Delete Global!");
            }
        }

        private void CascadeDeleteGlobal(QuestionSheet sheet)
        {
            var children = context.QuestionSheets
                .Include(x => x.GlobalQuestions)
                .Include(x => x.PersonalQuestions)
                .Where(x => x.QuestionSheetId == sheet.Id && x.IsGlobal == true);

            foreach (var child in children)
            {
                this.CascadeDeleteGlobal(child);
            }

            context.PersonalQuestionPackages.RemoveRange(sheet.PersonalQuestions);
            context.GlobalQuestionPackages.RemoveRange(sheet.GlobalQuestions);
            context.QuestionSheets.Remove(sheet);
        }

        public void DeletePersonal(int id, int userId)
        {
            var user = context.Users.SingleOrDefault(x => x.Id == userId);
            if (user == null)
            {
                throw new ServiceException("User Not Found!");
            }

            var sheet = this.context.QuestionSheets
                .Include(x => x.GlobalQuestions)
                .Include(x => x.PersonalQuestions)
                .SingleOrDefault(x => x.Id == id && x.IsGlobal == false);

            if (sheet == null)
            {
                throw new ServiceException("Personal Sheet to Delete Not Found!");
            }

            try
            {
                CascadeDeletePersonal(sheet);
                context.SaveChanges();
            }
            catch
            {
                throw new ServiceException("There Was a Problem With Cascade Delete Personal!");
            }
        }

        private void CascadeDeletePersonal(QuestionSheet sheet)
        {
            var children = context.QuestionSheets
                .Include(x => x.GlobalQuestions)
                .Include(x => x.PersonalQuestions)
                .Where(x => x.QuestionSheetId == sheet.Id && x.IsGlobal == false);

            foreach (var child in children)
            {
                this.CascadeDeletePersonal(child);
            }

            context.PersonalQuestionPackages.RemoveRange(sheet.PersonalQuestions);
            context.GlobalQuestionPackages.RemoveRange(sheet.GlobalQuestions);
            context.QuestionSheets.Remove(sheet);
        }

        #endregion

        #region GetAll
        public PersonalSheetForAllFolders[] GetAllFoldersPersonal(int userId)
        {
            var user = context.Users.SingleOrDefault(x => x.Id == userId);
            if (user == null)
            {
                throw new ServiceException("User Not Found!");
            }

            var allQuestionSheets = context.QuestionSheets
                .Where(x => x.IsGlobal == false && x.UserId == user.Id)
                .To<PersonalSheetForAllFolders>()
                .ToArray();

            return allQuestionSheets;
        }

        public GlobalSheetForAllItems[] GetAllItemsGlobal()
        {
            var allSheets = this.context.QuestionSheets
                .Where(x => x.IsGlobal == true)
                .To<GlobalSheetForAllItems>()
                .ToArray();

            return allSheets;
        }

        public GlobalSheetForAllFolders[] GetAllFoldersGlobal()
        {
            var allSheets = this.context.QuestionSheets
                .Where(x => x.IsGlobal == true)
                .To<GlobalSheetForAllFolders>()
                .ToArray();

            return allSheets;
        }
        #endregion

        #region Edit
        public void EditPersonal(QuestionSheetEdit data, int userId)
        {
            var user = context.Users.SingleOrDefault(x => x.Id == userId);

            if (user == null)
            {
                throw new ServiceException("User Not Found!");
            }

            var sheet = context.QuestionSheets
                .SingleOrDefault(x => x.Id == data.Id && x.IsGlobal == false);

            if (sheet == null)
            {
                throw new ServiceException("Sheet Not Found!");
            }

            if (sheet.UserId != user.Id)
            {
                throw new ServiceException("Sheet Does Not Belong To You!");
            }

            sheet.Name = data.Name;
            sheet.Description = data.Description;
            sheet.Difficulty = data.Difficulty;
            sheet.Importance = data.Importance.Value;

            context.SaveChanges();
        }

        public void EditGlobal(QuestionSheetEdit data)
        {
            var sheet = context.QuestionSheets
                .SingleOrDefault(x => x.Id == data.Id && x.IsGlobal == true);

            if (sheet == null)
            {
                throw new ServiceException("Sheet Not Found!");
            }

            sheet.Name = data.Name;
            sheet.Description = data.Description;
            sheet.Difficulty = data.Difficulty;
            sheet.Importance = data.Importance.Value;

            context.SaveChanges();
        }
        #endregion

        #region Reorder 
        public void ReorderPesonal(ReorderSheet data, int userId)
        {
            var user = context.Users.SingleOrDefault(x => x.Id == userId);
            if(user == null)
            {
                throw new ServiceException("User not found!");
            }

            var parentSheet = context.QuestionSheets
                .SingleOrDefault(x => x.Id == data.SheetId && x.IsGlobal == false);
            if(parentSheet == null)
            {
                throw new ServiceException("Sheet Parent Sheet Not Found!");
            }

            if(parentSheet.UserId != user.Id)
            {
                throw new ServiceException("Sheet does not belong to you!");
            }

            this.ReorderSheets(data.SheetId, data.Orderings);
        }

        public void ReorderGlobal(ReorderSheet data)
        {
            var sheet = context.QuestionSheets
                .SingleOrDefault(x => x.Id == data.SheetId && x.IsGlobal == true);
            if(sheet == null)
            {
                throw new ServiceException("Parent Sheet Not Found!");
            }

            this.ReorderSheets(data.SheetId, data.Orderings);
        }

        private void ReorderSheets(int sheetId, int[][] orderings)
        {
            var sheets = context.QuestionSheets.Where(x => x.QuestionSheetId == sheetId).ToArray();
            var sheetIds = sheets.Select(x=>x.Id).OrderBy(x=>x).ToArray();

            var sentSheetIds = orderings.Select(x => x[0]).OrderBy(x=>x).ToArray();

            if (!sheetIds.SequenceEqual(sentSheetIds))
            {
                throw new ServiceException("Bad Data");
            }

            for (int i = 0; i < orderings.Length; i++)
            {
                var currentId = orderings[i][0];
                var currentOrder = orderings[i][1];
                var currentSheet = sheets.Single(x => x.Id == currentId);
                currentSheet.Order = currentOrder;
            }

            context.SaveChanges();
        }
        #endregion

        #region Other
        ///For Test
        public int[] GetQuestionIds(int sheetId, int userId)
        {
            var sheet = this.context.QuestionSheets
                .SingleOrDefault(x => x.Id == sheetId && x.IsGlobal == false);
            if (sheet == null)
            {
                throw new ServiceException("Sheet Not Found!");
            }

            var user = this.context.Users.SingleOrDefault(x => x.Id == userId);
            if (user == null)
            {
                throw new ServiceException("User Not Found!");
            }

            if (sheet.UserId != user.Id)
            {
                throw new ServiceException("Sheet Does Not Belong To You!");
            }

            var questionIds = this.context.PersonalQuestionPackages
                .Where(x => x.QuestionSheetId == sheet.Id)
                .OrderBy(x => x.Order)
                .Select(x => x.Id)
                .ToArray();

            return questionIds;
        }
        #endregion
    }
}
