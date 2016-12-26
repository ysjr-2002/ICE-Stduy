using Common.Log;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
namespace AirPort.Server.Repository
{
    class PersonDB : IRepository<person>
    {
        public void Test()
        {
            using (var db = new personrepositoryEntities())
            {
                try
                {
                    var count = db.persons.Count();
                    print("test ok->" + count);
                }
                catch
                {
                    print("test error");
                }
            }
        }

        public bool UUIDExist(string uuid)
        {
            using (var db = new personrepositoryEntities())
            {
                try
                {
                    var count = db.persons.Count(s => s.UUID == uuid);
                    return count > 0;
                }
                catch (DbEntityValidationException ex)
                {
                    throw;
                }
            }
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

        public IEnumerable<person> Search(Pagequery page, string faceId, string uuid, string code, string[] tags)
        {
            List<person> list = new List<Repository.person>();
            using (var db = new personrepositoryEntities())
            {
                try
                {
                    Stopwatch sw = Stopwatch.StartNew();
                    var tagLink = GetTagIn(tags);
                    var persontags = (db.persontags.Where(c => tags.Contains(c.TagName))
                        .Select(s => new
                        {
                            FaceID = s.FaceID
                        })).Distinct();

                    var query = (from n in db.persons
                                 join tag in persontags on n.FaceID equals tag.FaceID
                                 select n);

                    if (!faceId.IsEmpty())
                        query = query.Where(s => s.FaceID.StartsWith(faceId));

                    if (!uuid.IsEmpty())
                        query = query.Where(s => s.UUID.StartsWith(uuid));

                    if (!code.IsEmpty())
                        query = query.Where(s => s.Code.StartsWith(code));

                    page.TotalCount = query.Count();
                    list = query.Select(n => n).OrderBy(n => n.CreateTime).Skip(page.Offset).Take(page.Pagesize).ToList();

                    sw.Stop();
                    Console.WriteLine("查询->" + sw.ElapsedMilliseconds);
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
                    query = query.Skip(page.Offset).Take(page.Pagesize);

                    list = query.ToList();
                }
                catch (DbEntityValidationException ex)
                {
                    throw;
                }
            }
            return list;
        }

        public void Update(person person)
        {
            using (var db = new personrepositoryEntities())
            {
                try
                {
                    db.persons.Attach(person);
                    db.Entry(person).State = System.Data.Entity.EntityState.Modified;
                    var count = db.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    throw;
                }
            }
        }

        public void AddPersonTag(string faceId, string[] tags)
        {
            using (var db = new personrepositoryEntities())
            {
                //删除旧标签
                var sql = "delete from persontags where faceid='" + faceId + "'";
                var count = db.Database.ExecuteSqlCommand(sql);
                print("删除旧标签->" + count);
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

        public string[] GetPersonTags(string faceId)
        {
            List<string> tags = new List<string>();
            using (var db = new personrepositoryEntities())
            {
                tags = db.persontags.Where(s => s.FaceID == faceId).Select(s => s.TagName).ToList();
            }
            return tags.ToArray();
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

        private void print(string content)
        {
            LogHelper.Info(content);
        }
    }
}
