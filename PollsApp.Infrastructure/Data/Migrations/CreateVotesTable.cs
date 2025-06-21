using FluentMigrator;

namespace PollsApp.Infrastructure.Data.Migrations;

[Migration(20250619_004)]
public class CreateVotesTable : Migration
{
    public override void Up()
    {
        if (!Schema.Table("votes").Exists())
        {
            Create.Table("votes")
                .WithColumn("id").AsGuid().PrimaryKey()
                .WithColumn("poll_id").AsGuid().NotNullable()
                .WithColumn("poll_option_id").AsGuid().NotNullable()
                .WithColumn("user_id").AsGuid().Nullable()
                .WithColumn("voted_at").AsDateTime().NotNullable();

            Create.ForeignKey()
                .FromTable("votes").ForeignColumn("poll_id")
                .ToTable("polls").PrimaryColumn("id");

            Create.ForeignKey()
                .FromTable("votes").ForeignColumn("poll_option_id")
                .ToTable("poll_options").PrimaryColumn("id");
        }
    }

    public override void Down()
    {
        if (Schema.Table("votes").Exists())
        {
            Delete.Table("votes");
        }
    }
}