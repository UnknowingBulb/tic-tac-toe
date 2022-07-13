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
    }
}