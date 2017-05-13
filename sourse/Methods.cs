using System;

public static class Methods
{
	/// <summary>
	/// Возвращает тип ячеек по названию столбца
	/// </summary>
	/// <param name="s">S.</param>
	public static string Type(string s)
	{
		if (s == "id")
			return "INTEGER PRIMARY KEY";
		if (s == "dt")
			return "TEXT";
		if (s == "product_id")
			return "INTEGER";
		if (s == "amount")
			return "REAL";
		throw new Exception("Неверное название поля");
	}

	/// <summary>
	/// Проверяет правильность содержимого ячейки id
	/// </summary>
	/// <returns><c>true</c>, if identifier was checked, <c>false</c> otherwise.</returns>
	/// <param name="s">S.</param>
	public static bool Check_id(string s)
	{
		int id;
		if (int.TryParse(s, out id))
			return true;
		return false;
	}

	/// <summary>
	/// Проверяет правильность содержимого ячейки dt
	/// </summary>
	/// <returns><c>true</c>, if dt was checked, <c>false</c> otherwise.</returns>
	/// <param name="s">S.</param>
	public static bool Check_dt(string s)
	{
		DateTime dt;
		if (DateTime.TryParse(s, out dt))
			return true;
		return false;

	}

	/// <summary>
	/// Проверяет правильность содержимого ячейки amount
	/// </summary>
	/// <returns><c>true</c>, if amount was checked, <c>false</c> otherwise.</returns>
	/// <param name="s">S.</param>
	public static bool Check_amount(string s)
	{
		double amount;
		if (double.TryParse(s, out amount))
			return true;
		return false;
	}

	/// <summary>
	/// Проверяет правильность содержимого ячейки product_id
	/// </summary>
	/// <returns><c>true</c>, if product identifier was checked, <c>false</c> otherwise.</returns>
	/// <param name="s">S.</param>
	public static bool Check_product_id(string s)
	{
		int id;
		if (int.TryParse(s, out id))
		{
			if (0 < id && id < 8)
				return true;
		}
		return false;
	}

	public static string ChangeDataFormat(string dt)
	{
		string[] tmp1 = dt.Split(' ');
		string[] tmp2 = tmp1[0].Split('.');
		string[] tmp3 = tmp1[1].Split(':');
		if (tmp3[0].Length == 1)
		{
			tmp3[0] = "0" + tmp3[0];
		}
		string date = tmp2[2] + "-" + tmp2[1] + "-" + tmp2[0] + "T" + tmp3[0] + ":" + tmp3[1] + ":" + tmp3[2];
		return date;
	}
}