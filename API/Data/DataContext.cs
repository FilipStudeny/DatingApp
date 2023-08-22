using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class DataContext: DbContext{

    public DataContext(DbContextOptions options): base(options){

    }

    public DbSet<User> Users { get; set; }
    public DbSet<UserLike> Likes { get; set; }

    protected override void OnModelCreating(ModelBuilder builder){
        base.OnModelCreating(builder);
        builder.Entity<UserLike>()
            .HasKey(KEY => new {KEY.SourceUserId, KEY.TargetUserId}); //PRIAMRY KEY
        builder.Entity<UserLike>().HasOne(S => S.SourceUser).WithMany(L => L.LikedUsers).HasForeignKey(S => S.SourceUserId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Entity<UserLike>().HasOne(S => S.TargetUser).WithMany(L => L.LikedByUsers).HasForeignKey(S => S.TargetUserId)
            .OnDelete(DeleteBehavior.Cascade);

    }

}