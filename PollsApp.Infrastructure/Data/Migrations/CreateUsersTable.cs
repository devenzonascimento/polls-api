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
                .WithColumn("email").AsString(200).Unique().NotNullable()
                .WithColumn("password_hash").AsString(int.MaxValue).NotNullable()
                .WithColumn("deleted").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("created_at").AsDateTime().NotNullable().WithDefaultValue(DateTime.Now);
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
