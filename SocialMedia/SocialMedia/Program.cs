using API;
using API.Domain.Entites;
using API.Hubs;
using Application.Interfaces;
using Application.Services;
using Application.UseCases.Auth;
using Application.UseCases.Comments;
using Application.UseCases.Following;
using Application.UseCases.Likes;
using Application.UseCases.Post;
using Application.UseCases.Shares;
using Application.UseCases.User;
using Application.UseCases.Chat;
using Infrastructure.Identity;
using Infrastructure.Presistence;
using Infrastructure.Presistence.Repositories;
using Infrastructure.RealTime;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Text;

namespace SocialMedia
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("DevCors", policy =>
                    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });

                // Add the JWT Bearer definition
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' followed by a space and the JWT token."
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
       {
   {
       new OpenApiSecurityScheme
       {
           Reference = new OpenApiReference
           {
               Type = ReferenceType.SecurityScheme,
               Id = "Bearer"
           }
       },
       Array.Empty<string>()
        }
       });

                var basePath = AppContext.BaseDirectory;

                // API project XML
                var apiXml = Path.Combine(basePath, "E-Learning.xml");
                if (File.Exists(apiXml))
                    c.IncludeXmlComments(apiXml, true);

                // Application project XML
                var appXmlPath = Path.Combine(AppContext.BaseDirectory, @"..\..\..\..\Application\bin\Debug\net8.0\Application.xml");
                appXmlPath = Path.GetFullPath(appXmlPath);
                if (File.Exists(appXmlPath))
                    c.IncludeXmlComments(appXmlPath, true);

                // Domain project XML (optional)
                var domainXmlPath = Path.Combine(AppContext.BaseDirectory, @"..\..\..\..\Domain\bin\Debug\net8.0\Domain.xml");
                domainXmlPath = Path.GetFullPath(domainXmlPath);
                if (File.Exists(domainXmlPath))
                    c.IncludeXmlComments(domainXmlPath, true);
            });

            // Add SignalR
            builder.Services.AddSignalR();

            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddDbContext<SocialMediaContext>
            (options => options.UseSqlServer(builder.Configuration.GetConnectionString("constr")));

                
            builder.Services.AddIdentity<User, IdentityRole>(options =>
            {
                // Password settings (optional)
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            }).AddEntityFrameworkStores<SocialMediaContext>()
              .AddDefaultTokenProviders();
            

            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IAccountServices, AccountServices>();
            builder.Services.AddScoped<IFollowing , Following>();
            builder.Services.AddScoped<IPost, PostRepo>();
            builder.Services.AddScoped<IUserContext, UserContext>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUserManagerService, UserManagerService>();
            
            // Chat Repositories and Services
            builder.Services.AddSingleton<IOnlineUserTracker, OnlineUserTracker>();
            builder.Services.AddScoped<IConversationRepository, ConversationRepository>();
            builder.Services.AddScoped<IMessageRepository, MessageRepository>();
            builder.Services.AddScoped<IChatHubNotifier, ChatHubNotifier>();

            //Authentication usecases registeration
            builder.Services.AddScoped<LoginUseCase>();
            builder.Services.AddScoped<RegisterUserCase>();

            // user usescases registeration
            builder.Services.AddScoped<GetUserProfileUseCase>();
            builder.Services.AddScoped<SearchAllUseCase>();
            builder.Services.AddScoped<SearchUsersUseCase>();
            builder.Services.AddScoped<UpdateUserProfileUseCase>();
            builder.Services.AddScoped<IJWTService,JWTservice>();

            // following usecases registeration
            builder.Services.AddScoped<FollowingUseCase>();
            builder.Services.AddScoped<GetAllFollowersUseCase>();
            builder.Services.AddScoped<GetAllFollowingUseCase>();
            builder.Services.AddScoped<UnFollowUseCase>();
            // posts use cases registeration 
            builder.Services.AddScoped<CreatePostUseCase>();
            builder.Services.AddScoped<DeletePostUseCase>();
            builder.Services.AddScoped<GetPostByIDUseCase>();
            builder.Services.AddScoped<GetUserPostsUseCase>();
            builder.Services.AddScoped<SearchPostsUseCase>();
            builder.Services.AddScoped<UpdatePostUseCase>();
            builder.Services.AddScoped<AddCommentUseCase>();
            builder.Services.AddScoped<GetPostCommentsUseCase>();
            builder.Services.AddScoped<DeleteCommentUseCase>();

            builder.Services.AddScoped<SharePostUseCase>();
            builder.Services.AddScoped<UnSharePostUseCase>();
            builder.Services.AddScoped<GetAllPostSharesUseCase>();
            builder.Services.AddScoped<LikePostUseCase>();
            builder.Services.AddScoped<UnLikePostUseCase>();

            // Chat use cases registration
            builder.Services.AddScoped<CreateDirectConversationUseCase>();
            builder.Services.AddScoped<CreateGroupConversationUseCase>();
            builder.Services.AddScoped<GetUserConversationsUseCase>();
            builder.Services.AddScoped<SendMessageUseCase>();
            builder.Services.AddScoped<GetConversationMessagesUseCase>();
            builder.Services.AddScoped<MarkMessageAsReadUseCase>();
            builder.Services.AddScoped<GetMessageReadReceiptsUseCase>();
            builder.Services.AddScoped<AddGroupMemberUseCase>();
            builder.Services.AddScoped<RemoveGroupMemberUseCase>();



            builder.Services.AddHttpContextAccessor();
            // Authentication with JWT
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
               .AddJwtBearer(options =>
               {
                   options.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuer = true,
                       ValidateAudience = true,

                       ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                       ValidAudience = builder.Configuration["JWT:ValidAudience"],
                       IssuerSigningKey = new SymmetricSecurityKey(
                           Encoding.UTF8.GetBytes(builder.Configuration["JWT:secret"]))
                   };
                   options.Events = new JwtBearerEvents
                   {
                       OnMessageReceived = context =>
                       {
                           var accessToken = context.Request.Query["access_token"];
                           var path = context.HttpContext.Request.Path;
                           if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/chat"))
                           {
                               context.Token = accessToken;
                           }
                           return Task.CompletedTask;
                       }
                   };
               });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("DevCors");

            app.UseAuthorization();

            app.MapControllers();
            app.MapHub<ChatHub>("/hubs/chat");

            app.Run();
        }
    }
}
