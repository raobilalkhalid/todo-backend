using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Task.Models.DBModels
{
    public partial class vueappContext : DbContext
    {

        public vueappContext(DbContextOptions<vueappContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Todos> Todos { get; set; }
        public virtual DbSet<User> User { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySql("server=localhost;port=3306;user=root;password=121775;database=vueapp", x => x.ServerVersion("8.0.25-mysql"));
            }
        }


    }
}
