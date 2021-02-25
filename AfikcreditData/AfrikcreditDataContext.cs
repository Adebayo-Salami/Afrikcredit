using AfikcreditData.Models;
using Microsoft.EntityFrameworkCore;

namespace AfikcreditData
{
    public class AfrikcreditDataContext : DbContext
    {
        public AfrikcreditDataContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Investment> Investments { get; set; }
        public DbSet<UserInvestment> UserInvestments { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<Notification> Notifications { get; set; }
    }
}
