using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace OnlineStore.models
{
    public class Item
    {
        [Key]
        public string ID { get; set; }
        public bool amount { get; set; }
        public bool description { get; set; }
    }
}
