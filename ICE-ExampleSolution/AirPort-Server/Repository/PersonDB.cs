using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirPort.Server.Repository
{
    class PersonDB : IRepository<person>
    {
        public void Test()
        {
            var db = new personrepositoryEntities();
            var count = db.persons.Count();
            Console.WriteLine("text ok->" + count);
        }

        public void Add(person person)
        {
            using (var db = new personrepositoryEntities())
            {
                try
                {
                    db.persons.Add(person);
                    db.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    throw;
                }
            }
        }

        public void Delete(person person)
        {
            using (var db = new personrepositoryEntities())
            {
                try
                {
                    var sql = "DELETE FROM persons WHERE faceID='{0}'";
                    sql = string.Format(sql, person.FaceID);
                    db.Database.ExecuteSqlCommand(sql);
                }
                catch (DbEntityValidationException ex)
                {
                    throw;
                }
            }
        }

        public IEnumerable<person> Search(Pagequery page, string[] tags)
        {
            List<person> list = new List<Repository.person>();
            using (var db = new personrepositoryEntities())
            {
                try
                {
                    Stopwatch sw = Stopwatch.StartNew();
                    var tagLink = GetTagIn(tags);
                    var t = db.persontags.Where(c => tags.Contains(c.TagName)).Select(s => s.FaceID).Distinct();
                    var query = db.persons.Where(p => t.Contains(p.FaceID)).OrderBy(s => s.CreateTime);
                    page.TotalCount = query.Count();
                    list = query.Skip(page.Offset).Take(page.Pagesize).ToList();

                    sw.Stop();
                    Console.WriteLine("数据库耗时->" + sw.ElapsedMilliseconds);
                }
                catch (DbEntityValidationException ex)
                {
                    throw;
                }
            }
            return list;
        }

        public IEnumerable<person> Search1VN(Pagequery page, string[] faceIds, string[] tags)
        {
            List<person> list = new List<Repository.person>();
            using (var db = new personrepositoryEntities())
            {
                try
                {
                    var tagLink = GetTagIn(tags);

                    IEnumerable<string> tagtofaceID = null;
                    if (tags.Length > 0)
                        tagtofaceID = db.persontags.Where(c => tags.Contains(c.TagName)).Select(s => s.FaceID).Distinct();

                    IQueryable<person> query = db.persons.Where(f => f.Code.Length > 0);
                    if (tagtofaceID != null)
                    {
                        query = query.Where(p => tagtofaceID.Contains(p.FaceID));
                    }
                    query = query.Where(p => faceIds.Contains(p.FaceID)).OrderBy(s => s.CreateTime);
                    page.TotalCount = query.Count();
                    list = query.Skip(page.Offset).Take(page.Pagesize).ToList();
                }
                catch (DbEntityValidationException ex)
                {
                    throw;
                }
            }
            return list;
        }

        public void Update(person t)
        {

        }

        public void AddPersonTag(string faceId, string[] tags)
        {
            using (var db = new personrepositoryEntities())
            {
                foreach (var tag in tags)
                {
                    persontag pt = new Repository.persontag()
                    {
                        FaceID = faceId,
                        TagName = tag
                    };
                    db.persontags.Add(pt);
                }
                db.SaveChanges();
            }
        }

        public void UpdatePersonTag(string faceId, string[] tags)
        {
            using (var db = new personrepositoryEntities())
            {
                var haveTags = db.persontags.Where(s => s.FaceID == faceId).Select(s => s.TagName).ToArray();
                var newTags = tags.Except(haveTags).ToArray();
                foreach (var tag in newTags)
                {
                    persontag pt = new Repository.persontag()
                    {
                        FaceID = faceId,
                        TagName = tag
                    };
                    db.persontags.Add(pt);
                }
                db.SaveChanges();
            }
        }

        public void DeletePersonTag(string faceId, string[] tags)
        {
            using (var db = new personrepositoryEntities())
            {
                if (tags.Length == 0)
                {
                    var sql = "delete from persontags where faceId='{0}'";
                    sql = string.Format(sql, faceId);
                    db.Database.ExecuteSqlCommand(sql);
                }
                else
                {
                    foreach (var tag in tags)
                    {
                        var sql = "delete from persontags where faceId='{0}' and TagName='{1}'";
                        sql = string.Format(sql, faceId, tag);
                        db.Database.ExecuteSqlCommand(sql);
                    }
                }
            }
        }

        public int DeleteByTags(string[] tags)
        {
            var affectcount = 0;
            var tagLink = GetTagIn(tags);
            using (var db = new personrepositoryEntities())
            {
                var deltags = "delete from persontags where tagname in({0})";
                var delPerson = "delete from persons where faceId in(select faceId from persontags where tagname in({0}))";

                delPerson = string.Format(delPerson, tagLink);
                deltags = string.Format(deltags, tagLink);

                affectcount = db.Database.ExecuteSqlCommand(delPerson);
                db.Database.ExecuteSqlCommand(deltags);
            }
            return affectcount;
        }

        private string GetTagIn(string[] tags)
        {
            if (tags.Length == 0)
                return string.Empty;

            var sb = new StringBuilder();
            foreach (var tag in tags)
            {
                sb.Append(string.Format("'{0}',", tag));
            }

            var tagLink = sb.ToString();
            tagLink = tagLink.Remove(tagLink.Length - 1, 1);
            return tagLink;
        }
    }
}
