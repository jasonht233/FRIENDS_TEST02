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

            //example_jsontest();
            //sqltest();
            //inserttest(); 
            //searchUsrTest();
            //getallTskTest();
            //addtskTest();
            takeTskTest();
        }

        static void example_jsontest()
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

        static void inserttest()
        {
            SQLHandler sqlHandler = new SQLHandler();

            inputMessage tmp = new inputMessage();
            tmp.way = "addUsr";
            tmp.argument.Add("name", "wo shi shabi");
            tmp.argument.Add("pwd", "wsl");
            string msgIn = JsonConvert.SerializeObject(tmp);
            string msgOut = sqlHandler.recvMsg(msgIn);
            Console.WriteLine(msgOut);

            msgOut = sqlHandler.recvMsg(msgIn);
            Console.WriteLine(msgOut);
        }
    
    
        static void searchUsrTest()
        {
            SQLHandler sqlHandler = new SQLHandler();

            inputMessage tmp = new inputMessage();
            tmp.way = "searchUsr";
            tmp.argument.Add("name", "wo shi shabi");
            tmp.argument.Add("pwd", "wsl");
            string msgIn = JsonConvert.SerializeObject(tmp);
            string msgOut = sqlHandler.recvMsg(msgIn);
            Console.WriteLine(msgOut);

            tmp.argument["name"] = "wo shi shabi";
            tmp.argument["pwd"] = "wnd";
            msgIn = JsonConvert.SerializeObject(tmp);
            msgOut = sqlHandler.recvMsg(msgIn);
            Console.WriteLine(msgOut);


            tmp.argument["name"] = "wo shi ni baba";
            tmp.argument["pwd"] = "wnd";
            msgIn = JsonConvert.SerializeObject(tmp);
            msgOut = sqlHandler.recvMsg(msgIn);
            Console.WriteLine(msgOut);

            tmp = new inputMessage();
            tmp.way = "searchUsr";
            tmp.argument.Add("name", "wo shi shabi");
            msgIn = JsonConvert.SerializeObject(tmp);
            msgOut = sqlHandler.recvMsg(msgIn);
            Console.WriteLine(msgOut);
        }
        
        static void getallTskTest()
        {
            SQLHandler sqlHandler = new SQLHandler();

            inputMessage tmp = new inputMessage();
            tmp.way = "getallTsk";
            string msgIn = JsonConvert.SerializeObject(tmp);
            string msgOut = sqlHandler.recvMsg(msgIn);
            Console.WriteLine(msgOut);

        }
    

        static void addtskTest()
        {
            SQLHandler sqlHandler = new SQLHandler();

            inputMessage tmp = new inputMessage();
            tmp.way = "addTsk";
            tmp.argument.Add("title", "wo shi shabi");
            tmp.argument.Add("content", "wsl");
            tmp.argument.Add("coin", "123");
            tmp.argument.Add("owner", "wo shi ni yeye");
            string msgIn = JsonConvert.SerializeObject(tmp);
            string msgOut = sqlHandler.recvMsg(msgIn);
            Console.WriteLine(msgOut);

            msgOut = sqlHandler.recvMsg(msgIn);
            Console.WriteLine(msgOut);

        }
    
        static void takeTskTest()
        {
            SQLHandler sqlHandler = new SQLHandler();

            inputMessage tmp = new inputMessage();
            tmp.way = "takeTsk";
            tmp.argument.Add("id", "5");
            tmp.argument.Add("taker", "yeye");
            string msgIn = JsonConvert.SerializeObject(tmp);
            string msgOut = sqlHandler.recvMsg(msgIn);
            Console.WriteLine(msgOut);

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
            string connStr = "Database=test;Data Source=127.0.0.1;User Id=root;Password=3358;port=3306";
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
                    return addUsr(msg);
                    break;
                case "searchUsr":
                    optMessage.lst["result"]["way"] = "searchUsr";
                    return searchUsr(msg);
                    break;
                case "getallTsk":
                    optMessage.lst["result"]["way"] = "getallTsk";
                    return getallTsk(msg);
                    break;
                case "addTsk":
                    optMessage.lst["result"]["way"] = "addTsk";
                    return addTsk(msg);
                    break;
                case "takeTsk":
                    optMessage.lst["result"]["way"] = "takeTsk";
                    return takeTsk(msg);
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

            Boolean exitCheck = checkUsrExist(name);

            if(exitCheck)
            {
                output.success = false;
                output.ErrorMessage = "Sorry Already Exits";
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

            exitCheck = checkUsrExist(name);
            if (!exitCheck)
            {
                output.success = false;
                output.ErrorMessage = "Sorry, Unable to login, please try again";
            }
            else
            {
                output.lst["result"] = new Dictionary<string, string>();
                output.lst["result"]["name"] = name;
                output.lst["result"]["pwd"] = pwd;
                output.lst["result"]["exp"] = "0";
                output.lst["result"]["coin"] = "100";
            }



            return JsonConvert.SerializeObject(output);

        }

        /*
         * a helper function to check User exist or not 
         * for before the insert , and after the insert. 
         */
        private Boolean checkUsrExist(string name)
        {
            MySqlConnection sqlConn = GetSqlConn();
            sqlConn.Open();
            String strUsr = "SELECT * FROM usr where name=@name;";
            MySqlCommand findUsr = new MySqlCommand(strUsr, sqlConn);
            findUsr.Parameters.AddWithValue("@name", name);
            MySqlDataReader resUsr = findUsr.ExecuteReader();
            resUsr.Read();

            if (resUsr.HasRows)
            {
                return true;
            }
            else
            {
                return false;
            }
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

            string name = "null";
            string pwd = "null";

            try
            {
                name = input.argument["name"];
                pwd = input.argument["pwd"];
            } catch( Exception ex)
            {
                output.success = false;
                output.ErrorMessage = "name/pwd cannot be empty, Please try again";
                return JsonConvert.SerializeObject(output);
            }

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
                String strsql = "SELECT * FROM tsk WHERE taker IS NULL;";
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


        //TODO: Import decide ADDUSR and ADDTSK's return value 
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

            string title, content, owner;
            int coin; 

            try
            {
                title = input.argument["title"];
                content = input.argument["content"];
                coin = Convert.ToInt32(input.argument["coin"]) ;
                owner = input.argument["owner"];
            }
            catch(Exception e)
            {
                output.ErrorMessage = "addTsk: Invalid input please re-enter";
                output.success = false;
                return JsonConvert.SerializeObject(output);
            }

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

        public int find_id(string title, string content, string owner, int coin)
        {
            MySqlConnection sqlConn = GetSqlConn();
            sqlConn.Open();
            String strUsr = "SELECT * FROM tsk WHERE title=@title AND content=@content AND coin= @coin AND owner= @owner);";
            MySqlCommand instUsr = new MySqlCommand(strUsr, sqlConn);
            instUsr.Parameters.AddWithValue("@title", title);
            instUsr.Parameters.AddWithValue("@content", content);
            instUsr.Parameters.AddWithValue("@coin", coin);
            instUsr.Parameters.AddWithValue("@owner", owner);

            MySqlDataReader sqlRes = instUsr.ExecuteReader();
            
            while (sqlRes.Read())
            {
                int id = (int)sqlRes["id"];
                return id; 
            }

            return -1; 
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

            int id = Convert.ToInt32(input.argument["id"]) ;
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
