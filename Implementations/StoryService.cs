
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using ProjectName.ControllersExceptions;
using ProjectName.Interfaces;
using ProjectName.Types;

namespace ProjectName.Services
{
    public class StoryService : IStoryService
    {
        private readonly IDbConnection _dbConnection;

        public StoryService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> CreateStory(CreateStoryDto createStoryDto)
        {
            ValidateCreateStoryDto(createStoryDto);

            var steps = await FetchSteps(createStoryDto.Steps);
            ValidateSteps(steps, createStoryDto.Steps);

            var completedCondition = await FetchCompletedCondition(createStoryDto.CompletedCondition);
            ValidateCompletedCondition(completedCondition, createStoryDto.CompletedCondition);

            var story = new Story
            {
                Id = Guid.NewGuid(),
                Title = createStoryDto.Title,
                Description = createStoryDto.Description,
                Number = createStoryDto.Number,
                CompletedCondition = completedCondition?.Id,
                Created = DateTime.UtcNow,
                Changed = DateTime.UtcNow,
                Steps = steps
            };

            var storySteps = createStoryDto.Steps.Select(stepId => new StoryStep
            {
                Id = Guid.NewGuid(),
                StoryId = story.Id,
                StepId = stepId
            }).ToList();

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    await _dbConnection.ExecuteAsync(
                        "INSERT INTO Stories (Id, Title, Description, Number, CompletedCondition, Created, Changed) VALUES (@Id, @Title, @Description, @Number, @CompletedCondition, @Created, @Changed)",
                        story, transaction);

                    await _dbConnection.ExecuteAsync(
                        "INSERT INTO StorySteps (Id, StoryId, StepId) VALUES (@Id, @StoryId, @StepId)",
                        storySteps, transaction);

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "A technical exception has occurred, please contact your system administrator");
                }
            }

            return story.Id.ToString();
        }

        public async Task<Story> GetStory(RequestStoryDto requestStoryDto)
        {
            ValidateRequestStoryDto(requestStoryDto);

            Story story;
            if (requestStoryDto.Id.HasValue)
            {
                story = await FetchStoryById(requestStoryDto.Id.Value);
            }
            else if (!string.IsNullOrEmpty(requestStoryDto.Title))
            {
                story = await FetchStoryByTitle(requestStoryDto.Title);
            }
            else
            {
                throw new BusinessException("DP-422", "Invalid request payload");
            }

            if (story == null)
            {
                throw new TechnicalException("DP-404", "Story not found");
            }

            var steps = await FetchSteps(story.Steps.Select(s => s.Id).ToList());
            ValidateSteps(steps, story.Steps.Select(s => s.Id).ToList());

            var completedCondition = await FetchCompletedCondition(story.CompletedCondition);
            ValidateCompletedCondition(completedCondition, story.CompletedCondition);

            story.Steps = steps;
            return story;
        }

        public async Task<string> UpdateStory(UpdateStoryDto updateStoryDto)
        {
            ValidateUpdateStoryDto(updateStoryDto);

            var existingStory = await FetchStoryById(updateStoryDto.Id);
            if (existingStory == null)
            {
                throw new TechnicalException("DP-404", "Story not found");
            }

            var steps = await FetchSteps(updateStoryDto.Steps);
            ValidateSteps(steps, updateStoryDto.Steps);

            var completedCondition = await FetchCompletedCondition(updateStoryDto.CompletedCondition);
            ValidateCompletedCondition(completedCondition, updateStoryDto.CompletedCondition);

            existingStory.Title = updateStoryDto.Title;
            existingStory.Description = updateStoryDto.ShortDescription;
            existingStory.Number = updateStoryDto.StoryNumber;
            existingStory.CompletedCondition = completedCondition?.Id;
            existingStory.Changed = DateTime.UtcNow;

            var existingSteps = await FetchStorySteps(existingStory.Id);
            var stepsToRemove = existingSteps.Where(es => !updateStoryDto.Steps.Contains(es.StepId)).ToList();
            var stepsToAdd = updateStoryDto.Steps.Where(us => !existingSteps.Select(es => es.StepId).Contains(us)).ToList();

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    await _dbConnection.ExecuteAsync(
                        "UPDATE Stories SET Title = @Title, Description = @Description, Number = @Number, CompletedCondition = @CompletedCondition, Changed = @Changed WHERE Id = @Id",
                        existingStory, transaction);

                    if (stepsToRemove.Any())
                    {
                        await _dbConnection.ExecuteAsync(
                            "DELETE FROM StorySteps WHERE Id IN @Ids",
                            new { Ids = stepsToRemove.Select(s => s.Id) }, transaction);
                    }

                    if (stepsToAdd.Any())
                    {
                        var newStorySteps = stepsToAdd.Select(stepId => new StoryStep
                        {
                            Id = Guid.NewGuid(),
                            StoryId = existingStory.Id,
                            StepId = stepId
                        }).ToList();

                        await _dbConnection.ExecuteAsync(
                            "INSERT INTO StorySteps (Id, StoryId, StepId) VALUES (@Id, @StoryId, @StepId)",
                            newStorySteps, transaction);
                    }

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "A technical exception has occurred, please contact your system administrator");
                }
            }

            return existingStory.Id.ToString();
        }

        public async Task<bool> DeleteStory(DeleteStoryDto deleteStoryDto)
        {
            ValidateDeleteStoryDto(deleteStoryDto);

            var existingStory = await FetchStoryById(deleteStoryDto.Id);
            if (existingStory == null)
            {
                throw new TechnicalException("DP-404", "Story not found");
            }

            var storySteps = await FetchStorySteps(existingStory.Id);

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    await _dbConnection.ExecuteAsync(
                        "DELETE FROM StorySteps WHERE StoryId = @StoryId",
                        new { StoryId = existingStory.Id }, transaction);

                    await _dbConnection.ExecuteAsync(
                        "DELETE FROM Stories WHERE Id = @Id",
                        new { Id = existingStory.Id }, transaction);

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "A technical exception has occurred, please contact your system administrator");
                }
            }

            return true;
        }

        public async Task<List<Story>> GetListStory(ListStoryRequestDto listStoryRequestDto)
        {
            ValidateListStoryRequestDto(listStoryRequestDto);

            var stories = await FetchStories(listStoryRequestDto);

            foreach (var story in stories)
            {
                var steps = await FetchSteps(story.Steps.Select(s => s.Id).ToList());
                ValidateSteps(steps, story.Steps.Select(s => s.Id).ToList());

                var completedCondition = await FetchCompletedCondition(story.CompletedCondition);
                ValidateCompletedCondition(completedCondition, story.CompletedCondition);

                story.Steps = steps;
            }

            return stories;
        }

        private void ValidateCreateStoryDto(CreateStoryDto createStoryDto)
        {
            if (string.IsNullOrEmpty(createStoryDto.Title) || createStoryDto.Number == 0 || createStoryDto.Steps == null || !createStoryDto.Steps.Any())
            {
                throw new BusinessException("DP-422", "Invalid request payload");
            }
        }

        private void ValidateRequestStoryDto(RequestStoryDto requestStoryDto)
        {
            if (!requestStoryDto.Id.HasValue && string.IsNullOrEmpty(requestStoryDto.Title))
            {
                throw new BusinessException("DP-422", "Invalid request payload");
            }
        }

        private void ValidateUpdateStoryDto(UpdateStoryDto updateStoryDto)
        {
            if (updateStoryDto.Id == Guid.Empty || string.IsNullOrEmpty(updateStoryDto.Title) || updateStoryDto.StoryNumber == 0)
            {
                throw new BusinessException("DP-422", "Invalid request payload");
            }
        }

        private void ValidateDeleteStoryDto(DeleteStoryDto deleteStoryDto)
        {
            if (deleteStoryDto.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Invalid request payload");
            }
        }

        private void ValidateListStoryRequestDto(ListStoryRequestDto listStoryRequestDto)
        {
            if (listStoryRequestDto.PageLimit <= 0 || listStoryRequestDto.PageOffset < 0)
            {
                throw new BusinessException("DP-422", "Invalid request payload");
            }
        }

        private async Task<List<Step>> FetchSteps(List<Guid> stepIds)
        {
            return (await _dbConnection.QueryAsync<Step>("SELECT * FROM Steps WHERE Id IN @Ids", new { Ids = stepIds })).ToList();
        }

        private void ValidateSteps(List<Step> steps, List<Guid> stepIds)
        {
            if (steps.Count != stepIds.Count)
            {
                throw new TechnicalException("DP-404", "One or more steps not found");
            }
        }

        private async Task<GettingStartedCompletedCondition> FetchCompletedCondition(Guid? completedConditionId)
        {
            if (!completedConditionId.HasValue)
            {
                return null;
            }

            return await _dbConnection.QuerySingleOrDefaultAsync<GettingStartedCompletedCondition>("SELECT * FROM GettingStartedCompletedConditions WHERE Id = @Id", new { Id = completedConditionId });
        }

        private void ValidateCompletedCondition(GettingStartedCompletedCondition completedCondition, Guid? completedConditionId)
        {
            if (completedConditionId.HasValue && completedCondition == null)
            {
                throw new TechnicalException("DP-404", "Completed condition not found");
            }
        }

        private async Task<Story> FetchStoryById(Guid id)
        {
            return await _dbConnection.QuerySingleOrDefaultAsync<Story>("SELECT * FROM Stories WHERE Id = @Id", new { Id = id });
        }

        private async Task<Story> FetchStoryByTitle(string title)
        {
            return await _dbConnection.QuerySingleOrDefaultAsync<Story>("SELECT * FROM Stories WHERE Title = @Title", new { Title = title });
        }

        private async Task<List<StoryStep>> FetchStorySteps(Guid storyId)
        {
            return (await _dbConnection.QueryAsync<StoryStep>("SELECT * FROM StorySteps WHERE StoryId = @StoryId", new { StoryId = storyId })).ToList();
        }

        private async Task<List<Story>> FetchStories(ListStoryRequestDto listStoryRequestDto)
        {
            var query = "SELECT * FROM Stories";
            if (!string.IsNullOrEmpty(listStoryRequestDto.SortField) && !string.IsNullOrEmpty(listStoryRequestDto.SortOrder))
            {
                query += $" ORDER BY {listStoryRequestDto.SortField} {listStoryRequestDto.SortOrder}";
            }
            query += " OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY";

            return (await _dbConnection.QueryAsync<Story>(query, new { Offset = listStoryRequestDto.PageOffset, Limit = listStoryRequestDto.PageLimit })).ToList();
        }
    }
}
