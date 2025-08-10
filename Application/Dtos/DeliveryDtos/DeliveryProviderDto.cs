using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace onlineshopowner_api.Application.Dtos.DeliveryDtos
{
    public class DeliveryProviderDto
    {
       public int delivery_id {  get; set; }
         public string provider_type {  get; set; }
        public string note_text {  get; set; }

        public bool active_bit {  get; set; }
        public List<string> regionnames { get; set; }

        public List<DeliveryWorkingHourDto> workHours { get; set; }

       // public RegionDto region { get; set; }
    }
}