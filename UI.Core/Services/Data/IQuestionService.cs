﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Quibble.Core.Entities;
using Quibble.UI.Core.Entities;

namespace Quibble.UI.Core.Services.Data
{
    public interface IQuestionService
    {
        public Task CreateAsync(Guid parentRoundId);

        public Task<List<DtoQuestion>> GetForQuizAsync(Guid quizId);

        public Task DeleteAsync(Guid id);

        public Task UpdateTextAsync(Guid id, string newText);
        public Task UpdateAnswerAsync(Guid id, string newAnswer);
        public Task UpdateStateAsync(Guid id, QuestionState newState);
    }
}