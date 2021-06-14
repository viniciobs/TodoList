using Domains;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace DataAccess.Migrations
{
	public partial class SeedUserWithAdminDefaultData : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql($"INSERT INTO Users (Id, Name, Login, Password, CreatedAt, Role) VALUES ('{Guid.NewGuid()}', 'Admin', 'admin', 'admin1234', '{DateTime.UtcNow.ToString("yyyy-MM-dd hh:mm:ss")}', {Convert.ToInt16(UserRole.Admin)})");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql($"DELETE FROM Users WHERE Name='Admin' AND Login='admin'");
		}
	}
}