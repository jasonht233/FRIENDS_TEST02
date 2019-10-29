using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DB_test02
{
    public class outputMessage
    {
        public Boolean success;
        public string ErrorMessage;
        public Dictionary<string, Dictionary<string, string>> lst;

        public outputMessage()
        {
            this.lst = new Dictionary<string, Dictionary<string, string>>();
        }
    }

    public class inputMessage
    {
        public string way;
        public Dictionary<string, string> argument;

        public inputMessage()
        {
            this.argument = new Dictionary<string, string>();
        }
    }

    class ProgramM
    {
        static void Main(string[] args)
        {

            jsontest();
            //sqltest();
        }

        static void jsontest()
        {
            //for the frontend 
            //if you want to send message to the backend 
            //(1) create Message
            inputMessage tmp = new inputMessage();

            //(2) addValue <PLEASE FOLLOW THE COMMENT FOR EACH FUNCTION, it specifies each inputMessage>
            tmp.way = "addUsr";
            tmp.argument.Add("name", "wo shi shabi");
            tmp.argument.Add("pwd", "wsl");


            //(3) Synchronization and send to server
            string msg = JsonConvert.SerializeObject(tmp);

            SQLHandler sqlHandler = new SQLHandler();

            // server.sendMessage(msg); 

            //(4) get result
            // string msg = getMessageFromServer(msg);
            msg = sqlHandler.recvMsg(msg);
            outputMessage output = JsonConvert.DeserializeObject<outputMessage>(msg);


            //(5) see the result
            Console.WriteLine(output.lst["result"]["way"]);


        }

        static void sqltest()
        {
            return;
        }
    }




    public class SQLHandler
    {
        /// <summary>
        /// 建立数据库连接
        /// </summary>
        public MySqlConnection GetSqlConn()
        {
            // 数据库
            MySqlConnection sqlConn;
            string connStr = "Database=test;Data Source=127.0.0.1;User Id=root;Password=0129;port=3306";
            sqlConn = new MySqlConnection(connStr);
            return sqlConn;
        }

        public string recvMsg(string msg)
        {
            inputMessage iptMessage = JsonConvert.DeserializeObject<inputMessage>(msg);
            string way = iptMessage.way;

            outputMessage optMessage = new outputMessage();
            optMessage.success = true;
            optMessage.lst["result"] = new Dictionary<string, string>();

            switch (way)
            {
                case "addUsr":
                    optMessage.lst["result"]["way"] = "addUsr";
                    break;
                case "searchUsr":
                    optMessage.lst["result"]["way"] = "searchUsr";
                    break;
                case "getallTsk":
                    optMessage.lst["result"]["way"] = "getallTsk";
                    break;
                case "addTsk":
                    optMessage.lst["result"]["way"] = "addTsk";
                    break;
                case "takeTsk":
                    optMessage.lst["result"]["way"] = "takeTsk";
                    break;
                default:
                    optMessage.success = false;
                    optMessage.ErrorMessage = "unable to match the way";
                    break;
            }
            return JsonConvert.SerializeObject(optMessage);

        }

        /// <summary>
        /// Opens the sql.
        /// </summary>
        public void OpenSql()
        {
            jsontest();
            // 数据库
            MySqlConnection sqlConn = GetSqlConn();
            try
            {
                sqlConn.Open();
                Console.WriteLine("NO ERROR!!!!!!!Connection success!");
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                Console.WriteLine("ERROR!!!!!!");
                return;
            }


        }

        public void jsontest()
        {

        }

        /// <summary>
        ///  Initialize:
        ///  (X) Two databse: (do the check first and then set up the database structure)
        ///  user
        ///  task
        ///  (X) addUsr()
        ///  (X) searchUsr()
        ///  () addCoin(String name, int coin)
        ///  () addExp(Strig name, int exp)
        ///  () losCoin(String name, int coin)
        /// 
        ///  (X) getTsk()
        ///  () addTsk(String title, String content, int coin)
        ///  () deleteTask(String title)
        ///  
        /// </summary>
        /// 

        public void setTable()
        {
            MySqlConnection sqlConn = GetSqlConn();
            try
            {
                sqlConn.Open();
                Console.WriteLine("suppose to set up the table");
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return;
            }

            try
            {

                Console.WriteLine("Start run");
                String SetUsr = "CREATE TABLE usr ( name CHAR(32) PRIMARY KEY , pwd CHAR(32) NOT NULL , coin int DEFAULT 100, exp int DEFAULT 0 );";
                String SetTsk = "CREATE TABLE tsk ( id int AUTO_INCREMENT PRIMARY KEY, title CHAR(32) , content CHAR(255), coin int, exp int, owner CHAR(32) NOT NULL, taker CHAR(32) );";
                Console.WriteLine("unable to run");
                MySqlCommand setBaseUsr = new MySqlCommand(SetUsr);
                setBaseUsr.Connection = sqlConn;
                MySqlCommand setBaseTsk = new MySqlCommand(SetTsk);
                setBaseTsk.Connection = sqlConn;
                Console.WriteLine("hahah");
                setBaseUsr.ExecuteNonQuery();
                setBaseTsk.ExecuteNonQuery();
                Console.WriteLine("have run");
                sqlConn.Close();

            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Console.Write("Create TABLE (maybe it has been created)");
                Console.Write(ex.ToString());
            }

        }



        /*
         * string addUsr(string msg)
         * 
         * input msg is actually a inputMessage(I will do the serialize part): 
         * way: "addUsr"
         * argument [it is a dictionary]:
         *  key(string) : value(string)
         *  "name"       : "shabi"
         *  "pwd"       : "wo si le"
         * 
         * 
         * output msg(string)
         * 
         * success: True (addSuccess) / False (add doesn't success)
         * ErrorMessage (if False) : will have the reason why it is false; 
         * lst (if success)  : lst["result"] = a dictionary: {"name":"shabi", "pwd":"zhe shi mi ma", "coin":"100", "exp": "0"}
         *     (False : null):
         */
        public string addUsr(string msg)
        {
            inputMessage input = JsonConvert.DeserializeObject<inputMessage>(msg);
            outputMessage output = new outputMessage();
            string name = input.argument["name"];
            string pwd = input.argument["pwd"];

            MySqlConnection sqlConn = GetSqlConn();
            try
            {
                sqlConn.Open();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                output.ErrorMessage = "addUsr: Connection between mysql doesn't work correctly";
                output.success = false;
                return JsonConvert.SerializeObject(output);
            }

            try
            {
                output.success = true;
                String strUsr = "INSERT usr(name , pwd) VALUES (@name, @pwd);";
                MySqlCommand instUsr = new MySqlCommand(strUsr, sqlConn);
                instUsr.Parameters.AddWithValue("@name", name);
                instUsr.Parameters.AddWithValue("@pwd", pwd);
                instUsr.ExecuteNonQuery();
                sqlConn.Close();
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Console.WriteLine("INSERT INTO Usr may can not work");
                Console.WriteLine(ex.Message);
                output.success = false;
                output.ErrorMessage = "INSERT INTO Usr may can not work / Invalid input by name , pwd";
            }

            return JsonConvert.SerializeObject(output);

        }


        /*
         * string search(string msg)
         * 
         * input msg is actually a inputMessage(I will do the serialize part): 
         * way: "searchUsr"
         * argument [it is a dictionary]:
         *  key(string) : value(string)
         *  "name"       : "shabi"
         *  "pwd"       : "wo si le"
         * 
         * 
         * output msg(string)
         * Defining success: there is a user 
         *          false  : (1)there is no such a usr (2) the pwd is not correct  
         * success: True (search Success) / False (search doesn't success)
         * ErrorMessage (if False) : will have the reason why it is false; 
         * lst (if success)  : lst["result"] = a dictionary: {"name":"shabi", "pwd":"zhe shi mi ma", "coin":"#", "exp": "#"}
         *     (False : null):
         */
        public string searchUsr(string msg)
        {
            inputMessage input = JsonConvert.DeserializeObject<inputMessage>(msg);
            outputMessage output = new outputMessage();

            string name = input.argument["name"];
            string pwd = input.argument["pwd"];

            MySqlConnection sqlConn = GetSqlConn();
            try
            {
                sqlConn.Open();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                output.ErrorMessage = "searchUsr: Connection between mysql doesn't work correctly";
                output.success = false;
                return JsonConvert.SerializeObject(output);
            }


            try
            {
                String strUsr = "SELECT * FROM usr where name=@name;";
                MySqlCommand findUsr = new MySqlCommand(strUsr, sqlConn);
                findUsr.Parameters.AddWithValue("@name", name);
                MySqlDataReader resUsr = findUsr.ExecuteReader();
                resUsr.Read();
                if (resUsr.HasRows)
                {
                    if (pwd.Equals(resUsr["pwd"]))
                    {
                        output.success = true;
                        output.lst["result"] = new Dictionary<string, string>();
                        output.lst["result"]["name"] = resUsr["name"].ToString();
                        output.lst["result"]["pwd"] = resUsr["pwd"].ToString();
                        output.lst["result"]["coin"] = resUsr["coin"].ToString();
                        output.lst["result"]["exp"] = resUsr["exp"].ToString();
                    }
                    else
                    {
                        output.success = false;
                        output.ErrorMessage = "Wrong password, please try again";

                    }

                    return JsonConvert.SerializeObject(output);
                }
                else
                {
                    sqlConn.Close();
                    output.success = false;
                    output.ErrorMessage = "Cannot find the usr, please re-enter";
                    return JsonConvert.SerializeObject(output);
                }
            }
            catch (Exception ex)
            {
                sqlConn.Close();

                output.success = false;
                output.ErrorMessage = "searchUsr has some problem, please try again or ask Developer about that";
                return JsonConvert.SerializeObject(output);
            }
        }

        /*
         * string getallTsk()
         * 
         * input msg is actually a inputMessage(I will do deserialize part): 
         * way: "getallTsk"
         * 
         * 
         * 
         * output msg(string)
         *   
         * success: True (connect Success) / False (connect doesn't success)
         * ErrorMessage (if False) : will have the reason why it is false; 
         * lst (if success)  : lst["id"] = a dictionary: {"id":"#", "title":"X", "content":"XXX", "coin": "#" , "exp":"XX", "owner":"XX"}
         *     (False : null):
         */
        public string getallTsk(string msg)
        {
            inputMessage input = JsonConvert.DeserializeObject<inputMessage>(msg);
            outputMessage output = new outputMessage();
            MySqlConnection sqlConn = GetSqlConn();
            try
            {
                sqlConn.Open();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                output.ErrorMessage = "searchUsr: Connection between mysql doesn't work correctly";
                output.success = false;
                return JsonConvert.SerializeObject(output);
            }


            try
            {
                String strsql = "SELECT * FROM tsk WHERE taker != NULL;";
                MySqlCommand sqlComm = new MySqlCommand(strsql, sqlConn);
                MySqlDataReader sqlRes = sqlComm.ExecuteReader();

                output.success = true;

                while (sqlRes.Read())
                {
                    string id = sqlRes["id"].ToString();
                    string title = sqlRes["title"].ToString();
                    string content = (string)sqlRes["content"];
                    string owner = (string)sqlRes["owner"];
                    string coin = sqlRes["coin"].ToString();
                    string exp = sqlRes["exp"].ToString();


                    output.lst[id] = new Dictionary<string, string>();
                    output.lst[id]["id"] = id;
                    output.lst[id]["title"] = title;
                    output.lst[id]["content"] = content;
                    output.lst[id]["coin"] = coin;
                    output.lst[id]["exp"] = exp;
                    output.lst[id]["owner"] = owner;
                }
            }
            catch (Exception ex)
            {
                sqlConn.Close();

                Console.Write("getTsk  may can not work");
                Console.Write(ex.Message);

                output.success = false;
                output.ErrorMessage = "have problem in getallTsk";
            }
            return JsonConvert.SerializeObject(output);
        }



        /*
         * string addTsk(string msg)
         * 
         * input msg is actually a inputMessage(I will do deserialize part): 
         * way: "addTsk"
         * argument [it is a dictionary]:
         *  key(string) : value(string)
         *  "title"       : "shabi"
         *  "content"     : "wo si le"
         *  "coin"        : "23"
         *  "owner"       : "woshiibaba"
         * 
         * output msg(string)
         * 
         * success: True (add Success) / False (add doesn't success)
         * ErrorMessage (if False) : will have the reason why it is false; 
         * lst (if success)  : lst["result"] = a dictionary: {"id":"#", "content":"XX", "content":"XX", "coin": "#" , "owner":"XXX"}
         *     (False : null):
         */
        public string addTsk(string msg)
        {
            inputMessage input = JsonConvert.DeserializeObject<inputMessage>(msg);
            outputMessage output = new outputMessage();
            MySqlConnection sqlConn = GetSqlConn();
            try
            {
                sqlConn.Open();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                output.ErrorMessage = "addTsk: Connection between mysql doesn't work correctly";
                output.success = false;
                return JsonConvert.SerializeObject(output);
            }

            string title = input.argument["title"];
            string content = input.argument["content"];
            string coin = input.argument["coin"];
            string owner = input.argument["owner"];

            try
            {
                String strUsr = "INSERT tsk(title , content, coin, owner) VALUES (@title, @content, @coin, @owner);";
                MySqlCommand instUsr = new MySqlCommand(strUsr, sqlConn);
                instUsr.Parameters.AddWithValue("@title", title);
                instUsr.Parameters.AddWithValue("@content", content);
                instUsr.Parameters.AddWithValue("@coin", coin);
                instUsr.Parameters.AddWithValue("@owner", owner);
                instUsr.ExecuteNonQuery();
                sqlConn.Close();


                output.success = true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Console.WriteLine("INSERT INTO Tsk may can not work");
                Console.WriteLine(ex.ToString());
                output.success = false;
                output.ErrorMessage = "INSERT INTO Tsk may can not work";
            }
            return JsonConvert.SerializeObject(output);
        }

        /*
         * string takeTsk(string msg)
         * 
         * input msg is actually a inputMessage(I will do deserialize part): 
         * way: "takeTsk"
         * argument [it is a dictionary]:
         *  key(string) : value(string)
         *  "id"        : "XXXX"
         *  "taker"     : "nibaba"
         * 
         * output msg(string)
         * 
         * success: True (take Success) / False (take doesn't success)
         * ErrorMessage (if False) : will have the reason why it is false; 
         * lst (if success)  : lst["result"] = a dictionary: {"id":"#", "content":"XX", "title":"XX", "coin": "#" , "owner":"XXX", "taker": "XXXXX"}
         *     (False : null):
         */
        public string takeTsk(string msg)
        {
            inputMessage input = JsonConvert.DeserializeObject<inputMessage>(msg);
            outputMessage output = new outputMessage();
            MySqlConnection sqlConn = GetSqlConn();
            try
            {
                sqlConn.Open();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                output.ErrorMessage = "take: Connection between mysql doesn't work correctly";
                output.success = false;
                return JsonConvert.SerializeObject(output);
            }

            string id = input.argument["id"];
            string taker = input.argument["taker"];


            try
            {
                String strUsr = "UPDATE tsk SET taker = @taker WHERE id = @id;";

                MySqlCommand instUsr = new MySqlCommand(strUsr, sqlConn);
                instUsr.Parameters.AddWithValue("@id", id);
                instUsr.Parameters.AddWithValue("@taker", taker);
                instUsr.ExecuteNonQuery();
                sqlConn.Close();

                output.success = true;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Console.WriteLine("UPDATE tsk taker may can not work");
                Console.WriteLine(ex.ToString());
                output.success = false;
                output.ErrorMessage = "UPDATE tsk taker may can not work";
            }
            return JsonConvert.SerializeObject(output);
        }


    }

}
