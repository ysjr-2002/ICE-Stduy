using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
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
                    var sql = "DELETE FROM persons WHERE uuid='{0}'";
                    sql = string.Format(sql, person.UUID);
                    db.Database.ExecuteSqlCommand(sql);
                }
                catch (DbEntityValidationException ex)
                {
                    throw;
                }
            }
        }

        public IEnumerable<person> Search()
        {
            List<person> list = new List<Repository.person>();
            using (var db = new personrepositoryEntities())
            {
                try
                {
                    list = db.persons.ToList();
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
            using (var db = new personrepositoryEntities())
            {
                foreach (var tag in tags)
                {

                }
            }
            return affectcount;
        }
    }
}
