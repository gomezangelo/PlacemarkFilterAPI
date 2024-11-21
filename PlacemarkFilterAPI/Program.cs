
using PlacemarkFilterAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Adiciona servi�os ao cont�iner

// Adiciona suporte a controladores
builder.Services.AddControllers();

// Configura o Swagger para documenta��o da API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registra o KmlService como um servi�o singleton
builder.Services.AddSingleton<KmlService>();

var app = builder.Build();

// Configura o pipeline de requisi��es HTTP
if (app.Environment.IsDevelopment())
{
    // Configura o Swagger apenas no ambiente de desenvolvimento
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redireciona requisi��es HTTP para HTTPS
app.UseHttpsRedirection();

// Configura a autoriza��o (pode ser removido se n�o for necess�rio)
app.UseAuthorization();

// Mapeia os controladores
app.MapControllers();

// Executa o aplicativo
app.Run();
