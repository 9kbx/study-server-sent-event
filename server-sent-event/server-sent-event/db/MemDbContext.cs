using Microsoft.EntityFrameworkCore;

namespace server_sent_event.db
{
    public class MemDbContext : DbContext
    {

        public DbSet<ChatMsg>? ChatMsgs
        {
            get; set;
        }

        public MemDbContext(DbContextOptions<MemDbContext> options)
            : base(options)
        {

        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);
        //    modelBuilder.Entity<ChatMsg>().HasNoKey();
        //}
    }

    public class ChatMsg {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Msg { get; set; }
        public DateTime CreateTime { get; set; }
    
    }

}
