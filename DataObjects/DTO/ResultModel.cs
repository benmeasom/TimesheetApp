using System;
using System.Collections.Generic;

namespace DataObjects.DTO
{
    public class ResultModel<T>
    {
        public string Msg { get; set; }
        public Boolean IsSuccess { get; set; } = true;
        public string TargetForm { get; set; }
        public bool AlreadyExists { get; set; }
        public Int64 IdentityId { get; set; }
        public T Data { get; set; }
        public List<T> DataList { get; set; }
        public Exception Exception { get; set; }
    }

    public class ResultModel
    {
        public string Msg { get; set; }
        public Boolean IsSuccess { get; set; } = true;
        public string TargetForm { get; set; }
        public bool AlreadyExists { get; set; }
        public Int64 IdentityId { get; set; }
        public Exception Exception { get; set; }
    }
}
