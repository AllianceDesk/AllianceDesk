﻿using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Basecode.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Repositories
{
    public class TicketActivityRepository : BaseRepository, ITicketActivityRepository
    {
        public TicketActivityRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

        public void Add(TicketActivity ticketActivity)
        {
            this.GetDbSet<TicketActivity>().Add(ticketActivity);
            UnitOfWork.SaveChanges();
        }

        public IQueryable<TicketActivity> RetrieveAll()
        {
            return this.GetDbSet<TicketActivity>();
        }

        public void Delete(Guid id)
        {
            var ticketActivityToDelete = this.GetDbSet<TicketActivity>().FirstOrDefault(x => x.TicketId == id);

            if (ticketActivityToDelete != null)
            {
                this.GetDbSet<TicketActivity>().Remove(ticketActivityToDelete);
                UnitOfWork.SaveChanges();
            }
        }
        public IQueryable<TicketActivity> GetActivitiesByTicketId(Guid ticketId)
        {
            return this.GetDbSet<TicketActivity>().Where(x => x.TicketId == ticketId);
        }

        public IQueryable<TicketActivity> GetActivitiesByTicketIds(List<Guid> ticketId)
        {
            return this.GetDbSet<TicketActivity>().Where(x => ticketId.Contains(x.TicketId));
        }

        public async Task AddTicketActivitiesAsync(List<TicketActivity> ticketActivities)
        {
            foreach (var ticketActivity in ticketActivities)
            {
                this.GetDbSet<TicketActivity>().Add(ticketActivity);
                await UnitOfWork.SaveChangesAsync();
            }
        }
    }
}
