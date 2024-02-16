using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TED.API.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LimiteTed",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ValorMaximoDia = table.Column<double>(type: "float", nullable: false),
                    QuantidadeMaximaDia = table.Column<int>(type: "int", nullable: false),
                    ValorMaximoPorSaque = table.Column<double>(type: "float", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LimiteTed", x => x.Id);
                });

            // Inserindo dados na tabela
            migrationBuilder.InsertData(
                table: "LimiteTed",
                columns: new[] { "ValorMaximoDia", "QuantidadeMaximaDia", "ValorMaximoPorSaque", "CriadoEm", "AtualizadoEm" },
                values: new object[] { 10000.00, 15, 5000.00, DateTime.UtcNow, DateTime.UtcNow }
            );

            migrationBuilder.CreateTable(
                name: "Ted",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ClienteId = table.Column<int>(type: "int", nullable: false),
                    NomeCliente = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataAgendamento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValorSolicitado = table.Column<double>(type: "float", nullable: false),
                    NumeroAgencia = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumeroConta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DigitoConta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumeroBanco = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NomeBanco = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MotivoReprovacao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SinacorConfirmacaoId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ted", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ted_ClienteId",
                table: "Ted",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Ted_Status",
                table: "Ted",
                column: "Status");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LimiteTed");

            migrationBuilder.DropTable(
                name: "Ted");
        }
    }
}
