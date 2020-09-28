using Microsoft.EntityFrameworkCore;
using Quibble.Host.Common.Data;

namespace Quibble.Host.Hosted.Data
{
    public class QuibbleServerSideDbContext : QuibbleDbContext
    {
        public QuibbleServerSideDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}