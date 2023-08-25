using API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class DataContext: IdentityDbContext<User, Role, int, 
        IdentityUserClaim<int>, UserRole, IdentityUserLogin<int>, 
        IdentityRoleClaim<int>, IdentityUserToken<int>>{
    public DataContext(DbContextOptions options): base(options){

    }

    public DbSet<UserLike> Likes { get; set; }
    public DbSet<Message> Messages { get; set; }

    protected override void OnModelCreating(ModelBuilder builder){
        base.OnModelCreating(builder);

        //JOIN TABLE
            builder.Entity<User>()
                .HasMany(ur => ur.UserRoles)
                .WithOne(u => u.User)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();

            builder.Entity<Role>()
                .HasMany(ur => ur.UserRoles)
                .WithOne(u => u.Role)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();
        //MODELS
        builder.Entity<UserLike>()
            .HasKey(KEY => new {KEY.SourceUserId, KEY.TargetUserId}); //PRIAMRY KEY
        builder.Entity<UserLike>().HasOne(S => S.SourceUser).WithMany(L => L.LikedUsers).HasForeignKey(S => S.SourceUserId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Entity<UserLike>().HasOne(S => S.TargetUser).WithMany(L => L.LikedByUsers).HasForeignKey(S => S.TargetUserId)
            .OnDelete(DeleteBehavior.Cascade);

        //MESSAGES
        builder.Entity<Message>().HasOne(S => S.Recipient).WithMany(L => L.MessagesReceived)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Entity<Message>().HasOne(S => S.Sender).WithMany(L => L.MessagesSend)
            .OnDelete(DeleteBehavior.Restrict);
    }

}