using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace onlineshopowner_api.Domain.Constant
{
    public class ResultCheckdb<T>
    {
        public bool IsSuccess { get; set; }
        public T Value { get; set; }
        public string Error { get; set; }

        public bool IsFound {  get; set; }

        public int page {  get; set; }
        public int pageSize { get; set; }

        public int totalCount { get; set; }

    }
}