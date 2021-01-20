using System;
using Microsoft.EntityFrameworkCore;
using QuotesAPI.Models;

namespace QuotesAPI.Data
{
    public class QuotesDbC: DbContext
    {
        public QuotesDbC(DbContextOptions<QuotesDbC> options) : base(options)
        {
        }

        public DbSet<Quote> Quotes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder){
            modelBuilder.Entity<Quote>().ToTable("Quote");
        }
    }
}
