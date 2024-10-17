using CareSmartChatBot.Models;
using Microsoft.EntityFrameworkCore;

namespace CareSmartChatBot.DBContext
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
            : base(options)
        {
        }

        public DbSet<Conversation> Conversations { get; set; }
    }
}
