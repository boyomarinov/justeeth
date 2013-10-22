﻿namespace JusTeeth.Data
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using JusTeeth.Models;
    using Microsoft.AspNet.Identity.EntityFramework;

    public class ApplicationDbContext : IdentityDbContextWithCustomUser<ApplicationUser>
    {
        public IDbSet<Place> Places { get; set; }
 
        public IDbSet<Feedback> Feedbacks { get; set; }
 
        public IDbSet<HungryGroup> HungryGroups { get; set; }
 
        public IDbSet<Department> Departments { get; set; } 
        
        public IDbSet<Workplace> Workplaces { get; set; }

        public IDbSet<Image> Images { get; set; } 

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Department>()
                .HasMany<ApplicationUser>(d => d.Employees);

            modelBuilder.Entity<Workplace>()
                .HasMany<ApplicationUser>(u => u.Employees);

            modelBuilder.Entity<Place>()
                .HasMany<ApplicationUser>(u => u.Visitors);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany<Place>(u => u.LastPlaces);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.LastPlaces)
                .WithMany(x => x.Visitors)
                .Map(w =>
                {
                    w.ToTable("User_Places")
                        .MapLeftKey("UserId")
                        .MapRightKey("PlaceId");
                });

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Friends)
                .WithMany().Map(w =>
                {
                    w.ToTable("User_Friends")
                    .MapLeftKey("UserId")
                    .MapRightKey("FriendId");
                });

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.GroupHistory)
                .WithMany(x => x.HungryUsers)
                .Map(w => {
                    w.ToTable("User_Groups")
                        .MapLeftKey("UserId")
                        .MapRightKey("GroupId");
                });
            //base.OnModelCreating(modelBuilder);
        }
    }
}
