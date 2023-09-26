using System;
using System.Data.SqlClient;
namespace ordini
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string connStr = @"Server=localhost;" + "initial catalog=orders;" + "User Id =sa;" + "Password=password123;";  // integrated security
            SqlConnection con = new SqlConnection(connStr);

            using (con)
            {

                con.Open();
                Console.WriteLine("Connessione Aperta!");
                SqlCommand cmd;
                SqlDataReader n;

                CheckAdmin(con, out cmd, out n);
                n.Close();
                bool t = true;

                while (t)
                {

                    Console.WriteLine("Salve , Scegli azione da fare : \n (1) - Login \n (2) - Registrati ");
                    var x = Console.ReadLine();
                    switch (x)
                    {


                        case "1":
                            while (t)
                            {
                                t = Login(con, out cmd, t);
                            } 
                            while (true)
                            {
                                Console.Clear();
                                Menu(con);
                            }






                        case "2":

                            string username, password;
                            SqlDataReader n2;
                            Registrati(con, out cmd, out username, out password, out n2);

                            break;

                        default:

                            Console.WriteLine("Azione non esistente");
                            break;
                    }

                    con.Close();
                }
            }
        }

        private static void Menu(SqlConnection con)
        {
            Console.Clear();
            Console.WriteLine("Login Effettuato , Cosa desidera?\n (1) Visualizza Lista Ordini (2) Dettaglio Ordine (3) Creazione nuovo ordine  (4) Esci ");
            var p = Console.ReadLine();
            SqlCommand cmd2;
            switch (p)
            {
                case "1":
                      SelectOrdiniCustomer(con);
                    break;
                case "2":
                      LeggiOrderItems(con);
                    break;
                case "3":

                    var item = LeggiItem(con);
                    Console.WriteLine("-------------------------------------------------------------------------------------------------------");
                    Console.WriteLine("Inserisci Quantità");
                    int qty = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("-------------------------------------------------------------------------------------------------------");
                    Console.WriteLine("Inserisci Prezzo ");
                    int price = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("-------------------------------------------------------------------------------------------------------");
                    var customer = LeggiCustomer(con);


                    Console.WriteLine($" Item : {item} qty : {qty} price : {price} customer : {customer}");
                    Console.ReadLine();
                    break;
                case "4":
                    Environment.Exit(0);
                    break;
            }


        }

        private static string  LeggiCustomer(SqlConnection con)
        {
            SqlCommand cmd = new SqlCommand("select * from customers ", con);

            using (var customer = cmd.ExecuteReader())
            {

                while (customer.Read()) { Console.WriteLine( customer["customer"]); }

            }
            Console.WriteLine("-------------------------------------------------------------------------------------------------------");
            while (true)
            {
          
                Console.WriteLine("Inserisci quale Customer sta   ordinando  ");
                var customer = Console.ReadLine();

                cmd = new SqlCommand("SELECT customer FROM customers WHERE customer  = @customer", con);
                cmd.Parameters.AddWithValue("@customer", customer);

                using (var CheckCustomer = cmd.ExecuteReader())
                {
                    if (CheckCustomer.HasRows)
                    {
                        Console.WriteLine("Customer selezionato: " + customer);
                        return customer;
                    
                    }
                    else
                    {
                        Console.WriteLine("Costumer  non esistente. Riprova.");
                    }
                }
            }

        }
    

        private static  void  SelectOrdiniCustomer(SqlConnection con)
        {
            SqlCommand cmd2 = new SqlCommand("select o.orderid, customer , orderdate ,sum(price) as TotSpeso  from orders as o join orderitems as oi  on o.orderid = oi.orderid group by customer, orderdate , o.orderid; ", con);
            using (var res = cmd2.ExecuteReader())

            {
                while (res.Read())
                {
                    int orderid = res.GetInt32(0);
                    string customer = res.GetString(1);
                    DateTime orderdate = res.GetDateTime(2);
                    int totSpeso = res.GetInt32(3);

                    Console.WriteLine($"OrderID: {orderid}, Customer: {customer}, OrderDate: {orderdate}, TotSpeso: {totSpeso}");
                }

            }
            Console.WriteLine("Invio per continuare ");
            Console.ReadLine();
          
        }


       
        private static string LeggiItem(SqlConnection con)
        {

            SqlCommand cmd = new SqlCommand("select item from items ", con);

            using (var items = cmd.ExecuteReader())
            {

                while (items.Read()) { Console.WriteLine(items["item"]); }

            }
            Console.WriteLine("Inserisci quale item vuoi ordinare ");
            while (true)
            {
                Console.WriteLine("Inserisci un elemento:");
                var item = Console.ReadLine();

                cmd = new SqlCommand("SELECT item FROM items WHERE item = @item", con);
                cmd.Parameters.AddWithValue("@item", item);

                using (var CheckItem = cmd.ExecuteReader())
                {
                    if (CheckItem.HasRows)
                    {
                        Console.WriteLine("Item selezionato: " + item);
                        return item;
                 
                    }
                    else
                    {
                        Console.WriteLine("Item non esistente. Riprova.");
                    }
                }
            }

        }
        private static SqlCommand LeggiOrderItems(SqlConnection con)
        {
            Console.WriteLine("Inserisci id dell'ordine ");
            int id = Convert.ToInt32(Console.ReadLine());

            
            SqlCommand cmd = new SqlCommand("select * from orders where orderid = @id ", con);
            cmd.Parameters.AddWithValue("@id", id);
           using (var orderItems = cmd.ExecuteReader())
                while (orderItems.Read())
            {
                int orderid = orderItems.GetInt32(0);
                string customer = orderItems.GetString(1);
                DateTime orderdate = orderItems.GetDateTime(2);
                

                    Console.WriteLine($"OrderID: {orderid}, Customer: {customer}, OrderDate: {orderdate}");
            }

            cmd = new SqlCommand("select * from orders join orderitems on orders.orderid = orderitems.orderid where orderitems.orderid = @id", con );
            cmd.Parameters.AddWithValue("@id", id);
            using (var items = cmd.ExecuteReader())
            {

                while (items.Read())
                {
                    id = items.GetInt32(items.GetOrdinal("orderid"));
                    string item = items.GetString(items.GetOrdinal("item"));
                    int qty = items.GetInt32(items.GetOrdinal("qty"));
                    decimal prezzo = items.GetInt32(items.GetOrdinal("price"));

                    Console.WriteLine($"OrderID: {id}, Item: {item}, Quantità: {qty}, Prezzo: {prezzo}");
                }

            }

            Console.WriteLine("Invio per continuare ");
            Console.ReadLine();
            return cmd;
        }
        private static void Registrati(SqlConnection con, out SqlCommand cmd, out string username, out string password, out SqlDataReader n)
        {
            Console.WriteLine("Inserisci Username e Password ");
            username = Console.ReadLine();
            password = Console.ReadLine();
            cmd = new SqlCommand($"select * from Utenti where username = @username and password = @password", con);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@password", password);
            n = cmd.ExecuteReader();

            if (n.HasRows)
            {
                Console.WriteLine("Utente gia Registrato");
            }
            else
            {
                n.Close();
                cmd = new SqlCommand("insert into Utenti  VALUES ( @username , @password)", con);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", password);
                cmd.ExecuteNonQuery();
                Console.WriteLine("Utente Registrato");
            }
        }


        private static bool Login(SqlConnection con, out SqlCommand cmd, bool t)
        {
            Console.WriteLine("Inserisci Username e Password ");
            var username = Console.ReadLine();
            var password = Console.ReadLine();
            cmd = new SqlCommand($"select * from Utenti where username = @username and password = @password", con);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@password", password);

            using (var res = cmd.ExecuteReader())
            {
                if (res.HasRows) { return false; } else { Console.WriteLine("Utente non esistente "); return true; }
            }
        }

        private static void CheckAdmin(SqlConnection con, out SqlCommand cmd, out SqlDataReader n)
        {

            cmd = new SqlCommand("select * from Utenti ", con);
            n = cmd.ExecuteReader();
            if (n.HasRows) { }
            else
            {
                n.Close();
                cmd = new SqlCommand("insert into Utenti  VALUES ( 'admin' , 'admin')", con);

                cmd.ExecuteNonQuery();

            }
        }
    }
}
