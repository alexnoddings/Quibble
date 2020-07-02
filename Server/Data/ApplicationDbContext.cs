using System;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Quibble.Server.Models.Participants;
using Quibble.Server.Models.Questions;
using Quibble.Server.Models.Quizzes;
using Quibble.Server.Models.Rounds;
using Quibble.Server.Models.Users;

namespace Quibble.Server.Data
{
    /// <summary>
    /// Entity Framework database context used for data storage.
    /// </summary>
    public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
    {
        // DbSets are initialised as default! as EntityFramework will initialise them for us
        /// <summary>
        /// Gets or sets the <see cref="DbSet{Quiz}"/>.
        /// </summary>
        public DbSet<Quiz> Quizzes { get; set; } = default!;
        
        /// <summary>
        /// Gets or sets the <see cref="DbSet{Round}"/>.
        /// </summary>
        public DbSet<Round> Rounds { get; set; } = default!;
        
        /// <summary>
        /// Gets or sets the <see cref="DbSet{Question}"/>.
        /// </summary>
        public DbSet<Question> Questions { get; set; } = default!;
        
        /// <summary>
        /// Gets or sets the <see cref="DbSet{Participant}"/>.
        /// </summary>
        public DbSet<Participant> Participants { get; set; } = default!;
        
        /// <summary>
        /// Gets or sets the <see cref="DbSet{SubmittedAnswer}"/>.
        /// </summary>
        public DbSet<SubmittedAnswer> SubmittedAnswers { get; set; } = default!;

        /// <summary>
        /// Gets or sets the <see cref="DbSet{UserSettings}"/>.
        /// </summary>
        public DbSet<UserSettings> UserSettings { get; set; } = default!;

        /// <summary>
        /// Initializes a new instance of <see cref="ApplicationDbContext"/>.
        /// </summary>
        /// <param name="options">The <see cref="DbContextOptions"/>.</param>
        /// <param name="operationalStoreOptions">The <see cref="IOptions{OperationalStoreOptions}"/>.</param>
        public ApplicationDbContext(DbContextOptions options, IOptions<OperationalStoreOptions> operationalStoreOptions) 
            : base(options, operationalStoreOptions)
        {
        }

        /// <summary>
        /// Configures the schema for the database model.
        /// </summary>
        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder = modelBuilder ?? throw new ArgumentNullException(nameof(modelBuilder));

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Quiz>(entityBuilder =>
            {
                entityBuilder.HasKey(quiz => quiz.Id);

                entityBuilder
                    .HasOne<ApplicationUser>()
                    .WithMany()
                    .HasForeignKey(quiz => quiz.OwnerId)
                    .IsRequired();

                entityBuilder
                    .HasMany<Round>()
                    .WithOne()
                    .HasForeignKey(round => round.QuizId)
                    .IsRequired();

                entityBuilder
                    .HasMany<Participant>()
                    .WithOne()
                    .HasForeignKey(participant => participant.QuizId)
                    .IsRequired();

                entityBuilder.ToTable("Quizzes");
            });

            modelBuilder.Entity<Round>(entityBuilder =>
            {
                entityBuilder.HasKey(round => round.Id);

                entityBuilder
                    .HasOne<Quiz>()
                    .WithMany()
                    .HasForeignKey(round => round.QuizId)
                    .IsRequired();

                entityBuilder
                    .HasMany<Question>()
                    .WithOne()
                    .HasForeignKey(question => question.RoundId)
                    .IsRequired();

                entityBuilder.ToTable("QuizRounds");
            });

            modelBuilder.Entity<Question>(entityBuilder =>
            {
                entityBuilder.HasKey(question => question.Id);

                entityBuilder
                    .HasOne<Round>()
                    .WithMany()
                    .HasForeignKey(question => question.RoundId)
                    .IsRequired();

                entityBuilder
                    .HasMany<SubmittedAnswer>()
                    .WithOne()
                    .HasForeignKey(submittedAnswer => submittedAnswer.QuestionId)
                    .IsRequired();

                entityBuilder.ToTable("QuizQuestions");
            });

            modelBuilder.Entity<Participant>(entityBuilder =>
            {
                entityBuilder.HasKey(participant => participant.Id);

                entityBuilder
                    .HasOne<ApplicationUser>()
                    .WithMany()
                    .HasForeignKey(participant => participant.UserId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entityBuilder
                    .HasOne<Quiz>()
                    .WithMany()
                    .HasForeignKey(participant => participant.QuizId)
                    .IsRequired();

                entityBuilder
                    .HasMany<SubmittedAnswer>()
                    .WithOne()
                    .HasForeignKey(submittedAnswer => submittedAnswer.ParticipantId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entityBuilder.ToTable("QuizParticipants");
            });

            modelBuilder.Entity<SubmittedAnswer>(entityBuilder =>
            {
                entityBuilder.HasKey(submittedAnswer => submittedAnswer.Id);

                entityBuilder
                    .HasOne<Participant>()
                    .WithMany()
                    .HasForeignKey(submittedAnswer => submittedAnswer.ParticipantId)
                    .IsRequired();

                entityBuilder
                    .HasOne<Question>()
                    .WithMany()
                    .HasForeignKey(submittedAnswer => submittedAnswer.QuestionId)
                    .IsRequired();

                entityBuilder.ToTable("QuizSubmittedAnswers");
            });

            modelBuilder.Entity<UserSettings>(entityBuilder =>
            {
                entityBuilder.HasKey(userSettings => userSettings.Id);

                entityBuilder
                    .HasOne<ApplicationUser>()
                    .WithMany()
                    .HasForeignKey(userSettings => userSettings.UserId)
                    .IsRequired();

                entityBuilder.ToTable("AspNetUserSettings");
            });
        }
    }
}
