using System;
using System.Collections.Generic;
using System.Text;

namespace Caradhras.Tasks.Domain.Entities
{
    public class ResultSimple<T>
    {
        public bool Sucess { get; set; }
        public T Data { get; set; }
        public List<string> Messages { get; set; }

        public string Content { get; set; }
        public ResultSimple()
        {
            Messages = new List<string>();
        }
    }
}
