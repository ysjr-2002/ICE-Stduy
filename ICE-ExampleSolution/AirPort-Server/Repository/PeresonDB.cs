using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirPort.Server.Repository
{
    class PeresonDB : IRepository<persons>
    {
        public void Add(persons person)
        {
            using (var db = new personrepositoryEntities1())
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

        public void Delete(int faceId)
        {

        }

        public IEnumerable<persons> Search()
        {
            List<persons> list = new List<Repository.persons>();

            return list;
        }

        public void Update(persons t)
        {

        }
    }
}
