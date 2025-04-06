using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAlunos.Models;
using System.IO;
using MySql.Data.MySqlClient;
using System.Diagnostics; // Adicione este namespace


namespace WebAlunos.Controllers
{
    public class AlunoController : Controller
    {
        public ActionResult ListaAluno()
        {
            try
            {
                if (Session["Login"] != null)
                {
                    ConexaoBD conn = new ConexaoBD("localhost", 3306, "root", "", "formacao");
                    List<Aluno> lista = new List<Aluno>();

                    using (MySqlConnection conexao = conn.ObterConexao())
                    {
                        if (conexao != null)
                        {
                            Debug.WriteLine("Conexão estabelecida com sucesso!");

                            using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM alunos", conexao))
                            {
                                using (MySqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        Debug.WriteLine($"Aluno encontrado: {reader["primeiroNome"]}");
                                        lista.Add(new Aluno()
                                        {
                                            NAluno = reader.GetInt32("id"),
                                            PrimeiroNome = reader.GetString("primeiroNome"),
                                            UltimoNome = reader.GetString("ultimoNome"),
                                            Morada = reader.GetString("morada"),
                                            Sexo = reader.GetString("sexo") == "Masculino" ? Sexo.Masculino : Sexo.Feminino,
                                            DataNascimento = reader.GetDateTime("dataNascimento"),
                                            AnoEscolaridade = reader.GetInt16("ano"),
                                            ImagePath = reader["foto"] != DBNull.Value ? reader.GetString("foto") : null
                                        });
                                    }
                                }
                            }
                            Debug.WriteLine("Total de alunos: " + lista.Count);
                            return View(lista);
                        }
                        else
                        {
                            Debug.WriteLine("Erro: conexão com o banco de dados não foi estabelecida.");
                        }
                    }
                    return View(lista);
                }
                else
                {
                    return RedirectToAction("Login", "Registo");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro: {ex.Message}");
                return View("Erro", new HandleErrorInfo(ex, "Aluno", "ListaAluno"));
            }
        }

        // GET: Aluno
        public ActionResult CriaAluno()
        {
            try
            {
                if (Session["Login"] == null) return RedirectToAction("Login", "Registo");
                return View();

            }
            catch (Exception ex)
            {
                return View("Erro", new HandleErrorInfo(ex, "Aluno", "CriaAluno"));
            }

        }
        [HttpPost]
        public ActionResult CriaAluno(Aluno aluno)
        {
            try
            {
                if (Session["Login"] == null) return RedirectToAction("Login", "Registo");

                //Verificar se existe algum erro com a submissão do formulário
                if (ModelState.IsValid)
                {
                    string ImagemNome = Path.GetFileNameWithoutExtension(aluno.Imagem.FileName);
                    string ImagemExt = Path.GetExtension(aluno.Imagem.FileName);
                    ImagemNome = DateTime.Now.ToString("yyyyMMddHHmmss") + " - " + ImagemNome.Trim() + ImagemExt;
                    aluno.ImagePath = @"\Content\Imagens" + ImagemNome;
                    aluno.Imagem.SaveAs(ControllerContext.HttpContext.Server.MapPath(aluno.ImagePath));
                    //Password do mysql é Admin
                    ConexaoBD conn = new ConexaoBD("localhost", 3306, "root", "", "formacao");

                    using (MySqlConnection conexao = conn.ObterConexao())
                    {
                        if (conexao != null)
                        {
                            string stm = ("insert into alunos values(0,@primeiroNome,@ultimoNome,@morada,@sexo,@dataNascimento,@ano,@foto)");
                            using (MySqlCommand cmd = new MySqlCommand(stm, conexao))
                            {
                                cmd.Parameters.AddWithValue("@primeiroNome", aluno.PrimeiroNome);
                                cmd.Parameters.AddWithValue("@ultimoNome", aluno.UltimoNome);
                                cmd.Parameters.AddWithValue("@morada", aluno.Morada);
                                cmd.Parameters.AddWithValue("@sexo", aluno.Sexo);
                                cmd.Parameters.AddWithValue("@dataNascimento", aluno.DataNascimento);
                                cmd.Parameters.AddWithValue("@ano", aluno.AnoEscolaridade);
                                cmd.Parameters.AddWithValue("@foto", aluno.ImagePath);

                                int nRegistos = cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
                return RedirectToAction("ListaAluno");

            }
            catch (Exception ex)
            {
                return View("Erro", new HandleErrorInfo(ex, "Aluno", "CriaAluno"));
            }
        }

        public ActionResult DetalheAluno(int? id)
        {
            try
            {
                if (Session["Login"] == null) return RedirectToAction("Login", "Registo");

                ConexaoBD conn = new ConexaoBD("localhost", 3306, "root", "", "formacao");
                Aluno aluno = null;

                using (MySqlConnection conexao = conn.ObterConexao())
                {
                    if (conexao != null)
                    {

                        using (MySqlCommand cmd = new MySqlCommand("Select * from alunos where id=@idaluno", conexao))
                        {
                            cmd.Parameters.AddWithValue("@idaluno", id);
                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    aluno = new Aluno()
                                    {
                                        NAluno = reader.GetInt32("id"),
                                        PrimeiroNome = reader.GetString("primeiroNome"),
                                        UltimoNome = reader.GetString("ultimoNome"),
                                        Morada = reader.GetString("morada"),
                                        Sexo = reader.GetString("sexo") == "Masculino" ? Sexo.Masculino : Sexo.Feminino,
                                        DataNascimento = reader.GetDateTime("dataNascimento"),
                                        AnoEscolaridade = reader.GetInt16("ano"),
                                        ImagePath = reader.GetString("foto")
                                    };

                                    return View(aluno);
                                }
                            }
                        }
                    }
                }
                return RedirectToAction("ListaAluno");
            }
            catch (Exception ex)
            {
                return View("Erro", new HandleErrorInfo(ex, "Aluno", "DetalheAluno"));
            }

        }
        //Get
        public ActionResult EditaAluno(int? id)
        {
            try
            {
                if (Session["Login"] == null) return RedirectToAction("Login", "Registo");


                ConexaoBD conn = new ConexaoBD("localhost", 3306, "root", "", "formacao");
                Aluno aluno = null;

                using (MySqlConnection conexao = conn.ObterConexao())
                {
                    if (conexao != null)
                    {

                        using (MySqlCommand cmd = new MySqlCommand("Select * from alunos where id=@idaluno", conexao))
                        {
                            cmd.Parameters.AddWithValue("@idaluno", id);
                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    aluno = new Aluno()
                                    {
                                        NAluno = reader.GetInt32("id"),
                                        PrimeiroNome = reader.GetString("primeiroNome"),
                                        UltimoNome = reader.GetString("ultimoNome"),
                                        Morada = reader.GetString("morada"),
                                        Sexo = reader.GetString("sexo") == "Masculino" ? Sexo.Masculino : Sexo.Feminino,
                                        DataNascimento = reader.GetDateTime("dataNascimento"),
                                        AnoEscolaridade = reader.GetInt16("ano"),
                                        ImagePath = reader.GetString("foto")
                                    };

                                    return View(aluno);
                                }
                            }
                        }
                    }
                }
                return RedirectToAction("ListaAluno");

            }
            catch (Exception ex)
            {
                return View("Erro", new HandleErrorInfo(ex, "Aluno", "EditaAluno"));
            }


        }
        //Post
        [HttpPost]
        public ActionResult EditaAluno(Aluno aluno)
        {
            try
            {
                if (Session["Login"] == null) return RedirectToAction("Login", "Registo");

                //Variável booleana da imagem inicializada a false
                bool img = false;
                //Verificar se o utilizador atuali
                if (aluno.Imagem != null)
                {
                    string ImagemNome = Path.GetFileNameWithoutExtension(aluno.Imagem.FileName);
                    string ImagemExt = Path.GetExtension(aluno.Imagem.FileName);
                    ImagemNome = DateTime.Now.ToString("yyyyMMddHHmmss") + " - " + ImagemNome.Trim() + ImagemExt;
                    aluno.ImagePath = @"\Content\Imagens" + ImagemNome;
                    aluno.Imagem.SaveAs(ControllerContext.HttpContext.Server.MapPath(aluno.ImagePath));
                    img = true;
                }
                //Password do mysql é Admin
                ConexaoBD conn = new ConexaoBD("localhost", 3306, "root", "", "formacao");

                using (MySqlConnection conexao = conn.ObterConexao())
                {
                    if (conexao != null)
                    {
                        string strFoto = (img) ? ",foto=@foto" : "";
                        string stm = "update alunos set primeiroNome=@primeiroNome, " +
                            "ultimoNome=@ultimoNome, " +
                            "morada=@morada, " +
                            "sexo=@sexo, " +
                            "dataNascimento=@dataNascimento, " +
                            "ano=@ano " +
                            strFoto + " where id=@idaluno";
                        using (MySqlCommand cmd = new MySqlCommand(stm, conexao))
                        {
                            cmd.Parameters.AddWithValue("@idaluno", aluno.NAluno);
                            cmd.Parameters.AddWithValue("@primeiroNome", aluno.PrimeiroNome);
                            cmd.Parameters.AddWithValue("@ultimoNome", aluno.UltimoNome);
                            cmd.Parameters.AddWithValue("@morada", aluno.Morada);
                            cmd.Parameters.AddWithValue("@sexo", aluno.Sexo);
                            cmd.Parameters.AddWithValue("@dataNascimento", aluno.DataNascimento);
                            cmd.Parameters.AddWithValue("@ano", aluno.AnoEscolaridade);
                            if (img)
                                cmd.Parameters.AddWithValue("@foto", aluno.ImagePath);

                            int nRegistos = cmd.ExecuteNonQuery();
                        }
                    }
                }

                return RedirectToAction("ListaAluno");

            }
            catch (Exception ex)
            {
                return View("Erro", new HandleErrorInfo(ex, "Aluno", "EditaAluno"));
            }
        }
        //Get
        // Método para carregar os detalhes do aluno e confirmar a eliminação
        public ActionResult EliminaAluno(int? id)
        {
            try
            {
                if (Session["Login"] == null) return RedirectToAction("Login", "Registo");

                ConexaoBD conn = new ConexaoBD("localhost", 3306, "root", "", "formacao");
                Aluno aluno = null;

                using (MySqlConnection conexao = conn.ObterConexao())
                {
                    if (conexao != null)
                    {
                        using (MySqlCommand cmd = new MySqlCommand("Select * from alunos where id=@idaluno", conexao))
                        {
                            cmd.Parameters.AddWithValue("@idaluno", id);
                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    aluno = new Aluno()
                                    {
                                        NAluno = reader.GetInt32("id"),
                                        PrimeiroNome = reader.GetString("primeiroNome"),
                                        UltimoNome = reader.GetString("ultimoNome"),
                                        Morada = reader.GetString("morada"),
                                        Sexo = reader.GetString("sexo") == "Masculino" ? Sexo.Masculino : Sexo.Feminino,
                                        DataNascimento = reader.GetDateTime("dataNascimento"),
                                        AnoEscolaridade = reader.GetInt16("ano"),
                                        ImagePath = reader.GetString("foto")
                                    };

                                    TempData["ImagemPath"] = aluno.ImagePath;

                                    return View(aluno);
                                }
                            }
                        }
                    }
                }
                return RedirectToAction("ListaAluno");
            }
            catch (Exception ex)
            {
                return View("Erro", new HandleErrorInfo(ex, "Aluno", "EliminaAluno"));
            }
        }

        [HttpPost, ActionName("EliminaAluno")]
        public ActionResult EliminaAlunoConfirmacao(int? id)
        {
            try
            {
                if (Session["Login"] == null) return RedirectToAction("Login", "Registo");

                ConexaoBD conn = new ConexaoBD("localhost", 3306, "root", "", "formacao");

                using (MySqlConnection conexao = conn.ObterConexao())
                {
                    if (conexao != null)
                    {
                        string stm = "delete from alunos where id=@idaluno";
                        using (MySqlCommand cmd = new MySqlCommand(stm, conexao))
                        {
                            cmd.Parameters.AddWithValue("@idaluno", id);

                            int nRegistos = cmd.ExecuteNonQuery();
                            if (nRegistos == 1)
                            {
                                // Apagar a imagem se necessário
                                new FileInfo(ControllerContext.HttpContext.Server.MapPath(TempData["ImagemPath"].ToString())).Delete();
                            }
                        }
                    }
                }
                return RedirectToAction("ListaAluno");
            }
            catch (Exception ex)
            {
                return View("Erro", new HandleErrorInfo(ex, "Aluno", "EliminaAluno"));
            }
        }
    }
}
