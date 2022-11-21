using HazelcastDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace HazelcastDemo.Common
{
    public class HazelCastContext : DbContext
    {
        public HazelCastContext(DbContextOptions<HazelCastContext> options) : base(options)
        {
        }
        public DbSet<HazelcastModel> hazelcasts { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseOracle("USER ID=hazelcast01;Password=hazelcast01;DATA SOURCE=10.26.7.214:1521/hazelcast01");
        }

    }
}
    