using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank

{
    /// <summary>
    /// Собственное исключение
    /// </summary>
    public  class MyException: Exception
    {
        public int Code { get; set; }
        public MyException() : base()
        {
           

        }
        public MyException(string Msg, int Code):base(Msg)
        {
           this. Code = Code;

        }
        
        
    }
}
