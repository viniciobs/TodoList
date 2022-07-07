using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
	public partial class AlterUserProperties : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropIndex(
				name: "IX_User_Email",
				table: "Users");

			migrationBuilder.DropColumn(
				name: "Email",
				table: "Users");

			migrationBuilder.RenameColumn(
				name: "LastName",
				table: "Users",
				newName: "Login");

			migrationBuilder.RenameColumn(
				name: "FirstName",
				table: "Users",
				newName: "Name");

			migrationBuilder.CreateIndex(
				name: "IX_User_Login",
				table: "Users",
				column: "Login",
				unique: true);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropIndex(
				name: "IX_User_Login",
				table: "Users");

			migrationBuilder.RenameColumn(
				name: "Name",
				table: "Users",
				newName: "FirstName");

			migrationBuilder.RenameColumn(
				name: "Login",
				table: "Users",
				newName: "LastName");

			migrationBuilder.AddColumn<string>(
				name: "Email",
				table: "Users",
				type: "varchar(50)",
				unicode: false,
				maxLength: 50,
				nullable: false,
				defaultValue: "");

			migrationBuilder.CreateIndex(
				name: "IX_User_Email",
				table: "Users",
				column: "Email",
				unique: true);
		}
	}
}