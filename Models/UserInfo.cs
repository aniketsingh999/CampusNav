using MongoDB.Bson;
using Realms;
using System.ComponentModel;

namespace CampusNav.Models
{
    public class UserInfo : RealmObject
    {
        [PrimaryKey]
        [MapTo("_id")]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

        [MapTo("name")]
        [Required]
        public string Name { get; set; }

        [MapTo("email")]
        [Required]
        public string Email { get; set; }

        [MapTo("password")]
        [Required]
        public string Password { get; set; }

        [MapTo("is_sub")]
        [DefaultValue(false)]
        public bool IsSubscribed { get; set; }
    }
}
