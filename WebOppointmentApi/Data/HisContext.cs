using Microsoft.EntityFrameworkCore;
using WebOppointmentApi.Models;


namespace WebOppointmentApi.Data
{
    public class HisContext : DbContext
    {

        public HisContext(DbContextOptions<HisContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
          
        }
    }
}
