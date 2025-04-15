using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication5.Models;

namespace WebApplication5.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Tiers> Tiers { get; set; }
        public DbSet<Contacts> Contacts { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Commercial> Commercials { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Visit> Visits { get; set; }
        public DbSet<VisitChecklist> VisitChecklists { get; set; }
        public DbSet<VisitOrderItem> VisitOrderItems { get; set; }
        public DbSet<CommercialTask> CommercialTasks { get; set; }
        public DbSet<CompetitorProduct> CompetitorProducts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderLine> OrderLines { get; set; }
        public DbSet<Quote> Quotes { get; set; }
        public DbSet<QuoteLine> QuoteLines { get; set; }
        public DbSet<Recovery> Recoveries { get; set; }
        public DbSet<Notification> Notifications { get; set; }
     

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Article>()
                .Property(a => a.PrixAchat)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Article>()
                .Property(a => a.PrixVente)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<VisitOrderItem>()
                .Property(oi => oi.UnitPriceHT)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<VisitOrderItem>()
                .Property(oi => oi.Discount)
                .HasColumnType("decimal(18,2)");
        }
    }
}