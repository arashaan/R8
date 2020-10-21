namespace R8.Lib.AspNetCore.EntityFrameworkCore
{
    //public static class DatabaseInitializer
    //{
    //    public static IApplicationBuilder UseDbContextInitializerR8<TContext>(this IApplicationBuilder app, Action<TContext> dbContext = null) where TContext : DbContextBase
    //    {
    //        var scopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
    //        using var scope = scopeFactory.CreateScope();
    //        using var context = scope.ServiceProvider.GetService<TContext>();
    //        if (context == null)
    //            throw new NullReferenceException($"Unable to find DbContext");

    //        var database = context.Database;
    //        if (database == null)
    //            throw new NullReferenceException($"Unable to find Database");

    //        // var deleted = database.EnsureDeleted();
    //        // if (deleted)
    //        // {
    //        //var created = database.EnsureCreated();
    //        //if (!created)
    //        //    context.Database.Migrate();

    //        // }
    //        //var connect = database.CanConnect();
    //        //if (connect)
    //        //    return app;

    //        //var pendingMigrations = context.Database.GetPendingMigrations().ToList();
    //        //if (pendingMigrations.Count == 0)
    //        //    throw new Exception("First you need to initialize your DbContext, by using Add-Migration [MigrationName]");

    //        //// initial db
    //        //context.Database.Migrate();
    //        dbContext?.Invoke(context);

    //        var isChanged = context.ChangeTracker.HasChanges();
    //        if (isChanged)
    //            context.SaveChanges();

    //        context.Dispose();
    //        scope.Dispose();
    //        return app;
    //    }
    //}
}