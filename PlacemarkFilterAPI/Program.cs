
using PlacemarkFilterAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Adiciona serviços ao contêiner

// Adiciona suporte a controladores
builder.Services.AddControllers();

// Configura o Swagger para documentação da API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registra o KmlService como um serviço singleton
builder.Services.AddSingleton<KmlService>();

var app = builder.Build();

// Configura o pipeline de requisições HTTP
if (app.Environment.IsDevelopment())
{
    // Configura o Swagger apenas no ambiente de desenvolvimento
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redireciona requisições HTTP para HTTPS
app.UseHttpsRedirection();

// Configura a autorização (pode ser removido se não for necessário)
app.UseAuthorization();

// Mapeia os controladores
app.MapControllers();

// Executa o aplicativo
app.Run();
