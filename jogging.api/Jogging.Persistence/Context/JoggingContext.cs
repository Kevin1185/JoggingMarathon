using Jogging.Persistence.Models;
using Jogging.Persistence.Models.Account;
using Jogging.Persistence.Models.Address;
using Jogging.Persistence.Models.AgeCategory;
using Jogging.Persistence.Models.Competition;
using Jogging.Persistence.Models.CompetitionPerCategory;
using Jogging.Persistence.Models.Person;
using Jogging.Persistence.Models.Registration;
using Jogging.Persistence.Models.School;
using Jogging.Persistence.Models.SearchModels.Registration;
using Jogging.Persistence.Models.SearchModels.Result;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Jogging.Persistence.Context;

public partial class JoggingContext : DbContext
{
    protected readonly IConfiguration _configuration;
    protected readonly ILogger<JoggingContext> _logger;

    public JoggingContext(IConfiguration configuration, ILogger<JoggingContext> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public JoggingContext(DbContextOptions<JoggingContext> options, IConfiguration configuration, ILogger<JoggingContext> logger)
        : base(options)
    {
        _configuration = configuration;
        _logger = logger;
    }


    public virtual DbSet<SimpleAddress> SimpleAddresses { get; set; }
    public virtual DbSet<ExtendedAddress> ExtendedAddresses { get; set; }

    public virtual DbSet<SimpleAgeCategory> Agecategories { get; set; }

    public virtual DbSet<AuthUser> AuthUsers { get; set; }

    public virtual DbSet<ExtendedCompetition> Competitions { get; set; }

    public virtual DbSet<ExtendedCompetitionpercategory> Competitionpercategories { get; set; }

    public virtual DbSet<ExtendedPerson> People { get; set; }

    public virtual DbSet<Personview> Personviews { get; set; }

    public virtual DbSet<Profile> Profiles { get; set; }

    public virtual DbSet<ExtendedRegistration> ExtendedRegistrations { get; set; }
    public virtual DbSet<SimpleRegistration> SimpleRegistrations { get; set; }

    public virtual DbSet<Runningclub> Runningclubs { get; set; }

    public virtual DbSet<ExtendedSchool> ExtendedSchools { get; set; }

    public virtual DbSet<SimpleSchool> SimpleSchools { get; set; }

    public virtual DbSet<PersonRegistration> PersonRegistrations { get; set; }

    public virtual DbSet<ExtendedRegistrationSearchByPerson> ExtendedRegistrationSearchByPeople { get; set; }

    public virtual DbSet<ExtendedResultFunctionResponse> ExtendedResultFunctions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");
        modelBuilder.Entity<PersonRegistration>().HasNoKey();
        modelBuilder.Entity<ExtendedResultFunctionResponse>().HasNoKey();

        modelBuilder.Entity<PersonRegistration>(entity =>
        {

            // Map the Id property to the Registration table
            entity.Property(pr => pr.Id)
                  .HasColumnName("Id");

            // Map RunNumber, RunTime, and Paid directly to the Registration table
            entity.Property(pr => pr.RunNumber)
                  .HasColumnName("RunNumber");
            entity.Property(pr => pr.RunTime)
                  .HasColumnName("RunTime");
            entity.Property(pr => pr.Paid)
                  .HasColumnName("Paid");

            // Map CompetitionPerCategoryId and CompetitionId to the Registration table
            entity.Property(pr => pr.CompetitionPerCategoryId)
                  .HasColumnName("CompetitionPerCategoryId");
            entity.Property(pr => pr.CompetitionId)
                  .HasColumnName("CompetitionId");

            // Map PersonId to the Registration table
            entity.Property(pr => pr.PersonId)
                  .HasColumnName("PersonId");

            //// Define relationships
            //entity.HasOne(pr => pr.Person)
            //      .WithMany(p => p.Registrations) // Assuming ExtendedPerson has a collection of registrations
            //      .HasForeignKey(pr => pr.PersonId)
            //      .OnDelete(DeleteBehavior.Cascade);

            //entity.HasOne(pr => pr.Competition)
            //      .WithMany(c => c.Registrations) // Assuming ExtendedCompetition has a collection of registrations
            //      .HasForeignKey(pr => pr.CompetitionId)
            //      .OnDelete(DeleteBehavior.Cascade);
        });


        modelBuilder.Entity<ExtendedRegistrationSearchByPerson>(entity =>
        {
            entity.HasNoKey();

            entity.ToView("extended_registration_search_by_person_view");

            entity.Property(e => e.RegistrationId).HasColumnName("RegistrationId");
            entity.Property(e => e.RunNumber).HasColumnName("RunNumber");
            entity.Property(e => e.RunTime).HasColumnName("RunTime");
            entity.Property(e => e.CompetitionPerCategoryId).HasColumnName("CompetitionPerCategoryId");
            entity.Property(e => e.Paid).HasColumnName("Paid");
            entity.Property(e => e.PersonId).HasColumnName("PersonId");
            entity.Property(e => e.CompetitionId).HasColumnName("CompetitionId");

            entity.Property(e => e.DistanceName).HasColumnName("DistanceName");

            entity.Property(e => e.LastName).HasColumnName("LastName");
            entity.Property(e => e.FirstName).HasColumnName("FirstName");
            entity.Property(e => e.BirthDate).HasColumnName("BirthDate");
            entity.Property(e => e.IbanNumber).HasColumnName("IbanNumber");
            entity.Property(e => e.SchoolId).HasColumnName("SchoolId");
            entity.Property(e => e.AddressId).HasColumnName("AddressId");
            entity.Property(e => e.Gender).HasColumnName("Gender");
            entity.Property(e => e.UserId).HasColumnName("UserId");

            entity.Property(e => e.Street).HasColumnName("Street");
            entity.Property(e => e.HouseNumber).HasColumnName("HouseNumber");
            entity.Property(e => e.City).HasColumnName("City");
            entity.Property(e => e.ZipCode).HasColumnName("ZipCode");
        });

        modelBuilder.Entity<SimpleAddress>(entity =>
        {
            entity.HasKey(e => e.AddressId).HasName("PRIMARY");

            entity.ToTable("address");

            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.HouseNumber).HasMaxLength(10);
            entity.Property(e => e.Street).HasMaxLength(100);
            entity.Property(e => e.ZipCode).HasMaxLength(10);

            entity.HasDiscriminator<string>("Discriminator")
                .HasValue<SimpleAddress>("SimpleAddress")
                .HasValue<ExtendedAddress>("ExtendedAddress");

        });

        modelBuilder.Entity<ExtendedAddress>(entity =>
        {
            //entity.ToTable("address");
            entity.HasBaseType<SimpleAddress>();

            entity.HasMany(e => e.Competitions)
                .WithOne(c => c.Address)
                .HasForeignKey(c => c.AddressId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.People)
                .WithOne(p => p.Address)
                .HasForeignKey(p => p.AddressId)
                .OnDelete(DeleteBehavior.Restrict);

        });

        modelBuilder.Entity<SimpleAgeCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("AgeCategory");

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<AuthUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("auth_users");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .HasColumnName("username");
        });

        modelBuilder.Entity<SimpleCompetition>(entity =>
        {
            entity.ToTable("Competition");
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.AddressId, "AddressId");

            entity.Property(e => e.Active).HasDefaultValue(true);
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.ImgUrl)
                .HasColumnType("text")
                .HasColumnName("img_url");
            entity.Property(e => e.Information).HasColumnType("text");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.RankingActive).HasDefaultValue(true);
            entity.Property(e => e.Url)
                .HasColumnType("text")
                .HasColumnName("url");
            entity.HasDiscriminator<string>("Discriminator")
                .HasValue<SimpleCompetition>("SimpleCompetition")
                .HasValue<ExtendedCompetition>("ExtendedCompetition");
        });
        modelBuilder.Entity<ExtendedCompetition>(entity =>
        {
            //entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Competition");
            entity.HasBaseType<SimpleCompetition>();

            //entity.HasIndex(e => e.AddressId, "AddressId");

            //entity.Property(e => e.Active).HasDefaultValueSql("'0'");
            //entity.Property(e => e.Date).HasColumnType("datetime");
            //entity.Property(e => e.ImgUrl)
            //    .HasColumnType("text")
            //    .HasColumnName("img_url");
            //entity.Property(e => e.Information).HasColumnType("text");
            //entity.Property(e => e.Name).HasMaxLength(100);
            //entity.Property(e => e.RankingActive).HasDefaultValueSql("'0'");
            //entity.Property(e => e.Url)
            //    .HasColumnType("text")
            //    .HasColumnName("url");

            entity.HasOne(d => d.Address).WithMany(p => p.Competitions)
                .HasForeignKey(d => d.AddressId)
                .HasConstraintName("competition_ibfk_1");
        });

        modelBuilder.Entity<SimpleCompetitionpercategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("CompetitionPerCategory");

            entity.HasIndex(e => e.AgeCategoryId, "AgeCategoryId");

            entity.HasIndex(e => e.CompetitionId, "CompetitionId");

            entity.Property(e => e.DistanceName).HasMaxLength(30);
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsFixedLength();
            entity.Property(e => e.GunTime).HasColumnType("datetime");
            entity.HasDiscriminator<string>("Discriminator")
                .HasValue<SimpleCompetitionpercategory>("SimpleCompetitionpercategory")
                .HasValue<ExtendedCompetitionpercategory>("ExtendedCompetitionpercategory");
        });

        modelBuilder.Entity<ExtendedCompetitionpercategory>(entity =>
        {
            //entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("CompetitionPerCategory");
            entity.HasBaseType<SimpleCompetitionpercategory>();

            //entity.HasIndex(e => e.AgeCategoryId, "AgeCategoryId");

            //entity.HasIndex(e => e.CompetitionId, "CompetitionId");

            //entity.Property(e => e.DistanceName).HasMaxLength(30);
            //entity.Property(e => e.Gender)
            //    .HasMaxLength(1)
            //    .IsFixedLength();
            //entity.Property(e => e.GunTime).HasColumnType("datetime");

            entity.HasOne(d => d.AgeCategory).WithMany(p => p.Competitionpercategories)
                .HasForeignKey(d => d.AgeCategoryId)
                .HasConstraintName("competitionpercategory_ibfk_1");

            entity.HasOne(d => d.Competition).WithMany(p => p.Competitionpercategories)
                .HasForeignKey(d => d.CompetitionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("competitionpercategory_ibfk_2");
        });

        modelBuilder.Entity<SimplePerson>(entity =>
        {

            entity.ToTable("person");

            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.AddressId, "AddressId");

            entity.HasIndex(e => e.SchoolId, "SchoolId");

            entity.HasIndex(e => e.UserId, "UserId").IsUnique();

            entity.HasIndex(e => e.FirstName, "idx_person_firstname");

            entity.HasIndex(e => e.LastName, "idx_person_lastname");

            entity.HasIndex(e => e.RunningClubId, "person_ibfk_3");

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .HasDefaultValueSql("''");
            entity.Property(e => e.Ibannumber)
                .HasMaxLength(30)
                .HasColumnName("IBANNumber");
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.HasDiscriminator<string>("Discriminator")
                .HasValue<SimplePerson>("SimplePerson")
                .HasValue<ExtendedPerson>("ExtendedPerson");
        });

        modelBuilder.Entity<ExtendedPerson>(entity =>
        {
            entity.ToTable("person");

            //entity.HasKey(e => e.Id).HasName("PRIMARY");

            //entity.HasIndex(e => e.AddressId, "AddressId");

            //entity.HasIndex(e => e.SchoolId, "SchoolId");

            //entity.HasIndex(e => e.UserId, "UserId").IsUnique();

            //entity.HasIndex(e => e.FirstName, "idx_person_firstname");

            //entity.HasIndex(e => e.LastName, "idx_person_lastname");

            //entity.HasIndex(e => e.RunningClubId, "person_ibfk_3");

            //entity.Property(e => e.Email).HasMaxLength(100);
            //entity.Property(e => e.FirstName).HasMaxLength(50);
            //entity.Property(e => e.Gender)
            //    .HasMaxLength(10)
            //    .HasDefaultValueSql("''");
            //entity.Property(e => e.Ibannumber)
            //    .HasMaxLength(30)
            //    .HasColumnName("IBANNumber");
            //entity.Property(e => e.LastName).HasMaxLength(50);

            entity.HasBaseType<SimplePerson>();

            entity.HasOne(d => d.Address).WithMany(p => p.People)
                .HasForeignKey(d => d.AddressId)
                .HasConstraintName("person_ibfk_1");

            entity.HasOne(d => d.RunningClub).WithMany(p => p.People)
                .HasForeignKey(d => d.RunningClubId)
                .HasConstraintName("person_ibfk_3");

            entity.HasOne(d => d.Profile)
                .WithOne(p => p.Person)
                .HasForeignKey<ExtendedPerson>(d => d.UserId)
                .HasPrincipalKey<Profile>(p => p.ProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.School).WithMany(p => p.People)
                .HasForeignKey(d => d.SchoolId)
                .HasConstraintName("person_ibfk_2");
            entity.HasMany(d => d.Registrations).WithOne(p => p.Person)
                .HasForeignKey(p => p.PersonId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("registration_ibfk_2");
        });



        modelBuilder.Entity<Personview>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("personview");

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.Fullname)
                .HasMaxLength(101)
                .HasDefaultValueSql("''")
                .HasColumnName("fullname");
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .HasDefaultValueSql("''");
            entity.Property(e => e.Ibannumber)
                .HasMaxLength(30)
                .HasColumnName("IBANNumber");
            entity.Property(e => e.LastName).HasMaxLength(50);
        });

        modelBuilder.Entity<Profile>(entity =>
        {
            entity.HasKey(e => e.ProfileId).HasName("PRIMARY");

            entity.ToTable("Profile");

            entity.Property(e => e.ProfileId)
                .HasColumnName("id")
                .HasColumnType("char(36)");
            //.HasConversion(
            //    v => v.ToString(), 
            //    v => Guid.Parse(v)); ;

            entity.HasIndex(e => e.ProfileId)
                .IsUnique();

            entity.Property(e => e.Role)
                .HasMaxLength(255)
                .HasColumnName("role");

            entity.HasOne(p => p.Person)
              .WithOne(e => e.Profile)
              .HasForeignKey<ExtendedPerson>(e => e.UserId);
        });

        modelBuilder.Entity<SimpleRegistration>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Registration");

            entity.HasIndex(e => e.CompetitionId, "CompetitionId");

            entity.HasIndex(e => e.CompetitionPerCategoryId, "CompetitionPerCategoryId");

            entity.HasIndex(e => e.PersonId, "PersonId");

            entity.HasIndex(e => new { e.RunNumber, e.CompetitionId }, "RunNumber").IsUnique();

            entity.Property(e => e.RunTime).HasColumnType("text");

            entity.HasDiscriminator<string>("Discriminator")
                .HasValue<SimpleRegistration>("SimpleRegistration")
                .HasValue<ExtendedRegistration>("ExtendedRegistration");
        });

        modelBuilder.Entity<ExtendedRegistration>(entity =>
        {
            //entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Registration");
            entity.HasBaseType<SimpleRegistration>();

            //entity.HasIndex(e => e.CompetitionId, "CompetitionId");

            //entity.HasIndex(e => e.CompetitionPerCategoryId, "CompetitionPerCategoryId");

            //entity.HasIndex(e => e.PersonId, "PersonId");

            //entity.HasIndex(e => new { e.RunNumber, e.CompetitionId }, "RunNumber").IsUnique();

            //entity.Property(e => e.RunTime).HasColumnType("text");

            entity.HasOne(d => d.Competition).WithMany(p => p.Registrations)
                .HasForeignKey(d => d.CompetitionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("registration_ibfk_3");

            entity.HasOne(d => d.CompetitionPerCategory).WithMany(p => p.Registrations)
                .HasForeignKey(d => d.CompetitionPerCategoryId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("registration_ibfk_1");
        });

        modelBuilder.Entity<Runningclub>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("runningclub");

            entity.Property(e => e.Logo).HasColumnType("blob");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Url).HasMaxLength(200);
        });

        modelBuilder.Entity<SimpleSchool>(entity =>
        {
            entity.HasKey(e => e.SchoolId).HasName("PRIMARY");

            entity.ToTable("school");

            entity.Property(e => e.Name).HasMaxLength(100);
            entity.HasDiscriminator<string>("Discriminator")
                .HasValue<SimpleSchool>("SimpleSchool")
                .HasValue<ExtendedSchool>("ExtendedSchool");

        });

        modelBuilder.Entity<ExtendedSchool>(entity =>
        {
            //entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("school");
            entity.HasBaseType<SimpleSchool>();

            //entity.Property(e => e.Name).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
