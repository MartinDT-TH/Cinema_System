﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cinema.DataAccess.Data;
using Cinema.DataAccess.Repository.IRepository;
using Cinema.Models;

namespace Cinema.DataAccess.Repository
{
    public class ShowTimeRepository : Repository<ShowTime>, IShowTimeRepository
    {
        private readonly ApplicationDbContext _db;
        public ShowTimeRepository(ApplicationDbContext context) : base(context)
        {
            _db = context;
        }

        public void Update(ShowTime showTime)
        {
            _db.Update(showTime);
        }
    }
}
