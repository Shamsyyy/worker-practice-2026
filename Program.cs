using System.Globalization;
using System.Text;
using WorkerPractice;

Console.InputEncoding = Encoding.UTF8;
Console.OutputEncoding = Encoding.UTF8;
if (args.Contains("--test")) return WorkerTests.Run();
if (args.Contains("--demo"))
{
    var workers = new List<WORKER> {
        new("Иванов И.И.", "Преподаватель", 80000m, 2018),
        new("Петров П.П.", "Методист", 65000m, 2021),
        new("Сидорова А.А.", "Лаборант", 50000m, 2024)
    };
    Console.WriteLine("Учебные вымышленные данные. Расчётный год: 2026. Порог: 5 лет.");
    foreach (var worker in workers) worker.Display(Console.Out);
    PrintResult(WorkerSearch.MoreThan(workers, 5, 2026));
    return 0;
}

try
{
    Console.WriteLine("WORKER — учебный учёт работников кафедры цифровой экономики.");
    Console.WriteLine("Стаж рассчитывается по годам; условие отбора — строго больше порога.");
    int count = ReadInt("Количество сотрудников (0–10000): ", 0, 10000);
    var workers = new List<WORKER>(count);
    for (int i = 0; i < count; i++)
    {
        Console.WriteLine($"Сотрудник {i + 1}");
        workers.Add(new WORKER(ReadText("Фамилия и инициалы: "), ReadText("Должность: "),
            ReadSalary(), ReadInt("Год поступления на работу: ", 1900, DateTime.Today.Year)));
    }
    Console.WriteLine("Введённые данные:");
    foreach (var worker in workers) worker.Display(Console.Out);
    int threshold = ReadInt("Стаж должен превышать (лет): ", 0, 10000);
    PrintResult(WorkerSearch.MoreThan(workers, threshold, DateTime.Today.Year));
    return 0;
}
catch (EndOfStreamException)
{
    Console.Error.WriteLine("Ввод завершён до получения всех данных. Работа программы прекращена.");
    return 1;
}

static string ReadLineRequired() => Console.ReadLine() ?? throw new EndOfStreamException();
static string ReadText(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        string text = ReadLineRequired().Trim();
        if (text.Length > 0) return text;
        Console.WriteLine("Введите непустое значение.");
    }
}
static int ReadInt(string prompt, int min, int max)
{
    while (true)
    {
        Console.Write(prompt);
        if (int.TryParse(ReadLineRequired(), out int value) && value >= min && value <= max) return value;
        Console.WriteLine($"Введите целое число от {min} до {max}.");
    }
}
static decimal ReadSalary()
{
    while (true)
    {
        Console.Write("Зарплата, руб. (например, 65000,50): ");
        string text = ReadLineRequired().Trim().Replace(',', '.');
        if (decimal.TryParse(text, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
            CultureInfo.InvariantCulture, out decimal salary) && salary >= 0) return salary;
        Console.WriteLine("Введите неотрицательную сумму без разделителей тысяч.");
    }
}
static void PrintResult(List<WORKER> workers)
{
    Console.WriteLine("Результат поиска:");
    if (workers.Count == 0) Console.WriteLine("Работников со стажем, превышающим заданное значение, нет.");
    else foreach (var worker in workers) Console.WriteLine(worker.Surname);
}
