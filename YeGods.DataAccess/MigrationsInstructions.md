# How to create a new migrations:

## Package Manager Console

Make sure correct settings exist in appsettings.Development.json and set Phoneden.Web as Start Up Project

In "Package Manager Console" type:

`$env:ASPNETCORE_ENVIRONMENT='Development'`

Or what ever your target environment is, then  in **Package Manager Console** type:

`Add-Migration <UniqueMigrationName> -project YeGods.DataAccess -startupproject YeGods.Web`

## .NET Core CLI

In Bash, set the environment variable like so:

`export ASPNETCORE_ENVIRONMENT=Development`

To add a migration using the CLI, simply run the following, whilst changing what you need.

`dotnet-ef migrations add InitialCreate --project YeGods.DataAccess --startup-project YeGods.Web`

# How to create the database according to latest migrations

## Package Manager Console

To update your database. In "Package Manager Console" type:

`Update-Database -project YeGods.DataAccess -startupproject YeGods.Web`

## .NET Core CLI

Navigate to the project root directory, and run:

`dotnet-ef database update --project YeGods.DataAccess --startup-project YeGods.Web`

# How to create the SQL Scripts for deployment

## .NET Core CLI

Navigate to the project root directory, and run:

`dotnet-ef migrations script --project YeGods.DataAccess --startup-project YeGods.Web -o yegods-scripts.sql`
