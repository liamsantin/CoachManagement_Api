using System.Text;
using CoachManagement_Api.Data;
using CoachManagement_Api.Repositories;
using CoachManagement_Api.Repositories.interfaces;
using CoachManagement_Api.Services;
using CoachManagement_Api.Services.interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<CoachManagementDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddHttpContextAccessor();

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITeamRepository, TeamRepository>();
builder.Services.AddScoped<IClubRepository, ClubRepository>();
builder.Services.AddScoped<ILocaliteRepository, LocaliteRepository>();
builder.Services.AddScoped<ICantonRepository, CantonRepository>();
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<ITrainingRepository, TrainingRepository>();
builder.Services.AddScoped<IAttendanceRepository, AttendanceRepository>();
builder.Services.AddScoped<ILeagueRepository, LeagueRepository>();
builder.Services.AddScoped<IStatusMatchRepository, StatusMatchRepository>();
builder.Services.AddScoped<ITypesMatchRepository, TypesMatchRepository>();
builder.Services.AddScoped<ITypesTrainingRepository, TypesTrainingRepository>();
builder.Services.AddScoped<IPositionRepository, PositionRepository>();
builder.Services.AddScoped<IOpponentRepository, OpponentRepository>();
builder.Services.AddScoped<ISeasonRepository, SeasonRepository>();
builder.Services.AddScoped<IFormationRepository, FormationRepository>();
builder.Services.AddScoped<IClubsLocalitesRepository, ClubsLocalitesRepository>();
builder.Services.AddScoped<IMatchRepository, MatchRepository>();
builder.Services.AddScoped<IParticipationRepository, ParticipationRepository>();
builder.Services.AddScoped<IPlayersLineupRepository, PlayersLineupRepository>();
builder.Services.AddScoped<IReplacementRepository, ReplacementRepository>();
builder.Services.AddScoped<ILineupRepository, LineupRepository>();

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<IClubService, ClubService>();
builder.Services.AddScoped<ILocaliteService, LocaliteService>();
builder.Services.AddScoped<ICantonService, CantonService>();
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<ITrainingService, TrainingService>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<ILeagueService, LeagueService>();
builder.Services.AddScoped<IStatusMatchService, StatusMatchService>();
builder.Services.AddScoped<ITypesMatchService, TypesMatchService>();
builder.Services.AddScoped<ITypesTrainingService, TypesTrainingService>();
builder.Services.AddScoped<IPositionService, PositionService>();
builder.Services.AddScoped<IOpponentService, OpponentService>();
builder.Services.AddScoped<ISeasonService, SeasonService>();
builder.Services.AddScoped<IFormationService, FormationService>();
builder.Services.AddScoped<IClubsLocalitesService, ClubsLocalitesService>();
builder.Services.AddScoped<IMatchService, MatchService>();
builder.Services.AddScoped<IParticipationService, ParticipationService>();
builder.Services.AddScoped<IPlayersLineupService, PlayersLineupService>();
builder.Services.AddScoped<IReplacementService, ReplacementService>();
builder.Services.AddScoped<ILineupService, LineupService>();

var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not configured.");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "CoachManagement_Api";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "CoachManagement_Api";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
