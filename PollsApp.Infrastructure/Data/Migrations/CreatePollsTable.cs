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
                .WithColumn("is_open").AsBoolean().NotNullable().WithDefaultValue(true)
                .WithColumn("is_deleted").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("created_by").AsGuid().NotNullable()
                .WithColumn("created_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
                .WithColumn("closed_at").AsDateTime().Nullable()
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