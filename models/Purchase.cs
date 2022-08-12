using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace OnlineStore.models
{
    public class Purchase
    {
        [Key]
        public string ID { get; set; }

        [ForeignKey("orderID")]
        public string orderID { get; set; }

        [ForeignKey("itemID")]
        public string itemID { get; set; }
    }

}
