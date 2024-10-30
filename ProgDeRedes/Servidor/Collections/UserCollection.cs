using Servidor.Logics.UserLogic;

namespace Servidor.Collections;

public class UserCollection
{
    private static UserCollection _instance;
    private static readonly object _lock = new object();
    private List<User> users = new List<User>();

    private UserCollection() { }

    public static UserCollection Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new UserCollection();
                }
                return _instance;
            }
        }
    }

    public List<User> Users
    {
        get
        {
            lock (_lock)
            {
                return new List<User>(users);
            }
        }
    }

    public void AddUser(User user)
    {
        lock (_lock)
        {
            users.Add(user);
        }
    }

    public User FindUser(string name)
    {
        lock (_lock)
        {
            return users.Find(u => u.Name.Equals(name, StringComparison.OrdinalIgnoreCase))!;
        }
    }
    
    public bool ExistUser(string name, string password)
    {
        lock (_lock)
        {
            return users.Exists(u => u.Name.Equals(name) && u.Password == password)!;
        }
    }
}