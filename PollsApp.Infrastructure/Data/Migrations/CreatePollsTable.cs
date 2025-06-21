using FluentMigrator;

namespace PollsApp.Infrastructure.Data.Migrations;

[Migration(20250619_002)]
public class CreatePollsTable : Migration
{
    public override void Up()
    {
        if (!Schema.Table("polls").Exists())
        {
            Create.Table("polls")
                .WithColumn("id").AsGuid().PrimaryKey()
                .WithColumn("title").AsString(200).NotNullable()
                .WithColumn("description").AsString(int.MaxValue).Nullable()
                .WithColumn("status").AsString(50).NotNullable()
                .WithColumn("created_by").AsGuid().NotNullable()
                .WithColumn("created_at").AsDateTime().NotNullable()
                .WithColumn("closes_at").AsDateTime().Nullable();
        }
    }

    public override void Down()
    {
        if (Schema.Table("polls").Exists())
        {
            Delete.Table("polls");
        }
    }
}