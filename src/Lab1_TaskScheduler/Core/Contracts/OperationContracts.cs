using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartTaskScheduler.Library.Core.Expressions;
using static SmartTaskScheduler.Library.Core.Expressions.SimpleExpressions;

namespace SmartTaskScheduler.Library.Core.Contracts
{
    // Комбинация контрактов и WP в одном месте
    public static class OperationContracts
    {
        public static class AddTask
        {
            public static string PreConditions => "task ≠ null ∧ ¬IsEmpty(task.Title) ∧ task.Deadline > Now() ∧ task.Priority ∈ [1,4]";

            public static string PostConditions => "Tasks.Contains(task) ∧ Tasks.Count = old(Tasks.Count) + 1";
        }

        public static class  FilterTasks
        {
            public static string PreConditions => "byDeadline ∨ byPriority ∨ ¬IsEmpty(customFilter)";

            public static (Expression wp, List<string> steps) CalculateWP(
                string filterCondition, Expression postCondition)
            {
                var steps = new List<string>();

                // Упрощенный WP-расчет: заменяем "result" на условие фильтра
                var wp = postCondition.Substitute("result", filterCondition);

                steps.Add($"WP-расчет для фильтрации:");
                steps.Add($"Постусловие: {postCondition}");
                steps.Add($"Условие фильтра: {filterCondition}");
                steps.Add($"WP-результат: {wp}");
                steps.Add($"Условия определенности: {string.Join(", ", wp.GetDefinitenessConditions())}");

                return (wp, steps);
            }
        }
    }
}
