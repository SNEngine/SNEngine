using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SNEngine.Editor.SNILSystem.InstructionHandlers
{
    public class InstructionHandlerManager
    {
        private static InstructionHandlerManager _instance;
        public static InstructionHandlerManager Instance => _instance ??= new InstructionHandlerManager();

        private readonly List<IInstructionHandler> _handlers;

        private InstructionHandlerManager()
        {
            _handlers = new List<IInstructionHandler>();
            RegisterDefaultHandlers();
        }

        private void RegisterDefaultHandlers()
        {
            // Регистрируем стандартные обработчики в порядке приоритета (от наиболее специфичных к общим)
            RegisterHandler(new NameInstructionHandler());
            RegisterHandler(new StartInstructionHandler());
            RegisterHandler(new EndInstructionHandler());
            RegisterHandler(new FunctionDefinitionInstructionHandler());
            RegisterHandler(new FunctionEndInstructionHandler());
            RegisterHandler(new CallInstructionHandler());
            RegisterHandler(new SetVariableInstructionHandler()); // Add the set variable instruction handler
            RegisterHandler(new ArithmeticInstructionHandler()); // Add the arithmetic instruction handler
            RegisterHandler(new IfShowVariantInstructionHandler()); // Add the new block instruction handler
            RegisterHandler(new SwitchShowVariantInstructionHandler()); // Add the new switch show variant instruction handler
            RegisterHandler(new DisplayedInstructionHandler()); // Add the new displayed instruction handler
            RegisterHandler(new GenericNodeInstructionHandler());
        }

        public void RegisterHandler(IInstructionHandler handler)
        {
            _handlers.Add(handler);
        }

        public void RegisterHandlersFromAssembly(Assembly assembly)
        {
            var handlerTypes = assembly.GetTypes()
                .Where(t => typeof(IInstructionHandler).IsAssignableFrom(t) && 
                           t.IsClass && !t.IsAbstract);

            foreach (var handlerType in handlerTypes)
            {
                var handler = Activator.CreateInstance(handlerType) as IInstructionHandler;
                if (handler != null)
                {
                    RegisterHandler(handler);
                }
            }
        }

        public IInstructionHandler GetHandlerForInstruction(string instruction)
        {
            return _handlers.FirstOrDefault(h => h.CanHandle(instruction));
        }

        public InstructionResult ProcessInstruction(string instruction, InstructionContext context)
        {
            var handler = GetHandlerForInstruction(instruction);
            if (handler != null)
            {
                return handler.Handle(instruction, context);
            }

            return InstructionResult.Error($"No handler found for instruction: {instruction}");
        }

        public List<IInstructionHandler> GetAllHandlers()
        {
            return new List<IInstructionHandler>(_handlers);
        }
    }
}