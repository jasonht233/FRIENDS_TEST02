using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.IO;


namespace DB_test02
{
 
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
            //takeTskTest();
            //findbyIdTest();
            //deleteIdTest();
            finishTskTest();

        }

        public static void  finishTskTest()
        {
            inputMessage iptMsg = new inputMessage();
            iptMsg.addWay("finishTsk");
            iptMsg.addArg("name", "shabi");
            iptMsg.addArg("id", "1");

            SQLHandler sql = new SQLHandler();
            string output = sql.recvMsg(iptMsg.getString());
            

            Console.WriteLine(output);
            return; 



        }

    }


/**
outputMessage:

*/
public class outputMessage
    {
        private JObject optJson;
        private JObject rstJson;
        private int index;
        public void addSuccess(Boolean success)
        {
            optJson.Add("success", success);
        }

        public void addErrorMsg(string reason)
        {
            optJson.Add("ErrorMessage", reason);
        }

        public void addResult(Dictionary<string, string> value)
        {
            JObject tmp = new JObject();
            foreach (KeyValuePair<string, string> itr in value)
            {
                tmp.Add(itr.Key, itr.Value);
            }

            rstJson.Add(index.ToString(), tmp);
            index++;
        }

        public Boolean getSuccess()
        {
            return optJson["success"].Value<Boolean>();
        }

        public string getErrorMsg()
        {
            return optJson["ErrorMessage"].Value<string>();
        }

        public Dictionary<String, Dictionary<String, String>> getResult()
        {
            Dictionary<String, Dictionary<String, String>> result = new Dictionary<string, Dictionary<string, string>>();
            int index = optJson["number"].Value<int>();

            for (int i = 0; i < index; i++)
            {
                var tmpJson = rstJson[i.ToString()].ToString();
                Dictionary<string, string> tmpDict = new Dictionary<string, string>();

                JsonTextReader tmpReader = new JsonTextReader(new StringReader(tmpJson));

                tmpReader.Read();

                while (tmpReader.Read())
                {
                    if (tmpReader.Value == null)
                        continue;
                    String key = tmpReader.Value.ToString();
                    tmpReader.Read();
                    String value = tmpReader.Value.ToString();
                    tmpDict[key] = value;
                }
                result.Add(i.ToString(), tmpDict);
            }

            return result;
        }

        /// <summary>
        /// Make the outputMessage to string type Json form 
        /// </summary>
        /// <returns></returns>
        public string getString()
        {
            optJson.Add("number", index);
            optJson.Add("result", rstJson);
            return optJson.ToString();
        }

        public outputMessage()
        {
            optJson = new JObject();
            rstJson = new JObject();
            index = 0;
        }

        /// <summary>
        /// Load the string type Json Type to its real form 
        /// </summary>
        /// <param name="msg"></param>
        public outputMessage(string msg)
        {
            optJson = JObject.Parse(msg);
            rstJson = optJson["result"].Value<JObject>();
            index = optJson["number"].Value<int>();
        }
    }

    /**
inputMessage:
(1) addWay(string method): method->addUsr
(2) addArg(string argument, string value)
(3) getString(): return the string (which is a json object); 
(4) getWay(): return the way
(5) getArg(string arg): return the specific argument 
*/
    public class inputMessage
    {
        private JObject iptJson;
        private JObject argJson;

        public void addWay(string method)
        {
            iptJson.Add("way", method);
        }

        public void addArg(string name, string value)
        {
            argJson.Add(name, value);
        }

        public string getString()
        {
            iptJson.Add("argument", argJson);
            return iptJson.ToString();
        }

        public string getWay()
        {
            return iptJson["way"].Value<string>();
        }

        public string getArg(string arg)
        {
            return argJson[arg].Value<string>();
        }

        public inputMessage()
        {
            iptJson = new JObject();
            argJson = new JObject();
        }

        public inputMessage(string strJson)
        {
            iptJson = JObject.Parse(strJson);
            argJson = iptJson["argument"].Value<JObject>();
        }
    }


    public class SQLHandler 
    {
        //public PlayerIndicator playerIndicator;
        /// <summary>
        /// 建立数据库连接
        /// </summary>
        /// 

        public MySqlConnection GetSqlConn()
        {
            // 数据库
            MySqlConnection sqlConn;
            string connStr = "Database=test;Data Source=127.0.0.1;User Id=root;Password=3358;port=3306";
            sqlConn = new MySqlConnection(connStr);
            return sqlConn;
        }

        /// <summary>
        /// Opens the sql.
        /// </summary>
        public void OpenSql()
        {

            connectTest();
        }

        public void connectTest()
        {
            MySqlConnection sqlConn = GetSqlConn();
            try
            {
                sqlConn.Open();
                Console.WriteLine("suppose to set up the table");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
        }


        public static void JsonObjectTest()
        {
            inputMessage iptMsg = new inputMessage();
            iptMsg.addWay("searchUsr");
            iptMsg.addArg("name", "test");
            iptMsg.addArg("pwd", "pwd");

            string msg = iptMsg.getString();
            Console.WriteLine(msg);

            inputMessage transMsg = new inputMessage(msg);
            string way = transMsg.getWay();
            string argName = transMsg.getArg("name");
            string argPwd = transMsg.getArg("pwd");

            Console.WriteLine(string.Format("\n\n name {0} pwd {1} way {2}", argName, argPwd, way));
        }

        public void sqlTest()
        {
            Console.WriteLine(">>>>>>>>>>>>>testing    searchUsr>>>>>>>>>>>>>>>\n");
            inputMessage ipt = new inputMessage();
            ipt.addWay("searchUsr");
            ipt.addArg("name", "test");
            ipt.addArg("pwd", "pwd");

            string msg = ipt.getString();
            string opt = recvMsg(msg);

            Console.WriteLine("\n\n" + opt + "\n\n");

            outputMessage optMsg = new outputMessage(opt);
            if (optMsg.getSuccess())
            {
                var result = optMsg.getResult();

                foreach (KeyValuePair<string, Dictionary<string, string>> itr in result)
                {
                    string ans = itr.Key + "\n";

                    foreach (KeyValuePair<string, string> subitr in itr.Value)
                    {
                        ans += subitr.Key + "  " + subitr.Value + "\n";
                    }

                    Console.WriteLine(ans);
                }
            }
            else
            {
                Console.WriteLine("we have some error " + optMsg.getErrorMsg());
            }

        }

        /// <summary>
        ///  Initialize:
        ///  (X) Two databse: (do the check first and then set up the database structure)
        ///  user
        ///  task
        ///  (X) addUsr(String name, String pwd)
        ///  (X) searchUsr(String name, String pwd)
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

        public string recvMsg(string msg)
        {
            outputMessage optMessage = new outputMessage();
            inputMessage iptMessage = new inputMessage(msg);

            string way = iptMessage.getWay();
            switch (way)
            {
                case "addUsr":
                    return addUsr(msg);
                case "searchUsr":
                    return searchUsr(msg);
                case "getallTsk":
                    return getallTsk(msg);
                case "addTsk":
                    return addTsk(msg);
                case "takeTsk":
                    return takeTsk(msg);
                case "finishTsk":
                    return finishTsk(msg);
                case "getAcceptedTsk":
                    return getAcceptedTsk(msg);
                default:
                    optMessage.addSuccess(false);
                    optMessage.addErrorMsg("unable to match the way");
                    break;
            }
            return optMessage.getString();
        }




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
        * input msg (inputMessage)
        * way: addUsr<getWay()>
        * arg: name (getArg("name"))  / pwd (getArg("pwd"))
        * 
        * output outpuMessage(string) 
        * success: True (addSuccess) / False (add doesn't success)
        * ErrorMessage (if False) : will have the reason why it is false); 
        */
        public string addUsr(string msg)
        {
            inputMessage input = new inputMessage(msg);
            outputMessage output = new outputMessage();
            string name = input.getArg("name");
            string pwd = input.getArg("pwd");

            MySqlConnection sqlConn = GetSqlConn();
            try
            {
                sqlConn.Open();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                output.addErrorMsg("addUsr: Connection between mysql doesn't work correctly");
                output.addSuccess(false);
                return output.getString();
            }

            Boolean exitCheck = checkUsrExist(name);

            if (exitCheck)
            {
                output.addSuccess(false);
                output.addErrorMsg("Sorry Already Exits");
                return output.getString();
            }


            try
            {
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
                output.addSuccess(false);
                output.addErrorMsg("INSERT INTO Usr may can not work / Invalid input by name , pwd");
                return output.getString();
            }

            exitCheck = checkUsrExist(name);
            if (!exitCheck)
            {
                output.addSuccess(false);
                output.addErrorMsg("Sorry, Unable to login, please try again");
            }
            else
            {
                output.addSuccess(true);
            }

            return output.getString();

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
         * ErrorMessage (if False) : will have the reason why it is false); 
         * lst (if success)  : lst["result"] = a dictionary: {"name":"shabi", "pwd":"zhe shi mi ma", "coin":"#", "exp": "#"}
         *     (False : null):
         */
        public string searchUsr(string msg)
        {
            inputMessage input = new inputMessage(msg);
            outputMessage output = new outputMessage();

            string name = "null";
            string pwd = "null";



            MySqlConnection sqlConn = GetSqlConn();
            try
            {
                sqlConn.Open();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                output.addErrorMsg("addUsr: Connection between mysql doesn't work correctly" + ex.ToString());
                output.addSuccess(false);
                return output.getString();
            }

            try
            {
                name = input.getArg("name");
                pwd = input.getArg("pwd");
            }
            catch (Exception ex)
            {
                output.addSuccess(false);
                output.addErrorMsg("name/pwd cannot be empty, Please try again");
                return output.getString();
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
                        var lst = new Dictionary<string, string>();
                        lst.Add("name", resUsr["name"].ToString());
                        lst.Add("pwd", resUsr["pwd"].ToString());
                        lst.Add("coin", resUsr["coin"].ToString());
                        lst.Add("exp", resUsr["exp"].ToString());

                        output.addResult(lst);
                        output.addSuccess(true);
                    }
                    else
                    {
                        output.addSuccess(false);
                        output.addErrorMsg("Wrong password, please try again");

                    }

                    return output.getString();
                }
                else
                {
                    sqlConn.Close();
                    output.addSuccess(false);
                    output.addErrorMsg("Cannot find the usr, please re-enter");
                    return output.getString();
                }
            }
            catch (Exception ex)
            {
                sqlConn.Close();

                output.addSuccess(false);
                output.addErrorMsg("searchUsr has some problem, please try again or ask Developer about that");
                return output.getString();
            }
        }

        public string getCoin(string name)
        {
            MySqlConnection sqlConn = GetSqlConn();
            sqlConn.Open();
            String strUsr = "SELECT coin FROM usr WHERE name =@name;";
            MySqlCommand instUsr = new MySqlCommand(strUsr, sqlConn);
            instUsr.Parameters.AddWithValue("@name", name);
            MySqlDataReader sqlRes = instUsr.ExecuteReader();

            while (sqlRes.Read())
            {
                String coin = (String)sqlRes["coin"];
                return coin;
            }

            return "-2333";

        }

        public string getExp(string name)
        {
            MySqlConnection sqlConn = GetSqlConn();
            sqlConn.Open();
            String strUsr = "SELECT exp FROM usr WHERE name =@name;";
            MySqlCommand instUsr = new MySqlCommand(strUsr, sqlConn);
            instUsr.Parameters.AddWithValue("@name", name);
            MySqlDataReader sqlRes = instUsr.ExecuteReader();

            while (sqlRes.Read())
            {
                String exp = (String)sqlRes["exp"];
                return exp;
            }

            return "-2333";
        }


        public void addCoin(string name, string addCoin_)
        {

            int addCoin = Convert.ToInt32(addCoin_);

            MySqlConnection sqlConn = GetSqlConn();
            sqlConn.Open();
            String strUsr = "UPDATE usr SET coin= coin+@addCoin WHERE name = @name;";
            MySqlCommand instUsr = new MySqlCommand(strUsr, sqlConn);
            instUsr.Parameters.AddWithValue("@addCoin", addCoin);
            instUsr.Parameters.AddWithValue("@name", name);
            instUsr.ExecuteNonQuery();
        }

        public void addExp(string name, string addExp_)
        {

            int addExp = Convert.ToInt32(addExp_);

            MySqlConnection sqlConn = GetSqlConn();
            sqlConn.Open();
            String strUsr = "UPDATE usr SET exp= exp+@addExp WHERE name = @name;";
            MySqlCommand instUsr = new MySqlCommand(strUsr, sqlConn);
            instUsr.Parameters.AddWithValue("@addExp", addExp);
            instUsr.Parameters.AddWithValue("@name", name);
            instUsr.ExecuteNonQuery();
        }


        /*
         * string getallTsk()
         * 
         *input msg is actually a inputMessage(I will do deserialize part): 
         * way: "getallTsk"
         * output msg(string)
         *   
         * success: True (connect Success) / False (connect doesn't success)
         * ErrorMessage (if False) : will have the reason why it is false); 
         * lst (if success)  : lst["id"] = a dictionary: {"id":"#", "title":"X", "content":"XXX", "coin": "#" , "exp":"XX", "owner":"XX"}
         *     (False : null):
         */
        public string getallTsk(string msg)
        {
            inputMessage input = new inputMessage(msg);
            outputMessage output = new outputMessage();
            MySqlConnection sqlConn = GetSqlConn();
            try
            {
                sqlConn.Open();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                output.addErrorMsg("addUsr: Connection between mysql doesn't work correctly");
                output.addSuccess(false);
                return output.getString();
            }


            try
            {
                String strsql = "SELECT * FROM tsk WHERE taker IS NULL;";
                MySqlCommand sqlComm = new MySqlCommand(strsql, sqlConn);
                MySqlDataReader sqlRes = sqlComm.ExecuteReader();

                output.addSuccess(true);

                while (sqlRes.Read())
                {
                    string id = sqlRes["id"].ToString();
                    string title = sqlRes["title"].ToString();
                    string content = (string)sqlRes["content"];
                    string owner = (string)sqlRes["owner"];
                    string coin = sqlRes["coin"].ToString();
                    string exp = sqlRes["exp"].ToString();


                    var lst = new Dictionary<string, string>();
                    lst["id"] = id;
                    lst["title"] = title;
                    lst["content"] = content;
                    lst["coin"] = coin;
                    lst["exp"] = exp;
                    lst["owner"] = owner;


                    output.addResult(lst);
                }
            }
            catch (Exception ex)
            {
                sqlConn.Close();

                Console.Write("getTsk  may can not work");
                Console.Write(ex.Message);

                output.addSuccess(false);
                output.addErrorMsg("have problem in getallTsk");
            }
            return output.getString();
        }

        /// <summary>
        /// go to find the tasks you have accepted.
        /// Input: argument must have "taker".
        /// Output: Outputmessage, Results are dictionary, each subDictionary is a detailed information for each dictionary
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public string getAcceptedTsk(string msg)
        {
            inputMessage input = new inputMessage(msg);
            outputMessage output = new outputMessage();
            MySqlConnection sqlConn = GetSqlConn();
            try
            {
                sqlConn.Open();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                output.addErrorMsg("getOwnedTsk: Connection between mysql doesn't work correctly");
                output.addSuccess(false);
                return output.getString();
            }

            string taker = input.getArg("taker");

            try
            {
                String strsql = "SELECT * FROM taker = @taker ;";
                MySqlCommand sqlComm = new MySqlCommand(strsql, sqlConn);
                sqlComm.Parameters.AddWithValue("@taker", taker);

                MySqlDataReader sqlRes = sqlComm.ExecuteReader();

                output.addSuccess(true);

                while (sqlRes.Read())
                {
                    string id = sqlRes["id"].ToString();
                    string title = sqlRes["title"].ToString();
                    string content = (string)sqlRes["content"];
                    string owner = (string)sqlRes["owner"];
                    string coin = sqlRes["coin"].ToString();
                    string exp = sqlRes["exp"].ToString();


                    var lst = new Dictionary<string, string>();
                    lst["id"] = id;
                    lst["title"] = title;
                    lst["content"] = content;
                    lst["coin"] = coin;
                    lst["exp"] = exp;
                    lst["owner"] = owner;


                    output.addResult(lst);
                }
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Console.Write("getAcceptedTsk may can not work");
                Console.Write(ex.Message);
                output.addSuccess(false);
                output.addErrorMsg("have problem in getAcceptedTsk");
            }
            return output.getString();
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
         * ErrorMessage (if False) : will have the reason why it is false); 
         *
         */
        public string addTsk(string msg)
        {
            inputMessage input = new inputMessage(msg);
            outputMessage output = new outputMessage();
            MySqlConnection sqlConn = GetSqlConn();
            try
            {
                sqlConn.Open();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                output.addErrorMsg("addUsr: Connection between mysql doesn't work correctly");
                output.addSuccess(false);
                return output.getString();
            }

            string title, content, owner;
            int coin;

            try
            {
                title = input.getArg("title");
                content = input.getArg("content");
                coin = Convert.ToInt32(input.getArg("coin"));
                owner = input.getArg("owner");
            }
            catch (Exception e)
            {
                output.addErrorMsg("addTsk: Invalid input please re-enter");
                output.addSuccess(false);
                return output.getString();
            }

            try
            {
                String strUsr = "INSERT tsk(title , content, coin, owner) VALUES (@title, @content, @coin, @owner;";
                MySqlCommand instUsr = new MySqlCommand(strUsr, sqlConn);
                instUsr.Parameters.AddWithValue("@title", title);
                instUsr.Parameters.AddWithValue("@content", content);
                instUsr.Parameters.AddWithValue("@coin", coin);
                instUsr.Parameters.AddWithValue("@owner", owner);
                instUsr.ExecuteNonQuery();
                sqlConn.Close();


                output.addSuccess(true);
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Console.WriteLine("INSERT INTO Tsk may can not work");
                Console.WriteLine(ex.ToString());
                output.addSuccess(false);
                output.addErrorMsg("INSERT INTO Tsk may can not work");
            }
            return output.getString();
        }

        /// <summary>
        /// Helper Function: which help use title, content, to find the id. 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="owner"></param>
        /// <param name="coin"></param>
        /// <returns></returns>
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
             * ErrorMessage (if False) : will have the reason why it is false); 
             */
        public string takeTsk(string msg)
        {
            inputMessage input = new inputMessage(msg);
            outputMessage output = new outputMessage();
            MySqlConnection sqlConn = GetSqlConn();
            try
            {
                sqlConn.Open();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                output.addErrorMsg("takeTsk: Connection between mysql doesn't work correctly");
                output.addSuccess(false);
                return output.getString();
            }

            int id = Convert.ToInt32(input.getArg("id"));
            string taker = input.getArg("taker");

            string owner = getOwner8id(id.ToString());
            if (owner.Equals(taker))
            {
                output.addSuccess(false);
                output.addErrorMsg("you can not take your own task");
                return output.getString();
            }




            try
            {
                String strUsr = "UPDATE tsk SET taker = @taker WHERE id = @id;";

                MySqlCommand instUsr = new MySqlCommand(strUsr, sqlConn);
                instUsr.Parameters.AddWithValue("@id", id);
                instUsr.Parameters.AddWithValue("@taker", taker);
                instUsr.ExecuteNonQuery();
                sqlConn.Close();

                output.addSuccess(true);
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                Console.WriteLine("UPDATE tsk taker may can not work");
                Console.WriteLine(ex.ToString());
                output.addSuccess(false);
                output.addErrorMsg("UPDATE tsk taker may can not work");
            }
            return output.getString();
        }

        /// <summary>
        /// get coin and exp from db by its id. 
        /// </summary>
        /// <param name="strId"></param>
        /// <returns></returns>
        public Tuple<int, int> getTsk8id(string strId)
        {
            int id = Convert.ToInt32(strId);

            MySqlConnection sqlConn = GetSqlConn();
            sqlConn.Open();
            String strUsr = "SELECT * FROM tsk WHERE id = @id;";
            MySqlCommand instUsr = new MySqlCommand(strUsr, sqlConn);
            instUsr.Parameters.AddWithValue("@id", id);


            MySqlDataReader sqlRes = instUsr.ExecuteReader();
            int coin = -1;
            int exp = -1;


            while (sqlRes.Read())
            {
                coin = (int)sqlRes["coin"];
                exp = (int)sqlRes["exp"];
            }
            return new Tuple<int, int>(coin, exp);
        }

        public string getOwner8id(string strId)
        {
            int id = Convert.ToInt32(strId);

            MySqlConnection sqlConn = GetSqlConn();
            sqlConn.Open();
            String strUsr = "SELECT owner FROM tsk WHERE id = @id;";
            MySqlCommand instUsr = new MySqlCommand(strUsr, sqlConn);
            instUsr.Parameters.AddWithValue("@id", id);


            MySqlDataReader sqlRes = instUsr.ExecuteReader();
            String owner = "None";

            while (sqlRes.Read())
            {
                owner = (String)sqlRes["owner"];
            }
            return owner;
        }

        public string getDetailsTsk(string id_)
        {
            return "hello";

        }

        /// <summary>
        /// delete the Tsk by id from DB  
        /// </summary>
        /// <param name="strId">strId you need to delete</param>
        public void deleteTsk(string strId)
        {
            int id = Convert.ToInt32(strId);

            MySqlConnection sqlConn = GetSqlConn();
            sqlConn.Open();
            String strUsr = "DELETE FROM tsk WHERE id = @id;";
            MySqlCommand instUsr = new MySqlCommand(strUsr, sqlConn);
            instUsr.Parameters.AddWithValue("@id", id);
            instUsr.ExecuteNonQuery();
        }

        /// <summary>
        /// Finish tsk: Delete the Tsk from the TSk DB, and add the coin and exp to the users
        /// </summary>
        /// <param name="msg">inputMessage: getWay = "finishTsk" and arugment: "name","id"(tskid)</param>
        /// <returns>string(outputMessage)</returns>
        public string finishTsk(string msg)
        {
            inputMessage input = new inputMessage(msg);
            outputMessage output = new outputMessage();
            MySqlConnection sqlConn = GetSqlConn();
            try
            {
                sqlConn.Open();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                output.addErrorMsg("takeTsk: Connection between mysql doesn't work correctly");
                output.addSuccess(false);
                return output.getString();
            }

            string taker = input.getArg("name");
            string id_ = input.getArg("id");
            int id = Convert.ToInt32(id_);
            string owner = getOwner8id(id_);

            Tuple<int, int> CoinExp = getTsk8id(id_);
            int coin = CoinExp.Item1;
            int exp = CoinExp.Item2;

            addCoin(taker, coin.ToString());
            addCoin(owner, (0 - coin).ToString());
            addExp(taker, exp.ToString());
            deleteTsk(id_);

            output.addSuccess(true);
            return output.getString();
        }






      

    }




}
