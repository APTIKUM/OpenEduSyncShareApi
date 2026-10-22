using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenEduSyncShare.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangeAnswerToJsonb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                                 ALTER TABLE "Answers"
                                 ALTER COLUMN answer TYPE jsonb
                                 USING (
                                     CASE
                                         WHEN answer LIKE '%&%'
                                             THEN to_jsonb(string_to_array(answer, '&'))
                                         ELSE to_jsonb(answer)
                                     END
                                 );
                                 """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                                 ALTER TABLE "Answers"
                                 ALTER COLUMN answer TYPE text
                                 USING answer::text;
                                 """);
        }
    }
}
