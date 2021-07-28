using System;
using System.Collections.Generic;
using System.Text;

namespace Caradhras.Tasks.Domain.Entities
{
 
    public class AutenticacaoCaradhras
    {
        public AutenticacaoCaradhras()
        {

        }
        public AutenticacaoCaradhras(string access_token, int expires_in, string token_type)
        {
            this.access_token = access_token;
            this.expires_in = expires_in;
            this.token_type = token_type;
        }

        public string access_token { get; set; }

        public int expires_in { get; set; }

        public string token_type { get; set; }
    }
}
