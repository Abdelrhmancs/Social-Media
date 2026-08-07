using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddChatSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "conversations",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    type = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    group_picture = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_conversations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "conversation_members",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    conversation_id = table.Column<long>(type: "bigint", nullable: false),
                    user_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    role = table.Column<int>(type: "int", nullable: false),
                    joined_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_conversation_members", x => x.Id);
                    table.ForeignKey(
                        name: "FK_conversation_members_AspNetUsers_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_conversation_members_conversations_conversation_id",
                        column: x => x.conversation_id,
                        principalTable: "conversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "messages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    conversation_id = table.Column<long>(type: "bigint", nullable: false),
                    sender_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    content = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    type = table.Column<int>(type: "int", nullable: false),
                    sent_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_messages_AspNetUsers_sender_id",
                        column: x => x.sender_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_messages_conversations_conversation_id",
                        column: x => x.conversation_id,
                        principalTable: "conversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "message_read_receipts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    message_id = table.Column<long>(type: "bigint", nullable: false),
                    user_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    read_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_message_read_receipts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_message_read_receipts_AspNetUsers_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_message_read_receipts_messages_message_id",
                        column: x => x.message_id,
                        principalTable: "messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_conversation_members_conversation_id_user_id",
                table: "conversation_members",
                columns: new[] { "conversation_id", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_conversation_members_user_id",
                table: "conversation_members",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_message_read_receipts_message_id_user_id",
                table: "message_read_receipts",
                columns: new[] { "message_id", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_message_read_receipts_user_id",
                table: "message_read_receipts",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_messages_conversation_id",
                table: "messages",
                column: "conversation_id");

            migrationBuilder.CreateIndex(
                name: "IX_messages_sender_id",
                table: "messages",
                column: "sender_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "conversation_members");

            migrationBuilder.DropTable(
                name: "message_read_receipts");

            migrationBuilder.DropTable(
                name: "messages");

            migrationBuilder.DropTable(
                name: "conversations");
        }
    }
}
