using Microsoft.EntityFrameworkCore.Migrations;

namespace AfrikcreditData.Migrations
{
    public partial class newfeature : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "AmountToWithdraw",
                table: "UserInvestments",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "IsWithdrawalAllowed",
                table: "Investments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountToWithdraw",
                table: "UserInvestments");

            migrationBuilder.DropColumn(
                name: "IsWithdrawalAllowed",
                table: "Investments");
        }
    }
}
