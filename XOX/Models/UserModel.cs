using System;
using System.ComponentModel.DataAnnotations.Schema;
using XOX.BLObjects;

namespace XOX.Models
{
    public class UserModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Mark { get; set; }

        public UserModel() { }

        public UserModel(User user)
        {
            Id = user.Id;
            Name = user.Name;
            Mark = user.Mark;
        }

        //TODO: think about methods placement. I guess it's bad to store them with model? idk
        public User ToUser()
        {
            return new User(Id, Name, Mark);
        }

        public UserModel ChangeWithUser(User user)
        {
            Name = user.Name;
            Mark = user.Mark;
            return this;
        }

        public bool IsEqualByData(User user)
        {
            return (Name == user.Name && Mark == user.Mark);
        }
    }
}