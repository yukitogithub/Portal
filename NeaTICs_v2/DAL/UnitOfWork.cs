using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NeaTICs_v2.Models;

namespace NeaTICs_v2.DAL
{
    public class UnitOfWork : IDisposable
    {
        private Context context = new Context();
        private Repository<New> newsRepository;
        private Repository<Events> eventsRepository;
        private Repository<Users> usersRepository;
        private Repository<ContactMessage> contactMessageRepository;

        //Singleton para el Repository de Noticias
        public Repository<New> NewsRepository
        {
            get
            {
                if (this.newsRepository == null)
                {
                    this.newsRepository = new Repository<New>(context);
                }
                return newsRepository;
            }
        }

        public Repository<Events> EventsRepository
        {
            get
            {
                if (this.eventsRepository == null)
                {
                    this.eventsRepository = new Repository<Events>(context);
                }
                return eventsRepository;
            }
        }

        public Repository<Users> UsersRepository
        {
            get
            {

                if (this.usersRepository == null)
                {
                    this.usersRepository = new Repository<Users>(context);
                }
                return usersRepository;
            }
        }

        public Repository<ContactMessage> ContactMessageRepository
        {
            get
            {

                if (this.contactMessageRepository == null)
                {
                    this.contactMessageRepository = new Repository<ContactMessage>(context);
                }
                return contactMessageRepository;
            }
        }

        public void Save()
        {
            context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}