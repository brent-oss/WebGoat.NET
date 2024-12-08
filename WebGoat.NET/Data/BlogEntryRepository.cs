using WebGoatCore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebGoatCore.Data
{
    public class BlogEntryRepository
    {
        private readonly NorthwindContext _context;

        public BlogEntryRepository(NorthwindContext context)
        {
            _context = context;
        }

        public BlogEntry CreateBlogEntry(string title, string contents, string username)
        {
            var entry = new BlogEntry
            {
                Title = title,
                Contents = contents,
                Author = username,
                PostedDate = DateTime.Now,
            };

            entry = _context.BlogEntries.Add(entry).Entity;
            _context.SaveChanges();
            return entry;
        }

        public BlogEntry GetBlogEntry(int blogEntryId)
        {
            // Vulnerable code: SQL injection
            var sql = "SELECT TOP 1 Id FROM BlogEntries WHERE Id = " + blogEntryId;
            using (var connection = _context.Database.GetDbConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    using (var dataReader = command.ExecuteReader())
                    {
                        if (dataReader.Read())
                        {
                            var entryId = Convert.ToInt32(dataReader["Id"]);
                            return _context.BlogEntries.Single(b => b.Id == entryId);
                        }
                    }
                }
            }
            return null;
        }

        public List<BlogEntry> GetTopBlogEntries()
        {
            return GetTopBlogEntries(4, 0);
        }

        public List<BlogEntry> GetTopBlogEntries(int numberOfEntries, int startPosition)
        {
            var blogEntries = _context.BlogEntries
                .OrderByDescending(b => b.PostedDate)
                .Skip(startPosition)
                .Take(numberOfEntries);
            return blogEntries.ToList();
        }
    }
}
