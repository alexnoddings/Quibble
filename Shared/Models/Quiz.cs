using System;

namespace Quibble.Shared.Models
{
    public class Quiz : IEntity
    {
        public Guid Id { get; set; }
        public Guid OwnerId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime? Created { get; set; }
        public DateTime? Opened { get; set; }
    }
}
