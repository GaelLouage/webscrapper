using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//if (!builder.Environment.IsDevelopment())
//{
//    builder.Services.AddHttpsRedirection(options =>
//    {
//        options.RedirectStatusCode = (int)HttpStatusCode.PermanentRedirect;
//        options.HttpsPort = 3000;
//    });
//}
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHsts();





app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
