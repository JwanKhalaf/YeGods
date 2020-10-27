namespace YeGods.DataAccess
{
  using System;
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Identity;
  using Microsoft.EntityFrameworkCore;
  using Microsoft.Extensions.Logging;
  using System.Linq;

  public class DbInitializer
  {
    public static async Task Initialize(
      YeGodsContext context,
      UserManager<IdentityUser> userManager,
      RoleManager<IdentityRole> roleManager,
      ILogger<DbInitializer> logger)
    {
      Console.WriteLine("Attempting to migrate database.");

      context.Database.Migrate();

      if (context.Users.Any())
      {
        return; // DB has been seeded
      }

      await CreateApplicationUsersWithRoles(userManager, roleManager, logger);
    }

    private static async Task CreateApplicationUsersWithRoles(
      UserManager<IdentityUser> userManager,
      RoleManager<IdentityRole> roleManager,
      ILogger<DbInitializer> logger)
    {
      const string administratorRole = "Admin";

      await CreateRole(roleManager, logger, administratorRole);

      const string johnEmail = "jhavison@yahoo.co.uk";
      const string johnPassword = "rz8NTneCyZBBPhAxWeZQkbx@yzfYxt6oE3XyVhEC";

      IdentityUser johnUser = await CreateDefaultUser(userManager, logger, johnEmail);

      await SetPasswordForUser(userManager, logger, johnEmail, johnUser, johnPassword);

      await AssignRoleToUser(userManager, logger, johnEmail, administratorRole, johnUser);
    }

    private static async Task CreateRole(
      RoleManager<IdentityRole> roleManager,
      ILogger<DbInitializer> logger,
      string role)
    {
      logger.LogInformation($"Create the role `{role}` for application");

      IdentityResult result = await roleManager.CreateAsync(new IdentityRole(role));

      if (result.Succeeded)
      {
        logger.LogDebug($"Created the role `{role}` successfully");
      }
      else
      {
        ApplicationException exception = new ApplicationException($"Default role `{role}` cannot be created");

        logger.LogError(exception, GetIdentiryErrorsInCommaSeperatedList(result));

        throw exception;
      }
    }

    private static async Task<IdentityUser> CreateDefaultUser(
      UserManager<IdentityUser> userManager,
      ILogger<DbInitializer> logger,
      string email)
    {
      logger.LogInformation($"Create default user with email `{email}` for application");

      IdentityUser user = new IdentityUser()
      {
        Email = email,
        UserName = email
      };

      IdentityResult identityResult = await userManager.CreateAsync(user);

      if (identityResult.Succeeded)
      {
        logger.LogDebug($"Created default user `{email}` successfully");
      }
      else
      {
        ApplicationException exception = new ApplicationException($"Default user `{email}` cannot be created");
        logger.LogError(exception, GetIdentiryErrorsInCommaSeperatedList(identityResult));
        throw exception;
      }

      IdentityUser createdUser = await userManager.FindByEmailAsync(email);
      return createdUser;
    }

    private static async Task SetPasswordForUser(
      UserManager<IdentityUser> userManager,
      ILogger<DbInitializer> logger,
      string email,
      IdentityUser user,
      string password)
    {
      logger.LogInformation($"Set password for default user `{email}`");

      IdentityResult identityResult = await userManager.AddPasswordAsync(user, password);

      if (identityResult.Succeeded)
      {
        logger.LogTrace($"Set password `{password}` for default user `{email}` successfully");
      }
      else
      {
        ApplicationException exception = new ApplicationException($"Password for the user `{email}` cannot be set");

        logger.LogError(exception, GetIdentiryErrorsInCommaSeperatedList(identityResult));

        throw exception;
      }
    }

    private static async Task AssignRoleToUser(
      UserManager<IdentityUser> userManager,
      ILogger<DbInitializer> logger,
      string email,
      string role,
      IdentityUser user)
    {
      logger.LogInformation($"Add default user `{email}` to role '{role}'");

      IdentityResult identityResult = await userManager.AddToRoleAsync(user, role);

      if (identityResult.Succeeded)
      {
        logger.LogDebug($"Added the role '{role}' to default user `{email}` successfully");
      }
      else
      {
        ApplicationException exception = new ApplicationException($"The role `{role}` cannot be set for the user `{email}`");

        logger.LogError(exception, GetIdentiryErrorsInCommaSeperatedList(identityResult));

        throw exception;
      }
    }

    private static string GetIdentiryErrorsInCommaSeperatedList(IdentityResult ir)
    {
      string errors = null;

      foreach (IdentityError identityError in ir.Errors)
      {
        errors += identityError.Description;

        errors += ", ";
      }

      return errors;
    }
  }
}
