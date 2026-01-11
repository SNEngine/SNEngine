using System.Collections.Generic;

namespace SNEngine.Editor.SNILSystem.InstructionHandlers
{
    public interface IInstructionHandler
    {
        /// <summary>
        /// Проверяет, может ли обработчик обработать данную инструкцию
        /// </summary>
        /// <param name="instruction">Строка инструкции</param>
        /// <returns>True, если обработчик может обработать инструкцию</returns>
        bool CanHandle(string instruction);

        /// <summary>
        /// Обрабатывает инструкцию
        /// </summary>
        /// <param name="instruction">Строка инструкции</param>
        /// <param name="context">Контекст обработки</param>
        /// <returns>Результат обработки</returns>
        InstructionResult Handle(string instruction, InstructionContext context);
    }

    public class InstructionResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public object Data { get; set; }

        public static InstructionResult Ok(object data = null)
        {
            return new InstructionResult { Success = true, Data = data };
        }

        public static InstructionResult Error(string message)
        {
            return new InstructionResult { Success = false, ErrorMessage = message };
        }
    }

    public class InstructionContext
    {
        public string CurrentGraphName { get; set; }
        public object Graph { get; set; } // DialogueGraph
        public Dictionary<string, object> Variables { get; set; } = new Dictionary<string, object>();
        public Dictionary<string, object> Functions { get; set; } = new Dictionary<string, object>();
        public List<object> Nodes { get; set; } = new List<object>(); // Список созданных нод
        public string CurrentFunctionName { get; set; } // null если в основном скрипте
        public List<object> CurrentFunctionNodes { get; set; } = new List<object>(); // Ноды текущей функции
        public object LastNode { get; set; } // Последняя созданная нода для соединения
    }
}