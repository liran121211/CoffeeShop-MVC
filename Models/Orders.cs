using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace CoffeeShop.Models
{
    public class Orders
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int UserId { get; set; }


        public string TransactionId { get; set; }

        public string Barista { get; set; }

        public string Status { get; set; }

        public string Date { get; set; }


    }

    public enum OrderStatus
    {
        Pending,
        Waiting_For_Confirmation,
        Confirmed
    }
}
