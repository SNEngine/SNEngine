using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SNEngine.Editor.SNILSystem.Validators
{
    public class InstructionValidatorManager
    {
        private static InstructionValidatorManager _instance;
        public static InstructionValidatorManager Instance => _instance ??= new InstructionValidatorManager();

        private readonly List<IInstructionValidator> _validators;

        private InstructionValidatorManager()
        {
            _validators = new List<IInstructionValidator>();
            RegisterDefaultValidators();
        }

        private void RegisterDefaultValidators()
        {
            // Регистрируем стандартные валидаторы
            RegisterValidator(new SetVariableInstructionValidator()); // Add the set variable instruction validator
            RegisterValidator(new ArithmeticInstructionValidator()); // Add the arithmetic instruction validator
            RegisterValidator(new TemplateBasedInstructionValidator());
        }

        public void RegisterValidator(IInstructionValidator validator)
        {
            _validators.Add(validator);
        }

        public void RegisterValidatorsFromAssembly(Assembly assembly)
        {
            var validatorTypes = assembly.GetTypes()
                .Where(t => typeof(IInstructionValidator).IsAssignableFrom(t) && 
                           t.IsClass && !t.IsAbstract);

            foreach (var validatorType in validatorTypes)
            {
                var validator = Activator.CreateInstance(validatorType) as IInstructionValidator;
                if (validator != null)
                {
                    RegisterValidator(validator);
                }
            }
        }

        public IInstructionValidator GetValidatorForInstruction(string instruction)
        {
            return _validators.FirstOrDefault(h => h.CanValidate(instruction));
        }

        public ValidationInstructionResult ValidateInstruction(string instruction)
        {
            var validator = GetValidatorForInstruction(instruction);
            if (validator != null)
            {
                return validator.Validate(instruction);
            }

            // Если нет подходящего валидатора, считаем инструкцию валидной
            // (это специальные инструкции, которые обрабатываются отдельно)
            return ValidationInstructionResult.Ok();
        }

        public List<IInstructionValidator> GetAllValidators()
        {
            return new List<IInstructionValidator>(_validators);
        }
    }
}