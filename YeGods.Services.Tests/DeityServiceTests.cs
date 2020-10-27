namespace YeGods.Services.Tests
{
  using Common.Enums;
  using DataAccess;
  using FluentAssertions;
  using Microsoft.Extensions.Options;
  using NUnit.Framework;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading.Tasks;
  using Domain;
  using ViewModels;
  using YeGods.Tests;
  using YeGods.ViewModels.Shared;

  public class DeityServiceTests : DataAccessTestBase<YeGodsContext>
  {
    private IOptions<PaginationSettings> _paginationSettings;
    private IDeityService deityService;

    [SetUp]
    public void Setup()
    {
      this.BuildServiceProvider();
      this.AddTestDataToYeGodsContext();
      PaginationSettings paginationSettings = new PaginationSettings();
      paginationSettings.RecordsPerPage = 10;
      this._paginationSettings = Options.Create<PaginationSettings>(paginationSettings);
      this.deityService = new DeityService(this._paginationSettings, this.DbInMemoryContext);
    }


    [Test]
    public void GetPagedDeitiesAsyncReturnsTaskOfDeityPageViewModel()
    {
      // Arrange
      bool showDeleted = false;
      int pageNumber = 1;
      SearchViewModel search = new SearchViewModel();
      search.ShowDeleted = showDeleted;

      // Act
      Task<DeityPageViewModel> result = this.deityService.GetPagedDeitiesAsync(search, pageNumber);

      // Assert
      result.Should().BeOfType<Task<DeityPageViewModel>>();
    }

    [Test]
    public void GetPagedDeitiesAsyncReturnsACollectionOfDeities()
    {
      // Arrange
      bool showDeleted = false;
      int pageNumber = 1;
      SearchViewModel search = new SearchViewModel();
      search.ShowDeleted = showDeleted;

      // Act
      DeityPageViewModel result = this.deityService.GetPagedDeitiesAsync(search, pageNumber).Result;

      // Assert
      result.Deities.Any().Should().BeTrue();
    }

    [Test]
    public void PassingFalseForShowDeletedOnGetPagedDeitiesAsyncReturnsDeitiesThatAreNotDeleted()
    {
      // Arrange
      bool showDeleted = false;
      int pageNumber = 1;
      SearchViewModel search = new SearchViewModel();
      search.ShowDeleted = showDeleted;

      // Act
      DeityPageViewModel result = this.deityService.GetPagedDeitiesAsync(search, pageNumber).Result;

      // Assert
      result.Deities.Any(d => d.IsDeleted).Should().BeFalse();
    }

    [Test]
    public void PassingTrueForShowDeletedOnGetPagedDeitiesAsyncReturnsDeitiesThatAreAlsoDeleted()
    {
      // Arrange
      bool showDeleted = true;
      int pageNumber = 1;
      SearchViewModel search = new SearchViewModel();
      search.ShowDeleted = showDeleted;

      // Act
      DeityPageViewModel result = this.deityService.GetPagedDeitiesAsync(search, pageNumber).Result;

      // Assert
      result.Deities.Any(d => d.IsDeleted).Should().BeTrue();
    }

    [Test]
    public void GetPagedDeitiesAsyncReturnsAMaximumOfTenDeities()
    {
      // Arrange
      bool showDeleted = false;
      int pageNumber = 1;
      SearchViewModel search = new SearchViewModel();
      search.ShowDeleted = showDeleted;

      // Act
      DeityPageViewModel result = this.deityService.GetPagedDeitiesAsync(search, pageNumber).Result;

      // Assert
      result.Deities.Count().Should().Be(10);
    }

    [Test]
    public void GetDeityAsyncReturnsTaskOfDeityViewModel()
    {
      // Arrange
      string requestedDeitySlug = "james";

      // Act
      Task<DeityViewModel> result = this.deityService.GetDeityBySlugAsync(requestedDeitySlug);

      // Assert
      result.Should().BeOfType<Task<DeityViewModel>>();
    }

    [Test]
    public void GetDeityAsyncReturnsTheDeityViewModelForCorrectDeity()
    {
      // Arrange
      string requestedDeitySlug = "zeus";
      string expectedName = "Zeus";
      string expectedOrigin = "Greek";
      string expectedCategoryName = "Deity";

      // Act
      DeityViewModel requestDeity = this.deityService.GetDeityBySlugAsync(requestedDeitySlug).Result;

      // Assert
      requestDeity.Slug.Should().Be(requestedDeitySlug);
      requestDeity.Name.Should().Be(expectedName);
      requestDeity.Category.Name.Should().Be(expectedCategoryName);
      requestDeity.Origin.Should().Be(expectedOrigin);
    }

    [Test]
    public void GetDeityAsyncReturnsNullWhenRequestedDeityIsNotFound()
    {
      // Arrange
      string requestedDeitySlug = "jesus";

      // Act
      DeityViewModel requestDeity = this.deityService.GetDeityBySlugAsync(requestedDeitySlug).Result;

      // Assert
      requestDeity.Should().BeNull();
    }

    private void AddTestDataToYeGodsContext()
    {
      this.AddTestCategoriesToYeGodsContext();
      this.AddTestDeitiesToYeGodsContext();
    }

    private void AddTestDeitiesToYeGodsContext()
    {
      Category deity = this.DbInMemoryContext.Categories.First(d => d.Name == "Deity");
      Deity zeus = new Deity();
      zeus.Name = "Zeus";
      zeus.Sex = Sex.Male;
      zeus.Origin = "Greek";
      zeus.Description = "The god of all gods in Greece.";
      zeus.Category = deity;
      zeus.CategoryId = deity.Id;
      zeus.CreatedAt = DateTime.UtcNow;
      zeus.IsDeleted = false;
      zeus.Slugs = this.GenerateListOfSlugs(zeus);
      this.DbInMemoryContext.Deities.Add(zeus);

      Deity venus = new Deity();
      venus.Name = "Venus";
      venus.Sex = Sex.Female;
      venus.Origin = "Roman";
      venus.Description = "The god of love and beauty.";
      venus.Category = deity;
      venus.CategoryId = deity.Id;
      venus.CreatedAt = DateTime.UtcNow;
      venus.IsDeleted = false;
      venus.Slugs = this.GenerateListOfSlugs(venus);
      this.DbInMemoryContext.Deities.Add(venus);

      Deity odin = new Deity();
      odin.Name = "Odin";
      odin.Sex = Sex.Male;
      odin.Origin = "Norsk";
      odin.Description = "The god of wisdom.";
      odin.Category = deity;
      odin.CategoryId = deity.Id;
      odin.CreatedAt = DateTime.UtcNow;
      odin.IsDeleted = false;
      odin.Slugs = this.GenerateListOfSlugs(odin);
      this.DbInMemoryContext.Deities.Add(odin);

      Deity loki = new Deity();
      loki.Name = "Loki";
      loki.Sex = Sex.Male;
      loki.Origin = "Norsk";
      loki.Description = "The god of tricks.";
      loki.Category = deity;
      loki.CategoryId = deity.Id;
      loki.CreatedAt = DateTime.UtcNow;
      loki.IsDeleted = false;
      loki.Slugs = this.GenerateListOfSlugs(loki);
      this.DbInMemoryContext.Deities.Add(loki);

      Deity ashur = new Deity();
      ashur.Name = "Ashur";
      ashur.Sex = Sex.Male;
      ashur.Origin = "Mesopotamian";
      ashur.Description = "The cheif god of the city of Assur.";
      ashur.Category = deity;
      ashur.CategoryId = deity.Id;
      ashur.CreatedAt = DateTime.UtcNow;
      ashur.IsDeleted = false;
      ashur.Slugs = this.GenerateListOfSlugs(ashur);
      this.DbInMemoryContext.Deities.Add(ashur);

      Deity nabu = new Deity();
      nabu.Name = "Nabu";
      nabu.Sex = Sex.Male;
      nabu.Origin = "Mesopotamian";
      nabu.Description = "The god of writing and scribes.";
      nabu.Category = deity;
      nabu.CategoryId = deity.Id;
      nabu.CreatedAt = DateTime.UtcNow;
      nabu.IsDeleted = false;
      nabu.Slugs = this.GenerateListOfSlugs(nabu);
      this.DbInMemoryContext.Deities.Add(nabu);

      Deity thor = new Deity();
      thor.Name = "Thor";
      thor.Sex = Sex.Male;
      thor.Origin = "Norsk";
      thor.Description = "The god of thunder.";
      thor.Category = deity;
      thor.CategoryId = deity.Id;
      thor.CreatedAt = DateTime.UtcNow;
      thor.IsDeleted = true;
      thor.Slugs = this.GenerateListOfSlugs(thor);
      this.DbInMemoryContext.Deities.Add(thor);

      Deity nergal = new Deity();
      nergal.Name = "Nergal";
      nergal.Sex = Sex.Male;
      nergal.Origin = "Mesopotamian";
      nergal.Description = "The god of the underworld.";
      nergal.Category = deity;
      nergal.CategoryId = deity.Id;
      nergal.CreatedAt = DateTime.UtcNow;
      nergal.IsDeleted = false;
      nergal.Slugs = this.GenerateListOfSlugs(nergal);
      this.DbInMemoryContext.Deities.Add(nergal);

      Deity tiamat = new Deity();
      tiamat.Name = "Tiamat";
      tiamat.Sex = Sex.Female;
      tiamat.Origin = "Mesopotamian";
      tiamat.Description = "The sea goddess.";
      tiamat.Category = deity;
      tiamat.CategoryId = deity.Id;
      tiamat.CreatedAt = DateTime.UtcNow;
      tiamat.IsDeleted = false;
      tiamat.Slugs = this.GenerateListOfSlugs(tiamat);
      this.DbInMemoryContext.Deities.Add(tiamat);

      Deity marduk = new Deity();
      marduk.Name = "Marduk";
      marduk.Sex = Sex.Male;
      marduk.Origin = "Mesopotamian";
      marduk.Description = "The patron god of Babylon.";
      marduk.Category = deity;
      marduk.CategoryId = deity.Id;
      marduk.CreatedAt = DateTime.UtcNow;
      marduk.IsDeleted = false;
      marduk.Slugs = this.GenerateListOfSlugs(marduk);
      this.DbInMemoryContext.Deities.Add(marduk);

      Deity ishtar = new Deity();
      ishtar.Name = "Ishtar";
      ishtar.Sex = Sex.Female;
      ishtar.Origin = "Mesopotamian";
      ishtar.Description = "The goddess of love and protection.";
      ishtar.Category = deity;
      ishtar.CategoryId = deity.Id;
      ishtar.CreatedAt = DateTime.UtcNow;
      ishtar.IsDeleted = false;
      ishtar.Slugs = this.GenerateListOfSlugs(ishtar);
      this.DbInMemoryContext.Deities.Add(ishtar);

      Deity mammetun = new Deity();
      mammetun.Name = "Mammetun";
      mammetun.Sex = Sex.Female;
      mammetun.Origin = "Mesopotamian";
      mammetun.Description = "The goddess of fate.";
      mammetun.Category = deity;
      mammetun.CategoryId = deity.Id;
      mammetun.CreatedAt = DateTime.UtcNow;
      mammetun.IsDeleted = false;
      mammetun.Slugs = this.GenerateListOfSlugs(mammetun);
      this.DbInMemoryContext.Deities.Add(mammetun);

      this.DbInMemoryContext.SaveChangesAsync();
    }

    private void AddTestCategoriesToYeGodsContext()
    {
      Category deity = new Category();
      deity.Name = "Deity";
      deity.CreatedAt = DateTime.UtcNow;
      this.DbInMemoryContext.Categories.Add(deity);

      Category demigod = new Category();
      demigod.Name = "Demigod";
      demigod.CreatedAt = DateTime.UtcNow;
      this.DbInMemoryContext.Categories.Add(demigod);

      this.DbInMemoryContext.SaveChangesAsync();
    }

    private List<DeitySlug> GenerateListOfSlugs(Deity deity)
    {
      List<DeitySlug> slugs = new List<DeitySlug>();
      DeitySlug deitySlug = new DeitySlug();
      deitySlug.Name = deity.Name.Trim().Replace(" ", "-").ToLowerInvariant();
      deitySlug.CreatedAt = DateTime.UtcNow;
      deitySlug.IsDefault = true;
      deitySlug.Deity = deity;
      slugs.Add(deitySlug);
      return slugs;
    }
  }
}
