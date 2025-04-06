using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAlunos.Models;

namespace WebAlunos.Controllers
{
    public class RegistoController : Controller
    {
        // GET: Registo
        public ActionResult Registo()
        {
            return View();
        }

        //Post
        [HttpPost]
        public ActionResult Registo(Utilizador utilizador)
        {
            if (ModelState.IsValid)
            {
                ConexaoBD conn = new ConexaoBD("localhost", 3306, "root", "", "formacao");
                using (MySqlConnection conexao = conn.ObterConexao())
                {
                    if (conexao != null)
                    {
                        // Verificar se o email já existe
                        string verificarEmail = "SELECT COUNT(*) FROM utilizadores WHERE email = @email";
                        using (MySqlCommand verificarCmd = new MySqlCommand(verificarEmail, conexao))
                        {
                            verificarCmd.Parameters.AddWithValue("@email", utilizador.Email);
                            int count = Convert.ToInt32(verificarCmd.ExecuteScalar());

                            if (count > 0)
                            {
                                ModelState.AddModelError("Email", "Email indisponível");
                                return View(utilizador); // volta à view com a mensagem de erro
                            }
                        }

                        // Inserir novo utilizador
                        string stm = "INSERT INTO utilizadores VALUES (0, @email, MD5(@password))";
                        using (MySqlCommand cmd = new MySqlCommand(stm, conexao))
                        {
                            cmd.Parameters.AddWithValue("@email", utilizador.Email);
                            cmd.Parameters.AddWithValue("@password", utilizador.Password);

                            int nregistos = cmd.ExecuteNonQuery();

                            if (nregistos == 1)
                                return RedirectToAction("Login");
                        }
                    }
                }
            }

            return View(utilizador); 
        }

        //GET
        public ActionResult Login()
        {
            return View();
        }

        //Post
        [HttpPost]
        public ActionResult Login(Utilizador utilizador)
        {
            if (ModelState.IsValid)
            {
                ConexaoBD conn = new ConexaoBD("localhost", 3306, "root", "", "formacao");
                using (MySqlConnection conexao = conn.ObterConexao())
                {
                    if (conexao != null)
                    {
                        string stm = "select * from utilizadores  where email=@email and password=MD5(@password)";
                        using (MySqlCommand cmd = new MySqlCommand(stm, conexao))
                        {
                            cmd.Parameters.AddWithValue("@email", utilizador.Email);
                            cmd.Parameters.AddWithValue("@password", utilizador.Password);

                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    Session["login"] = 2;
                                    Session["email"] = utilizador.Email;

                                    return RedirectToAction("ListaAluno", "Aluno");
                                }
                            }

                        }
                    }
                }
            }
            return RedirectToAction("Login");
        }

        public ActionResult Logout()
        {
            if (Session["login"] != null)
            {
                Session.Abandon();
            }

            return RedirectToAction("Login");
        }
    }
}


