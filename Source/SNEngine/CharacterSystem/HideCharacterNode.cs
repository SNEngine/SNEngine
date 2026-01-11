using SNEngine.Services;

namespace SNEngine.CharacterSystem
{
    public class HideCharacterNode : CharacterNode
    {
        public override void Operation(Character character)
        {
            var serviceCharacters = NovelGame.Instance.GetService<CharacterService>();

            serviceCharacters.HideCharacter(character);
        }

        public override bool CanSkip()
        {
            return false;
        }
    }
}
