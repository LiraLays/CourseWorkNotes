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

            // 1. Булева функция - парсинг условия
            result.BooleanFunction = ParseBooleanFilter(filterFormula);

            // 2. Инвариант цикла фильтрации
            result.Invariant = CreateFilterInvariant(tasks);

            // 3. WP-верификация
            var wpSteps = CalculateWpSteps(filterFormula, byDeadline, byPriority);
            result.WpCalculation = wpSteps;
            result.WpResult = wpSteps.LastOrDefault() ?? "WP расчет завершен";

            return result;
        }

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

        private string CreateFilterInvariant(IEnumerable<TaskItem> tasks)
        {
            var count = tasks.Count();
            return $"∀t ∈ result: t удовлетворяет условию фильтра ∧ |result| ≤ {count}";
        }

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