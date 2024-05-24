using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using book_store_app_marian.Models;
using Microsoft.Extensions.Logging;
using Bogus;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.AspNetCore.Identity;

namespace book_store_app_marian.Data
{
    public class DbInitializer
    {
        public static void Initialize(IServiceProvider serviceProvider, ILogger<DbInitializer> logger)
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
                                .RuleFor(p => p.ProductName, f => f.Commerce.ProductName())
                                .RuleFor(p => p.Price, f => Math.Round(f.Random.Double(10, 100), 2))
                                .RuleFor(p => p.CreatedTimestamp, f => f.Date.Past())
                                .RuleFor(p => p.Description, f => f.Lorem.Sentence(10)); // Generate a paragraph with 3 sentences

                    var productList = productFaker.Generate(55); // Generate 50 products


                    context.Products.AddRange(productList);

                    try
                    {
                        context.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        // Debugging: Log any exceptions
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
                        int imageCount = random.Next(3, 5); // Randomly choose 1 to 4 images
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

                // Seed Reviews
                if (!context.Reviews.Any())
                {
                    var productsList = context.Products.ToList();
                    var usersList = context.Users.ToList();

                    var reviews = new Faker<Reviews>()
                        .RuleFor(r => r.Id, f => Guid.NewGuid())
                        .RuleFor(r => r.ProductId, f => f.PickRandom(productsList).Id)
                        .RuleFor(r => r.UserId, f => f.PickRandom(usersList).Id)
                        .RuleFor(r => r.Review, f => f.Lorem.Sentence(10))
                        .RuleFor(r => r.Likes, f => f.Random.Byte(1, 5));

                    context.Reviews.AddRange(reviews.Generate(200)); // Generate 200 reviews
                    context.SaveChanges();
                }

                // seed users
                // Seed Users
                if (context.Users.Count() < 20)
                {
                    var passwordHasher = new PasswordHasher<IdentityUser>();
                    var users = new Faker<IdentityUser>()
                        .RuleFor(u => u.Id, f => Guid.NewGuid().ToString())
                        .RuleFor(u => u.UserName, f => f.Internet.UserName())
                        .RuleFor(u => u.NormalizedUserName, (f, u) => u.UserName.ToUpper())
                        .RuleFor(u => u.Email, f => f.Internet.Email())
                        .RuleFor(u => u.NormalizedEmail, (f, u) => u.Email.ToUpper())
                        .RuleFor(u => u.EmailConfirmed, f => true)
                        .RuleFor(u => u.PasswordHash, (f, u) => passwordHasher.HashPassword(u, "User123@"))
                        .RuleFor(u => u.SecurityStamp, f => Guid.NewGuid().ToString("D"));

                    var userList = users.Generate(20);
                    context.Users.AddRange(userList);
                    context.SaveChanges();
                }
            }
        }
    }
}
