using CHMS.DAL.Common;
using CHMS.DAL.Data;
using CHMS.DAL.Entities;
using CHMS.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHMS.DAL.Repositories.Implementations
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(CoastalHomestayDBContext context) : base(context)
        {
        }
    }
}
