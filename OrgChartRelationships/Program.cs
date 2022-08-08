using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EFCollectionsConsole
{
    class Program
    {
        static void CreateData(OrgContext db)
        {
            var org3 = new Org { Title = "Cyber" };
            var org2 = new Org { Title = "Defense Programs" };
            var org1 = new Org { Title = "PCAE", Children = new Org[] { org2, org3 } };

            db.Add(org1);

            db.SaveChanges();
        }

        static void Main()
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var options = new DbContextOptionsBuilder<OrgContext>()
                .UseSqlServer(config.GetConnectionString("OrgDB"))
                .Options;

            using var db = new OrgContext(options);

            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            CreateData(db);

            var org = db.Orgs.Where(e => e.Title.Contains("Defense")).First();
            Console.WriteLine(org);
        }
    }

    class Org {
        public int OrgId { get; set; }
        public string Title { get; set; }
        public Org? Parent { get; set;  }
        public ICollection<Org> Children { get; set; }
    }

    class OrgContext : DbContext {
         public OrgContext()
        {

        }

        public OrgContext(DbContextOptions<OrgContext> options)
            :base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Org>().HasMany(e => e.Children).WithOne(e => e.Parent);
        }
        public DbSet<Org> Orgs { get; set; }
    }
}
