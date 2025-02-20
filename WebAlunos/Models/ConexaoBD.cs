using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAlunos.Models
{
    public class ConexaoBD
    {
        private string host;
        private int porta;
        private string bd;
        private string utilizador;
        private string password;

        private ConexaoBD(string host, int porta, string bd, string utilizador, string password) {
            this.host = host;
            this.porta = porta;
            this.bd = bd;
            this.utilizador = utilizador;
            this.password = password;
        }
    }
}