namespace WorkerPractice;

internal static class WorkerTests
{
    public static int Run()
    {
        int passed = 0, failed = 0;
        void Check(string name, Action test)
        {
            try { test(); passed++; Console.WriteLine("PASS " + name); }
            catch (Exception e) { failed++; Console.WriteLine("FAIL " + name + ": " + e.Message); }
        }
        static void Require(bool value) { if (!value) throw new Exception("Условие не выполнено"); }
        static void Throws<T>(Action action) where T : Exception
        {
            try { action(); } catch (T) { return; }
            throw new Exception("Ожидалось исключение " + typeof(T).Name);
        }
        WORKER Make(int year = 2020) => new("Иванов И.И.", "Преподаватель", 80000.25m, year);
        Check("Конструктор по умолчанию", () => Require(new WORKER().HireYear == DateTime.Today.Year));
        Check("Конструктор с двумя параметрами", () => Require(new WORKER("Иванов И.И.", "Методист").Salary == 0));
        Check("Полный конструктор", () => Require(Make().Salary == 80000.25m && Make().HireYear == 2020));
        Check("Расчёт стажа", () => Require(Make().Experience(2026) == 6));
        Check("Строго больше, равный исключён", () => Require(WorkerSearch.MoreThan(new[]{Make(2020),Make(2021),Make(2022)},5,2026).Count == 1));
        Check("Пустая коллекция", () => Require(WorkerSearch.MoreThan(Array.Empty<WORKER>(),0,2026).Count == 0));
        Check("Нет подходящих работников", () => Require(WorkerSearch.MoreThan(new[]{Make()},100,2026).Count == 0));
        Check("Нулевой стаж исключён при пороге 0", () => Require(WorkerSearch.MoreThan(new[]{Make(2026)},0,2026).Count == 0));
        Check("Пустое имя", () => Throws<ArgumentException>(() => new WORKER(" ","Методист",1,2020)));
        Check("Пустая должность", () => Throws<ArgumentException>(() => new WORKER("Иванов"," ",1,2020)));
        Check("Отрицательная зарплата", () => Throws<ArgumentOutOfRangeException>(() => new WORKER("Иванов","Методист",-1,2020)));
        Check("Будущий год приёма", () => Throws<ArgumentOutOfRangeException>(() => Make(DateTime.Today.Year+1)));
        Check("Недопустимый ранний год", () => Throws<ArgumentOutOfRangeException>(() => Make(1899)));
        Check("Отрицательный порог", () => Throws<ArgumentOutOfRangeException>(() => WorkerSearch.MoreThan(new[]{Make()},-1,2026)));
        Check("Расчёт раньше приёма", () => Throws<ArgumentOutOfRangeException>(() => Make().Experience(2019)));
        Check("Атомарность изменения", () => { var w=Make(); Throws<ArgumentOutOfRangeException>(()=>w.Update("Петров","Методист",-1,2021)); Require(w.Surname=="Иванов" && w.HireYear==2020); });
        Check("Изменение данных", () => { var w=Make(); w.Update(" Петров П.П. ","Методист",90000,2022); Require(w.Surname=="Петров" && w.Salary==90000 && w.Experience(2026)==4); });
        Check("Отображение полей", () => { var output=new StringWriter(); Make().Display(output); Require(output.ToString().Contains("Иванов И.И.") && output.ToString().Contains("2020")); });
        Check("Повторяющиеся фамилии сохраняются", () => Require(WorkerSearch.MoreThan(new[]{Make(),Make()},0,2026).Count==2));
        Check("Нулевая зарплата", () => Require(new WORKER("Иванов","Стажёр",0,2026).Salary==0));
        Console.WriteLine($"Итого: {passed} успешно, {failed} ошибок.");
        return failed == 0 ? 0 : 1;
    }
}
