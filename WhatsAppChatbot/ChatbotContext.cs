using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

public class ChatbotContext : DbContext
{

    public ChatbotContext(DbContextOptions<ChatbotContext> options)
           : base(options)
    {
    }

    public DbSet<UserMessage> UserMessages { get; set; }
    public DbSet<BotResponse> BotResponses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            //optionsBuilder.UseMySQL("server=127.0.0.1;user=root;password=Steeprise@123;database=wpchatbot;");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserMessage>(entity =>
        {
            entity.ToTable("UserMessages").HasKey("Id");
        });

        modelBuilder.Entity<BotResponse>(entity =>
        {
            entity.ToTable("BotResponses").HasKey("Id");
        });
    }
}

public class UserMessage
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public string Message { get; set; }
    public DateTime Timestamp { get; set; }
}

public class BotResponse
{
    public int Id { get; set; }
    public int UserMessageId { get; set; }
    public string Response { get; set; }
    public DateTime Timestamp { get; set; }
}
