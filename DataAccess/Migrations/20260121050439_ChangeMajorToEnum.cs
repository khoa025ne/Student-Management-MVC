using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ChangeMajorToEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Chuyển đổi dữ liệu hiện có từ text sang int
            // Nếu có dữ liệu text, chuyển thành 0 (Undefined)
            migrationBuilder.Sql(@"
                UPDATE Students 
                SET Major = CASE 
                    WHEN Major IS NULL OR Major = '' THEN '0'
                    WHEN Major REGEXP '^[0-9]+$' THEN Major
                    ELSE '0'
                END;
            ");

            migrationBuilder.AlterColumn<int>(
                name: "Major",
                table: "Students",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Major",
                table: "Students",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
