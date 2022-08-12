using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace OnlineStore.models
{
    public class User
    {
        [Key]
        public string ID { get; set; }
        public string userName { get; set; }
        public string email { get; set; }
        public string role { get; set; }
        public bool enabled { get; set; }
        public virtual ICollection<Payment> payments { get; set; }

        [JsonProperty("password")]
        [Required]
        public string pwd { get; set; }

    }

    public class Login
    {
        public string userName { get; set; }

        [JsonPropertyName("password")]
        public string password { get; set; }

    }

}
