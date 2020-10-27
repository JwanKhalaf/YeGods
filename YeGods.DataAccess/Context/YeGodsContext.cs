namespace YeGods.DataAccess
{
  using Domain;
  using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
  using Microsoft.EntityFrameworkCore;
  using Microsoft.EntityFrameworkCore.Metadata;

  public class YeGodsContext : IdentityDbContext
  {
    public YeGodsContext(DbContextOptions<YeGodsContext> options)
      : base(options)
    {
    }

    public DbSet<Deity> Deities { get; set; }

    public DbSet<DeityAlias> DeityAliases { get; set; }

    public DbSet<DeitySlug> DeitySlugs { get; set; }

    public DbSet<Category> Categories { get; set; }

    public DbSet<BeliefSystem> BeliefSystems { get; set; }

    public DbSet<BeliefSystemAlias> BeliefSystemAliases { get; set; }

    public DbSet<BeliefSystemSlug> BeliefSystemSlugs { get; set; }

    public DbSet<Glossary> Glossaries { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);

      foreach (IMutableEntityType entity in builder.Model.GetEntityTypes())
      {
        // replace table names
        entity.Relational().TableName = entity.Relational().TableName.ToSnakeCase();

        // replace column names            
        foreach (IMutableProperty property in entity.GetProperties())
        {
          property.Relational().ColumnName = property.Name.ToSnakeCase();
        }

        foreach (IMutableKey key in entity.GetKeys())
        {
          key.Relational().Name = key.Relational().Name.ToSnakeCase();
        }

        foreach (IMutableForeignKey key in entity.GetForeignKeys())
        {
          key.Relational().Name = key.Relational().Name.ToSnakeCase();
        }

        foreach (IMutableIndex index in entity.GetIndexes())
        {
          index.Relational().Name = index.Relational().Name.ToSnakeCase();
        }
      }
    }
  }
}
