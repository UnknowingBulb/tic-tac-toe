using System;
using System.ComponentModel.DataAnnotations.Schema;
using XOX.BLObjects;

namespace XOX.Models
{
    public class UserModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id;
        public string Name;
        public string Mark;

        public UserModel(User user)
        {
            Id = user.Id;
            Name = user.Name;
            Mark = user.Mark;
        }
    }
}