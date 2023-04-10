using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeSysUserAndMsgUserLessCoupled : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_AspNetUsers_FlitchIdentity",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_FlitchIdentity",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "FlitchIdentity",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "FlitchIdentity",
                table: "Users",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_FlitchIdentity",
                table: "Users",
                column: "FlitchIdentity",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_AspNetUsers_FlitchIdentity",
                table: "Users",
                column: "FlitchIdentity",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
