﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Quibble.Host.Common.Data.Entities;

namespace Quibble.Host.Common.Repositories
{
    public interface IRoundRepository : IEntityRepository<Guid, DbRound>
    {
        public Task<List<DbRound>> GetForQuizAsync(Guid quizId);
        public Task UpdateTitleAsync(Guid id, string newTitle);
    }
}