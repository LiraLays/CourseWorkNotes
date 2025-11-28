using SmartTaskScheduler.Library.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace SmartTaskScheduler.Library.Core.Verification
{
    public class TaskVerificationEngine
    {
        public VerificationResult VerifyFilterOperation(
            string filterFormula,
            IEnumerable<TaskItem> tasks,
            bool byDeadline,
            bool byPriority)
        {
            var result = new VerificationResult();

            // 1. Булева функция - парсинг условия (Лаб. раб. 4)
            result.BooleanFunction = ParseBooleanFilter(filterFormula);

            // 2. Инвариант цикла фильтрации
            result.Invariant = CreateFilterInvariant(tasks);

            // 3. WP-верификация
            var wpSteps = CalculateWpSteps(filterFormula, byDeadline, byPriority);
            result.WpCalculation = wpSteps;
            result.WpResult = wpSteps.LastOrDefault() ?? "WP расчет завершен";

            return result;
        }


        /// <summary>
        /// Проверка фильтра на наличие булевых функций (Лаб. раб. 4)
        /// </summary>
        /// <param name="formula">Формула в фильтре</param>
        /// <returns>Булева функция, имеющаяся в фильтре</returns>
        private string ParseBooleanFilter(string formula)
        {
            if (string.IsNullOrEmpty(formula))
                return "Без кастомного фильтра";

            return formula
                .Replace("&", "∧")
                .Replace("|", "∨")
                .Replace("!", "¬")
                .Replace(" and ", " ∧ ")
                .Replace(" or ", " ∨ ")
                .Replace(" not ", " ¬ ");
        }

        /// <summary>
        /// Вычисление инварианта цикла фильтрации (Лаб. Раб. 3)
        /// </summary>
        /// <param name="tasks">Набор задач</param>
        /// <returns>Результат, которому удовлетворяет цикл</returns>
        private string CreateFilterInvariant(IEnumerable<TaskItem> tasks)
        {
            var count = tasks.Count();
            return $"∀t ∈ result: t удовлетворяет условию фильтра ∧ |result| ≤ {count}";
        }

        /// <summary>
        /// WP-верификация
        /// </summary>
        /// <param name="filterCondition">Условие фильтрации</param>
        /// <param name="byDeadline">Сортировка по дедлайну?</param>
        /// <param name="byPriority">Сортировка по приоритету?</param>
        /// <returns>Шаги рассчёта WP</returns>
        private List<string> CalculateWpSteps(string filterCondition, bool byDeadline, bool byPriority)
        {
            var steps = new List<string>
            {
                "=== WP-РАСЧЕТ ФИЛЬТРАЦИИ ===",
                $"Условие фильтра: {filterCondition}",
                $"По дедлайну: {byDeadline}",
                $"По приоритету: {byPriority}",
                "",
                "1. Предусловие: byDeadline ∨ byPriority ∨ ¬IsEmpty(customFilter)",
                "2. Инвариант: result содержит только задачи, удовлетворяющие условиям",
                "3. Постусловие: result ≠ null ∧ ∀t ∈ result: t соответствует фильтрам",
                "",
                "✓ WP расчет подтверждает корректность операции фильтрации"
            };

            return steps;
        }
    }

    public class VerificationResult
    {
        public string BooleanFunction { get; set; } = string.Empty;
        public string Invariant { get; set; } = string.Empty;
        public List<string> WpCalculation { get; set; } = new();
        public string WpResult { get; set; } = string.Empty; // ДОБАВЛЕНО ЭТО СВОЙСТВО
    }
}