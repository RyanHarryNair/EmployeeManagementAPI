using EmployeeManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementAPI.Data.Context
{
    public class EmployeeContext : DbContext
    {
        // Connect Database with .Net Core // Choose context name that is relevant to the project..ie EmployeeContext
        public EmployeeContext(DbContextOptions<EmployeeContext> options):base(options) // Pass DBContextOptions for this Employee Context. We pass this 'options' inside 'builder.Services' inside 'Program.cs'
        {

        }
        //Map all the models with my tables or Set all the properties with my tables
        //The 'UserModels' property represents the collection of 'UserModel' entities in the database. It can be used to query, insert, update, and delete 'UserModel' objects using Entity Framework's API.
        public DbSet<UserModel> UserModels { get; set; } // Ctrl + . to quickly fix issues
        public DbSet<RoleModel> RoleModels { get; set; }
        public DbSet<LeaveModel> LeaveModels { get; set; }
        public DbSet<DepartmentModel> DepartmentModels { get; set; }

        //This model belongs to.Net. We are here to store it in database. For that there is 'OnModelCreating'
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserModel>().ToTable("tbl_User"); // modelbuilder holds the 'usermodel' entity. I want this entity to be send to a table named tbl_User in the database.
            modelBuilder.Entity<LeaveModel>().ToTable("tbl_Leave"); // modelBuilder takes your data and helps you to shape it in a SQL table format
            modelBuilder.Entity<DepartmentModel>().ToTable("tbl_Department");
            modelBuilder.Entity<RoleModel>().ToTable("tbl_Role");
        }
    }
}
//Now we need to connect the model to the database...startup.cs