using System;
using System.Collections.Generic;
using System.Data.Entity;
using TheCCPConnection.Models;

namespace TheCCPConnection.DAL
{
    public class RequestDBInitializer : CreateDatabaseIfNotExists<RequestContext>
    {
        protected override void Seed(RequestContext requestContext)
        {
            // seed terms for next 20 years
            var termSeasonList = new string[] { "Spring", "Summer", "Autumn" };
            var terms = new List<Term>();
            for (int year = 21; year < 41; year++)
            {
                for (int i = 0; i < termSeasonList.Length; i++)
                {
                    var term = new Term { Name = termSeasonList[i] + " " + "20" + year.ToString() };
                    terms.Add(term);
                }
            }
            terms.ForEach(t => requestContext.Terms.Add(t));
            requestContext.SaveChanges();

            // seed only potential schools for beta testing
            var schools = new List<School>
            {
                new School { Name = "Columbus State Community College", Type = SchoolType.College },
                new School { Name = "Reynoldsburg (HS)2", Type = SchoolType.HighSchool },
                new School { Name = "Reynoldsburg BELL", Type = SchoolType.HighSchool },
                new School { Name = "Reynoldsburg eSTEM", Type = SchoolType.HighSchool },
                new School { Name = "Reynoldsburg Encore", Type = SchoolType.HighSchool },
                new School { Name = "Reynoldsburg 9x", Type = SchoolType.HighSchool }
            };
            schools.ForEach(s => requestContext.Schools.Add(s));
            requestContext.SaveChanges();

            // reseed PK identity with new start value
            requestContext.Database.ExecuteSqlCommand("DBCC CHECKIDENT ('ParentPIN', RESEED, 1000)");
            requestContext.SaveChanges();
        }
    }
}