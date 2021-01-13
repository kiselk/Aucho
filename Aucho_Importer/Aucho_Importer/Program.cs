using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;
using System.IO;
using System.Collections;
using System;
using System.Threading;

namespace Aucho_Importer
{
    class Program
    {
        
        private static ArrayList tables = new ArrayList();
        private static String[] lines = new String[4];
        private static ManualResetEvent doneEvent;
        private static int numBusy;
        private static void Main(string[] args)
        {
            const int NumThreads = 5;
            int executed = 0;
            string name = "", count = "", price = "";
            int cur = 0, cur_prev = 0;
            string timestamp = "";

        
            /*MySqlDataAdapter data = new MySqlDataAdapter("SELECT name, count, price FROM `all`", connection);
            data.MissingSchemaAction = MissingSchemaAction.AddWithKey;
            DataSet myDataSet = new DataSet();
            MySqlCommand cmd = new MySqlCommand();
            data.Fill(myDataSet, "all"); 
            object[] rowVals = new object[3];
            cmd.Connection = connection;
            cmd = new MySqlCommand("INSERT INTO `all` (name,count,price) VALUES (?name, ?count, ?price)", connection);
            cmd.Parameters.Add("?name", MySqlDbType.VarChar, 32, "name");
            cmd.Parameters.Add("?count", MySqlDbType.Double, 0, "count");
            cmd.Parameters.Add("?price", MySqlDbType.Double, 0, "price");
            data.InsertCommand = cmd;
            */

            string filePath = @"C:\Aucho.lua";
            string line;
            if (File.Exists(filePath))
            {
                StreamReader file = null;
                try
                {
                    file = new StreamReader(filePath, System.Text.Encoding.UTF8);
                    while ((line = file.ReadLine()) != null)
                    {

                        try
                        {
                            cur = 0;
                            cur_prev = 0;
                            cur_prev = line.IndexOf("\"");
                            cur = line.IndexOf(":");

                            name = line.Substring(cur_prev + 1, cur - cur_prev - 1);

                            cur_prev = cur;
                            cur = line.IndexOf(":", cur_prev + 1);

                            count = line.Substring(cur_prev + 1, cur - cur_prev - 1);

                            cur_prev = cur;
                            cur = line.IndexOf(":", cur_prev + 1);

                            price = line.Substring(cur_prev + 1, cur - cur_prev - 1);

                            cur_prev = cur;
                            cur = line.IndexOf("\"", cur_prev + 1);

                            timestamp = line.Substring(cur_prev + 1, cur - cur_prev - 1);

                            // Console.Out.Write(name + "--" + count +"--" + price + "\n");


                            lines[0] = name;
                            lines[1] = count;
                            lines[2] = price;
                            lines[3] = timestamp;
                            if (timestamp != "0")
                            tables.Add(lines.Clone());
                            /*     rowVals[0] = name;
                 rowVals[1] = count;
                 rowVals[2] = price;
                 myDataSet.Tables["all"].Rows.Add(rowVals);
                                 */



                            // Console.Out.Write(executed + "\n");



                        }
                        catch (Exception e)
                        {
                            // Console.Out.WriteLine(e.Message);
                        }

                    }
                }
                finally
                {

                    if (file != null)
                        file.Close();
                    doneEvent = new ManualResetEvent(false);
                    numBusy = tables.Count;    
	        	int s = 1;
	            ThreadPool.QueueUserWorkItem(new WaitCallback(DoWork), (object)s);
                    doneEvent.WaitOne();
                    

                   
                   
                    //data.Update(myDataSet, "all");

                    

                    //Console.In.ReadLine();

                }
            }
        }
        private static void DoWork(object o)
        {
            try {
                

          MySqlConnection connection = new MySqlConnection();

          connection.ConnectionString = "server=gr0.in;database=onmap;uid=onmap;password=onmappassword;charset=utf8;";
            connection.Open();
            Console.OutputEncoding = Encoding.UTF8;
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                for (int s = 0; s < tables.Count; s++)
                {
                  String str = "INSERT INTO `all` (name,count,price,timestamp) VALUES ('" + (tables[s] as String[])[0] + "', '" + (tables[s] as String[])[1] + "', '" + (tables[s] as String[])[2] + "', '" + (tables[s] as String[])[3] + "')";
                    
			cmd.CommandText =str;
            System.Console.WriteLine("["+s+"]" + str);  
        	        cmd.ExecuteNonQuery();
		}
                connection.Close();

            }
            catch
            {
                // error handling goes here
            }
            finally
            {
                if (Interlocked.Decrement(ref numBusy) == 0)
                {
                    doneEvent.Set();
                }
            }
        }

    }

}
