using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeManagementAPI.Data.Migrations
{
    public partial class addedpasswordhash : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "tbl_User");

            migrationBuilder.AddColumn<byte[]>(
                name: "PasswordHash",
                table: "tbl_User",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "PasswordSalt",
                table: "tbl_User",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "tbl_User");

            migrationBuilder.DropColumn(
                name: "PasswordSalt",
                table: "tbl_User");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "tbl_User",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
