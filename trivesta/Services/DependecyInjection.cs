using App.Services;
using Microsoft.EntityFrameworkCore;
using Trivesta.Business;
using Trivesta.Data;
using Trivesta.Data.Interface;
using Trivesta.Data.Repository;

namespace trivesta.Services
{
    public class DependecyInjection
    {
        public static void Register(IServiceCollection services)
        {
            services

                .AddTransient<IHttpContextAccessor, HttpContextAccessor>()
                .AddTransient<FlutterwaveServices>()
                .AddSingleton<SingletonBusiness>()
                .AddTransient<LoginValidator>()
                .AddScoped<TrivestaContext>()
                .AddTransient<ICoinTransaction, CoinTransactionRepository>()
                .AddTransient<IRoom, RoomRepository>()
                .AddTransient<IRoomType, RoomTypeRepository>()
                .AddTransient<IRoomMember, RoomMemberRepository>()
                .AddTransient<IMedia, MediaRepository>()
                .AddTransient<ISubscriber, SubscriberRepository>()
                .AddTransient<IUnitOfWork, UnitOfWork>()
                .AddTransient<IUser, UserRepository>()
                .AddTransient<ILoginMonitor, LoginMonitorRepository>()
                .AddTransient<INotification, NotificationRepository>()

                .AddScoped<GeneralBusiness>()
                .AddScoped<NotificationBusiness>()
                .AddScoped<SingletonBusiness>()
                .AddScoped<RoomBusiness>()
                .AddScoped<ManagerBusiness>()
                .AddScoped<UserBusiness>()
                ;
                //.AddTransient<ILoginMonitor, LoginMonitorRepository>()


}
    }
    }