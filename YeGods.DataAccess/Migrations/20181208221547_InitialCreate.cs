using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using System;

namespace YeGods.DataAccess.Migrations
{
  public partial class InitialCreate : Migration
  {
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
          name: "asp_net_roles",
          columns: table => new
          {
            id = table.Column<string>(nullable: false),
            name = table.Column<string>(maxLength: 256, nullable: true),
            normalized_name = table.Column<string>(maxLength: 256, nullable: true),
            concurrency_stamp = table.Column<string>(nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("pk_asp_net_roles", x => x.id);
          });

      migrationBuilder.CreateTable(
          name: "asp_net_users",
          columns: table => new
          {
            id = table.Column<string>(nullable: false),
            user_name = table.Column<string>(maxLength: 256, nullable: true),
            normalized_user_name = table.Column<string>(maxLength: 256, nullable: true),
            email = table.Column<string>(maxLength: 256, nullable: true),
            normalized_email = table.Column<string>(maxLength: 256, nullable: true),
            email_confirmed = table.Column<bool>(nullable: false),
            password_hash = table.Column<string>(nullable: true),
            security_stamp = table.Column<string>(nullable: true),
            concurrency_stamp = table.Column<string>(nullable: true),
            phone_number = table.Column<string>(nullable: true),
            phone_number_confirmed = table.Column<bool>(nullable: false),
            two_factor_enabled = table.Column<bool>(nullable: false),
            lockout_end = table.Column<DateTimeOffset>(nullable: true),
            lockout_enabled = table.Column<bool>(nullable: false),
            access_failed_count = table.Column<int>(nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("pk_asp_net_users", x => x.id);
          });

      migrationBuilder.CreateTable(
          name: "belief_systems",
          columns: table => new
          {
            id = table.Column<int>(nullable: false)
                  .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
            is_deleted = table.Column<bool>(nullable: false),
            created_at = table.Column<DateTime>(nullable: false),
            modified_at = table.Column<DateTime>(nullable: true),
            name = table.Column<string>(nullable: true),
            geographical_region = table.Column<string>(nullable: true),
            description = table.Column<string>(nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("pk_belief_systems", x => x.id);
          });

      migrationBuilder.CreateTable(
          name: "categories",
          columns: table => new
          {
            id = table.Column<int>(nullable: false)
                  .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
            is_deleted = table.Column<bool>(nullable: false),
            created_at = table.Column<DateTime>(nullable: false),
            modified_at = table.Column<DateTime>(nullable: true),
            name = table.Column<string>(nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("pk_categories", x => x.id);
          });

      migrationBuilder.CreateTable(
          name: "glossaries",
          columns: table => new
          {
            id = table.Column<int>(nullable: false)
                  .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
            is_deleted = table.Column<bool>(nullable: false),
            created_at = table.Column<DateTime>(nullable: false),
            modified_at = table.Column<DateTime>(nullable: true),
            name = table.Column<string>(nullable: true),
            origin = table.Column<string>(nullable: true),
            description = table.Column<string>(nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("pk_glossaries", x => x.id);
          });

      migrationBuilder.CreateTable(
          name: "asp_net_role_claims",
          columns: table => new
          {
            id = table.Column<int>(nullable: false)
                  .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
            role_id = table.Column<string>(nullable: false),
            claim_type = table.Column<string>(nullable: true),
            claim_value = table.Column<string>(nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("pk_asp_net_role_claims", x => x.id);
            table.ForeignKey(
                      name: "fk_asp_net_role_claims_asp_net_roles_role_id",
                      column: x => x.role_id,
                      principalTable: "asp_net_roles",
                      principalColumn: "id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "asp_net_user_claims",
          columns: table => new
          {
            id = table.Column<int>(nullable: false)
                  .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
            user_id = table.Column<string>(nullable: false),
            claim_type = table.Column<string>(nullable: true),
            claim_value = table.Column<string>(nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("pk_asp_net_user_claims", x => x.id);
            table.ForeignKey(
                      name: "fk_asp_net_user_claims_asp_net_users_user_id",
                      column: x => x.user_id,
                      principalTable: "asp_net_users",
                      principalColumn: "id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "asp_net_user_logins",
          columns: table => new
          {
            login_provider = table.Column<string>(maxLength: 128, nullable: false),
            provider_key = table.Column<string>(maxLength: 128, nullable: false),
            provider_display_name = table.Column<string>(nullable: true),
            user_id = table.Column<string>(nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("pk_asp_net_user_logins", x => new { x.login_provider, x.provider_key });
            table.ForeignKey(
                      name: "fk_asp_net_user_logins_asp_net_users_user_id",
                      column: x => x.user_id,
                      principalTable: "asp_net_users",
                      principalColumn: "id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "asp_net_user_roles",
          columns: table => new
          {
            user_id = table.Column<string>(nullable: false),
            role_id = table.Column<string>(nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("pk_asp_net_user_roles", x => new { x.user_id, x.role_id });
            table.ForeignKey(
                      name: "fk_asp_net_user_roles_asp_net_roles_role_id",
                      column: x => x.role_id,
                      principalTable: "asp_net_roles",
                      principalColumn: "id",
                      onDelete: ReferentialAction.Cascade);
            table.ForeignKey(
                      name: "fk_asp_net_user_roles_asp_net_users_user_id",
                      column: x => x.user_id,
                      principalTable: "asp_net_users",
                      principalColumn: "id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "asp_net_user_tokens",
          columns: table => new
          {
            user_id = table.Column<string>(nullable: false),
            login_provider = table.Column<string>(maxLength: 128, nullable: false),
            name = table.Column<string>(maxLength: 128, nullable: false),
            value = table.Column<string>(nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("pk_asp_net_user_tokens", x => new { x.user_id, x.login_provider, x.name });
            table.ForeignKey(
                      name: "fk_asp_net_user_tokens_asp_net_users_user_id",
                      column: x => x.user_id,
                      principalTable: "asp_net_users",
                      principalColumn: "id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "belief_system_aliases",
          columns: table => new
          {
            id = table.Column<int>(nullable: false)
                  .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
            is_deleted = table.Column<bool>(nullable: false),
            created_at = table.Column<DateTime>(nullable: false),
            modified_at = table.Column<DateTime>(nullable: true),
            name = table.Column<string>(nullable: true),
            belief_system_id = table.Column<int>(nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("pk_belief_system_aliases", x => x.id);
            table.ForeignKey(
                      name: "fk_belief_system_aliases_belief_systems_belief_system_id",
                      column: x => x.belief_system_id,
                      principalTable: "belief_systems",
                      principalColumn: "id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "belief_system_slugs",
          columns: table => new
          {
            id = table.Column<int>(nullable: false)
                  .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
            is_deleted = table.Column<bool>(nullable: false),
            created_at = table.Column<DateTime>(nullable: false),
            modified_at = table.Column<DateTime>(nullable: true),
            name = table.Column<string>(nullable: true),
            is_default = table.Column<bool>(nullable: false),
            belief_system_id = table.Column<int>(nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("pk_belief_system_slugs", x => x.id);
            table.ForeignKey(
                      name: "fk_belief_system_slugs_belief_systems_belief_system_id",
                      column: x => x.belief_system_id,
                      principalTable: "belief_systems",
                      principalColumn: "id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "deities",
          columns: table => new
          {
            id = table.Column<int>(nullable: false)
                  .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
            is_deleted = table.Column<bool>(nullable: false),
            created_at = table.Column<DateTime>(nullable: false),
            modified_at = table.Column<DateTime>(nullable: true),
            name = table.Column<string>(nullable: true),
            origin = table.Column<string>(nullable: true),
            description = table.Column<string>(nullable: true),
            category_id = table.Column<int>(nullable: false),
            sex = table.Column<int>(nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("pk_deities", x => x.id);
            table.ForeignKey(
                      name: "fk_deities_categories_category_id",
                      column: x => x.category_id,
                      principalTable: "categories",
                      principalColumn: "id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "deity_aliases",
          columns: table => new
          {
            id = table.Column<int>(nullable: false)
                  .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
            is_deleted = table.Column<bool>(nullable: false),
            created_at = table.Column<DateTime>(nullable: false),
            modified_at = table.Column<DateTime>(nullable: true),
            name = table.Column<string>(nullable: true),
            deity_id = table.Column<int>(nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("pk_deity_aliases", x => x.id);
            table.ForeignKey(
                      name: "fk_deity_aliases_deities_deity_id",
                      column: x => x.deity_id,
                      principalTable: "deities",
                      principalColumn: "id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "deity_slugs",
          columns: table => new
          {
            id = table.Column<int>(nullable: false)
                  .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
            is_deleted = table.Column<bool>(nullable: false),
            created_at = table.Column<DateTime>(nullable: false),
            modified_at = table.Column<DateTime>(nullable: true),
            name = table.Column<string>(nullable: true),
            is_default = table.Column<bool>(nullable: false),
            deity_id = table.Column<int>(nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("pk_deity_slugs", x => x.id);
            table.ForeignKey(
                      name: "fk_deity_slugs_deities_deity_id",
                      column: x => x.deity_id,
                      principalTable: "deities",
                      principalColumn: "id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateIndex(
          name: "ix_asp_net_role_claims_role_id",
          table: "asp_net_role_claims",
          column: "role_id");

      migrationBuilder.CreateIndex(
          name: "role_name_index",
          table: "asp_net_roles",
          column: "normalized_name",
          unique: true);

      migrationBuilder.CreateIndex(
          name: "ix_asp_net_user_claims_user_id",
          table: "asp_net_user_claims",
          column: "user_id");

      migrationBuilder.CreateIndex(
          name: "ix_asp_net_user_logins_user_id",
          table: "asp_net_user_logins",
          column: "user_id");

      migrationBuilder.CreateIndex(
          name: "ix_asp_net_user_roles_role_id",
          table: "asp_net_user_roles",
          column: "role_id");

      migrationBuilder.CreateIndex(
          name: "email_index",
          table: "asp_net_users",
          column: "normalized_email");

      migrationBuilder.CreateIndex(
          name: "user_name_index",
          table: "asp_net_users",
          column: "normalized_user_name",
          unique: true);

      migrationBuilder.CreateIndex(
          name: "ix_belief_system_aliases_belief_system_id",
          table: "belief_system_aliases",
          column: "belief_system_id");

      migrationBuilder.CreateIndex(
          name: "ix_belief_system_slugs_belief_system_id",
          table: "belief_system_slugs",
          column: "belief_system_id");

      migrationBuilder.CreateIndex(
          name: "ix_deities_category_id",
          table: "deities",
          column: "category_id");

      migrationBuilder.CreateIndex(
          name: "ix_deity_aliases_deity_id",
          table: "deity_aliases",
          column: "deity_id");

      migrationBuilder.CreateIndex(
          name: "ix_deity_slugs_deity_id",
          table: "deity_slugs",
          column: "deity_id");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "asp_net_role_claims");

      migrationBuilder.DropTable(
          name: "asp_net_user_claims");

      migrationBuilder.DropTable(
          name: "asp_net_user_logins");

      migrationBuilder.DropTable(
          name: "asp_net_user_roles");

      migrationBuilder.DropTable(
          name: "asp_net_user_tokens");

      migrationBuilder.DropTable(
          name: "belief_system_aliases");

      migrationBuilder.DropTable(
          name: "belief_system_slugs");

      migrationBuilder.DropTable(
          name: "deity_aliases");

      migrationBuilder.DropTable(
          name: "deity_slugs");

      migrationBuilder.DropTable(
          name: "glossaries");

      migrationBuilder.DropTable(
          name: "asp_net_roles");

      migrationBuilder.DropTable(
          name: "asp_net_users");

      migrationBuilder.DropTable(
          name: "belief_systems");

      migrationBuilder.DropTable(
          name: "deities");

      migrationBuilder.DropTable(
          name: "categories");
    }
  }
}
