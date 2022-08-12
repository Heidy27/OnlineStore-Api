using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace OnlineStore.models
{
    public class Payment
    {
        [Key]
        public string ID { get; set; }
        public string number { get; set; }
        public string holderName { get; set; }
        public string expirationDate { get; set; }

        [ForeignKey("userID")]
        public string userID { get; set; }
    }
}
