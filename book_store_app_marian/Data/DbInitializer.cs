using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using book_store_app_marian.Models;
using Microsoft.Extensions.Logging;
using Bogus;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Bogus.DataSets;

namespace book_store_app_marian.Data
{
    public class DbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider, ILogger<DbInitializer> logger, UserManager<IdentityUser> userManager)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // Look for any categories already in the database.
                if (!context.Categories.Any())
                {
                    var categories = new Categories[]
                    {
                        new Categories { Id = Guid.NewGuid(), CategoryName = "Fantasy" },
                        new Categories { Id = Guid.NewGuid(), CategoryName = "Adventure" },
                        new Categories { Id = Guid.NewGuid(), CategoryName = "Romance" },
                        new Categories { Id = Guid.NewGuid(), CategoryName = "Contemporary" },
                        new Categories { Id = Guid.NewGuid(), CategoryName = "Dystopian" },
                        new Categories { Id = Guid.NewGuid(), CategoryName = "Mystery" },
                        new Categories { Id = Guid.NewGuid(), CategoryName = "Horror" },
                        new Categories { Id = Guid.NewGuid(), CategoryName = "Thriller" },
                        new Categories { Id = Guid.NewGuid(), CategoryName = "Paranormal" },
                        new Categories { Id = Guid.NewGuid(), CategoryName = "Historical fiction" },
                        new Categories { Id = Guid.NewGuid(), CategoryName = "Science Fiction" },
                        new Categories { Id = Guid.NewGuid(), CategoryName = "Children's" }
                    };

                    context.Categories.AddRange(categories);
                    context.SaveChanges();
                }

                // Fetch the seeded categories
                var categoriesList = context.Categories.ToList();

                // Products seed
                if (!context.Products.Any())
                {

                    var productFaker = new Faker<Products>()
                                .RuleFor(p => p.Id, f => Guid.NewGuid())
                                .RuleFor(p => p.CategoryId, f => f.PickRandom(categoriesList).Id)
                                .RuleFor(p => p.ProductAuthor, f => f.Name.FullName())
                                .RuleFor(p => p.ProductName, f => f.Commerce.ProductName())
                                .RuleFor(p => p.Price, f => Math.Round(f.Random.Double(10, 100), 2))
                                .RuleFor(p => p.CreatedTimestamp, f => f.Date.Past())
                                .RuleFor(p => p.Description, f => f.Lorem.Paragraph());

                    var productList = productFaker.Generate(70); // Generate 70 products


                    context.Products.AddRange(productList);

                    try
                    {
                        context.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        // Debugging by logging any exceptions
                        logger.LogError($"Error saving changes: {ex.Message}");
                    }


                    var imageUrls = new List<string>
                    {
                        "/images/fantasy1.jpg",
                        "/images/Kafka.jpg",
                        "/images/adventure1.jpg",
                        "/images/Ds1.jpg",
                        "/images/Ds2.jpg",
                        "/images/Ds3.jpg"
                    };

                    var random = new Random();

                    foreach (var product in productList)
                    {
                        int imageCount = random.Next(3, 5); // Randomly choose 3 to 5 images
                        var selectedImages = imageUrls.OrderBy(x => random.Next()).Take(imageCount).ToList();

                        bool mainImageSet = false;

                        var productImages = selectedImages.Select(imageUrl => new ProductImages
                        {
                            Id = Guid.NewGuid(),
                            ProductId = product.Id,
                            ProductImage = imageUrl,
                            MainImage = !mainImageSet && (mainImageSet = true), // Set the first image as the main image
                            CreatedTimestamp = DateTime.UtcNow
                        }).ToList();

                        context.ProductImages.AddRange(productImages);
                        context.SaveChanges();
                    }

                }

                // Seed users
                if (context.Users.Count() < 20)
                {
                    var users = new Faker<IdentityUser>()
                            .RuleFor(u => u.UserName, f => f.Internet.Email()) // Use email as username
                            .RuleFor(u => u.Email, (f, u) => u.UserName)
                            .RuleFor(u => u.NormalizedUserName, (f, u) => u.UserName.ToUpper())
                            .RuleFor(u => u.NormalizedEmail, (f, u) => u.Email.ToUpper())
                            .RuleFor(u => u.EmailConfirmed, f => true)
                            .RuleFor(u => u.SecurityStamp, f => Guid.NewGuid().ToString("D"));

                    var userList = users.Generate(20);

                    foreach (var user in userList)
                    {
                        var result = userManager.CreateAsync(user, "User@1234").Result;
                    }
                }

                // Seed purchases and reviews
                if (!context.Reviews.Any())
                {
                    var productsList = context.Products.ToList();
                    var usersList = context.Users.ToList();

                    // Generate fake purchases
                    if (!context.Purchases.Any())
                    {
                        var purchaseFaker = new Faker<Purchases>()
                            .RuleFor(p => p.Id, f => Guid.NewGuid())
                            .RuleFor(p => p.ProductId, f => f.PickRandom(productsList).Id)
                            .RuleFor(p => p.UserId, f => f.PickRandom(usersList).Id)
                            .RuleFor(p => p.Price, (f, p) => context.Products.First(prod => prod.Id == p.ProductId).Price)
                            .RuleFor(p => p.Status, f => f.PickRandom(new[] { "Pending", "Completed", "Shipped", "Cancelled" }))
                            .RuleFor(p => p.CreatedTimestamp, f => f.Date.Past());

                        var purchases = purchaseFaker.Generate(200);
                        context.Purchases.AddRange(purchases);
                        context.SaveChanges();
                    }

                    var purchasesList = context.Purchases.Include(p => p.Products).ToList();

                    // Generate reviews for users who have purchased products
                    var reviewFaker = new Faker<Reviews>()
                        .RuleFor(p => p.Id, f => Guid.NewGuid())
                        .RuleFor(r => r.ProductId, f => f.PickRandom(purchasesList).ProductId)
                        .RuleFor(r => r.UserId, (f, r) => f.PickRandom(purchasesList.Where(p => p.ProductId == r.ProductId)).UserId)
                        .RuleFor(r => r.PurchaseId, (f, r) => f.PickRandom(purchasesList.Where(p => p.ProductId == r.ProductId)).Id)
                        .RuleFor(r => r.Review, f => f.Lorem.Sentence(10))
                        .RuleFor(r => r.CreatedTimestamp, f => f.Date.Past())
                        .RuleFor(r => r.Rating, f => f.Random.Byte(1, 5));

                    var reviews = reviewFaker.Generate(200);
                    context.Reviews.AddRange(reviews);
                    context.SaveChanges();
                }

                var adminEmail = "admin@admin.com";
                var adminUser = await userManager.FindByEmailAsync(adminEmail);

                if (adminUser == null)
                {
                    adminUser = new IdentityUser { UserName = adminEmail, Email = adminEmail };
                    await userManager.CreateAsync(adminUser, "Admin@123");
                }

                // Seed purchases for the admin account
                if (!context.Purchases.Any(p => p.UserId == adminUser.Id))
                {
                    var productsList = context.Products.ToList();

                    var purchaseFaker = new Faker<Purchases>()
                        .RuleFor(p => p.Id, f => Guid.NewGuid())
                        .RuleFor(p => p.ProductId, f => f.PickRandom(productsList).Id)
                        .RuleFor(p => p.UserId, f => adminUser.Id)
                        .RuleFor(p => p.Price, (f, p) => productsList.First(prod => prod.Id == p.ProductId).Price)
                        .RuleFor(p => p.Status, f => f.PickRandom(new[] { "Pending", "Completed", "Shipped", "Cancelled" }))
                        .RuleFor(p => p.CreatedTimestamp, f => f.Date.Past());

                    var purchases = purchaseFaker.Generate(20);
                    context.Purchases.AddRange(purchases);
                    await context.SaveChangesAsync();
                }

                var adminPurchases = context.Purchases
                    .Include(p => p.Products)
                    .Where(p => p.UserId == adminUser.Id)
                    .ToList();

                // Seed reviews for the admin account
                if (!context.Reviews.Any(r => r.UserId == adminUser.Id))
                {
                    var reviewFaker = new Faker<Reviews>()
                    .RuleFor(r => r.Id, f => Guid.NewGuid())
                    .RuleFor(r => r.ProductId, f => f.PickRandom(adminPurchases).ProductId)
                    .RuleFor(r => r.UserId, f => adminUser.Id)
                    .RuleFor(r => r.PurchaseId, (f, r) => adminPurchases.First(p => p.ProductId == r.ProductId).Id)
                    .RuleFor(r => r.Review, f => f.Lorem.Sentence(10))
                    .RuleFor(r => r.CreatedTimestamp, f => f.Date.Past())
                    .RuleFor(r => r.Rating, f => f.Random.Byte(1, 5));

                    var AdminReviews = reviewFaker.Generate(20);
                    context.Reviews.AddRange(AdminReviews);
                    await context.SaveChangesAsync();
                }

            }
        }
    }
}
