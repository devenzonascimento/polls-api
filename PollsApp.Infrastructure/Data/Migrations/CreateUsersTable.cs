using FluentMigrator;

namespace PollsApp.Infrastructure.Data.Migrations;

[Migration(20250619_001)]
public class CreateUsersTable : Migration
{
    public override void Up()
    {
        if (!Schema.Table("users").Exists())
        {
            Create.Table("users")
                .WithColumn("id").AsGuid().PrimaryKey()
                .WithColumn("username").AsString(100).NotNullable()
                .WithColumn("email").AsString(256).Unique().NotNullable()
                .WithColumn("password_hash").AsString(500).NotNullable()
                .WithColumn("is_deleted").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("created_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);
        }
    }

    public override void Down()
    {
        if (Schema.Table("users").Exists())
        {
            Delete.Table("users");
        }
    }
}
