using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;
using WineShop.Domain.EMail;
using WineShop.Domain.Identity;
using WineShop.Repository;
using WineShop.Repository.Implementation;
using WineShop.Repository.Interface;
using WineShop.Services;
using WineShop.Services.Implementation;
using WineShop.Services.Interface;

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");
try
{
    

    var builder = WebApplication.CreateBuilder(args);


    builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("EmailSettings"));


    // Configure NLog as the logging provider
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    // Add services to the container.
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(connectionString));
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();

    builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
    builder.Services.AddScoped(typeof(IUserRepository), typeof(UserRepository));
    builder.Services.AddScoped(typeof(IOrderRepository), typeof(OrderRepository));

    builder.Services.AddScoped<IProductService, ProductService>();
    builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();
    builder.Services.AddScoped<IEmailService, EmailService>();
    builder.Services.AddScoped<IOrderService, OrderService>();
    builder.Services.AddScoped<IBackGroundEmailSender, BackGroundEmailSender>();
    //builder.Services.AddHostedService<ConsumeScopedHostedService>();



    builder.Services.AddDefaultIdentity<EShopApplicationUser>(options =>
        options.SignIn.RequireConfirmedAccount = true)
        .AddEntityFrameworkStores<ApplicationDbContext>();

    builder.Services.AddControllersWithViews()
        .AddNewtonsoftJson(options =>
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

   
    builder.Services.AddRazorPages();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseMigrationsEndPoint();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
    app.MapRazorPages();

    app.Run();
}
catch (Exception exception)
{
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    LogManager.Shutdown();
}
