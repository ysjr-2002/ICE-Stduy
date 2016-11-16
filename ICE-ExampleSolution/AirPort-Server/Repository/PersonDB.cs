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
    }
}
