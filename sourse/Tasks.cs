using System;
using Mono.Data.Sqlite;
using System.Collections.Generic;
using System.Globalization;

public static class Tasks
{
	static string GetName(int x)
	{
		switch (x)
		{
			case 1: return "A";
			case 2: return "B";
			case 3: return "C";
			case 4: return "D";
			case 5: return "E";
			case 6: return "F";
			case 7: return "G";
			default: return "XXX";
		}
	}

	static string GetMonth(int x)
	{
		string[] months = { "Янв", "Фев", "Мар", "Апр", "Май", "Июн", "Июл", "Авг", "Сен", "Окт", "Ноя", "Дек" };
		return months[x-1];
	}

	/// <summary>
	/// Вывести количество и сумму заказов по каждому продукту за текущей месяц.
	/// </summary>
	/// <param name="from">From.</param>
	/// <param name="to">To.</param>
	/// <param name="Connection">Connection.</param>
	public static void Task1(string from, string to, SqliteConnection Connection) //задается месяц
	{
		var cmd = new SqliteCommand("SELECT * FROM Products", Connection);
		var rdr = cmd.ExecuteReader();
		//Console.WriteLine("от " + from + " до " + to);

		Console.WriteLine("\nЗадание 1:\nКоличество и сумма заказов по каждому продукту за текущей месяц:");

		while (rdr.Read())
		{
			var id = rdr["id"];
			//var cmd_first = new SqliteCommand($"SELECT sum(amount), count(amount) FROM Info WHERE product_id = '{id}' AND strftime('%s', dt)  BETWEEN strftime('%s', '{from}') AND strftime('%s', '{to}');", Connection);
			var cmd_first = new SqliteCommand($"SELECT sum(amount), count(amount) FROM Info WHERE product_id = '{id}' AND strftime('%s', Datetime(dt)) BETWEEN strftime('%s', date('now','start of month')) AND strftime('%s', date('now','start of month','+1 month','-1 day'))", Connection);
			SqliteDataReader answer = cmd_first.ExecuteReader();
			Console.WriteLine(rdr["name"] + ": " + answer["sum(amount)"] + " " + answer["count(amount)"]);
		}
	}

	/// <summary>
	/// Вывести все продукты, которые были заказаны в текущем месяце, но которых не было в прошлом.
	/// </summary>
	/// <param name="first_start">First start.</param>
	/// <param name="first_end">First end.</param>
	/// <param name="second_start">Second start.</param>
	/// <param name="second_end">Second end.</param>
	/// <param name="Connection">Connection.</param>
	public static void Task2a(string first_start, string first_end, string second_start, string second_end, SqliteConnection Connection, string Task) //начало прошлого, этого, конец этого месяца
	{
		//Console.WriteLine(first_start + " " + first_end + " " + second_start + " " + second_end);
		var cmd1_second_a = new SqliteCommand($"SELECT product_id, dt FROM Info WHERE strftime('%s', Datetime(dt))  BETWEEN strftime('%s', '{first_start}') AND strftime('%s', '{first_end}') ;", Connection);
		var current = cmd1_second_a.ExecuteReader();
		var cmd2_second_a = new SqliteCommand($"SELECT product_id, dt FROM Info WHERE strftime('%s', Datetime(dt))  BETWEEN strftime('%s', '{second_start}') AND strftime('%s', '{second_end}') ;", Connection);
		var not_prev = cmd2_second_a.ExecuteReader();


		SortedSet<object> ss = new SortedSet<object>();
		Console.WriteLine();
		//Console.WriteLine(" Первое множество:");
		while (current.Read())
		{
			//Console.WriteLine(current["product_id"]);
			ss.Add(current["product_id"]);
		}

		//Console.WriteLine("\nВторое множество:");

		SortedSet<object> ss1 = new SortedSet<object>();
		while (not_prev.Read())
		{	
			//Console.WriteLine(not_prev["product_id"]);
			ss1.Add(not_prev["product_id"]);
		}

		//ss.SymmetricExceptWith(ss1);
		ss.ExceptWith(ss1);

		Console.Write(Task);
		if (ss.Count != 0)
		{
			foreach (var elem in ss)
			{
				Console.WriteLine(GetName(Convert.ToInt32(elem)));
			}
		}
		else
		{
			Console.WriteLine("Таких продуктов нет.");
		}

	}

	/// <summary>
	/// Вывести все продукты, которые были только в прошлом месяце, но не в текущем, а какие — только в текущем месяце, но не в прошлом
	/// </summary>
	/// <param name="first_start">First start.</param>
	/// <param name="first_end">First end.</param>
	/// <param name="second_start">Second start.</param>
	/// <param name="second_end">Second end.</param>
	/// <param name="Connection">Connection.</param>
	public static void Task2b(string first_start, string first_end, string second_start, string second_end, SqliteConnection Connection)
	{
		Console.Write("Продукты, которые были заказаны в текущем месяце, но которых не было в прошлом:");
		Task2a(first_start, first_end, second_start, second_end, Connection, "");
		Console.Write("Продукты, которые были только в прошлом месяце, но не в текущем:");
		Task2a(second_start, second_end, first_start, first_end, Connection, "");
	}

	public static void Task3(SqliteConnection Connection)
	{
		Console.WriteLine("Период\tПродукт\tСумма\tДоля");
		var cmd_getDates = new SqliteCommand($"SELECT min(dt), max(dt) FROM Info", Connection);
		var start_date = cmd_getDates.ExecuteReader();
		var time = Convert.ToDateTime(Convert.ToString(start_date["min(dt)"]));
		time = time.AddMonths(-1);
		int i =-1;
		var curent_time = DateTime.Now;
		//DateTime start_time = time;
		DateTime bound = Convert.ToDateTime(start_date["max(dt)"]);
		while ((time.AddMonths(i)) <= bound)
		{
			//time = time.AddMonths(1);
			i++;
			string StringTime = Methods.ChangeDataFormat(time.ToString());
			var cmd = new SqliteCommand("SELECT * FROM Products", Connection);
			var rdr = cmd.ExecuteReader();
			var time_between = $"date('{StringTime}','+{i} month', 'start of month') AND  date('{StringTime}','+{i} month', 'start of month', '+1 month', '-1 day')";
			var total_sum = new SqliteCommand($"SELECT sum(amount) FROM Info WHERE dt BETWEEN {time_between};", Connection);
			var sum = total_sum.ExecuteReader();
			double dSum = 0; 
			try
			{
				dSum = Convert.ToDouble(sum["sum(amount)"]);
			}
			catch
			{
				continue; // извините
			}
			string max_id = "";
			double max = 0;
			while (rdr.Read())
			{
				var id = rdr["id"];
				var for_max = new SqliteCommand($"SELECT sum(amount) FROM Info WHERE dt BETWEEN {time_between} AND product_id = '{id}'", Connection);
				var maximal = for_max.ExecuteReader();
				//if ((int)maximal["sum(amount)"] > max)
				try
				{
					if (Convert.ToInt32(maximal["sum(amount)"]) > max)
					{
						max = Convert.ToDouble(maximal["sum(amount)"]);
						max_id = Convert.ToString(rdr["name"]);
					}
				}
				catch
				{
					continue; // извините еще раз
				}

			}

			Console.WriteLine(GetMonth(Convert.ToInt32((time.AddMonths(i)).Month)) + " " + (time.AddMonths(i)).Year + "\t" + max_id + "\t" + max + "\t" + (max / dSum).ToString("F4"));
			//var new_time = Convert.ToDateTime(tim
				
		}
	}
}
