using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingAppAPI.Entities;

namespace DatingAppAPI.Interfaces;

public interface IUserService
{
    AppUser CreateUser(string username, string password);
}