using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace onlineshopowner_api.Domain.Entities.PaymentAndOrder
{
    public class OrderForDelivery
    {
        public int orderid {  get; set; }

        public int shopid { get; set; }

        public DateTime orderdate { get; set; }

        public string shopname {  get; set; }

        public string ShopEmail {  get; set; }

        public string ShopPhoneNumber {  get; set; }

        public string clientname {  get; set; }
        public string clientphonenumber { get; set; }

        public string clientemail {  get; set; }


        public decimal shoplatitude {  get; set; }

        public decimal shoplongitude { get; set; }

        public decimal clientlatitude {  get; set; }
        public decimal clientlongitude { get; set; }

        public string HashDeliveryShopPin {  get; set; }


    }
}