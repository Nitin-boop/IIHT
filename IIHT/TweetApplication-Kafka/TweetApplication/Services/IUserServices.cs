using System.Collections.Generic;
using TweetApplication.Models;

namespace TweetApplication.Services
{
    public interface IUserServices
    {
        bool? AddUser(User user);
        bool Login(User user);
        void Forgot(User user);
        List<string> GetUsers();
    }
}
