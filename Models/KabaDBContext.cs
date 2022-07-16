using Microsoft.EntityFrameworkCore;

namespace KabaLockIntegration.Models
{
    public partial class KabaDBContext : DbContext
    {
        public KabaDBContext()
        {

        }
        public KabaDBContext(DbContextOptions<KabaDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Apilog> Apilogs { get; set; } = null!;
   
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Persist Security Info=False;Initial Catalog=PNBAlgoDB;Data Source=10.0.1.4;User ID=cdpuser;Password=Admin@2019");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<Apilog>(entity =>
            {
                entity.HasKey(e => e.RequestId)
                    .HasName("PK_LogMetadata1");

                entity.ToTable("APILog");

                entity.Property(e => e.RequestId).HasColumnName("RequestID");

                entity.Property(e => e.RequestContentType).HasMaxLength(200);

                entity.Property(e => e.RequestMethod).HasMaxLength(200);

                entity.Property(e => e.RequestTimestamp).HasColumnType("datetime");

                entity.Property(e => e.RequestUri).HasMaxLength(1000);

                entity.Property(e => e.ResponseContentType).HasMaxLength(200);

                entity.Property(e => e.ResponseStatusCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ResponseTimestamp).HasColumnType("datetime");
            });
          
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        //public virtual DbSet<Apilog> Apilogs { get; set; } = null!;
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    var configuration = new ConfigurationBuilder()
        //        .SetBasePath(Directory.GetCurrentDirectory())
        //        .AddJsonFile("appsettings.json")
        //        .Build();

        //    var connectionString = configuration.GetConnectionString("PNBAlgoDB");
        //    optionsBuilder.UseSqlServer(connectionString);
        //}

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Apilog>().HasNoKey();
        //}
    }
}
