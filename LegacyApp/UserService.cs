using System;

namespace LegacyApp
{
    public class UserService : IUserService
    {
        private ClientRepository clientRepository;
        private UserCreditServiceClient userCreditServiceClient;
    
        private const int MinimumCreditLimit = 500;
        private const int MinimumAgeLimit = 21;
        private const int MultipliedCreditLimitValue = 2;

        public UserService()
        {
            clientRepository = new ClientRepository();
            userCreditServiceClient = new UserCreditServiceClient();
        }

        public User CreateUser(Client client, IncomingUserData incomingUser)
        {
            var user = new User
            {
                Client = client,
                DateOfBirth = incomingUser.dateOfBirth,
                EmailAddress = incomingUser.email,
                FirstName = incomingUser.firName,
                Surname = incomingUser.surname
            };

            if (client.Name == Name.VeryImportantClient)
            {
                user.HasCreditLimit = false;
            }
            else
            {
                user.HasCreditLimit = true;

                var creditLimit = userCreditServiceClient.GetCreditLimit(user.FirstName, user.Surname, user.DateOfBirth);

                if (client.Name == Name.ImportantClient && creditLimit > 0)
                {
                    creditLimit *= MultipliedCreditLimitValue;
                }

                user.CreditLimit = creditLimit;

                if (user.CreditLimit < MinimumCreditLimit)
                {
                    return null;
                }
            }

            return user;
        }
        public bool ValidateUser(IncomingUserData incomingUserData)
        {
            if (string.IsNullOrEmpty(incomingUserData.firName) || string.IsNullOrEmpty(incomingUserData.surname)
                || (!incomingUserData.email.Contains("@") && !incomingUserData.email.Contains(".")))
            {
                return false;
            }

            var now = DateTime.Now;
            int age = now.Year - incomingUserData.dateOfBirth.Year;

            if (now.Month < incomingUserData.dateOfBirth.Month ||
                (now.Month == incomingUserData.dateOfBirth.Month && now.Day < incomingUserData.dateOfBirth.Day))
            {
                age--;
            }

            if (age < MinimumAgeLimit)
            {
                return false;
            }

            return true;
        }
        public bool AddUser(string firName, string surname, string email, DateTime dateOfBirth, int clientId)
        {
            var incomingUserData = new IncomingUserData
            {
                clientId = clientId,
                dateOfBirth = dateOfBirth,
                email = email,
                firName = firName,
                surname = surname
            };

            var onCheckUserValidation = ValidateUser(incomingUserData);

            if (!onCheckUserValidation)
            {
                return false;
            }

            var client = clientRepository.GetById(incomingUserData.clientId);

            var user = CreateUser(client, incomingUserData);

            if (user == null)
            {
                return false;
            }

            UserDataAccess.AddUser(user);

            return true;
        }
    }
}