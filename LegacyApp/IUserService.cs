using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegacyApp
{
    public interface IUserService
    {
        User CreateUser(Client client, IncomingUserData incomingUser);
        bool ValidateUser(IncomingUserData incomingUserData);
        bool AddUser(string firName, string surname, string email, DateTime dateOfBirth, int clientId);
    }
}
