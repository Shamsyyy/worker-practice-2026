namespace WorkerPractice;

/// <summary>Учебная модель сотрудника. Зарплата хранится в рублях.</summary>
public sealed class WORKER
{
    public string SurnameAndInitials { get; private set; }
    public string Position { get; private set; }
    public decimal Salary { get; private set; }
    public int HireYear { get; private set; }

    public WORKER() : this("Не указан", "Не указана", 0m, DateTime.Today.Year) { }
    public WORKER(string name, string position)
        : this(name, position, 0m, DateTime.Today.Year) { }
    public WORKER(string name, string position, decimal salary, int hireYear)
    {
        SurnameAndInitials = ValidateText(name, nameof(name));
        Position = ValidateText(position, nameof(position));
        Salary = ValidateSalary(salary);
        HireYear = ValidateYear(hireYear);
    }

    // Финализатор включён по требованию задания. Неуправляемых ресурсов нет.
    // Не используется для бизнес-логики: момент вызова сборщиком мусора не определён.
    ~WORKER() { }

    public void Update(string name, string position, decimal salary, int hireYear)
    {
        // Сначала проверяются ВСЕ значения, чтобы ошибка не приводила к частичному изменению.
        string validName = ValidateText(name, nameof(name));
        string validPosition = ValidateText(position, nameof(position));
        decimal validSalary = ValidateSalary(salary);
        int validYear = ValidateYear(hireYear);
        SurnameAndInitials = validName;
        Position = validPosition;
        Salary = validSalary;
        HireYear = validYear;
    }

    public int Experience(int referenceYear)
    {
        if (referenceYear < HireYear || referenceYear > DateTime.Today.Year)
            throw new ArgumentOutOfRangeException(nameof(referenceYear), "Расчётный год должен быть между годом приёма и текущим годом.");
        return referenceYear - HireYear;
    }

    public string Surname => SurnameAndInitials.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0];

    public void Display(TextWriter output) => output.WriteLine(
        $"{SurnameAndInitials} | {Position} | {Salary:F2} руб. | принят(а): {HireYear}");

    private static string ValidateText(string text, string parameter)
    {
        if (string.IsNullOrWhiteSpace(text)) throw new ArgumentException("Поле не должно быть пустым.", parameter);
        return text.Trim();
    }

    private static decimal ValidateSalary(decimal value)
    {
        if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), "Зарплата не может быть отрицательной.");
        return value;
    }

    private static int ValidateYear(int year)
    {
        if (year < 1900 || year > DateTime.Today.Year)
            throw new ArgumentOutOfRangeException(nameof(year), "Год приёма должен быть от 1900 до текущего года.");
        return year;
    }
}

public static class WorkerSearch
{
    public static List<WORKER> MoreThan(IEnumerable<WORKER> workers, int threshold, int referenceYear)
    {
        ArgumentNullException.ThrowIfNull(workers);
        if (threshold < 0) throw new ArgumentOutOfRangeException(nameof(threshold));
        if (referenceYear < 1900 || referenceYear > DateTime.Today.Year)
            throw new ArgumentOutOfRangeException(nameof(referenceYear));
        return workers.Where(w => w.Experience(referenceYear) > threshold).ToList();
    }
}
