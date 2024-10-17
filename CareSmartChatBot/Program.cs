using CareSmartChatBot.DBContext;
using CareSmartChatBot.Repositories;
using CareSmartChatBot.Services;
using Microsoft.EntityFrameworkCore;
using Quartz;
using CareSmartChatBot.Scheduler;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add Quartz services
builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();

    // Register the job and trigger
    var jobKey = new JobKey("IntercomJob");
    q.AddJob<IntercomJob>(opts => opts.WithIdentity(jobKey));
    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("IntercomJob-trigger")
        .WithCronSchedule("0 0 0 * * ?") // Runs every midnight
        .StartNow());
});

// Add Quartz.NET hosted service
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IIntercomRepo, IntercomRepo>();
builder.Services.AddScoped<IIntercomService, IntercomService>();

// Register DbContext
builder.Services.AddDbContext<ApplicationDBContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<TimedHostedService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
