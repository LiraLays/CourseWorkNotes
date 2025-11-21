using System.Collections.Generic;

namespace Lab1_TaskScheduler.Contracts
{
	public static class ContractProvider
	{
		public static Dictionary<string, OperationContract> Contracts = new Dictionary<string, OperationContract>
		{
			{
				"AddTask", new OperationContract
				{
					Name = "Добавить задачу",
					PreCondition = "1. Задача не null\n2. Название не пустое\n3. Дедлайн в будущем\n4. Приоритет от 1 до 4",
					PostCondition = "1. Задача добавлена в список\n2. Список содержит добавленную задачу",
					Effects = "Увеличивает количество задач в планировщике",
					ValidExample = "Title: 'Meeting', Deadline: завтра, Priority: 3",
					InvalidExample = "Title: '', Deadline: вчера, Priority: 6"
				}
			},
			{
				"MoveTask", new OperationContract
				{
					Name = "Перенести задачу",
					PreCondition = "1. Задача не null\n2. Задача существует в списке\n3. Новый дедлайн в будущем",
					PostCondition = "1. Дедлайн задачи изменен\n2. Старый дедлайн ≠ новому",
					Effects = "Изменяет дедлайн существующей задачи",
					ValidExample = "Задача: существующая, Новый дедлайн: послезавтра",
					InvalidExample = "Задача: null, Новый дедлайн: вчера"
				}
			},
			{
				"FilterTasks", new OperationContract
				{
					Name = "Фильтровать задачи",
					PreCondition = "1. Выбран хотя бы один критерий фильтрации",
					PostCondition = "1. Результат не null\n2. Все задачи соответствуют критериям",
					Effects = "Возвращает отфильтрованный список задач",
					ValidExample = "Фильтр по дедлайну: true, по приоритету: true",
					InvalidExample = "Фильтр по дедлайну: false, по приоритету: false"
				}
			}
		};
	}
}