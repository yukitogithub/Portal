using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NeaTICs_v2.Models;
using NeaTICs_v2.DAL;

namespace NeaTICs_v2.Repositories
{
    public class ContactMessageRepository : Repository<ContactMessage>, IContactMessageRepository
    {
        public void New(ContactMessage message)
        {
            throw new NotImplementedException();
        }

        public ContactMessage SearchById(int id)
        {
            throw new NotImplementedException();
        }

        public void Edit(ContactMessage message)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ContactMessage> ObtainList()
        {
            throw new NotImplementedException();
        }
    }
}