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
                .WithColumn("description").AsString(500).Nullable()
                .WithColumn("active").AsBoolean().NotNullable().WithDefaultValue(true)
                .WithColumn("deleted").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("created_by").AsGuid().NotNullable()
                .WithColumn("created_at").AsDateTime().NotNullable().WithDefaultValue(DateTime.Now)
                .WithColumn("closes_at").AsDateTime().Nullable();

            Create.ForeignKey()
                .FromTable("polls").ForeignColumn("created_by")
                .ToTable("users").PrimaryColumn("id");
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