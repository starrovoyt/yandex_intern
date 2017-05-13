using System;
using Mono.Data.Sqlite;
using System.IO;

class MainClass
{
	const string databaseName = @"DataBase.db";

	public static void Main(string[] args)
	{
		try
		{
			//string Path = args[0];
			string Path = args[0];
			//File.Create(Path).Dispose();

			// Creating table of products

			var Connection = new SqliteConnection(string.Format($"Data Source={databaseName};"));
			var CreateProductTable = new SqliteCommand("CREATE TABLE IF NOT EXISTS Products (id INTEGER PRIMARY KEY, name TEXT);", Connection);
			Connection.Open();
			CreateProductTable.ExecuteNonQuery();
			char x = 'A';
			for (int i = 1; i < 8; i++)
			{
				var InsertIntoProducts = new SqliteCommand($"INSERT OR IGNORE INTO Products (id, name) VALUES ({i}, '{x}');", Connection);
				InsertIntoProducts.ExecuteNonQuery();
				x = (char)(x + 1);
			}
			Console.WriteLine("Создана таблица Product.");

			string[] Titles; //  названия колонок
			using (StreamReader sr = File.OpenText(Path))
			{
				string s = sr.ReadLine();
				Titles = s.Split('\t');

				if (Titles.Length != 4)
					throw new Exception("Неверное количество полей.");
				foreach (string tmp in Titles)
				{
					if (!(tmp == "id" || tmp == "amount" || tmp == "product_id" || tmp == "dt"))
						throw new Exception("Неверное название поля.");
				}

				// Creating the table "Info"

				Console.WriteLine("Создается таблица Info...");
				//Console.WriteLine($"CREATE TABLE Info ({Titles[0]} {Type(Titles[0])}, {Titles[1]} {Type(Titles[1])}, " + $"{Titles[2]} {Type(Titles[2])}, {Titles[3]} {Type(Titles[3])});");

				var CreateInfoTable = new SqliteCommand($"CREATE TABLE IF NOT EXISTS Info ({Titles[0]} {Methods.Type(Titles[0])}, {Titles[1]} {Methods.Type(Titles[1])}, " +
															$"{Titles[2]} {Methods.Type(Titles[2])}, {Titles[3]} {Methods.Type(Titles[3])});", Connection);

				CreateInfoTable.ExecuteNonQuery();

				// Filling table "Info" with given values

				int j = 1; // Line counter
				while ((s = sr.ReadLine()) != null)
				{
					string[] Line = s.Split('\t');
					if (Line.Length != 4)
						Console.WriteLine($"Строка {j} содержит неверное количество значений.");
					else
					{
						bool fl = true;
						for (int i = 0; i < 4; i++)
						{
							//Console.WriteLine($"ща будут геттатры {Line[i]}");
							bool result = (bool)typeof(Methods).GetMethod("Check_" + Titles[i]).Invoke(null, new object[] { Line[i] });
							if (!result)
							{
								Console.WriteLine($"Неверные данные в строке {j}: " + s);
								fl = false;
								break;
							}
						}

						if (fl)
						{
							//Console.WriteLine("пилим запрос в бд");
							var InsertIntoInfo = new SqliteCommand($"INSERT OR REPLACE into Info ({Titles[0]},{Titles[1]},{Titles[2]},{Titles[3]}) VALUES ('{Line[0]}','{Line[1]}','{Line[2]}','{Line[3]}');", Connection);
							InsertIntoInfo.ExecuteNonQuery();
						}

					}
					j++;

				} // Closing While

			} // Closing StreamReader

			Console.WriteLine("Таблица Info сформирована.");


			var startCurrent = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1); 
			var endCurrent = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));

			Tasks.Task1(Methods.ChangeDataFormat(startCurrent.ToString()), Methods.ChangeDataFormat(endCurrent.ToString()), Connection);

			var startPrev = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-1);
			var endPrev = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddDays(-1);
			Console.Write("\nЗадание 2a:");
			string task = "Продукты, которые были заказаны в текущем месяце, но которых не было в прошлом:\n";
			Tasks.Task2a(Methods.ChangeDataFormat(startCurrent.ToString()), Methods.ChangeDataFormat(endCurrent.ToString()), Methods.ChangeDataFormat(startPrev.ToString()), Methods.ChangeDataFormat(endPrev.ToString()), Connection, task);
			Console.Write("\nЗадание 2b:\n");
			Tasks.Task2b(Methods.ChangeDataFormat(startCurrent.ToString()), Methods.ChangeDataFormat(endCurrent.ToString()), Methods.ChangeDataFormat(startPrev.ToString()), Methods.ChangeDataFormat(endPrev.ToString()), Connection);
			Console.WriteLine("\nЗадание 3:");
			Tasks.Task3(Connection);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message); 
		}
	}
}
