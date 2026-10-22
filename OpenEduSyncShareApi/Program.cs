var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var quickDatabase = new Dictionary<string, string>()
{
    {"f44d37aac13c5b189432_2_1", "Профиль"},
    {"0f1f63121d0bf7f492d1_2_1", "График открытия материалов"},
    {"842cadde7ba29cfb5278_2_1", "choice_2"},
    {"22cd477f6c2fe4f2c2a4_2_1", "Зачтено"},
    {"aa93778da50892b49f94_2_1", "Прогресс"},
    {"b4215ecdee6f3a7272e2_2_1", "openedu@urfu.ru"},
    {"84f7c4e1c407cbf96587_2_1", "01-12-2026"},
    {"fea201ad1e1144068349f87f0a025a8e_2_1", "choice_0&choice_2&choice_4"}
};

app.MapPost("/api/answers", (List<string> queries) =>
{
    var result = new Dictionary<string, string?>();

    foreach (var query in queries)
    {
        quickDatabase.TryGetValue(query, out var answer);
        result[query] = answer;
    }

    return Results.Ok(result);
});

app.Run();