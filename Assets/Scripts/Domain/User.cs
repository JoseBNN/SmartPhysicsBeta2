using System;
using System.Collections.Generic;

[Serializable]
public class User
{
    public string username;
    public string password;
}

[Serializable]
public class UserList
{
    public List<User> users = new List<User>();
}
