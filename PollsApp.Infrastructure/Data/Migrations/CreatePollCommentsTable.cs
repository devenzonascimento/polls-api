using FluentMigrator;

namespace PollsApp.Infrastructure.Data.Migrations;

[Migration(20250708_001)]
public class CreatePollCommentsTable : Migration
{
    public override void Up()
    {
        if (!Schema.Table("poll_comments").Exists())
        {
            Create.Table("poll_comments")
                .WithColumn("id").AsGuid().PrimaryKey()
                .WithColumn("poll_id").AsGuid().NotNullable()
                .WithColumn("text").AsString(500).NotNullable()
                .WithColumn("is_edited").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("reply_to").AsGuid().Nullable()
                .WithColumn("created_by").AsGuid().NotNullable()
                .WithColumn("is_deleted").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("created_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);

            Create.ForeignKey()
                .FromTable("poll_comments").ForeignColumn("poll_id")
                .ToTable("polls").PrimaryColumn("id");

            Create.ForeignKey()
                .FromTable("poll_comments").ForeignColumn("reply_to")
                .ToTable("poll_comments").PrimaryColumn("id");

            Create.ForeignKey()
                .FromTable("poll_comments").ForeignColumn("created_by")
                .ToTable("users").PrimaryColumn("id");
        }
    }

    public override void Down()
    {
        if (Schema.Table("poll_comments").Exists())
        {
            Delete.Table("poll_comments");
        }
    }
}
