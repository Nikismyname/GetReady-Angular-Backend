#region INIT
namespace GetReady.Services.Implementations
{
    using AutoMapper;
    using GetReady.Data;
    using GetReady.Data.Models.QuestionModels;
    using GetReady.Services.Contracts;
    using GetReady.Services.Exceptions;
    using GetReady.Services.Mapping;
    using GetReady.Services.Models.QuestionModels;
    using GetReady.Services.Utilities;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class QuestionService : IQuestionService
    {
        private readonly GetReadyDbContext context;

        public QuestionService(GetReadyDbContext context)
        {
            this.context = context;
        }
        #endregion

        #region Create 
        public int CreateGlobal(QuestionCreate data)
        {
            var parentSheet = this.context.QuestionSheets
                .Select(qs => new
                {
                    qs.Id,
                    qs.IsGlobal,
                    orders = qs.GlobalQuestions.Select(gq => gq.Order).ToArray(),
                })
                .SingleOrDefault(x => x.Id == data.SheetId && x.IsGlobal == true);

            if (parentSheet == null)
            {
                throw new ServiceException("Invalid Parent Sheet!");
            }

            var order = 0;
            if (parentSheet.orders.Length > 0)
            {
                order = parentSheet.orders.Max() + 1;
            }

            var result = new GlobalQuestionPackage
            {
                Name = data.Name,
                Question = data.Question,
                Answer = data.Answer,
                Comment = data.Comment,
                Difficulty = data.Difficulty.Value,
                QuestionSheetId = data.SheetId,
                Order = order,
            };

            context.GlobalQuestionPackages.Add(result);
            context.SaveChanges();
            return result.Id;
        }

        public int CreatePersonal(QuestionCreate data, int userId)
        {
            var parentSheet = this.context.QuestionSheets
                .Select(qs => new
                {
                    qs.Id,
                    qs.IsGlobal,
                    qs.UserId,
                    orders = qs.PersonalQuestions.Select(gq => gq.Order).ToArray(),
                })
                .SingleOrDefault(x => x.Id == data.SheetId && x.IsGlobal == false);

            if (parentSheet == null)
            {
                throw new ServiceException("Invalid Parent Sheet!");
            }

            if (parentSheet.IsGlobal == true)
            {
                throw new ServiceException("Invalid Parent Sheet!");
            }

            if (parentSheet.UserId != userId)
            {
                throw new ServiceException("Parent Sheet does not belong to you!");
            }

            var user = context.Users.SingleOrDefault(x => x.Id == userId);
            if (user == null)
            {
                throw new ServiceException("Invalid User!");
            }

            var order = 0;
            if (parentSheet.orders.Length > 0)
            {
                order = parentSheet.orders.Max() + 1;
            }

            var result = new PersonalQuestionPackage
            {
                Name = data.Name,
                Question = data.Question,
                Answer = data.Answer,
                Comment = data.Comment,
                Difficulty = data.Difficulty.Value,
                QuestionSheetId = data.SheetId,
                Order = order,

                TimesBeingAnswered = 0,
                YourBestAnswer = "Good try buddy!",
                AnswerRate = 0,
            };

            context.PersonalQuestionPackages.Add(result);
            context.SaveChanges();
            return result.Id;
        }
        #endregion

        #region Delete
        public void DeleteGlobal(int id)
        {
            var questionToDelete = context.GlobalQuestionPackages.SingleOrDefault(x => x.Id == id);
            if (questionToDelete == null)
            {
                throw new ServiceException("Question to delete does not exist!");
            }

            context.GlobalQuestionPackages.Remove(questionToDelete);
            context.SaveChanges();
        }

        public void DeletePersonal(int id, int userId)
        {
            var user = this.context.Users.SingleOrDefault(x => x.Id == userId);
            if (user == null)
            {
                throw new ServiceException(Constants.UserNotFoundMessage);
            }

            var queston = this.context.PersonalQuestionPackages.SingleOrDefault(x => x.Id == id);
            if (queston == null)
            {
                throw new ServiceException(Constants.PersonalQuestionNotFoundMessage);
            }

            var parentSheetUserId = this.context.QuestionSheets
                .SingleOrDefault(x => x.Id == queston.QuestionSheetId && x.IsGlobal == false)?
                .UserId;

            if (parentSheetUserId == null)
            {
                throw new ServiceException(Constants.ParentSheetIsInvalid);
            }

            if (parentSheetUserId != user.Id)
            {
                throw new ServiceException(Constants.ParentSheetDoesNotBelongToYou);
            }

            this.context.PersonalQuestionPackages.Remove(queston);
            this.context.SaveChanges();
        }


        public void DeleteAllPersonalForSheet(int id, int userId)
        {
            var user = this.context.Users.SingleOrDefault(x => x.Id == userId);
            if (user == null)
            {
                throw new ServiceException(Constants.UserNotFoundMessage);
            }

            var sheet = this.context.QuestionSheets
                .Include(x => x.PersonalQuestions)
                .SingleOrDefault(x => x.Id == id && x.IsGlobal == false);
            if (sheet == null)
            {
                throw new ServiceException("Sheet Not Found!");
            }

            if (sheet.UserId != user.Id)
            {
                throw new ServiceException("Sheet does not beling to you!");
            }

            context.PersonalQuestionPackages.RemoveRange(sheet.PersonalQuestions);
            context.SaveChanges();
        }
        #endregion

        #region Get
        public QuestionGlobalGet GetGlobal(int id)
        {
            var model = this.context.GlobalQuestionPackages
                .SingleOrDefault(x => x.Id == id);
            if (model == null)
            {
                throw new ServiceException(Constants.WantedQuestionDoesNotExist);
            }

            var result = Mapper.Map<QuestionGlobalGet>(model);
            return result;
        }

        public QuestionPersonalGet GetPersonal(int id, int userId)
        {
            var user = context.Users.SingleOrDefault(x => x.Id == userId);
            if (user == null)
            {
                throw new ServiceException(Constants.UserNotFoundMessage);
            }

            var personalQuestion = this.context.PersonalQuestionPackages
                .SingleOrDefault(x => x.Id == id);
            if (personalQuestion == null)
            {
                throw new ServiceException(Constants.WantedQuestionDoesNotExist);
            }

            var parentSheet = this.context.QuestionSheets
                .SingleOrDefault(x => x.Id == personalQuestion.QuestionSheetId);
            if (parentSheet == null || parentSheet.UserId != userId)
            {
                throw new ServiceException("This Question Does Not Belong To You!");
            }

            var result = Mapper.Map<QuestionPersonalGet>(personalQuestion);
            return result;
        }

        public int[] GetQuestionIdsForApproval()
        {
            var toBeApproved = context
                .GlobalQuestionPackages.Where(x => x.Approved == false)
                .Select(x => x.Id)
                .ToArray();

            return toBeApproved;
        }

        public PersonalQuestionSheetForUserReview[] GetAnsweredQuestions(int userId)
        {
            var user = context.Users.SingleOrDefault(x => x.Id == userId);
            if (user == null)
            {
                throw new ServiceException("User Not Found!");
            }

            var answeredQuestions = context.QuestionSheets
                .Where(x => x.UserId == userId && x.IsGlobal == false)
                .SelectMany(x => x.PersonalQuestions)
                .Where(x => x.AnswerRate != 0)
                .To<PersonalQuestionSheetForUserReview>()
                .ToArray();

            return answeredQuestions;
        }
        #endregion

        #region Edit
        public void EditPersonal(QuestionEdit data, int userId)
        {
            var user = this.context.Users.SingleOrDefault(x => x.Id == userId);
            if (user == null)
            {
                throw new ServiceException(Constants.UserNotFoundMessage);
            }

            var question = this.context.PersonalQuestionPackages
                .SingleOrDefault(x => x.Id == data.Id);
            if (question == null)
            {
                throw new ServiceException("Question Not Found!");
            }

            var questionSheetUserId = this.context.QuestionSheets
                .SingleOrDefault(x => x.Id == question.QuestionSheetId)?.UserId;
            if (questionSheetUserId == null)
            {
                throw new ServiceException("Invalid Parent Question Sheet!");
            }

            if (questionSheetUserId != user.Id)
            {
                throw new ServiceException("Question Dos Not Belong To You!");
            }

            question.Name = data.Name;
            question.Question = data.Question;
            question.Comment = data.Comment;
            question.Answer = data.Answer;
            question.Difficulty = data.Difficulty.Value;
            context.SaveChanges();
        }

        public void EditGlobal(QuestionEdit data)
        {
            var question = this.context.GlobalQuestionPackages
                .SingleOrDefault(x => x.Id == data.Id);
            if (question == null)
            {
                throw new ServiceException("Question Not Found!");
            }

            question.Name = data.Name;
            question.Question = data.Question;
            question.Comment = data.Comment;
            question.Answer = data.Answer;
            question.Difficulty = data.Difficulty.Value;
            context.SaveChanges();
        }
        #endregion

        #region Reorder
        //TODO: Fix Reorder.
        public void Reorder(ReorderData data, int userId)
        {
            var sheet = this.context.QuestionSheets
                .SingleOrDefault(x => x.Id == data.SheetId && x.IsGlobal == false);
            if (sheet == null)
            {
                throw new ServiceException("Sheet to Reorder Does not Exist!");
            }

            var user = this.context.Users
                .SingleOrDefault(x => x.Id == userId);
            if (user == null)
            {
                throw new ServiceException(Constants.UserNotFoundMessage);
            }

            if (user.Id != sheet.UserId)
            {
                throw new ServiceException("Sheet Does Not Belong To You!");
            }

            var questions = this.context.PersonalQuestionPackages
                .Where(x => x.QuestionSheetId == sheet.Id)
                .ToArray();

            var sentIds = data.Orderings.Select(x => x[0]).ToArray();

            if (sentIds.Length != sentIds.Distinct().ToArray().Length)
            {
                throw new ServiceException("Invalid Reorder Data!");
            }

            if (questions.Length != sentIds.Length)
            {
                throw new ServiceException("Invalid Reorder Data!");
            }

            if (questions.Select(x => x.Id).Any(x => !sentIds.Contains(x)))
            {
                throw new Exception("Invalid Reorder Data!");
            }

            for (int i = 0; i < questions.Length; i++)
            {
                var question = questions[i];
                var ordering = data.Orderings.Single(x => x[0] == question.Id);
                var order = ordering[1];
                var col = ordering[2];

                question.Order = order;
            }

            context.SaveChanges();
        }

        public void ReorderGlobal(ReorderData data)
        {
            var sheet = this.context.QuestionSheets
                .SingleOrDefault(x => x.Id == data.SheetId && x.IsGlobal == true);
            if (sheet == null)
            {
                throw new ServiceException("Sheet to Reorder Does not Exist!");
            }

            var questions = this.context.GlobalQuestionPackages
                .Where(x => x.QuestionSheetId == sheet.Id)
                .ToArray();

            var sentIds = data.Orderings.Select(x => x[0]).ToArray();

            if (sentIds.Length != sentIds.Distinct().ToArray().Length)
            {
                throw new ServiceException("Invalid Reorder Data!");
            }

            if (questions.Length != sentIds.Length)
            {
                throw new ServiceException("Invalid Reorder Data!");
            }

            if (questions.Select(x => x.Id).Any(x => !sentIds.Contains(x)))
            {
                throw new Exception("Invalid Reorder Data!");
            }

            for (int i = 0; i < questions.Length; i++)
            {
                var question = questions[i];
                var ordering = data.Orderings.Single(x => x[0] == question.Id);
                var order = ordering[1];
                var col = ordering[2];

                question.Order = order;
            }

            context.SaveChanges();
        }
        #endregion

        #region Approve/Reject Question
        public void ApproveQuestion(QuestionApprovalData data)
        {
            var question = context.GlobalQuestionPackages
                .SingleOrDefault(x => x.Id == data.QuestionId && x.Approved == false);
            if (question == null)
            {
                throw new ServiceException("Question Not Found!");
            }

            question.Approved = true;
            question.QuestionSheetId = data.GlobalParentSheetId;
            context.SaveChanges();
        }

        public void RejectQuestion(int questionId)
        {
            var question = context.GlobalQuestionPackages
                .SingleOrDefault(x => x.Id == questionId && x.Approved == false);
            if (question == null)
            {
                throw new ServiceException("Question Not Found!");
            }

            context.GlobalQuestionPackages.Remove(question);
            context.SaveChanges();
        }
        #endregion

        #region Other
        public void CopyQuestions(CopyQuestions data, int userId)
        {
            var user = this.context.Users.SingleOrDefault(x => x.Id == userId);
            if (user == null)
            {
                throw new ServiceException(Constants.UserNotFoundMessage);
            }

            var targetDirectory = this.context.QuestionSheets
                .Select(x=> new
                {
                    x.Id,
                    x.IsGlobal,
                    x.UserId,
                    orders = x.PersonalQuestions.Select(pq=> pq.Order).ToArray()
                })
                .SingleOrDefault(x => x.Id == data.SelectedDir && x.IsGlobal == false);

            if (targetDirectory == null)
            {
                throw new ServiceException("Target Directory Does Not Exist!");
            }

            if (targetDirectory.UserId != user.Id)
            {
                throw new ServiceException("Target Directory Does Not Belong To You!");
            }

            var globalQuestions = this.context.GlobalQuestionPackages
                .Where(x => data.SelectedQuestions.Contains(x.Id))
                .OrderBy(x => x.Order)
                .ToArray();

            var initOrder = 0;
            if(targetDirectory.orders.Length> 0)
            {
                initOrder = targetDirectory.orders.Max() + 1;
            }

            var personalQuestions = new List<PersonalQuestionPackage>();
            for (int i = 0; i < globalQuestions.Length; i++)
            {
                var globalQuestion = globalQuestions[i];
                personalQuestions.Add(new PersonalQuestionPackage
                {
                    Name = globalQuestion.Name,
                    Question = globalQuestion.Question,
                    Answer = globalQuestion.Answer,
                    Comment = globalQuestion.Comment,
                    Difficulty = globalQuestion.Difficulty,
                    AnswerRate = 0,
                    TimesBeingAnswered = 0,
                    YourBestAnswer = "This is the first time you are answering this question",
                    QuestionSheetId = targetDirectory.Id,
                    DerivedFromId = globalQuestion.Id,
                    Order = initOrder + i,
                });
            }

            this.context.PersonalQuestionPackages.AddRange(personalQuestions);
            context.SaveChanges();
        }

        const int numberOfStoredScores = 20;
        public void AddNewScore(NewScoreData data, int userId)
        {
            var score = data.Score;
            var questionId = data.QuestionId;

            if (score < 1 || score > 10)
            {
                throw new ServiceException("Score must be between 1 and 10!");
            }

            var user = context.Users.SingleOrDefault(x => x.Id == userId);
            if (user == null)
            {
                throw new ServiceException("User not found!");
            }

            var question = context.PersonalQuestionPackages.SingleOrDefault(x => x.Id == questionId);
            if (question == null)
            {
                throw new ServiceException("Question not found!");
            }

            var parentSheetUserId = context.QuestionSheets.SingleOrDefault(x => x.Id == question.QuestionSheetId)?.UserId;
            if (parentSheetUserId == null)
            {
                throw new ServiceException("Parent Sheet Not Found!");
            }

            if (parentSheetUserId != user.Id)
            {
                throw new ServiceException("Question does not beling to you!");
            }

            var scoresString = question.LatestScores;
            if (scoresString == null || scoresString.Split(",", StringSplitOptions.RemoveEmptyEntries).Length == 0)
            {
                question.LatestScores = score.ToString();
                question.AnswerRate = score;
            }
            else
            {
                var scores = scoresString.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList();

                if (scores.Count >= numberOfStoredScores)
                {
                    scores.RemoveAt(0);
                }
                scores.Add(score.ToString());
                question.LatestScores = string.Join(",", scores);

                question.AnswerRate = CalculateWaightedAvarage(scores.ToArray());
            }

            question.TimesBeingAnswered++;

            context.SaveChanges();
        }

        private float CalculateWaightedAvarage(string[] scores)
        {
            //Reversing the score so [0] is the latest score
            var intSocres = scores.Select(x => int.Parse(x)).Reverse().ToArray();
            var scoresLength = intSocres.Length;
            //taking only the latest 5 scores
            if (scoresLength > 5)
            {
                scoresLength = 5;
            }

            //60 is the waight for the latest score
            var weights = new float[] { 60, 20, 10, 5, 5 };

            //If less than 5 scores, we take the remaining weights and distribute then proportionally to 
            //to the weight who have coresponding score
            if (scoresLength < 5)
            {
                var lessBy = 5 - scoresLength;
                var weightToBeRedistributed = weights.Reverse().Take(lessBy).Sum();
                var valuesStillInPlay = weights.Take(scoresLength).ToArray();
                var newTotal = valuesStillInPlay.Sum();
                var remainderWeights = valuesStillInPlay.Select(x => x / newTotal * 100).ToArray();

                for (int i = 0; i < valuesStillInPlay.Length; i++)
                {
                    var scoreChunk = remainderWeights[i] * weightToBeRedistributed / 100f;
                    valuesStillInPlay[i] += scoreChunk;
                }

                //Redefining the weights so they are less than 5 but still sum up to 100
                weights = valuesStillInPlay;
            }

            //where we will accumulate the final rate
            float finalRate = 0;
            for (int i = 0; i < weights.Length; i++)
            {
                var scoreChunk = intSocres[i] * weights[i] / 100f;
                finalRate += scoreChunk;
            }

            return finalRate;
        }

        public void SuggestForPublishing(int personalQuestionId, int userId)
        {
            var user = context.Users
                .SingleOrDefault(x => x.Id == userId);
            if (user == null)
            {
                throw new ServiceException("User Not Found!");
            }

            var personalQuestion = context.PersonalQuestionPackages
                .SingleOrDefault(x => x.Id == personalQuestionId);
            if (personalQuestion == null)
            {
                throw new ServiceException("Question Not Found!");
            }

            var sheetUserId = context.QuestionSheets
                .SingleOrDefault(x => x.Id == personalQuestion.QuestionSheetId)?.UserId;
            if (sheetUserId == null)
            {
                throw new ServiceException("Parent Sheet Not Found!");
            }

            if (sheetUserId != user.Id)
            {
                throw new ServiceException("Question Does not belong to you!");
            }

            var proposedGlobalQuestion = Mapper.Map<GlobalQuestionPackage>(personalQuestion);
            proposedGlobalQuestion.Id = 0;
            proposedGlobalQuestion.Approved = false;
            proposedGlobalQuestion.QuestionSheetId = null;
            proposedGlobalQuestion.Order = 0;
            proposedGlobalQuestion.DerivedFromId = personalQuestion.Id;

            context.GlobalQuestionPackages.Add(proposedGlobalQuestion);
            context.SaveChanges();
        }
    }
    #endregion
}
