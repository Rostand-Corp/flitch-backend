﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAuth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "MessengerUserId",
                table: "AspNetUsers",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_MessengerUserId",
                table: "AspNetUsers",
                column: "MessengerUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Users_MessengerUserId",
                table: "AspNetUsers",
                column: "MessengerUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Users_MessengerUserId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_MessengerUserId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "MessengerUserId",
                table: "AspNetUsers");
        }
    }
}
