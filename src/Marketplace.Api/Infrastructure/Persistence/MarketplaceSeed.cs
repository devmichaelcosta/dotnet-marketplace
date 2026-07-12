using Marketplace.Api.Domain;
using Marketplace.Api.Features.Products.Admin.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Infrastructure.Persistence;

public static class MarketplaceSeed
{
    public const string AdminRole = "admin";
    public const string SellerRole = "vendedor";
    public const string CustomerRole = "comum";

    public static async Task SeedAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MarketplaceDbContext>();
        await context.Database.MigrateAsync(cancellationToken);

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        foreach (var role in new[] { AdminRole, SellerRole, CustomerRole })
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }
        }

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var admin = await EnsureUserAsync(userManager, "michael", "michael@marketplace.local", "Michael", "Admin", AdminRole);
        var seller = await EnsureUserAsync(userManager, "multisom", "multisom@marketplace.local", "Multisom", "", SellerRole);
        var techSeller = await EnsureUserAsync(userManager, "techstore", "techstore@marketplace.local", "Tech", "Store", SellerRole);
        var customer = await EnsureUserAsync(userManager, "tatiana", "tatiana@marketplace.local", "Tatiana", "Cliente", CustomerRole);

        if (!await context.States.AnyAsync(cancellationToken))
        {
            context.States.AddRange(
                new State { Name = "Rio Grande do Sul", Abbreviation = "RS" },
                new State { Name = "Santa Catarina", Abbreviation = "SC" });
        }

        await EnsureCategoryAsync(context, "Instrumentos musicais", "/uploads/categories/1094938_guitarra-jackson-monarkh-js22-585-transparent-black-ms_z1_637387173152514626.jpg", ["Guitarras", "Baixos", "Violoes"], cancellationToken);
        await EnsureCategoryAsync(context, "Livros", "/uploads/categories/51d1qVhmAmL.jpg", ["Tecnologia", "Agilidade"], cancellationToken);
        await EnsureCategoryAsync(context, "Informatica", "/uploads/categories/Acer-Predator-Helios-300-PH315-52-748u.jpg", ["Notebooks", "Acessorios"], cancellationToken);
        await EnsureCategoryAsync(context, "Bebidas", "/uploads/categories/cocacola_zero_350ml.jpg", ["Refrigerantes"], cancellationToken);
        await EnsureCategoryAsync(context, "Esporte", "/uploads/categories/910061_bici_foxer_aro26_1_z.jpg", ["Bicicletas"], cancellationToken);
        await EnsureCategoryAsync(context, "Suplementos", "/uploads/categories/whey-protein.png", ["Whey"], cancellationToken);

        if (!await context.Sellers.AnyAsync(cancellationToken))
        {
            context.Sellers.Add(new Seller
            {
                UserId = seller.Id,
                Email = "multisom@marketplace.local",
                Company = "Multisom",
                FantasyName = "Multisom",
                BranchOfActivity = "Venda de instrumentos musicais"
            });
        }

        if (!await context.Sellers.AnyAsync(item => item.UserId == techSeller.Id, cancellationToken))
        {
            context.Sellers.Add(new Seller
            {
                UserId = techSeller.Id,
                Email = "techstore@marketplace.local",
                Company = "Tech Store",
                FantasyName = "Tech Store",
                BranchOfActivity = "Tecnologia, livros e eletronicos"
            });
        }

        if (!await context.Attributes.AnyAsync(cancellationToken))
        {
            context.Attributes.AddRange(
                new AttributeDefinition { Name = "Peso" },
                new AttributeDefinition { Name = "Marca" },
                new AttributeDefinition { Name = "Modelo" },
                new AttributeDefinition { Name = "Cor" },
                new AttributeDefinition { Name = "Editora" });
        }

        await context.SaveChangesAsync(cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        await EnsureDemoProductsAsync(context, seller.Id, techSeller.Id, admin.Id, cancellationToken);
        await EnsureDemoCarouselAsync(context, cancellationToken);
        await EnsureDemoAddressAsync(context, customer.Id, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        await NormalizeStoredProductImagesAsync(context, cancellationToken);
    }

    private static async Task NormalizeStoredProductImagesAsync(MarketplaceDbContext context, CancellationToken cancellationToken)
    {
        var productImages = await context.ProductImages.ToListAsync(cancellationToken);
        var changed = false;

        foreach (var image in productImages)
        {
            var normalized = Marketplace.Api.Features.Products.ProductImageStorage.NormalizeFileName(image.FileName);
            if (string.IsNullOrWhiteSpace(normalized) || string.Equals(image.FileName, normalized, StringComparison.Ordinal))
            {
                continue;
            }

            image.FileName = normalized;
            changed = true;
        }

        if (changed)
        {
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    private static async Task EnsureCategoryAsync(
        MarketplaceDbContext context,
        string title,
        string image,
        string[] subCategories,
        CancellationToken cancellationToken)
    {
        var category = await context.Categories
            .Include(item => item.SubCategories)
            .FirstOrDefaultAsync(item => item.Title == title, cancellationToken);

        if (category is null)
        {
            category = new Category { Title = title, Image = image };
            context.Categories.Add(category);
        }
        else
        {
            category.Image = image;
        }

        foreach (var subCategoryTitle in subCategories)
        {
            if (category.SubCategories.All(item => item.Title != subCategoryTitle))
            {
                category.SubCategories.Add(new SubCategory { Title = subCategoryTitle });
            }
        }
    }

    private static async Task EnsureDemoProductsAsync(
        MarketplaceDbContext context,
        Guid musicSellerId,
        Guid techSellerId,
        Guid adminId,
        CancellationToken cancellationToken)
    {
        var subCategories = await context.SubCategories.ToDictionaryAsync(item => item.Title, item => item.Id, cancellationToken);
        var products = new[]
        {
            DemoProduct(musicSellerId, subCategories["Guitarras"], "DEMO-GUIT-FENDER", "Guitarra Fender Standard Telecaster Mexicana Black", "A Fender traz a Standard Telecaster para guitarristas que apreciam estilo e versatilidade por um super valor.", 5690.99m, 7, true, "fender-american-especial-stratocaster-maple2-color-sunburst2012.jpg", "fender-mex-black-014-5102-506.jpg"),
            DemoProduct(musicSellerId, subCategories["Guitarras"], "DEMO-GUIT-JACKSON", "Guitarra Jackson Monarkh JS22 Transparent Black", "Modelo com captadores humbucker, escala confortavel e acabamento moderno para rock e metal.", 3299.90m, 5, true, "1094938_guitarra-jackson-monarkh-js22-585-transparent-black-ms_z1_637387173152514626.jpg"),
            DemoProduct(musicSellerId, subCategories["Violoes"], "DEMO-VIOL-TAGIMA", "Violao Tagima Dallas Tuner Eletroacustico", "Violao com afinador embutido, cordas de aco e otima resposta para estudo e palco.", 899.90m, 11, true, "1090123_violao-tagima-dallas-tuner-eletrico-cordas-de-aco-e-com-afinador-ms_z2_637371635447323128.jpg"),
            DemoProduct(musicSellerId, subCategories["Baixos"], "DEMO-BAIXO-IBANEZ", "Baixo Ibanez RG 7420Z", "Instrumento versatil para linhas pesadas, com excelente sustain e acabamento robusto.", 2890.00m, 3, false, "Ibanez-RG-7420Z.jpg"),
            DemoProduct(techSellerId, subCategories["Tecnologia"], "DEMO-LIVRO-CLEAN-ARCH", "Arquitetura Limpa", "Guia pratico para estrutura, design e manutencao de software profissional.", 66.33m, 20, true, "clean-architecture.jpg"),
            DemoProduct(techSellerId, subCategories["Tecnologia"], "DEMO-LIVRO-CLEAN-CODE", "Codigo Limpo", "Boas praticas para escrever codigo legivel, testavel e sustentavel.", 79.90m, 15, true, "clean-code-1.jpg"),
            DemoProduct(techSellerId, subCategories["Agilidade"], "DEMO-LIVRO-CLEAN-AGILE", "Clean Agile", "Uma introducao direta aos principios ageis aplicados em times modernos.", 58.50m, 8, false, "clean_agile.jpg"),
            DemoProduct(techSellerId, subCategories["Notebooks"], "DEMO-NOTE-ACER-PREDATOR", "Notebook Acer Predator Helios 300", "Notebook gamer com alto desempenho para jogos, desenvolvimento e criacao.", 7499.00m, 4, true, "Acer-Predator-Helios-300-PH315-52-748u.jpg"),
            DemoProduct(adminId, subCategories["Refrigerantes"], "DEMO-COCA-ZERO", "Coca-Cola Zero 350ml", "Bebida gelada para acompanhar suas compras e momentos de lazer.", 4.99m, 60, false, "cocacola_zero_350ml.jpg"),
            DemoProduct(adminId, subCategories["Bicicletas"], "DEMO-BIKE-FOXER", "Bicicleta Foxer Aro 26", "Bicicleta resistente para uso urbano e passeios de fim de semana.", 1199.90m, 6, true, "910061_bici_foxer_aro26_1_z.jpg")
        };

        foreach (var product in products)
        {
            if (await context.Products.AnyAsync(item => item.Sku == product.Sku, cancellationToken))
            {
                continue;
            }

            context.Products.Add(product);
        }

        await context.SaveChangesAsync(cancellationToken);
        await EnsureGeneratedDemoProductsAsync(context, musicSellerId, techSellerId, adminId, subCategories, cancellationToken);
        await EnsureSimilarProductLinksAsync(context, cancellationToken);
    }

    private static async Task EnsureGeneratedDemoProductsAsync(
        MarketplaceDbContext context,
        Guid musicSellerId,
        Guid techSellerId,
        Guid adminId,
        IReadOnlyDictionary<string, int> subCategories,
        CancellationToken cancellationToken)
    {
        var attributes = await context.Attributes.ToDictionaryAsync(item => item.Name, item => item.Id, cancellationToken);
        var existingSkus = await context.Products
            .Where(item => item.Sku.StartsWith("DEMO-BULK-"))
            .Select(item => item.Sku)
            .ToHashSetAsync(cancellationToken);

        var products = new List<Product>(capacity: 1000);
        for (var index = 1; index <= 1000; index++)
        {
            var sku = $"DEMO-BULK-{index:0000}";
            if (existingSkus.Contains(sku))
            {
                continue;
            }

            products.Add(CreateGeneratedProduct(index, sku, musicSellerId, techSellerId, adminId, subCategories, attributes));
        }

        if (products.Count > 0)
        {
            context.Products.AddRange(products);
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    private static Product CreateGeneratedProduct(
        int index,
        string sku,
        Guid musicSellerId,
        Guid techSellerId,
        Guid adminId,
        IReadOnlyDictionary<string, int> subCategories,
        IReadOnlyDictionary<string, int> attributes)
    {
        var colors = new[] { "Preto", "Sunburst", "Vermelho", "Natural", "Branco", "Azul" };
        var brands = new[] { "Fender", "Jackson", "Tagima", "Ibanez", "Acer", "Robert C. Martin", "Coca-Cola", "Foxer" };
        var variation = index % 8;
        var color = colors[index % colors.Length];
        var brand = brands[index % brands.Length];

        return variation switch
        {
            0 => ProductWithAttributes(
                musicSellerId,
                subCategories["Guitarras"],
                sku,
                $"Guitarra {brand} Studio {index:0000} {color}",
                "Guitarra demonstrativa com boa tocabilidade, captadores versateis e acabamento inspirado no catalogo legado.",
                1590m + (index % 80 * 37.5m),
                index,
                index % 2 == 0 ? "fender-mex-black-014-5102-506.jpg" : "1094938_guitarra-jackson-monarkh-js22-585-transparent-black-ms_z1_637387173152514626.jpg",
                attributes,
                brand,
                $"ST-{index:0000}",
                color,
                $"{3 + (index % 4) * 0.2m:0.0} kg"),
            1 => ProductWithAttributes(
                musicSellerId,
                subCategories["Violoes"],
                sku,
                $"Violao Tagima Dallas Serie {index:0000}",
                "Violao eletroacustico para estudo, ensaio e palco com afinador embutido.",
                520m + (index % 70 * 12.9m),
                index,
                "1090123_violao-tagima-dallas-tuner-eletrico-cordas-de-aco-e-com-afinador-ms_z2_637371635447323128.jpg",
                attributes,
                "Tagima",
                $"DAL-{index:0000}",
                color,
                $"{2 + (index % 3) * 0.3m:0.0} kg"),
            2 => ProductWithAttributes(
                techSellerId,
                subCategories["Tecnologia"],
                sku,
                $"Livro Clean Series {index:0000}",
                "Livro de tecnologia para praticas modernas de desenvolvimento, arquitetura e qualidade de software.",
                49m + (index % 35 * 2.75m),
                index,
                index % 2 == 0 ? "clean-code-1.jpg" : "clean-architecture.jpg",
                attributes,
                "Alta Books",
                $"BOOK-{index:0000}",
                "Capa colorida",
                "0.6 kg",
                "Alta Books"),
            3 => ProductWithAttributes(
                techSellerId,
                subCategories["Notebooks"],
                sku,
                $"Notebook Acer Predator Helios {index:0000}",
                "Notebook para jogos, desenvolvimento e criacao com configuracao de alto desempenho.",
                3990m + (index % 120 * 45m),
                index,
                "Acer-Predator-Helios-300-PH315-52-748u.jpg",
                attributes,
                "Acer",
                $"PH-{index:0000}",
                "Preto",
                $"{2 + (index % 5) * 0.15m:0.0} kg"),
            4 => ProductWithAttributes(
                adminId,
                subCategories["Bicicletas"],
                sku,
                $"Bicicleta Foxer Aro 26 Modelo {index:0000}",
                "Bicicleta urbana para passeio e deslocamento diario, com quadro resistente.",
                890m + (index % 60 * 18m),
                index,
                "910061_bici_foxer_aro26_1_z.jpg",
                attributes,
                "Foxer",
                $"FX-{index:0000}",
                color,
                $"{13 + (index % 6) * 0.5m:0.0} kg"),
            5 => ProductWithAttributes(
                adminId,
                subCategories["Refrigerantes"],
                sku,
                $"Coca-Cola Zero 350ml Pack {index:0000}",
                "Bebida pronta para consumo, cadastrada para compor o catalogo de testes.",
                3.99m + (index % 10 * 0.45m),
                index,
                "cocacola_zero_350ml.jpg",
                attributes,
                "Coca-Cola",
                $"CZ-{index:0000}",
                "Lata",
                "0.35 kg"),
            6 => ProductWithAttributes(
                musicSellerId,
                subCategories["Baixos"],
                sku,
                $"Baixo Ibanez RG Serie {index:0000}",
                "Baixo eletrico com resposta firme para ensaios, gravacoes e apresentacoes.",
                1790m + (index % 80 * 25m),
                index,
                "Ibanez-RG-7420Z.jpg",
                attributes,
                "Ibanez",
                $"RG-{index:0000}",
                color,
                $"{4 + (index % 4) * 0.25m:0.0} kg"),
            _ => ProductWithAttributes(
                techSellerId,
                subCategories["Agilidade"],
                sku,
                $"Clean Agile Edicao {index:0000}",
                "Livro sobre metodos ageis e praticas de produto para equipes modernas.",
                39m + (index % 40 * 2.3m),
                index,
                "clean_agile.jpg",
                attributes,
                "Robert C. Martin",
                $"AGILE-{index:0000}",
                "Capa brochura",
                "0.5 kg",
                "Alta Books")
        };
    }

    private static Product ProductWithAttributes(
        Guid userId,
        int subCategoryId,
        string sku,
        string title,
        string description,
        decimal price,
        int index,
        string image,
        IReadOnlyDictionary<string, int> attributes,
        string brand,
        string model,
        string color,
        string weight,
        string? publisher = null)
    {
        var product = DemoProduct(userId, subCategoryId, sku, title, description, price, 5 + index % 45, index % 3 == 0, image);
        AddAttributeValue(product, attributes, "Marca", brand);
        AddAttributeValue(product, attributes, "Modelo", model);
        AddAttributeValue(product, attributes, "Cor", color);
        AddAttributeValue(product, attributes, "Peso", weight);

        if (!string.IsNullOrWhiteSpace(publisher))
        {
            AddAttributeValue(product, attributes, "Editora", publisher);
        }

        return product;
    }

    private static void AddAttributeValue(Product product, IReadOnlyDictionary<string, int> attributes, string name, string value)
    {
        if (attributes.TryGetValue(name, out var attributeId))
        {
            product.AttributeValues.Add(new ProductAttributeValue
            {
                AttributeDefinitionId = attributeId,
                Value = value
            });
        }
    }

    private static async Task EnsureSimilarProductLinksAsync(MarketplaceDbContext context, CancellationToken cancellationToken)
    {
        var generatedIds = await context.Products
            .Where(item => item.Sku.StartsWith("DEMO-BULK-"))
            .OrderBy(item => item.Id)
            .Select(item => item.Id)
            .ToListAsync(cancellationToken);

        if (generatedIds.Count < 4)
        {
            return;
        }

        var existing = await context.SimilarProducts
            .Where(item => generatedIds.Contains(item.ParentProductId))
            .Select(item => new { item.ParentProductId, item.ChildProductId })
            .ToListAsync(cancellationToken);
        var existingPairs = existing.Select(item => (item.ParentProductId, item.ChildProductId)).ToHashSet();

        for (var index = 0; index < generatedIds.Count; index++)
        {
            var parentId = generatedIds[index];
            for (var offset = 1; offset <= 3; offset++)
            {
                var childId = generatedIds[(index + offset) % generatedIds.Count];
                if (parentId == childId || !existingPairs.Add((parentId, childId)))
                {
                    continue;
                }

                context.SimilarProducts.Add(new SimilarProduct
                {
                    ParentProductId = parentId,
                    ChildProductId = childId
                });
            }
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    private static Product DemoProduct(
        Guid userId,
        int subCategoryId,
        string sku,
        string title,
        string description,
        decimal price,
        int stock,
        bool offer,
        params string[] images) =>
        new()
        {
            UserId = userId,
            SubCategoryId = subCategoryId,
            Title = title,
            Description = description,
            Price = price,
            CreatedBy = "seed",
            Offer = offer,
            Stock = stock,
            Sku = sku,
            Images = images
                .Select(Marketplace.Api.Features.Products.ProductImageStorage.NormalizeFileName)
                .Where(image => !string.IsNullOrWhiteSpace(image))
                .Select(image => new ProductImage { FileName = image! })
                .ToList()
        };

    private static async Task EnsureDemoCarouselAsync(MarketplaceDbContext context, CancellationToken cancellationToken)
    {
        await context.CarouselImages
            .Where(item => item.FileName == "guitar-1920x384-1.jpg"
                || item.FileName == "guitar-1920x384-2.jpg"
                || item.FileName == "guitar-1920x384-3.jpg"
                || item.FileName == "black-friday.png")
            .ExecuteDeleteAsync(cancellationToken);

        var items = new[]
        {
            (FileName: "/uploads/carousel/guitar-1920x384-2.jpg", SortOrder: 1),
            (FileName: "/uploads/carousel/guitar-1920x384-1.jpg", SortOrder: 2),
            (FileName: "/uploads/carousel/guitar-1920x384-3.jpg", SortOrder: 3),
            (FileName: "/uploads/carousel/black-friday.png", SortOrder: 4)
        };

        foreach (var item in items)
        {
            var image = await context.CarouselImages.FirstOrDefaultAsync(entity => entity.FileName == item.FileName, cancellationToken);
            if (image is null)
            {
                context.CarouselImages.Add(new CarouselImage { FileName = item.FileName, SortOrder = item.SortOrder });
            }
            else
            {
                image.SortOrder = item.SortOrder;
            }
        }
    }

    private static async Task EnsureDemoAddressAsync(MarketplaceDbContext context, Guid userId, CancellationToken cancellationToken)
    {
        if (await context.Addresses.AnyAsync(item => item.UserId == userId, cancellationToken))
        {
            return;
        }

        var state = await context.States.FirstAsync(item => item.Abbreviation == "RS", cancellationToken);
        context.Addresses.Add(new Address
        {
            UserId = userId,
            StateId = state.Id,
            Street = "Rua whatever, 376",
            Cep = "95000-000",
            Neighborhood = "Cruzeiro",
            City = "Caxias do Sul",
            Complement = "Casa"
        });
    }

    private static async Task<ApplicationUser> EnsureUserAsync(
        UserManager<ApplicationUser> userManager,
        string userName,
        string email,
        string name,
        string lastName,
        string role)
    {
        var user = await userManager.FindByNameAsync(userName);
        if (user is not null)
        {
            if (!await userManager.IsInRoleAsync(user, role))
            {
                await userManager.AddToRoleAsync(user, role);
            }

            return user;
        }

        user = new ApplicationUser
        {
            UserName = userName,
            Email = email,
            Name = name,
            LastName = lastName,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, "ChangeMe123!");
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(string.Join("; ", result.Errors.Select(error => error.Description)));
        }

        await userManager.AddToRoleAsync(user, role);
        return user;
    }
}
