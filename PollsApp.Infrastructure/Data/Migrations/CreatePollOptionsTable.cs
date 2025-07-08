using FluentMigrator;

namespace PollsApp.Infrastructure.Data.Migrations;

[Migration(20250619_003)]
public class CreatePollOptionsTable : Migration
{
    public override void Up()
    {
        if (!Schema.Table("poll_options").Exists())
        {
            Create.Table("poll_options")
                .WithColumn("id").AsGuid().PrimaryKey()
                .WithColumn("poll_id").AsGuid().NotNullable()
                .WithColumn("text").AsString(255).NotNullable()
                .WithColumn("order").AsInt16().NotNullable();

            Create.ForeignKey()
                .FromTable("poll_options").ForeignColumn("poll_id")
                .ToTable("polls").PrimaryColumn("id");

            Create.UniqueConstraint()
                .OnTable("poll_options").Columns("poll_id", "text");

            Create.UniqueConstraint()
                .OnTable("poll_options").Columns("poll_id", "order");
        }
    }

    public override void Down()
    {
        if (Schema.Table("poll_options").Exists())
        {
            Delete.Table("poll_options");
        }
    }
}